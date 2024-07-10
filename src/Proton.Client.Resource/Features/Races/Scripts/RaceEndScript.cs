using AltV.Net.Client;
using AsyncAwaitBestPractices;
<<<<<<< HEAD
using Proton.Client.Resource.Features.Ipls.Abstractions;
using Proton.Client.Resource.Features.UiViews.Abstractions;
=======
using Proton.Client.Infrastructure.Interfaces;
using Proton.Client.Resource.Features.Ipls.Abstractions;
>>>>>>> main
using Proton.Shared.Contants;
using Proton.Shared.Dtos;
using Proton.Shared.Interfaces;

namespace Proton.Client.Resource.Features.Races.Scripts;

public sealed class RaceEndScript : IStartup
{
    private readonly IUiView uiView;
    private readonly IRaceService raceService;
    private readonly IIplService iplService;

    public RaceEndScript(IUiView uiView, IRaceService raceService, IIplService iplService)
    {
        this.uiView = uiView;
        this.raceService = raceService;
        this.iplService = iplService;
<<<<<<< HEAD
        Alt.OnServer<RaceEndCountdownDto>(
            "race-end:countdown",
            (dto) =>
            {
                HandleServerCountdown(dto).SafeFireAndForget((exception) => Alt.LogError(exception.ToString()));
            }
        );
=======
        Alt.OnServer<RaceEndCountdownDto>("race-end:countdown", (dto) =>
        {
            HandleServerCountdown(dto).SafeFireAndForget((exception) => Alt.LogError(exception.ToString()));
        });
>>>>>>> main
        Alt.OnServer("race:destroy", HandleServerRaceDestroyAsync);
    }

    private async Task HandleServerCountdown(RaceEndCountdownDto dto)
    {
        if (!await uiView.TryMountAsync(Route.RaceEndCountdown))
            return;

        uiView.Emit("race-end-countdown:setData", dto);
    }

    private async Task HandleServerRaceDestroyAsync()
    {
        if (raceService.IsStarted)
        {
            raceService.Stop();
        }
        raceService.ClearRacePoints();
        uiView.Unmount(Route.RaceEndCountdown);
        Alt.Natives.SetLocalPlayerAsGhost(false, false);
        if (raceService.IplName is not null && iplService.IsLoaded(raceService.IplName))
        {
            await iplService.UnloadAsync(raceService.IplName);
        }
    }
}
