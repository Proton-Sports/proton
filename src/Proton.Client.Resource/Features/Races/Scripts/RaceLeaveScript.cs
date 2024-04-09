using AltV.Net.Client;
using AltV.Net.Client.Elements.Data;
using Proton.Client.Resource.Features.Ipls.Abstractions;
using Proton.Shared.Interfaces;

namespace Proton.Client.Resource.Features.Races.Scripts;

public sealed class RaceLeaveScript : IStartup
{
    private readonly IRaceService raceService;
    private readonly IIplService iplService;
    // TODO: Removed this when https://github.com/altmp/coreclr-module/pull/8 is released
    private bool added = false;

    public RaceLeaveScript(IRaceService raceService, IIplService iplService)
    {
        this.raceService = raceService;
        this.iplService = iplService;
        Alt.OnServer<long>("race:join", HandleServerJoin);
        Alt.OnServer<long, Task>("race:leave", HandleServerLeaveAsync);
        Alt.OnServer<long>("race:prepare", HandleServerPrepare);
    }

    private void HandleServerJoin(long raceId)
    {
        if (!added)
        {
            Alt.OnKeyUp += HandleKeyUp;
            added = true;
        }
    }

    private async Task HandleServerLeaveAsync(long raceId)
    {
        if (added)
        {
            Alt.OnKeyUp -= HandleKeyUp;
            added = false;
            if (raceService.IplName is not null && iplService.IsLoaded(raceService.IplName))
            {
                await iplService.UnloadAsync(raceService.IplName);
            }
        }
    }

    private void HandleServerPrepare(long raceId)
    {
        if (added)
        {
            Alt.OnKeyUp -= HandleKeyUp;
            added = false;
        }
    }

    private void HandleKeyUp(Key key)
    {
        if (key == Key.Q)
        {
            Alt.EmitServer("race-leave:leave");
        }
    }
}
