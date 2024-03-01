using AltV.Net;
using AltV.Net.Elements.Entities;
using Proton.Shared.Interfaces;

namespace Proton.Server.Resource.Features.Races.Scripts;

public sealed class RaceLeaveScript : IStartup
{
    private readonly IRaceService raceService;

    public RaceLeaveScript(IRaceService raceService)
    {
        this.raceService = raceService;
        Alt.OnClient<IPlayer>("race-leave:leave", HandleLeave);
    }

    private void HandleLeave(IPlayer player)
    {
        raceService.RemoveParticipantByPlayer(player);
    }
}
