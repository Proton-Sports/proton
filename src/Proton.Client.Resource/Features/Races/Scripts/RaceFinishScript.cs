using AltV.Net.Client;
using AltV.Net.Client.Elements.Data;
using Proton.Client.Resource.Commons;
using Proton.Client.Resource.Features.UiViews.Abstractions;
using Proton.Shared.Constants;
using Proton.Shared.Dtos;

namespace Proton.Client.Resource.Features.Races.Scripts;

public sealed class RaceFinishScript(IUiView uiView) : HostedService
{
    public override Task StartAsync(CancellationToken ct)
    {
        Alt.OnServer<MountScoreboardDto>("race-finish:mountScoreboard", OnServerMountScoreboard);
        Alt.OnServer("race:destroy", OnRaceDestroy);
        uiView.OnUnmount(Route.RaceFinishScoreboard, OnRaceFinishScoreboardUnmount);
        uiView.OnMount(Route.RaceFinishScoreboard, OnMount);
        return Task.CompletedTask;
    }

    private void OnServerMountScoreboard(MountScoreboardDto dto)
    {
        uiView.Mount(Route.RaceFinishScoreboard, dto);
    }

    private void OnRaceDestroy()
    {
        uiView.Unmount(Route.RaceFinishScoreboard);
    }

    private void OnRaceFinishScoreboardUnmount()
    {
        Alt.OnKeyUp -= OnKeyUp;
    }

    private void OnMount()
    {
        Alt.OnKeyUp += OnKeyUp;
    }

    private void OnKeyUp(Key key)
    {
        if (key == Key.X)
        {
            uiView.Emit("race-finish-scoreboard:toggle");
        }
    }
}
