using AltV.Net;
using AltV.Net.Elements.Entities;
using Microsoft.EntityFrameworkCore;
using Proton.Server.Infrastructure.Factorys;
using Proton.Server.Infrastructure.Persistence;
using Proton.Server.Resource.Features.Shop.Extentions;
using Proton.Shared.Helpers;
using Proton.Shared.Interfaces;
using Proton.Shared.Models;

namespace Proton.Server.Resource.Features.Shop.Scripts;

internal class VehicleScript : IStartup
{
    private readonly IDbContextFactory<DefaultDbContext> dbContext;

    public VehicleScript(IDbContextFactory<DefaultDbContext> dbContext)
    {
        this.dbContext = dbContext;

        Alt.OnClient<string, int>("shop:vehicle:purchase", (p, n, c) => BuyItem(p, n, c).GetAwaiter());
        Alt.OnClient("shop:vehicle:all", GetAllItems);
        Alt.OnClient("shop:vehicle:owned", GetOwnedItems);
    }

    public async Task BuyItem(IPlayer player, string Name, int Color)
    {
        var Player = (PPlayer)player;
        var db = dbContext.CreateDbContext();
        var vehicle = db.Vehicles.Where(x => x.AltVHash == Name).FirstOrDefault();
        var dbUser = db.Users.Where(x => x.Id == Player.Id).FirstOrDefault();

        if (vehicle != null && dbUser != null)
        {
            if (dbUser.Money >= vehicle.Price)
                dbUser.Money -= vehicle.Price;
            else
            {
                Player.Emit("shop:vehicle:purchase", (int)ShopStatus.NO_MONEY);
                return;
            }

            dbUser.Garages.Add(new Core.Models.Shop.Garage { VehicleId = vehicle.Id, AltVColor = Color, });

            db.Users.Update(dbUser);
            await db.SaveChangesAsync();
            Player.Emit("shop:vehicle:purchase", (int)ShopStatus.OK);
        }
        else
        {
            Player.Emit("shop:vehicle:purchase", (int)ShopStatus.ITEM_NOT_FOUND);
        }
    }

    public Task GetAllItems(IPlayer player)
    {
        var Player = (PPlayer)player;
        var db = dbContext.CreateDbContext();
        var user = db
            .Users.Where(x => x.Id == Player.ProtonId)
            .Include(x => x.Garages)
            .ThenInclude(x => x.VehicleItem)
            .FirstOrDefault();

        var vehicles = db.Vehicles.ToList();

        if (user != null)
        {
            var garageVehicles = user.Garages.Select(x => x.VehicleItem.ToShopItem()).ToList();
            foreach (var gv in garageVehicles)
            {
                var alreadyOwned = vehicles.Where(x => x.AltVHash == gv.ItemName).FirstOrDefault();
                if (alreadyOwned != null)
                {
                    vehicles.Remove(alreadyOwned);
                    Alt.LogWarning("Removed Already owned vehicle ID: " + gv.ItemName);
                }
            }
        }

        Alt.LogInfo("Sending Vehicles: " + vehicles.Count);
        Player.Emit("shop:vehicle:all", (int)ShopStatus.OK, vehicles.ToShopItems());
        return Task.CompletedTask;
    }

    public Task GetOwnedItems(IPlayer _Player)
    {
        var Player = (PPlayer)_Player;
        var db = dbContext.CreateDbContext();
        var user = db
            .Users.Where(x => x.Id == Player.ProtonId)
            .Include(x => x.Garages)
            .ThenInclude(x => x.VehicleItem)
            .FirstOrDefault();

        if (user != null)
        {
            var vehicles = user.Garages.Select(x => x.VehicleItem.ToShopItem()).ToList();
            Player.Emit("shop:vehicle:owned", (int)ShopStatus.OK, vehicles);
            return Task.CompletedTask;
        }

        Player.Emit("shop:vehicle:owned", (int)ShopStatus.ITEM_NOT_FOUND, new List<SharedShopItem>());

        return Task.CompletedTask;
    }
}
