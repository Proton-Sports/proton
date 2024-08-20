using AltV.Net.Client;
using Proton.Client.Resource.Features.UiViews.Abstractions;
using Proton.Shared.Contants;
using Proton.Shared.Dtos;
using Proton.Shared.Interfaces;

namespace Proton.Client.Resource.Features.Races.Scripts;

public sealed class RaceHudScript : IStartup
{
    private readonly IUiView uiView;

    public RaceHudScript(IUiView uiView)
    {
        this.uiView = uiView;
        uiView.OnMount(Route.RaceHud, OnMount);
        Alt.OnServer("race-prepare:exitTransition", OnServerRacePrepareExitTransition);
        Alt.OnServer<long>("race-hud:startTime", OnServerStartTime);
        Alt.OnServer<RaceHudDto>("race-hud:getData", OnServerGetData);
        Alt.OnServer<RaceHudTickDto>("race-hud:tick", OnServerTick);
        Alt.OnServer<long>("race-hud:lapTime", OnServerLapTime);
        Alt.OnServer("race:finish", OnFinish);
        Alt.OnServer("race:destroy", OnDestroy);
        Alt.OnServer("race-hud:mount", OnServerMount);
    }

    private void OnServerRacePrepareExitTransition()
    {
        uiView.Mount(Route.RaceHud);
    }

    private void OnServerMount()
    {
        uiView.Mount(Route.RaceHud);
    }

    private void OnServerStartTime(long startTime)
    {
        uiView.Emit("race-hud:startTime", startTime);
    }

    private void OnMount()
    {
        Alt.EmitServer("race-hud:getData");
    }

    private void OnServerGetData(RaceHudDto dto)
    {
        dto.LocalId = Alt.LocalPlayer.RemoteId;
        uiView.Emit("race-hud:getData", dto);
    }

    private void OnServerTick(RaceHudTickDto dto)
    {
        uiView.Emit("race-hud:tick", dto);
    }

    private void OnServerLapTime(long timestamp)
    {
        uiView.Emit("race-hud:lapTime", timestamp);
    }

    private void OnFinish()
    {
        uiView.Emit("race-hud:status", "stop");
    }

    private void OnDestroy()
    {
        uiView.Unmount(Route.RaceHud);
    }
}
