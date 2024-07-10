using AltV.Net;
using Proton.Server.Resource.Features.Races.Abstractions;
using Proton.Server.Resource.SharedKernel;
using Proton.Shared.Dtos;

namespace Proton.Server.Resource.Features.Races.Scripts;

public sealed class RaceStartScript(IRaceService raceService) : HostedService
{
    public override Task StartAsync(CancellationToken ct)
    {
        raceService.RaceStarted += HandleRaceStartedAsync;
        return Task.CompletedTask;
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
            participant.Player.Frozen = false;
            if (participant.Vehicle is not null)
            {
                participant.Vehicle!.Frozen = false;
                participant.Vehicle!.EngineOn = true;
            }
        }
        return Task.CompletedTask;
    }
}
