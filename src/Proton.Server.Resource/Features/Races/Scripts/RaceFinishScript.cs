using AltV.Net;
using Proton.Server.Resource.Features.Races.Models;
using Proton.Server.Resource.SharedKernel;
using Proton.Shared.Dtos;

namespace Proton.Server.Resource.Features.Races.Scripts;

public sealed class RaceFinishScript(IRaceService raceService) : HostedService
{
    public override Task StartAsync(CancellationToken ct)
    {
        raceService.ParticipantFinished += HandleFinished;
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
            raceService.Finish(race);
        }
        else if (finishedCount == 1)
        {
            Alt.EmitClients(
                [.. participants.Select(x => x.Player)],
                "race-end:countdown",
                new RaceEndCountdownDto
                {
                    EndTime = DateTimeOffset.UtcNow.AddSeconds(race.Duration).ToUnixTimeMilliseconds()
                }
            );
        }
    }
}
