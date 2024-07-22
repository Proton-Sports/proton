using AltV.Net;
using Proton.Server.Resource.Features.Races.Abstractions;
using Proton.Server.Resource.SharedKernel;
using Proton.Shared.Dtos;

namespace Proton.Server.Resource.Features.Races.Scripts;

public sealed class RaceFinishScript(IRaceService raceService, IMapCache mapCache) : HostedService
{
    public override Task StartAsync(CancellationToken ct)
    {
        raceService.ParticipantFinished += OnParticipantFinished;
        raceService.RaceFinished += OnRaceFinishedAsync;
        return Task.CompletedTask;
    }

    private void OnParticipantFinished(RaceParticipant participant)
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

    private async Task OnRaceFinishedAsync(Race race)
    {
        var map = await mapCache.GetAsync(race.MapId).ConfigureAwait(false);
        if (map is null)
        {
            return;
        }

        var dto = new MountScoreboardDto()
        {
            Name = map.Name,
            Participants =
            [
                .. race
                    .Participants.Select(x => new ScoreboardParticipantDto
                    {
                        Name = x.Player.Name,
                        Team = "Proton Sports",
                        TimeMs = x.FinishTime - race.StartTime.ToUnixTimeMilliseconds(),
                    })
                    .OrderBy(x => x.TimeMs)
            ]
        };
        Alt.EmitClients([.. race.Participants.Select(x => x.Player)], "race-finish:mountScoreboard", dto);
    }
}
