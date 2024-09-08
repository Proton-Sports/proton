using Microsoft.EntityFrameworkCore;
using Proton.Server.Core.Factorys;
using Proton.Server.Core.Interfaces;
using Proton.Server.Resource.Features.Races.Abstractions;
using Proton.Server.Resource.SharedKernel;

namespace Proton.Server.Resource.Features.Races.Scripts;

public sealed class RaceBestLapTimeRewardScript(IRaceService raceService, IDbContextFactory dbFactory) : HostedService
{
    public override Task StartAsync(CancellationToken ct)
    {
        raceService.RaceFinished += OnRaceFinishedAsync;
        return Task.CompletedTask;
    }

    private async Task OnRaceFinishedAsync(Race race)
    {
        var participants = race.Participants;
        var bestLapMs = double.MaxValue;
        RaceParticipant? bestLapParticipant = default;
        foreach (var participant in race.Participants.Where(a => a.FinishTime != 0))
        {
            foreach (var lap in participant.PointLogs.GroupBy(a => a.Lap).Where(a => a.Key > 0))
            {
                var points = lap.ToList();
                if (points.Count == 0)
                {
                    continue;
                }

                var startTime = points.Count == 1 ? race.StartTime : points[0].Time;
                var endTime = points[^1].Time;

                var lapMs = (endTime - startTime).TotalMilliseconds;
                if (lapMs < bestLapMs)
                {
                    bestLapMs = lapMs;
                    bestLapParticipant = participant;
                }
            }

            if (bestLapParticipant is null || bestLapParticipant.Player is not PPlayer pplayer)
            {
                return;
            }

            await using var db = await dbFactory.CreateDbContextAsync().ConfigureAwait(false);
            await db
                .Users.Where(a => a.Id == pplayer.ProtonId)
                .ExecuteUpdateAsync(a => a.SetProperty(a => a.Money, a => a.Money + 100))
                .ConfigureAwait(false);
        }
    }
}
