using AltV.Net.Client;
using AsyncAwaitBestPractices;
using Proton.Client.Infrastructure.Interfaces;
using Proton.Shared.Contants;
using Proton.Shared.Dtos;
using Proton.Shared.Interfaces;

namespace Proton.Client.Resource.Features.Races.Scripts;

public sealed class RaceEndScript : IStartup
{
    private readonly IUiView uiView;

    public RaceEndScript(IUiView uiView)
    {
        this.uiView = uiView;
        Alt.OnServer<RaceEndCountdownDto>("race-end:countdown", (dto) =>
        {
            HandleServerCountdown(dto).SafeFireAndForget((exception) => Alt.LogError(exception.ToString()));
        });
        Alt.OnServer("race:end", HandleServerRaceEnd);
    }

    private async Task HandleServerCountdown(RaceEndCountdownDto dto)
    {
        if (!await uiView.TryMountAsync(Route.RaceEndCountdown)) return;

        uiView.Emit("race-end-countdown:setData", dto);
    }

    private void HandleServerRaceEnd()
    {
        uiView.Unmount(Route.RaceEndCountdown);
        Alt.Natives.SetLocalPlayerAsGhost(false, false);
    }
}
