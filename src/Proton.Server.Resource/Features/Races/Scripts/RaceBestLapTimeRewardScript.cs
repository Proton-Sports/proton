using AltV.Net;
using Microsoft.EntityFrameworkCore;
using Proton.Server.Core.Interfaces;
using Proton.Server.Infrastructure.Factorys;
using Proton.Server.Resource.Features.Races.Abstractions;
using Proton.Server.Resource.SharedKernel;
using Proton.Shared.Dtos;

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

                var startTime = (lap.Key == 1 || points.Count == 1) ? race.StartTime : points[0].Time;
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

            pplayer.SendNotification(
                new NotificationDto
                {
                    Icon = "CHAR_BANK_MAZE",
                    Title = "Money rewards",
                    SecondaryTitle = "Best lap time",
                    Body = $"You have received 100$ for making best lap time in the race.",
                }
            );
            await using var db = await dbFactory.CreateDbContextAsync().ConfigureAwait(false);
            await db
                .Users.Where(a => a.Id == pplayer.ProtonId)
                .ExecuteUpdateAsync(a => a.SetProperty(a => a.Money, a => a.Money + 100))
                .ConfigureAwait(false);
        }
    }
}
