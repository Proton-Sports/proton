using AltV.Net;
using AsyncAwaitBestPractices;
using Microsoft.EntityFrameworkCore;
using Proton.Server.Core.Interfaces;
using Proton.Server.Infrastructure.Factorys;
using Proton.Server.Resource.Features.Races.Abstractions;
using Proton.Server.Resource.SharedKernel;
using Proton.Shared.Dtos;

namespace Proton.Server.Resource.Features.Races.Scripts;

public sealed class RaceRewardScript(IRaceService raceService, IDbContextFactory dbFactory) : HostedService
{
    public override Task StartAsync(CancellationToken ct)
    {
        raceService.ParticipantFinished += (participant) =>
            OnParticipantFinishedAsync(participant).SafeFireAndForget(e => Alt.LogError(e.ToString()));
        return Task.CompletedTask;
    }

    private async Task OnParticipantFinishedAsync(RaceParticipant participant)
    {
        if (
            !raceService.TryGetRaceByParticipant(participant.Player, out var race)
            || participant.Player is not PPlayer pplayer
        )
        {
            return;
        }

        await using var db = await dbFactory.CreateDbContextAsync().ConfigureAwait(false);

        var total = race.Participants.Count;
        var finished = race.Participants.Count(a => a.FinishTime != 0);
        var totalMin = Math.Min(total, 4);
        var splits = ((totalMin * (1 + totalMin)) >> 1) + Math.Max(total - 4, 0);
        participant.PrizePercent = 1f / splits * (Math.Max(Math.Min(total, 4) - finished, 0) + 1);
        var money = (int)Math.Round(participant.PrizePercent * race.PrizePool);

        ((PPlayer)participant.Player).SendNotification(
            new NotificationDto
            {
                Icon = "CHAR_BANK_MAZE",
                Title = "Money rewards",
                SecondaryTitle = "Race completed",
                Body =
                    $"You have received {money}$ for being the {finished switch {
                    1 => "1st",
                    2 => "2nd",
                    3 => "3rd",
                    _ => $"{finished + 1}th"}} racer to complete.",
            }
        );

        await db
            .Users.Where(a => a.Id == pplayer.ProtonId)
            .ExecuteUpdateAsync(a => a.SetProperty(a => a.Money, a => a.Money + money))
            .ConfigureAwait(false);
    }
}
