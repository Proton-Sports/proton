using AltV.Net.Client;
using Proton.Shared.Interfaces;

namespace Proton.Client.Resource.Features.Races.Scripts;

public sealed class RaceJoinScript : IStartup
{
    private readonly IRaceService raceService;

    public RaceJoinScript(IRaceService raceService)
    {
        this.raceService = raceService;
        Alt.OnServer<long>("race:join", OnJoin);
    }

    private void OnJoin(long id)
    {
        raceService.RaceId = id;
    }
}
