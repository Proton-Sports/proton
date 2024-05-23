using AltV.Net.Client;
using Proton.Client.Resource.Features.UiViews.Abstractions;
using Proton.Shared.Contants;
using Proton.Shared.Dtos;
using Proton.Shared.Interfaces;

namespace Proton.Client.Resource.Features.Races.Scripts;

public sealed class RaceCountdownScript : IStartup
{
    private readonly IUiView uiView;
    public RaceCountdownScript(IUiView uiView)
    {
        this.uiView = uiView;
        Alt.OnServer("race-countdown:mount", HandleServerMount);
        Alt.OnServer("race-countdown:unmount", HandleServerUnmount);
        Alt.OnServer<RaceCountdownDataDto>("race-countdown:getData", HandleServerGetData);
        Alt.OnServer<int>("race-countdown:setParticipants", HandleServerSetParticipants);
        uiView.On("race-countdown:getData", HandleUIGetData);
    }

    private void HandleServerMount()
    {
        uiView.Mount(Route.RaceCountdown);
    }

    private void HandleServerUnmount()
    {
        uiView.Unmount(Route.RaceCountdown);
    }

    private void HandleServerGetData(RaceCountdownDataDto dto)
    {
        uiView.Emit("race-countdown:getData", dto);
    }

    private void HandleServerSetParticipants(int participants)
    {
        uiView.Emit("race-countdown:setParticipants", participants);
    }

    private void HandleUIGetData()
    {
        Alt.EmitServer("race-countdown:getData");
    }
}
