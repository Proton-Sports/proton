using AltV.Net;
using AltV.Net.Elements.Entities;
using AsyncAwaitBestPractices;
using Microsoft.EntityFrameworkCore;
using Proton.Server.Core.Factorys;
using Proton.Server.Core.Interfaces;
using Proton.Server.Resource.SharedKernel;

namespace Proton.Server.Resource.Features.Players.Scripts;

public sealed class RewardForPlayingScript(IDbContextFactory dbFactory) : HostedService
{
    private Timer? timer;
    private readonly Dictionary<IPlayer, int> playerMinutes = [];

    public override Task StartAsync(CancellationToken ct)
    {
        timer = new Timer(
            (state) =>
            {
                TimerTickAsync().SafeFireAndForget(e => Alt.LogError(e.ToString()));
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
        foreach (var player in Alt.GetAllPlayers().Cast<PPlayer>())
        {
            if (!playerMinutes.TryGetValue(player, out var minutes))
            {
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
            }
        }

        if (rewardingPlayers.Count == 0)
        {
            return;
        }

        await using var db = await dbFactory.CreateDbContextAsync().ConfigureAwait(false);
        await db
            .Users.Where(a => rewardingPlayers.Select(a => a.ProtonId).Contains(a.Id))
            .ExecuteUpdateAsync(a => a.SetProperty(a => a.Money, a => a.Money + 50))
            .ConfigureAwait(false);
    }
}
