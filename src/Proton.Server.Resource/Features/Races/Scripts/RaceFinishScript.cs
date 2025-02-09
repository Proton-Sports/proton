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

        if (race.Participants.Count(x => x.FinishTime != 0) == race.Participants.Count)
        {
            raceService.Finish(race);
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
                        TimeMs = x.FinishTime == 0 ? 0 : x.FinishTime - race.StartTime.ToUnixTimeMilliseconds(),
                    })
                    .OrderBy(x => x.TimeMs, FinishTimeComparer.Instance)
            ]
        };
        Alt.EmitClients([.. race.Participants.Select(x => x.Player)], "race-finish:mountScoreboard", dto);
    }

    private class FinishTimeComparer : IComparer<long>
    {
        public static readonly FinishTimeComparer Instance = new();

        public int Compare(long x, long y)
        {
            return x == 0
                ? 1
                : y == 0
                    ? -1
                    : x > y
                        ? 1
                        : x < y
                            ? -1
                            : 0;
        }
    }
}
