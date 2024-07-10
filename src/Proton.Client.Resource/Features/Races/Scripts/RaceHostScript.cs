using AltV.Net.Client;
using Proton.Shared.Interfaces;
using Proton.Shared.Dtos;
using Proton.Client.Resource.Features.UiViews.Abstractions;

namespace Proton.Client.Resource.Features.Races.Scripts;

public sealed class RaceHostScript : IStartup
{
    private readonly IUiView uiView;

    public RaceHostScript(IUiView uiView)
    {
        this.uiView = uiView;
        uiView.On<RaceHostSubmitDto>("race-host:submit", HandleWebViewSubmit);
        uiView.On("race-host:availableMaps", HandleWebViewAvailableMaps);
        uiView.On<long>("race-host:getMaxRacers", HandleWebViewGetMaxRacers);
        Alt.OnServer("race-host:submit", HandleServerSubmit);
        Alt.OnServer<List<RaceMapDto>>("race-host:availableMaps", HandleServerAvailableMaps);
        Alt.OnServer<int>("race-host:getMaxRacers", HandleServerGetMaxRacers);
    }

    private void HandleWebViewSubmit(RaceHostSubmitDto dto)
    {
        Alt.EmitServer("race-host:submit", dto);
    }

    private void HandleWebViewAvailableMaps()
    {
        Alt.EmitServer("race-host:availableMaps");
    }

    private void HandleWebViewGetMaxRacers(long mapId)
    {
        Alt.EmitServer("race-host:getMaxRacers", mapId);
    }

    private void HandleServerSubmit()
    {
        uiView.Emit("race-menu-list:navigate", "races");
    }

    private void HandleServerAvailableMaps(List<RaceMapDto> dtos)
    {
        uiView.Emit("race-host:availableMaps", dtos);
    }

    private void HandleServerGetMaxRacers(int racers)
    {
        uiView.Emit("race-host:getMaxRacers", racers);
    }
}
