using AltV.Net.Client;
using Proton.Client.Resource.Commons;
using Proton.Client.Resource.Features.UiViews.Abstractions;
using Proton.Shared.Contants;
using Proton.Shared.Dtos;

namespace Proton.Client.Resource.Features.Races.Scripts;

public sealed class RaceFinishScript(IUiView uiView) : HostedService
{
    private MountScoreboardDto? mountScoreboardDto;

    public override Task StartAsync(CancellationToken ct)
    {
        Alt.OnServer<MountScoreboardDto>("race-finish:mountScoreboard", OnServerMountScoreboard);
        Alt.OnServer("race:destroy", OnRaceDestroy);
        uiView.On("race-finish:getData", OnViewGetData);
        uiView.OnUnmount(Route.RaceFinishScoreboard, OnRaceFinishScoreboardUnmount);
        return Task.CompletedTask;
    }

    private void OnServerMountScoreboard(MountScoreboardDto dto)
    {
        mountScoreboardDto = dto;
        uiView.Mount(Route.RaceFinishScoreboard);
    }

    private void OnRaceDestroy()
    {
        uiView.Unmount(Route.RaceFinishScoreboard);
    }

    private void OnViewGetData()
    {
        if (mountScoreboardDto is not null)
        {
            uiView.Emit("race-finish:getData", mountScoreboardDto);
        }
    }

    private void OnRaceFinishScoreboardUnmount()
    {
        mountScoreboardDto = null;
    }
}
