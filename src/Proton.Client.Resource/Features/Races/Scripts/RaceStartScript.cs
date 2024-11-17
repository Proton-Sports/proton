using AltV.Net.Client;
using Proton.Client.Resource.Features.UiViews.Abstractions;
using Proton.Shared.Constants;
using Proton.Shared.Dtos;
using Proton.Shared.Interfaces;

namespace Proton.Client.Resource.Features.Races.Scripts;

public sealed class RaceStartScript : IStartup
{
    private readonly IRaceService raceService;
    private readonly IUiView uiView;

    public RaceStartScript(IRaceService raceService, IUiView uiView)
    {
        this.raceService = raceService;
        this.uiView = uiView;
        Alt.OnServer<RaceStartDto>("race-start:start", HandleServerStart);
    }

    private void HandleServerStart(RaceStartDto dto)
    {
        Alt.GameControlsEnabled = true;
        raceService.Ghosting = dto.Ghosting;
        raceService.Start();
        uiView.Unmount(Route.RacePrepare);
    }
}
