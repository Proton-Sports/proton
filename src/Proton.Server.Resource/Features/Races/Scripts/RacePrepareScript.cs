using System.Numerics;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Microsoft.EntityFrameworkCore;
using Proton.Server.Core.Interfaces;
using Proton.Server.Resource.Features.Ipls.Abstractions;
using Proton.Server.Resource.Features.Races.Constants;
using Proton.Server.Resource.Features.Races.Models;
using Proton.Shared.Dtos;
using Proton.Shared.Interfaces;
using Proton.Shared.Models;

namespace Proton.Server.Resource.Features.Races.Scripts;

public sealed class RacePrepareScript : IStartup
{
    private readonly IRaceService raceService;
    private readonly IDbContextFactory dbContextFactory;
    private readonly IIplService iplService;
    private readonly Timer timer;

    public RacePrepareScript(IRaceService raceService, IDbContextFactory dbContextFactory, IIplService iplService)
    {
        this.raceService = raceService;
        this.dbContextFactory = dbContextFactory;
        this.iplService = iplService;

        raceService.RacePrepared += HandleRacePrepared;
        timer = new Timer((state) => HandleTimerTick(), null, 1000, Timeout.Infinite);
    }

    private async Task HandleRacePrepared(Race race)
    {
        await using var ctx = await dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
        var map = await ctx.RaceMaps
            .Where(x => x.Id == race.MapId)
            .Select(x => new { x.IplName, StartPoints = x.StartPoints.OrderBy(x => x.Index).ToArray(), RacePoints = x.RacePoints.OrderBy(x => x.Index).ToArray() })
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
        if (map is null)
        {
            raceService.DestroyRace(race);
            return;
        }

        var participants = raceService.GetParticipants(race.Id);
        var createTasks = new LinkedList<Task<IVehicle>>();
        var players = participants.Select(x => x.Player).ToArray();
        if (!string.IsNullOrEmpty(map.IplName))
        {
            var ok = await iplService.LoadAsync(players, map.IplName).ConfigureAwait(false);
            if (!ok)
            {
                Alt.LogWarning($"Could not load ipl {map.IplName}");
            }
        }

        foreach (var (point, participant) in map.StartPoints.Zip(participants))
        {
            createTasks.AddLast(AltAsync.CreateVehicle(race.VehicleModel, point.Position, point.Rotation, 256));
        }
        await Task.Delay(1000).ConfigureAwait(false);

        foreach (var (participant, vehicle) in participants.Zip(await Task.WhenAll(createTasks)))
        {
            participant.Player.Position = vehicle.Position;
            participant.Player.Dimension = (int)race.Id;

            participant.Vehicle = vehicle;
            vehicle.Dimension = (int)race.Id;

            var now = DateTimeOffset.UtcNow;
            participant.Player.SetDateTime(now.Day - 1, now.Month - 1, now.Year, race.Time.Hour, race.Time.Minute, race.Time.Second);
            participant.Player.SetWeather(race.Weather switch
            {
                "clear" => WeatherType.Clear,
                "foggy" => WeatherType.Foggy,
                "rainy" => WeatherType.Rain,
                _ => WeatherType.Clear
            });
            participant.Player.SetIntoVehicle(participant.Vehicle, 1);
        }

        Alt.EmitClients
        (
            players,
            "race-prepare:mount",
            new RacePrepareDto
            {
                EndTime = race.PreparationEndTime.ToUnixTimeMilliseconds(),
                RaceType = (byte)race.Type,
                Dimension = (int)race.Id,
                RacePoints = map.RacePoints.Select(x => new RacePointDto { Position = x.Position, Radius = x.Radius }).ToList(),
                IplName = map.IplName,
            }
        );
    }

    private void HandleTimerTick()
    {
        var now = DateTimeOffset.UtcNow;
        foreach (var race in raceService.Races.Where(x => x.Status == RaceStatus.Preparing && now >= x.PreparationEndTime))
        {
            raceService.Start(race);
        }
        timer.Change(1000, Timeout.Infinite);
    }
}
