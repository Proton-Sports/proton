using System.Globalization;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using AsyncAwaitBestPractices;
using Microsoft.EntityFrameworkCore;
using Proton.Server.Core.Interfaces;
using Proton.Server.Resource.Features.Races.Abstractions;
using Proton.Server.Resource.SharedKernel;
using Proton.Shared.Constants;
using Proton.Shared.Dtos;

namespace Proton.Server.Resource.Features.Races.Scripts;

public sealed class RaceHostScript(IRaceService raceService, IDbContextFactory dbContextFactory) : HostedService
{
    private long counter;
    private readonly Dictionary<long, Timer> prepareTimers = [];

    public override Task StartAsync(CancellationToken ct)
    {
        Alt.OnClient<IPlayer, RaceHostSubmitDto>("race-host:submit", HandleSubmit);
        AltAsync.OnClient<IPlayer, Task>("race-host:availableMaps", HandleAvailableMapsAsync);
        AltAsync.OnClient<IPlayer, long, Task>("race-host:getMaxRacers", HandleGetMaxRacersAsync);
        raceService.RaceDestroyed += (race) =>
            OnRaceDestroyedAsync(race).SafeFireAndForget(exception => Alt.LogError(exception.ToString()));
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken ct)
    {
        foreach (var (_, timer) in prepareTimers)
        {
            await timer.DisposeAsync().ConfigureAwait(false);
        }
        prepareTimers.Clear();
    }

    private void HandleSubmit(IPlayer player, RaceHostSubmitDto dto)
    {
        if (raceService.Races.Any(x => x.Host == player))
        {
            // TODO: Error handling
            return;
        }

        if (!Enum.TryParse<VehicleModel>(dto.VehicleName, true, out var model))
        {
            // TODO: Error handling
            return;
        }

        var race = new Race
        {
            Id = ++counter,
            Host = player,
            VehicleModel = model,
            MapId = dto.MapId,
            MaxParticipants = dto.Racers,
            Duration = dto.Duration,
            CountdownSeconds = dto.Countdown,
            Description = dto.Description,
            Ghosting = dto.Ghosting,
            Type = dto.Type switch
            {
                "byLaps" => RaceType.Laps,
                "pointToPoint" => RaceType.PointToPoint,
                _ => RaceType.Laps
            },
            Laps = dto.Laps,
            Time = dto.Time switch
            {
                "earlyMorning" => new TimeOnly(5, 0, 0),
                "midday" => new TimeOnly(12, 0, 0),
                "night" => new TimeOnly(19, 0, 0),
                "exactTime" => (
                        (Func<string?, TimeOnly>)(
                            static (string? exactTime) =>
                            {
                                if (
                                    TimeOnly.TryParseExact(
                                        exactTime,
                                        "h:mm",
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.None,
                                        out var time
                                    )
                                )
                                {
                                    return time;
                                }
                                return new TimeOnly(5, 0, 0);
                            }
                        )
                    )(dto.ExactTime),
                _ => new TimeOnly(5, 0, 0),
            },
            Weather = dto.Weather,
            CreatedTime = DateTimeOffset.UtcNow,
            Status = RaceStatus.Open,
        };
        raceService.AddRace(race);
        raceService.AddParticipant(race.Id, new RaceParticipant { Player = player });
        prepareTimers[race.Id] = new Timer(
            (state) => PrepareRace((Race)state!).SafeFireAndForget(exception => Alt.LogError(exception.ToString())),
            race,
            race.CountdownSeconds * 1000,
            Timeout.Infinite
        );
        player.Emit("race-host:submit");
    }

    private async Task HandleAvailableMapsAsync(IPlayer player)
    {
        await using var ctx = await dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
        var dtos = await ctx
            .RaceMaps.Select(x => new RaceMapDto { Id = x.Id, Name = x.Name })
            .ToListAsync()
            .ConfigureAwait(false);
        player.Emit("race-host:availableMaps", dtos);
    }

    private async Task HandleGetMaxRacersAsync(IPlayer player, long mapId)
    {
        await using var ctx = await dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
        var racers = await ctx
            .RaceMaps.Where(x => x.Id == mapId)
            .Select(x => x.StartPoints.Count)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
        player.Emit("race-host:getMaxRacers", Math.Max(racers, 1));
    }

    private async Task PrepareRace(Race race)
    {
        if (prepareTimers.Remove(race.Id, out var timer))
        {
            await AltAsync
                .Do(() =>
                {
                    raceService.Prepare(race);
                })
                .ConfigureAwait(false);
            await timer.DisposeAsync().ConfigureAwait(false);
        }
    }

    private async Task OnRaceDestroyedAsync(Race race)
    {
        if (prepareTimers.Remove(race.Id, out var timer))
        {
            await timer.DisposeAsync().ConfigureAwait(false);
        }
    }
}
