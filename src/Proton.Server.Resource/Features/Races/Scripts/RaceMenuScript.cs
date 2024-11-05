using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using Microsoft.EntityFrameworkCore;
using Proton.Server.Core.Interfaces;
using Proton.Server.Infrastructure.Factorys;
using Proton.Server.Resource.SharedKernel;

namespace Proton.Server.Resource.Features.Races.Scripts;

public sealed class RaceMenuScript(IDbContextFactory dbFactory) : HostedService
{
    public override Task StartAsync(CancellationToken ct)
    {
        AltAsync.OnClient<IPlayer, Task>(
            "race-menu.tokens.get",
            (player) =>
            {
                if (player is not PPlayer pplayer)
                {
                    return Task.CompletedTask;
                }
                return OnGetTokenAsync(pplayer);
            }
        );
        return Task.CompletedTask;
    }

    private async Task OnGetTokenAsync(PPlayer player)
    {
        await using var db = await dbFactory.CreateDbContextAsync().ConfigureAwait(false);
        var money = await db
            .Users.Where(a => a.Id == player.ProtonId)
            .Select(a => a.Money)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
        player.Emit("race-menu.tokens.get", money);
    }
}
