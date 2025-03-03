using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using AsyncAwaitBestPractices;
using Proton.Server.Resource.Features.Ipls.Abstractions;
using Proton.Server.Resource.Features.Races.Abstractions;
using Proton.Server.Resource.SharedKernel;
using Proton.Shared.Dtos;
using Proton.Shared.Models;

namespace Proton.Server.Resource.Features.Races.Scripts;

public sealed class RacePrepareScript(IRaceService raceService, IMapCache mapCache, IIplService iplService)
    : HostedService
{
    private readonly Dictionary<long, Timer> startTimers = [];

    public override Task StartAsync(CancellationToken ct)
    {
        raceService.RacePrepared += HandleRacePrepared;
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken ct)
    {
        foreach (var (_, timer) in startTimers)
        {
            await timer.DisposeAsync().ConfigureAwait(false);
        }
        startTimers.Clear();
    }

    private async Task HandleRacePrepared(Race race)
    {
        var map = await mapCache.GetAsync(race.MapId).ConfigureAwait(false);
        if (map is null)
        {
            raceService.DestroyRace(race);
            return;
        }

        var participants = race.Participants;
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
            createTasks.AddLast(AltAsync.CreateVehicle(participant.VehicleModel, point.Position, point.Rotation, 256));
            participant.Player.Emit("race-prepare:enterTransition", point.Position);
        }

        await Task.Delay(3000).ConfigureAwait(false);

        foreach (var (point, participant) in map.StartPoints.Zip(participants))
        {
            participant.Player.Position = point.Position;
            participant.Player.Dimension = (int)race.Id;
            var now = DateTimeOffset.UtcNow;
            participant.Player.SetDateTime(
                now.Day - 1,
                now.Month - 1,
                now.Year,
                race.Time.Hour,
                race.Time.Minute,
                race.Time.Second
            );
            participant.Player.SetWeather(
                race.Weather switch
                {
                    "clear" => WeatherType.Clear,
                    "foggy" => WeatherType.Foggy,
                    "rainy" => WeatherType.Rain,
                    _ => WeatherType.Clear
                }
            );
        }

        foreach (var (participant, vehicle) in participants.Zip(await Task.WhenAll(createTasks).ConfigureAwait(false)))
        {
            participant.Vehicle = vehicle;
            vehicle.PrimaryColorRgb = new Rgba(
                (byte)Random.Shared.NextInt64(0, 255),
                (byte)Random.Shared.NextInt64(0, 255),
                (byte)Random.Shared.NextInt64(0, 255),
                255
            );
            vehicle.SecondaryColorRgb = new Rgba(
                (byte)Random.Shared.NextInt64(0, 255),
                (byte)Random.Shared.NextInt64(0, 255),
                (byte)Random.Shared.NextInt64(0, 255),
                255
            );
            vehicle.Dimension = (int)race.Id;
            participant.Player.SetIntoVehicle(participant.Vehicle, 1);
        }

        Alt.EmitClients(players, "race-prepare:exitTransition");

        await Task.Delay(5000).ConfigureAwait(false);
        raceService.Countdown(race, TimeSpan.FromSeconds(3));

        var startDuration = TimeSpan.FromSeconds(8);
        race.StartTime = DateTimeOffset.UtcNow.Add(startDuration);
        startTimers[race.Id] = new Timer(
            (state) => StartRaceAsync((Race)state!).SafeFireAndForget(exception => Alt.LogError(exception.ToString())),
            race,
            (int)startDuration.TotalMilliseconds,
            Timeout.Infinite
        );
        Alt.EmitClients(
            players,
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
            }
        );
    }

    private async Task StartRaceAsync(Race race)
    {
        if (startTimers.Remove(race.Id, out var timer))
        {
            raceService.Start(race);
            await timer.DisposeAsync().ConfigureAwait(false);
        }
    }
}
