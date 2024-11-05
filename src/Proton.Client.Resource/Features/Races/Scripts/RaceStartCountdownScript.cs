using AltV.Net.Client;
using Proton.Client.Resource.Commons;
using Proton.Client.Resource.Features.UiViews.Abstractions;
using Proton.Shared.Constants;

namespace Proton.Client.Resource.Features.Races.Scripts;

public sealed class RaceStartCountdownScript(IUiView uiView) : HostedService
{
    public override Task StartAsync(CancellationToken ct)
    {
        Alt.OnServer("race-start-countdown:mount", OnCountdownMount);
        Alt.OnServer("race-start-countdown:start", OnCountdownStart);
        Alt.OnServer<long>("race:start", OnRaceStart);
        return Task.CompletedTask;
    }

    private void OnCountdownMount()
    {
        uiView.Mount(Route.RaceStartCountdown);
    }

    private void OnCountdownStart()
    {
        uiView.Emit("race-start-countdown:setStatus", "running");
    }

    private void OnRaceStart(long _)
    {
        Alt.SetTimeout(
            () =>
            {
                uiView.Unmount(Route.RaceStartCountdown);
            },
            1000
        );
    }
}
