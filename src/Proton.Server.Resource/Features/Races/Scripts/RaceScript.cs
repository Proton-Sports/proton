using AltV.Net;
using AltV.Net.Elements.Entities;
using Proton.Server.Resource.Features.Races.Models;
using Proton.Shared.Interfaces;

namespace Proton.Server.Resource.Features.Races.Scripts;

public sealed class RaceScript : IStartup
{
    private readonly IRaceService raceService;

    public RaceScript(IRaceService raceService)
    {
        this.raceService = raceService;
        raceService.ParticipantJoined += HandleParticipantJoined;
        raceService.ParticipantLeft += HandleParticipantLeft;
        raceService.RacePrepared += HandleRacePrepared;
        raceService.RaceStarted += HandleRaceStarted;
    }

    private void HandleParticipantJoined(Race race, IPlayer player)
    {
        player.Emit("race:join", race.Id);
    }

    private void HandleParticipantLeft(Race race, IPlayer player)
    {
        player.Emit("race:leave", race.Id);
    }

    private Task HandleRacePrepared(Race race)
    {
        var participants = race.Participants;
        Alt.EmitClients([.. participants.Select(x => x.Player)], "race:prepare", race.Id);
        return Task.CompletedTask;
    }

    private Task HandleRaceStarted(Race race)
    {
        var participants = race.Participants;
        Alt.EmitClients([.. participants.Select(x => x.Player)], "race:start", race.Id);
        return Task.CompletedTask;
    }
}
