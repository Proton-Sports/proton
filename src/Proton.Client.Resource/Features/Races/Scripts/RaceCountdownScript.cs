using AltV.Net.Client;
using Proton.Client.Resource.Features.UiViews.Abstractions;
using Proton.Shared.Constants;
using Proton.Shared.Dtos;
using Proton.Shared.Interfaces;

namespace Proton.Client.Resource.Features.Races.Scripts;

public sealed class RaceCountdownScript : IStartup
{
    private readonly IUiView uiView;

    public RaceCountdownScript(IUiView uiView)
    {
        this.uiView = uiView;
        Alt.OnServer<RaceCountdownDto>("race-countdown.mount", HandleServerMount);
        Alt.OnServer("race-countdown.unmount", HandleServerUnmount);
        Alt.OnServer<RaceCountdownParticipantDto>("race-countdown.participants.add", OnAddParticipant);
        Alt.OnServer<uint>("race-countdown.participants.remove", OnRemoveParticipant);
        uiView.On<bool>("race-countdown.ready.change", OnUiReadyChange);
        Alt.OnServer<uint, bool>("race-countdown.ready.change", OnServerReadyChange);
        Alt.OnServer<long>("race-countdown.countdown.set", OnServerSetCountdown);
        uiView.On<string>("race-countdown.vehicle.change", OnUiVehicleChange);
        Alt.OnServer<uint, string>("race-countdown.vehicle.change", OnServerVehicleChange);
    }

    private void HandleServerMount(RaceCountdownDto dto)
    {
        uiView.Mount(Route.RaceCountdown, dto);
    }

    private void HandleServerUnmount()
    {
        uiView.Unmount(Route.RaceCountdown);
    }

    private void OnAddParticipant(RaceCountdownParticipantDto dto)
    {
        uiView.Emit("race-countdown.participants.add", dto);
    }

    private void OnRemoveParticipant(uint id)
    {
        uiView.Emit("race-countdown.participants.remove", id);
    }

    private void OnUiReadyChange(bool ready)
    {
        Alt.EmitServer("race-countdown.ready.change", ready);
    }

    private void OnServerReadyChange(uint id, bool ready)
    {
        uiView.Emit("race-countdown.ready.change", id, ready);
    }

    private void OnServerSetCountdown(long endTimeMs)
    {
        uiView.Emit("race-countdown.countdown.set", endTimeMs);
    }

    private void OnUiVehicleChange(string name)
    {
        Alt.EmitServer("race-countdown.vehicle.change", name);
    }

    private void OnServerVehicleChange(uint id, string name)
    {
        uiView.Emit("race-countdown.vehicle.change", id, name);
    }
}
