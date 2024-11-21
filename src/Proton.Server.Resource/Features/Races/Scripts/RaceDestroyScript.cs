using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AsyncAwaitBestPractices;
using Proton.Server.Resource.Features.Races.Abstractions;
using Proton.Server.Resource.SharedKernel;

namespace Proton.Server.Resource.Features.Races.Scripts;

public sealed class RaceDestroyScript(IRaceService raceService) : HostedService
{
    private readonly Dictionary<Race, Timer> timers = [];

    public override Task StartAsync(CancellationToken ct)
    {
        raceService.ParticipantLeft += HandleParticipantLeft;
        raceService.RaceFinished += OnRaceFinished;
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken ct)
    {
        foreach (var (_, timer) in timers)
        {
            await timer.DisposeAsync().ConfigureAwait(false);
        }
        timers.Clear();
    }

    private void HandleParticipantLeft(Race race, IPlayer player)
    {
        if (race.Participants.Count == 0)
        {
            raceService.DestroyRace(race);
        }
    }

    private Task OnRaceFinished(Race race)
    {
        timers[race] = new Timer(
            (state) =>
                DestroyRaceAsync((Race)state!).SafeFireAndForget(exception => Alt.LogError(exception.ToString())),
            race,
            (int)TimeSpan.FromSeconds(10).TotalMilliseconds,
            Timeout.Infinite
        );
        return Task.CompletedTask;
    }

    private async Task DestroyRaceAsync(Race race)
    {
        if (timers.Remove(race, out var timer))
        {
            var players = race.Participants.Select(x => x.Player).ToArray();
            Alt.EmitClients(players, "race-destroy:enterTransition");
            await timer.DisposeAsync().ConfigureAwait(false);
            await Task.Delay(1000).ConfigureAwait(false);

            Alt.EmitClients(players, "race:destroy");
            foreach (var player in players)
            {
                player.Position = new Position(551.916f, 5562.336f, -96.042f);
                player.Dimension = 0;
            }
            raceService.DestroyRace(race);
        }
    }
}
