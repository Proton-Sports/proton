<<<<<<< HEAD
﻿using AltV.Net;
using AltV.Net.Elements.Entities;
using Microsoft.EntityFrameworkCore;
using Proton.Server.Infrastructure.Factorys;
using Proton.Server.Infrastructure.Persistence;
using Proton.Server.Infrastructure.Services;
using Proton.Server.Resource.Features.Shop.Extentions;
using Proton.Shared.Helpers;
using Proton.Shared.Interfaces;
using Proton.Shared.Models;

namespace Proton.Server.Resource.Features.Shop.Scripts;

internal class ClothScript : IStartup
{
    private readonly IDbContextFactory<DefaultDbContext> dbContext;
    private readonly OutfitService outfitService;

    public ClothScript(IDbContextFactory<DefaultDbContext> dbContext, OutfitService outfitService)
    {
        this.dbContext = dbContext;
        this.outfitService = outfitService;
        Alt.OnClient<long>("shop:cloth:purchase", (p, id) => BuyItem(p, id).GetAwaiter());
        Alt.OnClient("shop:cloth:all", GetAllItems);
        Alt.OnClient("shop:cloth:owned", GetOwnedItems);
        Alt.OnClient<long>("shop:cloth:showroom", (p, i) => SetPreviewCloth(p, i));
        Alt.OnClient("shop:cloth:clear", ClearPreviewCloth);
        Alt.OnClient<bool, long>("shop:cloth:equip", (p, s, id) => EquipToggle(p, s, id).GetAwaiter());
    }

    public async Task BuyItem(IPlayer player, long Id)
    {
        var Player = (PPlayer)player;
        var db = dbContext.CreateDbContext();
        var cloth = db.Cloths.Where(x => x.Id == Id).FirstOrDefault();
        var dbUser = db.Users.Where(x => x.Id == Player.ProtonId).FirstOrDefault();

        if (cloth != null && dbUser != null)
        {
            if (dbUser.Money >= cloth.Price)
                dbUser.Money -= cloth.Price;
            else
            {
                Player.Emit("shop:cloth:purchase", (int)ShopStatus.NO_MONEY);
                return;
            }

            dbUser.Closets.Add(new Core.Models.Shop.Closet { ClothId = cloth.Id, });

            db.Users.Update(dbUser);
            await db.SaveChangesAsync();
            Player.Emit("shop:cloth:purchase", (int)ShopStatus.OK);
        }
        else
        {
            Player.Emit("shop:cloth:purchase", (int)ShopStatus.ITEM_NOT_FOUND);
        }
    }

    public Task GetAllItems(IPlayer player)
    {
        var Player = (PPlayer)player;
        var db = dbContext.CreateDbContext();
        var user = db
            .Users.Where(x => x.Id == Player.ProtonId)
            .Include(x => x.Closets)
            .ThenInclude(y => y.ClothItem)
            .FirstOrDefault();

        var cloths = db.Cloths.ToList();

        if (user != null)
        {
            var playerCloths = user.Closets.Select(x => x.ClothItem).ToList();
            foreach (var pC in playerCloths)
            {
                var alreadyOwned = cloths.Where(x => x.Id == pC.Id).FirstOrDefault();
                if (alreadyOwned != null)
                {
                    cloths.Remove(alreadyOwned);
                    Alt.LogWarning("Removed Already owned cloth ID: " + pC.Id);
                }
            }
        }

        Alt.LogInfo("Sending cloth: " + cloths.Count);
        Player.Emit("shop:cloth:all", (int)ShopStatus.OK, cloths.ToShopItems());
        return Task.CompletedTask;
    }

    public Task GetOwnedItems(IPlayer _Player)
    {
        var Player = (PPlayer)_Player;
        var db = dbContext.CreateDbContext();
        var user = db
            .Users.Where(x => x.Id == Player.ProtonId)
            .Include(x => x.Closets)
            .ThenInclude(x => x.ClothItem)
            .FirstOrDefault();

        if (user != null)
        {
            var cloths = user.Closets.Select(x => x.ClothItem.ToShopItem(x.IsEquiped)).ToList();
            Player.Emit("shop:cloth:owned", (int)ShopStatus.OK, cloths);
            return Task.CompletedTask;
        }

        Player.Emit("shop:cloth:owned", (int)ShopStatus.ITEM_NOT_FOUND, new List<SharedClothShopItem>());

        return Task.CompletedTask;
    }

