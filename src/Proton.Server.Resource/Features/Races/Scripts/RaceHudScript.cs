using System.Numerics;
using AltV.Net;
using AltV.Net.Elements.Entities;
using AsyncAwaitBestPractices;
using Proton.Server.Resource.Features.Races.Abstractions;
using Proton.Server.Resource.Features.Races.Constants;
using Proton.Server.Resource.Features.Races.Models;
using Proton.Server.Resource.SharedKernel;
using Proton.Shared.Dtos;

namespace Proton.Server.Resource.Features.Races.Scripts;

public sealed class RaceHudScript(IRaceService raceService, IMapCache mapCache) : HostedService
{
    private Timer? timer;

    public override Task StartAsync(CancellationToken ct)
    {
        raceService.RaceStarted += OnRaceStarted;
        Alt.OnClient<IPlayer>("race-hud:getData", OnGetData);
        Alt.OnClient<IPlayer, int>("race:hit", OnHit);
        timer = new Timer(
            (_) => OnTick().SafeFireAndForget((exception) => Alt.LogError(exception.ToString())),
            null,
            TimeSpan.Zero,
            TimeSpan.FromMilliseconds(128)
        );
        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken ct)
    {
        timer?.Dispose();
        return Task.CompletedTask;
    }

    private void OnGetData(IPlayer player)
    {
        if (!raceService.TryGetRaceByParticipant(player, out var race))
        {
            return;
        }

        var participants = race.Participants;
        player.Emit(
            "race-hud:getData",
            new RaceHudDto
            {
                StartTime = race.PreparationEndTime,
                MaxLaps = race.Laps ?? 1,
                Participants = participants
                    .Select(x => new RaceHudParticipantDto
                    {
                        Id = x.Player.Id,
                        Lap = x.Lap,
                        Name = x.Player.Name,
                        Distance = x.AccumulatedDistance,
                        PartialDistance = 0,
                        SpeedPerHour = x.Player.IsInVehicle
                            ? ((Vector3)x.Player.Vehicle.Velocity).Length()
                            : x.Player.MoveSpeed,
                    })
                    .ToList(),
            }
        );
    }

    private Task OnRaceStarted(Race race)
    {
        foreach (var p in race.Participants)
        {
            p.NextRacePointIndex = 0;
        }
        Alt.EmitClients(
            [.. race.Participants.Select(x => x.Player)],
            "race-hud:startTime",
            race.PreparationEndTime.ToUnixTimeMilliseconds()
        );
        return Task.CompletedTask;
    }

    private void OnHit(IPlayer player, int index)
    {
        if (!raceService.TryGetRaceByParticipant(player, out var race))
        {
            return;
        }

        var participant = race.Participants.Find(x => x.Player == player);
        if (participant is null)
        {
            return;
        }

        if (index == 0 && participant.PointLogs.Count > 0)
        {
            player.Emit("race-hud:lapTime", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        }
    }

    private async Task OnTick()
    {
        foreach (var race in raceService.Races.Where(x => x.Status == RaceStatus.Started))
        {
            IPlayer[] players;
            RaceHudTickDto tickDto;
            var participants = race.Participants;
            var map = await mapCache.GetAsync(race.MapId).ConfigureAwait(false);
            if (map is null)
            {
                continue;
            }

            tickDto = new()
            {
                Participants =
                [
                    .. participants
                        .Select(
                            (x, i) =>
                            {
                                float partialDistance = 0;
                                var lastIndex = x.PointLogs.Last?.Value.Index;
                                var nextIndex = x.NextRacePointIndex;
                                if (nextIndex is not null)
                                {
                                    var nextPosition = map.RacePoints[nextIndex.Value].Position;
                                    partialDistance =
                                        Vector3.Distance(
                                            lastIndex is null
                                                ? map.StartPoints[i].Position
                                                : map.RacePoints[lastIndex.Value].Position,
                                            nextPosition
                                        )
                                        - Vector3.Distance(
                                            x.Player.Vehicle?.Position ?? x.Player.Position,
                                            nextPosition
                                        );
                                }
                                return new RaceHudParticipantTickDto
                                {
                                    Id = x.Player.Id,
                                    Lap = x.Lap,
                                    Distance = x.AccumulatedDistance,
                                    PartialDistance = partialDistance,
                                    SpeedPerHour = x.Player.IsInVehicle
                                        ? ((Vector3)x.Player.Vehicle!.Velocity).Length()
                                        : x.Player.MoveSpeed,
                                };
                            }
                        )
                        .OrderByDescending(x => x.Distance)
                ],
            };
            players = [.. participants.Select(x => x.Player)];
            Alt.EmitClients(players, "race-hud:tick", tickDto);
        }
    }
}
