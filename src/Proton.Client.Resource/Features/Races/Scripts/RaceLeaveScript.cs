using AltV.Net.Client;
using AltV.Net.Client.Elements.Data;
using Proton.Shared.Interfaces;

namespace Proton.Client.Resource.Features.Races.Scripts;

public sealed class RaceLeaveScript : IStartup
{
    // TODO: Removed this when https://github.com/altmp/coreclr-module/pull/8 is released
    private bool added = false;

    public RaceLeaveScript()
    {
        Alt.OnServer<long>("race:join", HandleServerJoin);
        Alt.OnServer<long>("race:leave", HandleServerLeave);
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

    private void HandleServerLeave(long raceId)
    {
        if (added)
        {
            Alt.OnKeyUp -= HandleKeyUp;
            added = false;
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
