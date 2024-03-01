using AltV.Net.Client;
using AltV.Net.Client.Elements.Data;
using Proton.Shared.Interfaces;

namespace Proton.Client.Resource.Features.Races.Scripts;

public sealed class RaceLeaveScript : IStartup
{
    public RaceLeaveScript()
    {
        Alt.OnServer<long>("race:join", HandleServerJoin);
        Alt.OnServer<long>("race:leave", HandleServerLeave);
        Alt.OnServer<long>("race:prepare", HandleServerPrepare);
    }

    private void HandleServerJoin(long raceId)
    {
        Alt.OnKeyUp += HandleKeyUp;
    }

    private void HandleServerLeave(long raceId)
    {
        Alt.OnKeyUp -= HandleKeyUp;
    }

    private void HandleServerPrepare(long raceId)
    {
        Alt.OnKeyUp -= HandleKeyUp;
    }

    private void HandleKeyUp(Key key)
    {
        if (key == Key.Q)
        {
            Alt.EmitServer("race-leave:leave");
        }
    }
}
