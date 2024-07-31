using AltV.Net.Client;
using AsyncAwaitBestPractices;
using Proton.Client.Resource.Commons;
using Proton.Client.Resource.Features.UiViews.Abstractions;
using Proton.Shared.Contants;
using Proton.Shared.Dtos;

namespace Proton.Client.Resource.Features.Races.Scripts;

public sealed class RaceFinishCountdownScript(IUiView uiView) : HostedService
{
    public override Task StartAsync(CancellationToken ct)
    {
        Alt.OnServer<RaceFinishCountdownDto>(
            "race-finish-countdown:start",
            (dto) =>
            {
                OnStart(dto).SafeFireAndForget((exception) => Alt.LogError(exception.ToString()));
            }
        );
        Alt.OnServer("race:destroy", OnDestroy);
        return Task.CompletedTask;
    }

    private async Task OnStart(RaceFinishCountdownDto dto)
    {
        if (!await uiView.TryMountAsync(Route.RaceFinishCountdown).ConfigureAwait(false))
        {
            return;
        }

        uiView.Emit("race-finish-countdown:setData", dto);
    }

    private Task OnDestroy()
    {
        uiView.Unmount(Route.RaceFinishCountdown);
        return Task.CompletedTask;
    }
}
