using AltV.Net.Client;
using Proton.Client.Infrastructure.Interfaces;
using Proton.Shared.Contants;
using Proton.Shared.Dtos;
using Proton.Shared.Interfaces;

namespace Proton.Client.Resource.Features.Races.Scripts;

public sealed class RaceHudScript : IStartup
{
    private readonly IUiView uiView;
    private readonly IRaceService raceService;

    public RaceHudScript(IUiView uiView, IRaceService raceService)
    {
        this.uiView = uiView;
        this.raceService = raceService;
        uiView.OnMount(Route.RaceHud, OnMount);
        Alt.OnServer<long>("race:prepare", OnServerRacePrepare);
        Alt.OnServer<long>("race-hud:startTime", OnServerStartTime);
        Alt.OnServer<RaceHudDto>("race-hud:getData", OnServerGetData);
        Alt.OnServer<RaceHudTickDto>("race-hud:tick", OnServerTick);
        Alt.OnServer<long>("race-hud:lapTime", OnServerLapTime);
        Alt.OnServer("race:finish", OnFinish);
        Alt.OnServer("race:destroy", OnDestroy);
    }

    private void OnServerRacePrepare(long id)
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