    public Task SetPreviewCloth(IPlayer p, long Id)
    {
        Console.WriteLine("Setting Client cloth " + Id);
        var db = dbContext.CreateDbContext();
        var cloth = db.Cloths.Where(x => x.Id == Id).FirstOrDefault();

        if (cloth == null)
            return Task.CompletedTask;
        uint dlcHash = 0;
        if (cloth.IsDlc)
            dlcHash = Alt.Hash($"mp_m_{cloth.DlcName}");

        p.SetDlcClothes(
            Convert.ToByte(cloth.Component),
            ushort.Parse(cloth.Drawable.ToString()),
            Convert.ToByte(cloth.Texture),
            Convert.ToByte(cloth.Palette),
            dlcHash
        );

        return Task.CompletedTask;
    }

    public async Task ClearPreviewCloth(IPlayer p)
    {
        var player = (PPlayer)p;
        player.ClearClothing();
        await outfitService.EquipPlayerClothes(player);
    }

    public async Task EquipToggle(IPlayer p, bool state, long id)
    {
        Alt.Log("Updating, " + state + id);
        var player = (PPlayer)p;
        var db = dbContext.CreateDbContext();
        var user = db
            .Users.Where(x => x.Id == player.ProtonId)
            .Include(x => x.Closets)
            .ThenInclude(x => x.ClothItem)
            .FirstOrDefault();
        var cloth = db.Cloths.Where(x => x.Id == id).FirstOrDefault();

        if (user == null || cloth == null)
        {
            return;
        }

        bool found = false;

        foreach (var c in user.Closets)
        {
            if (c.ClothItem.Component == cloth.Component)
            {
                c.IsEquiped = false;
                db.Closets.Update(c);
            }

            if (c.ClothItem.Id == id)
            {
                c.IsEquiped = state;
                db.Closets.Update(c);
                found = true;
            }
        }
        await db.SaveChangesAsync();
        await ClearPreviewCloth(p);

        await GetOwnedItems(p);

        if (!found)
            Alt.LogWarning(
                $"{p.Name}({player.ProtonId}) Tried to equip Item which he does not own! Item Cloth Id: {id}"
            );
=======
﻿using AltV.Net.Elements.Entities;
using AltV.Net;
using Microsoft.EntityFrameworkCore;
using Proton.Server.Core.Factorys;
using Proton.Server.Infrastructure.Persistence;
using Proton.Shared.Helpers;
using Proton.Shared.Interfaces;
using Proton.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proton.Server.Resource.Features.Shop.Extentions;
using System.Numerics;
using Proton.Server.Infrastructure.Services;

namespace Proton.Server.Resource.Features.Shop.Scripts
{
    internal class ClothScript : IStartup
    {
        private readonly IDbContextFactory<DefaultDbContext> dbContext;
        private readonly OutfitService outfitService;

        public ClothScript(IDbContextFactory<DefaultDbContext> dbContext,
            OutfitService outfitService)
        {
            this.dbContext = dbContext;
            this.outfitService = outfitService;
            Alt.OnClient<long>("shop:cloth:purchase", (p, id) => BuyItem(p, id).GetAwaiter());
            Alt.OnClient("shop:cloth:all", GetAllItems);
            Alt.OnClient("shop:cloth:owned", GetOwnedItems);
            Alt.OnClient<long>("shop:cloth:showroom", (p, i) => SetPreviewCloth(p, i));
            Alt.OnClient("shop:cloth:clear", ClearPreviewCloth);
            Alt.OnClient<bool, long>("shop:cloth:equip", (p, s, id) => EquipToggle(p, s, id).GetAwaiter());
        }

        public async Task BuyItem(IPlayer player, long Id)
        {
            var Player = (PPlayer)player;
            var db = dbContext.CreateDbContext();
            var cloth = db.Cloths.Where(x => x.Id == Id).FirstOrDefault();
            var dbUser = db.Users.Where(x => x.Id == Player.ProtonId).FirstOrDefault();

            if (cloth != null && dbUser != null)
            {
                if (dbUser.Money >= cloth.Price)
                    dbUser.Money -= cloth.Price;
                else
                {
                    Player.Emit("shop:cloth:purchase", (int)ShopStatus.NO_MONEY);
                    return;
                }

                dbUser.Closets.Add(new Core.Models.Shop.Closet
                {
                    ClothId = cloth.Id,
                });

                db.Users.Update(dbUser);
                await db.SaveChangesAsync();
                Player.Emit("shop:cloth:purchase", (int)ShopStatus.OK);
            }
            else
            {
                Player.Emit("shop:cloth:purchase", (int)ShopStatus.ITEM_NOT_FOUND);
            }
        }

        public Task GetAllItems(IPlayer player)
        {
            var Player = (PPlayer)player;
            var db = dbContext.CreateDbContext();
            var user = db.Users.Where(x => x.Id == Player.ProtonId)
                .Include(x => x.Closets)
                .ThenInclude(y => y.ClothItem)
                .FirstOrDefault();

            var cloths = db.Cloths.ToList();

            if (user != null)
            {
                var playerCloths = user.Closets.Select(x => x.ClothItem).ToList();
                foreach (var pC in playerCloths)
                {
                    var alreadyOwned = cloths.Where(x => x.Id == pC.Id).FirstOrDefault();
                    if (alreadyOwned != null)
                    {
                        cloths.Remove(alreadyOwned);
                        Alt.LogWarning("Removed Already owned cloth ID: " + pC.Id);
                    }
                }
            }

            Alt.LogInfo("Sending cloth: " + cloths.Count);
            Player.Emit("shop:cloth:all", (int)ShopStatus.OK, cloths.ToShopItems());
            return Task.CompletedTask;
        }

        public Task GetOwnedItems(IPlayer _Player)
        {
            var Player = (PPlayer)_Player;
            var db = dbContext.CreateDbContext();
            var user = db.Users.Where(x => x.Id == Player.ProtonId)
                .Include(x => x.Closets)
                .ThenInclude(x => x.ClothItem)
                .FirstOrDefault();

            if (user != null)
            {
                var cloths = user.Closets.Select(x => x.ClothItem.ToShopItem(x.IsEquiped)).ToList();
                Player.Emit("shop:cloth:owned", (int)ShopStatus.OK, cloths);
                return Task.CompletedTask;
            }

            Player.Emit("shop:cloth:owned", (int)ShopStatus.ITEM_NOT_FOUND, new List<SharedClothShopItem>());

            return Task.CompletedTask;
        }

        public Task SetPreviewCloth(IPlayer p, long Id)
        {
            Console.WriteLine("Setting Client cloth " + Id);
            var db = dbContext.CreateDbContext();
            var cloth = db.Cloths.Where(x => x.Id == Id).FirstOrDefault();

            if (cloth == null) return Task.CompletedTask;
            uint dlcHash = 0;
            if (cloth.IsDlc) dlcHash = Alt.Hash($"mp_m_{cloth.DlcName}");

            p.SetDlcClothes(Convert.ToByte(cloth.Component),
                    ushort.Parse(cloth.Drawable.ToString()),
                    Convert.ToByte(cloth.Texture),
                    Convert.ToByte(cloth.Palette),
                    dlcHash);

            return Task.CompletedTask;
        }

        public async Task ClearPreviewCloth(IPlayer p)
        {
            var player = (PPlayer)p;
            player.ClearClothing();
            await outfitService.EquipPlayerClothes(player);
        }

        public async Task EquipToggle(IPlayer p, bool state, long id)
        {
            Alt.Log("Updating, " + state + id);
            var player = (PPlayer)p;
            var db = dbContext.CreateDbContext();
            var user = db.Users.Where(x => x.Id == player.ProtonId)
                .Include(x => x.Closets)
                .ThenInclude(x => x.ClothItem)
                .FirstOrDefault();
            var cloth = db.Cloths.Where(x => x.Id == id).FirstOrDefault();

            if(user == null || cloth == null)
            {
                return;
            }

            bool found = false;

            foreach(var c in user.Closets)
            {
                if(c.ClothItem.Component == cloth.Component)
                {
                    c.IsEquiped = false;
                    db.Closets.Update(c);                    
                }

                if(c.ClothItem.Id == id)
                {                    
                    c.IsEquiped = state;
                    db.Closets.Update(c);
                    found = true;
                }
            }
            await db.SaveChangesAsync();
            await ClearPreviewCloth(p);

            await GetOwnedItems(p);

            if (!found)
                Alt.LogWarning($"{p.Name}({player.ProtonId}) Tried to equip Item which he does not own! Item Cloth Id: {id}");
        }
>>>>>>> fa66636fead8b440ddf791e624522826101fdfe1
    }
}
