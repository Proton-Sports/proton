using AltV.Net.Elements.Entities;
using Proton.Server.Resource.Features.Races.Models;
using Proton.Shared.Interfaces;

namespace Proton.Server.Resource.Features.Races.Scripts;

public sealed class RaceDestroyScript : IStartup
{
    private readonly IRaceService raceService;

    public RaceDestroyScript(IRaceService raceService)
    {
        this.raceService = raceService;
        raceService.ParticipantLeft += HandleParticipantLeft;
    }

    private void HandleParticipantLeft(Race race, IPlayer player)
    {
        if (race.Participants.Count == 0)
        {
            raceService.DestroyRace(race);
        }
    }
}
