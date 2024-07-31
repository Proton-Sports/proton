using AltV.Net;
using AsyncAwaitBestPractices;
using Proton.Server.Resource.Features.Races.Abstractions;
using Proton.Server.Resource.SharedKernel;

namespace Proton.Server.Resource.Features.Races.Scripts;

public sealed class RaceStartCountdownScript(IRaceService raceService) : HostedService
{
    public override Task StartAsync(CancellationToken ct)
    {
        raceService.RaceCountdown += (race, countDelay) =>
            OnRaceCountdownAsync(race, countDelay).SafeFireAndForget(exception => Alt.LogError(exception.ToString()));
        return base.StartAsync(ct);
    }

    private static async Task OnRaceCountdownAsync(Race race, TimeSpan countDelay)
    {
        var players = race.Participants.Select(x => x.Player).ToArray();
        Alt.EmitClients(players, "race-start-countdown:mount");
        await Task.Delay((int)countDelay.TotalMilliseconds).ConfigureAwait(false);
        Alt.EmitClients(players, "race-start-countdown:start");
    }
}
