<<<<<<< HEAD
﻿using Microsoft.EntityFrameworkCore;
using Proton.Server.Infrastructure.Factorys;
=======
﻿using AltV.Net;
using Microsoft.EntityFrameworkCore;
using Proton.Server.Core.Factorys;
>>>>>>> fa66636fead8b440ddf791e624522826101fdfe1
using Proton.Server.Infrastructure.Persistence;
using Proton.Server.Infrastructure.Services;
using Proton.Server.Resource.Authentication.Scripts;
using Proton.Shared.Interfaces;
<<<<<<< HEAD

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
=======
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Server.Resource.Features.Shop.Scripts
{
    internal class OutfitScript : IStartup
    {
        private readonly IDbContextFactory<DefaultDbContext> dbContext;
        private readonly OutfitService outfitService;

        public OutfitScript(IDbContextFactory<DefaultDbContext> dbContext,
            OutfitService outfitService)
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
>>>>>>> fa66636fead8b440ddf791e624522826101fdfe1
    }
}
