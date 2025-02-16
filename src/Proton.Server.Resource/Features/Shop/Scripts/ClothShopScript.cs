using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using Microsoft.EntityFrameworkCore;
using Proton.Server.Core.Interfaces;
using Proton.Server.Infrastructure.Factorys;
using Proton.Server.Infrastructure.Services;
using Proton.Server.Resource.SharedKernel;
using Proton.Shared.Dtos;
using Proton.Shared.Helpers;

namespace Proton.Server.Resource.Features.Shop.Scripts;

public sealed class ClothShopScript(IDbContextFactory dbFactory, OutfitService outfitService) : HostedService
{
    public override Task StartAsync(CancellationToken ct)
    {
        Alt.OnClient<long>("shop:cloth:purchase", (p, id) => BuyItem(p, id).GetAwaiter());
        Alt.OnClient<long>("shop:cloth:showroom", (p, i) => SetPreviewCloth(p, i));
        Alt.OnClient("shop:cloth:clear", ClearPreviewCloth);
        Alt.OnClient<bool, long>("shop:cloth:equip", (p, s, id) => EquipToggle(p, s, id).GetAwaiter());
        AltAsync.OnClient<PPlayer, Task>("cloth-shop.mount", OnMountAsync);
        return Task.CompletedTask;
    }

    private async Task OnMountAsync(PPlayer player)
    {
        await using var db = await dbFactory.CreateDbContextAsync().ConfigureAwait(false);
        var clothes = await db
            .Cloths.Select(a => new ClothShopClothesDto
            {
                Id = a.Id,
                Name = a.DisplayName,
                Category = a.Category,
                Price = a.Price,
            })
            .ToListAsync()
            .ConfigureAwait(false);
        var ownedClothes = await db
            .Closets.Where(a => a.OwnerId == player.ProtonId)
            .Select(a => new ClothShopOwnedClothesDto
            {
                Id = a.ClothId,
                Name = a.ClothItem.DisplayName,
                Category = a.ClothItem.Category,
                Selected = false,
            })
            .ToListAsync()
            .ConfigureAwait(false);

        // TODO: can be optimized
        clothes.RemoveAll(a => ownedClothes.FindIndex(b => b.Id == a.Id) != -1);

        player.Emit("cloth-shop.mount", new ClothShopMountDto { Clothes = clothes, OwnedClothes = ownedClothes });
    }

    public async Task BuyItem(IPlayer player, long Id)
    {
        var Player = (PPlayer)player;
        var db = dbFactory.CreateDbContext();
        var cloth = db.Cloths.Where(x => x.Id == Id).FirstOrDefault();
        var dbUser = db.Users.Where(x => x.Id == Player.ProtonId).FirstOrDefault();

        if (cloth != null && dbUser != null)
        {
            if (dbUser.Money >= cloth.Price)
            {
                dbUser.Money -= cloth.Price;
            }
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

    public Task SetPreviewCloth(IPlayer p, long Id)
    {
        Console.WriteLine("Setting Client cloth " + Id);
        var db = dbFactory.CreateDbContext();
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
        var db = dbFactory.CreateDbContext();
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

        if (!found)
            Alt.LogWarning(
                $"{p.Name}({player.ProtonId}) Tried to equip Item which he does not own! Item Cloth Id: {id}"
            );
    }
}
