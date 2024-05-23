using AltV.Net.Client;
using Proton.Client.Resource.Features.UiViews.Abstractions;
using Proton.Shared.Contants;
using Proton.Shared.Dtos;
using Proton.Shared.Interfaces;

namespace Proton.Client.Resource.Features.Races.Scripts;

public sealed class RaceMenuRacesTabScript : IStartup
{
    private readonly IUiView uiView;
    public RaceMenuRacesTabScript(IUiView uiView)
    {
        this.uiView = uiView;
        Alt.OnServer<List<RaceDto>>("race-menu-races:getRaces", HandleServerGetRaces);
        Alt.OnServer<RaceDetailsDto>("race-menu-races:getDetails", HandleServerGetDetails);
        Alt.OnServer<long, string, RaceParticipantDto>("race-menu-races:participantChanged", HandleServerSetParticipants);
        Alt.OnServer<string, RaceDto>("race-menu-races:raceChanged", HandleServerRaceChanged);
        uiView.On("race-menu-races:getRaces", HandleUIGetRaces);
        uiView.On<long>("race-menu-races:getDetails", HandleUIGetDetails);
        uiView.On<long>("race-menu-races:join", HandleUIJoin);
    }

    private void HandleServerGetRaces(List<RaceDto> dtos)
    {
        uiView.Emit("race-menu-races:getRaces", dtos);
    }

    private void HandleServerGetDetails(RaceDetailsDto dto)
    {
        uiView.Emit("race-menu-races:getDetails", dto);
    }

    private void HandleServerSetParticipants(long raceId, string type, RaceParticipantDto dto)
    {
        if (uiView.IsMounted(Route.RaceMainMenuList))
        {
            uiView.Emit("race-menu-races:participantChanged", raceId, type, dto);
        }
    }

    private void HandleUIGetRaces()
    {
        Alt.EmitServer("race-menu-races:getRaces");
    }

    private void HandleUIGetDetails(long id)
    {
        Alt.EmitServer("race-menu-races:getDetails", id);
    }

    private void HandleUIJoin(long id)
    {
        Alt.EmitServer("race-menu-races:join", id);
    }

    private void HandleServerRaceChanged(string type, RaceDto dto)
    {
        if (uiView.IsMounted(Route.RaceMainMenuList))
        {
            uiView.Emit("race-menu-races:raceChanged", type, dto);
        }
    }
}
