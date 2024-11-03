using Microsoft.EntityFrameworkCore;
using Proton.Server.Infrastructure.Factorys;
using Proton.Server.Infrastructure.Persistence;
using Proton.Server.Infrastructure.Services;
using Proton.Server.Resource.Authentication.Scripts;
using Proton.Shared.Interfaces;

namespace Proton.Server.Resource.Features.Shop.Scripts;

internal class OutfitScript : IStartup
{
    private readonly IDbContextFactory<DefaultDbContext> dbContext;
    private readonly OutfitService outfitService;

    public OutfitScript(IDbContextFactory<DefaultDbContext> dbContext, OutfitService outfitService)
    {
        this.dbContext = dbContext;
        this.outfitService = outfitService;
        AuthenticationScript.OnAuthenticationDoneEvent += AuthenticationScript_OnAuthenticationDoneEvent;
    }

    private async Task AuthenticationScript_OnAuthenticationDoneEvent(AltV.Net.Elements.Entities.IPlayer p)
    {
        var player = (PPlayer)p;
        player.ClearClothing();
        await outfitService.EquipPlayerClothes(player);
    }
}
