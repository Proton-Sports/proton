using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Proton.Server.Resource.Features.Races.Abstractions;
using Proton.Server.Resource.Features.Races.Constants;
using Proton.Server.Resource.Features.Races.Models;
using Proton.Server.Resource.SharedKernel;
using Proton.Shared.Dtos;
using Proton.Shared.Models;

namespace Proton.Server.Resource.Features.Races.Scripts;

public sealed class RacePrepareScript(IRaceService raceService, IMapCache mapCache) : HostedService
{
    private Timer? timer;

    public override Task StartAsync(CancellationToken ct)
    {
        raceService.RacePrepared += HandleRacePrepared;
        timer = new Timer((state) => HandleTimerTick(), null, 1000, 1000);
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken ct)
    {
        if (timer is not null)
        {
            await timer.DisposeAsync().ConfigureAwait(false);
        }
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

        foreach (var (point, participant) in map.StartPoints.Zip(participants))
        {
            createTasks.AddLast(
                AltAsync.CreateVehicle(race.VehicleModel, point.Position, point.Rotation, 256)
            );
        }

        await Task.Delay(1000).ConfigureAwait(false);
        foreach (var (participant, vehicle) in participants.Zip(await Task.WhenAll(createTasks)))
        {
            participant.Player.Position = vehicle.Position;
            participant.Player.Dimension = (int)race.Id;

            participant.Vehicle = vehicle;
            vehicle.Dimension = (int)race.Id;

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
            participant.Player.SetIntoVehicle(participant.Vehicle, 1);
        }

        Alt.EmitClients(
            [.. participants.Select(x => x.Player)],
            "race-prepare:mount",
            new RacePrepareDto
            {
                EndTime = race.PreparationEndTime.ToUnixTimeMilliseconds(),
                RaceType = (byte)race.Type,
                Dimension = (int)race.Id,
                RacePoints = map
                    .RacePoints.Select(x => new RacePointDto
                    {
                        Position = x.Position,
                        Radius = x.Radius
                    })
                    .ToList(),
            }
        );
    }

    private void HandleTimerTick()
    {
        var now = DateTimeOffset.UtcNow;
        foreach (
            var race in raceService.Races.Where(x =>
                x.Status == RaceStatus.Preparing && now >= x.PreparationEndTime
            )
        )
        {
            raceService.Start(race);
        }
    }
}
