using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AsyncAwaitBestPractices;
using Microsoft.EntityFrameworkCore;
using Proton.Server.Core.Interfaces;
using Proton.Server.Core.Models;
using Proton.Server.Infrastructure.Factorys;
using Proton.Server.Resource.Features.Ipls.Abstractions;
using Proton.Server.Resource.Features.Races.Abstractions;
using Proton.Server.Resource.SharedKernel;
using Proton.Shared.Dtos;
using Proton.Shared.Models;

namespace Proton.Server.Resource.Features.Races.Scripts;

public sealed class RaceRestoreScript(
    IRaceService raceService,
    IDbContextFactory dbFactory,
    IMapCache mapCache,
    IEnumerable<IRacePointResolver> resolvers,
    IIplService iplService
) : HostedService
{
    private readonly IRacePointResolver[] resolversArr = resolvers.ToArray();

    public override Task StartAsync(CancellationToken ct)
    {
        AltAsync.OnPlayerDisconnect += OnPlayerDisconnectAsync;
        // AltAsync.OnServer<IPlayer, Task>("auth:firstSignIn", OnSignInAsync);
        Alt.OnServer<IPlayer>(
            "auth:firstSignIn",
            (player) => OnSignInAsync(player).SafeFireAndForget(exception => Alt.LogError(exception.ToString()))
        );
        Alt.OnConsoleCommand += (command, args) =>
        {
            if (command == "kick")
            {
                foreach (var player in Alt.GetAllPlayers())
                {
                    player.Kick("");
                }
            }
        };
        return Task.CompletedTask;
    }

    private async Task OnPlayerDisconnectAsync(IPlayer player, string _)
    {
        if (!raceService.TryGetRaceByParticipant(player, out var race))
        {
            return;
        }

        foreach (var participant in race.Participants)
        {
            if (participant.Player == player)
            {
                raceService.RemoveParticipant(participant);
                await SaveAsync(race, participant).ConfigureAwait(false);
                break;
            }
        }
    }

    private async Task SaveAsync(Race race, RaceParticipant participant)
    {
        var player = (PPlayer)participant.Player;
        var position = player.Position;
        var rotation = player.Rotation;
        var restoration = new UserRaceRestoration
        {
            UserId = player.ProtonId,
            RaceId = race.Guid,
            Lap = participant.Lap,
            AccumulatedDistance = participant.AccumulatedDistance,
            PartialDistance = participant.PartialDistance,
            NextRacePointIndex = participant.NextRacePointIndex,
            Points = participant
                .PointLogs.Select(x => new UserRacePointRestoration
                {
                    UserId = player.ProtonId,
                    Index = x.Index,
                    Lap = x.Lap,
                    Time = x.Time
                })
                .ToArray(),
            FinishTime = participant.FinishTime,
            X = position.X,
            Y = position.Y,
            Z = position.Z,
            Roll = rotation.Roll,
            Pitch = rotation.Pitch,
            Yaw = rotation.Yaw,
        };
        await using var db = await dbFactory.CreateDbContextAsync().ConfigureAwait(false);
        db.Add(restoration);
        await db.SaveChangesAsync().ConfigureAwait(false);
    }

    private async Task OnSignInAsync(IPlayer player)
    {
        if (player is not PPlayer p)
        {
            return;
        }

        await using var db = await dbFactory.CreateDbContextAsync().ConfigureAwait(false);
        var restoration = await db
            .UserRaceRestorations.Where(x => x.UserId == p.ProtonId)
            .Include(x => x.Points)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
        if (restoration is null)
        {
            return;
        }

        var deleteTask = db.UserRaceRestorations.Where(x => x.UserId == p.ProtonId).ExecuteDeleteAsync();
        var race = raceService.Races.FirstOrDefault(x => x.Guid == restoration.RaceId);
        if (race is null)
        {
            await deleteTask.ConfigureAwait(false);
            return;
        }

        var map = await mapCache.GetAsync(race.MapId).ConfigureAwait(false);
        if (map is null)
        {
            await deleteTask.ConfigureAwait(false);
            return;
        }

        var resolver = Array.Find(resolversArr, x => x.SupportedRaceType == race.Type);
        if (resolver is null)
        {
            return;
        }

        IVehicle? vehicle = null;
        var position = new Position(restoration.X, restoration.Y, restoration.Z);
        var rotation = new Rotation(restoration.Roll, restoration.Pitch, restoration.Yaw);
        p.Position = position;
        Console.WriteLine("Set player to pos " + position);
        p.Rotation = rotation;
        p.Dimension = (int)race.Id;
        if (race.Status == RaceStatus.Started)
        {
            vehicle = await AltAsync.CreateVehicle(race.VehicleModel, position, rotation).ConfigureAwait(false);
            vehicle.Dimension = (int)race.Id;
            p.SetIntoVehicle(vehicle, 1);
            Console.WriteLine("Created vehicle " + vehicle);
        }
        var participant = new RaceParticipant
        {
            Player = player,
            Vehicle = vehicle,
            Lap = restoration.Lap,
            AccumulatedDistance = restoration.AccumulatedDistance,
            PartialDistance = restoration.PartialDistance,
            NextRacePointIndex = restoration.NextRacePointIndex,
            PointLogs = new LinkedList<RacePointLog>(
                restoration
                    .Points.OrderBy(x => x.Index)
                    .Select(x => new RacePointLog
                    {
                        Index = x.Index,
                        Lap = x.Lap,
                        Time = x.Time
                    })
            ),
            FinishTime = restoration.FinishTime,
        };
        raceService.AddParticipant(race.Id, participant);

        if (participant.FinishTime != 0)
        {
            return;
        }

        if (!string.IsNullOrEmpty(map.IplName))
        {
            await iplService.LoadAsync([player], map.IplName).ConfigureAwait(false);
        }

        p.Emit(
            "race-prepare:mount",
            new RacePrepareDto
            {
                EndTime = race.StartTime.ToUnixTimeMilliseconds(),
                RaceType = (byte)race.Type,
                Dimension = (int)race.Id,
                RacePoints = map
                    .RacePoints.Select(x => new RacePointDto { Position = x.Position, Radius = x.Radius })
                    .ToList(),
                IplName = map.IplName,
                DisableLoadingCheckpoint = true
            }
        );
        var output = resolver.Resolve(
            new RacePointResolverInput
            {
                Index = participant.PointLogs.Last?.Value.Index ?? 0,
                Lap = participant.Lap,
                TotalLaps = race.Laps ?? 1,
                TotalPoints = map.RacePoints.Count,
            }
        );
        p.Emit(
            "race:hit",
            new RaceHitDto
            {
                Lap = output.Lap,
                Index = output.Index,
                NextIndex = output.NextIndex,
                Finished = false,
            }
        );
        p.Emit("race-start:start", new RaceStartDto { Laps = race.Laps ?? 1, Ghosting = race.Ghosting });
        player.Emit("race-hud:mount");

        await deleteTask.ConfigureAwait(false);
        Console.WriteLine("End of restore");
    }
}
