using AltV.Net;
using AltV.Net.Elements.Entities;
using Microsoft.EntityFrameworkCore;
using Proton.Server.Core.Interfaces;
using Proton.Server.Infrastructure.Factorys;
using Proton.Server.Resource.SharedKernel;
using Proton.Shared.Dtos;

namespace Proton.Server.Resource.Features.Players.Scripts;

public sealed class RewardForPlayingScript(IDbContextFactory dbFactory) : HostedService
{
    private Timer? timer;
    private readonly Dictionary<IPlayer, int> playerMinutes = [];

    public override Task StartAsync(CancellationToken ct)
    {
        timer = new Timer(
            async (state) =>
            {
                await TimerTickAsync().ConfigureAwait(false);
            },
            null,
            0,
            (int)TimeSpan.FromMinutes(1).TotalMilliseconds
        );
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken ct)
    {
        if (timer is not null)
        {
            await timer.DisposeAsync().ConfigureAwait(false);
            timer = null;
        }
        playerMinutes.Clear();
    }

    private async ValueTask TimerTickAsync()
    {
        var rewardingPlayers = new List<PPlayer>();
        foreach (var player in Alt.GetAllPlayers().Cast<PPlayer>().Where(a => a.ProtonId != -1))
        {
            if (!playerMinutes.TryGetValue(player, out var minutes))
            {
                minutes = 1;
                playerMinutes.Add(player, 1);
            }
            else
            {
                minutes += 1;
                playerMinutes[player] = minutes;
            }

            if (minutes == 30)
            {
                rewardingPlayers.Add(player);
                playerMinutes[player] = 0;
            }
        }

        if (rewardingPlayers.Count == 0)
        {
            return;
        }

        // TODO: maybe a NotificationService in favor of PPlayer.SendNotification
        Alt.EmitClients(
            [.. rewardingPlayers],
            "player:sendNotification",
            new NotificationDto
            {
                Icon = "CHAR_BANK_MAZE",
                Title = "Money rewards",
                SecondaryTitle = "Playing every 30 minutes",
                Body = $"You have received 50$ for playing every 30 minutes.",
            }
        );

        await using var db = await dbFactory.CreateDbContextAsync().ConfigureAwait(false);
        await db
            .Users.Where(a => rewardingPlayers.Select(a => a.ProtonId).Contains(a.Id))
            .ExecuteUpdateAsync(a => a.SetProperty(a => a.Money, a => a.Money + 50))
            .ConfigureAwait(false);
    }
}
