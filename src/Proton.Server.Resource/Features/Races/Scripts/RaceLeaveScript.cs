using AltV.Net;
using AltV.Net.Elements.Entities;
using Proton.Server.Resource.Features.Races.Models;
using Proton.Shared.Interfaces;

namespace Proton.Server.Resource.Features.Races.Scripts;

public sealed class RaceLeaveScript : IStartup
{
    private readonly IRaceService raceService;

    public RaceLeaveScript(IRaceService raceService)
    {
        this.raceService = raceService;
        raceService.ParticipantLeft += HandleParticipantLeft;
        Alt.OnClient<IPlayer>("race-leave:leave", HandleClientLeave);
    }

    private void HandleParticipantLeft(Race _, IPlayer player)
    {
        player.Dimension = 0;
    }

    private void HandleClientLeave(IPlayer player)
    {
        raceService.RemoveParticipantByPlayer(player);
    }
}
