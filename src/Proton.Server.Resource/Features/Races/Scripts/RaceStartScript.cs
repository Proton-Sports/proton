using AltV.Net;
using Proton.Server.Resource.Features.Races.Models;
using Proton.Shared.Dtos;
using Proton.Shared.Interfaces;

namespace Proton.Server.Resource.Features.Races.Scripts;

public sealed class RaceStartScript : IStartup
{
    private readonly IRaceService raceService;

    public RaceStartScript(IRaceService raceService)
    {
        this.raceService = raceService;
        raceService.RaceStarted += HandleRaceStartedAsync;
        raceService.ParticipantFinished += HandleFinished;
    }

    private Task HandleRaceStartedAsync(Race race)
    {
        var participants = race.Participants;
        Alt.EmitClients(
            participants.Select(x => x.Player).ToArray(),
            "race-start:start",
            new RaceStartDto { Laps = race.Laps ?? 1, Ghosting = race.Ghosting }
        );
        foreach (var participant in participants)
        {
            participant.NextRacePointIndex = 0;
            participant.Lap = 0;
            if (participant.Vehicle is not null)
            {
                participant.Vehicle!.Frozen = false;
                participant.Vehicle!.EngineOn = true;
            }
        }
        return Task.CompletedTask;
    }

    private void HandleFinished(RaceParticipant participant)
    {
        if (!raceService.TryGetRaceByParticipant(participant.Player, out var race))
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var participants = race.Participants;
        var finishedCount = race.Participants.Count(x => x.FinishTime != 0);

        if (finishedCount == participants.Count)
        {
            Alt.EmitClients([.. participants.Select(x => x.Player)], "race:destroy");
            raceService.DestroyRace(race);
        }
        else if (finishedCount == 1)
        {
            Alt.EmitClients(
                [.. participants.Select(x => x.Player)],
                "race-end:countdown",
                new RaceEndCountdownDto
                {
                    EndTime = DateTimeOffset
                        .UtcNow.AddSeconds(race.Duration)
                        .ToUnixTimeMilliseconds()
                }
            );
        }
    }
}
