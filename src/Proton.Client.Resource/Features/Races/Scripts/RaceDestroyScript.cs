using AltV.Net.Client;
using AsyncAwaitBestPractices;
using Proton.Client.Resource.Commons;
using Proton.Client.Resource.Features.Ipls.Abstractions;
using Proton.Client.Resource.Features.UiViews.Abstractions;
using Proton.Shared.Contants;

namespace Proton.Client.Resource.Features.Races.Scripts;

public sealed class RaceDestroyScript(IRaceService raceService, IIplService iplService, IUiView uiView) : HostedService
{
    public override Task StartAsync(CancellationToken ct)
    {
        Alt.OnServer(
            "race:destroy",
            () => OnDestroyAsync().SafeFireAndForget(exception => Alt.LogError(exception.ToString()))
        );
        Alt.OnServer("race-destroy:enterTransition", OnEnterTransition);
        return Task.CompletedTask;
    }

    private void OnEnterTransition()
    {
        uiView.Mount(Route.RaceEndTransition);
        Alt.Natives.DoScreenFadeOut(1000);
        Alt.SetTimeout(
            () =>
            {
                uiView.Unmount(Route.RaceEndTransition);
                Alt.Natives.DoScreenFadeIn(1000);
            },
            3000
        );
    }

    private async Task OnDestroyAsync()
    {
        if (raceService.IsStarted)
        {
            raceService.Stop();
        }
        raceService.ClearRacePoints();
        Alt.Natives.SetLocalPlayerAsGhost(false, false);
        if (raceService.IplName is not null && iplService.IsLoaded(raceService.IplName))
        {
            await iplService.UnloadAsync(raceService.IplName).ConfigureAwait(false);
        }
    }
}
