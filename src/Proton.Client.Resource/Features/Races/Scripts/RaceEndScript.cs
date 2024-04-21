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
    private readonly IRaceService raceService;

    public RaceEndScript(IUiView uiView, IRaceService raceService)
    {
        this.uiView = uiView;
        this.raceService = raceService;
        Alt.OnServer<RaceEndCountdownDto>("race-end:countdown", (dto) =>
        {
            HandleServerCountdown(dto).SafeFireAndForget((exception) => Alt.LogError(exception.ToString()));
        });
        Alt.OnServer("race:destroy", HandleServerRaceDestroy);
    }

    private async Task HandleServerCountdown(RaceEndCountdownDto dto)
    {
        if (!await uiView.TryMountAsync(Route.RaceEndCountdown)) return;

        uiView.Emit("race-end-countdown:setData", dto);
    }

    private void HandleServerRaceDestroy()
    {
        if (raceService.IsStarted)
        {
            raceService.Stop();
        }
        raceService.ClearRacePoints();
        uiView.Unmount(Route.RaceEndCountdown);
        Alt.Natives.SetLocalPlayerAsGhost(false, false);
    }
}
