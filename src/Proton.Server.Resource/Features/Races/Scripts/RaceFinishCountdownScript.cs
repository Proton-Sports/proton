using AltV.Net;
using AsyncAwaitBestPractices;
using Proton.Server.Resource.Features.Races.Abstractions;
using Proton.Server.Resource.SharedKernel;
using Proton.Shared.Dtos;

namespace Proton.Server.Resource.Features.Races.Scripts;

public sealed class RaceFinishCountdownScript(IRaceService raceService) : HostedService
{
    private readonly Dictionary<Race, Timer> timers = [];

    public override Task StartAsync(CancellationToken ct)
    {
        raceService.ParticipantFinished += OnParticipantFinished;
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

    private void OnParticipantFinished(RaceParticipant participant)
    {
        if (!raceService.TryGetRaceByParticipant(participant.Player, out var race))
        {
            return;
        }

        if (race.Participants.Count(x => x.FinishTime != 0) == 1 && race.Participants.Count != 1)
        {
            Alt.EmitClients(
                [.. race.Participants.Select(x => x.Player)],
                "race-finish-countdown:start",
                new RaceFinishCountdownDto
                {
                    EndTime = DateTimeOffset.UtcNow.AddSeconds(race.Duration).ToUnixTimeMilliseconds()
                }
            );
            if (!timers.ContainsKey(race))
            {
                timers[race] = new Timer(
                    (state) =>
                        FinishRaceAsync((Race)state!)
                            .SafeFireAndForget(exception => Alt.LogError(exception.ToString())),
                    race,
                    (int)TimeSpan.FromSeconds(race.Duration).TotalMilliseconds,
                    Timeout.Infinite
                );
            }
        }
    }

    private async Task FinishRaceAsync(Race race)
    {
        if (timers.Remove(race, out var timer))
        {
            raceService.Finish(race);
            await timer.DisposeAsync().ConfigureAwait(false);
        }
    }

    private async Task OnRaceFinished(Race race)
    {
        if (timers.Remove(race, out var timer))
        {
            await timer.DisposeAsync().ConfigureAwait(false);
        }
    }
}
