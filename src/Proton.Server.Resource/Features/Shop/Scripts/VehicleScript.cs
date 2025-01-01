using AltV.Net;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Microsoft.EntityFrameworkCore;
using Proton.Server.Core.Models.Shop;
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
        if (!Enum.TryParse(Name, out VehicleModel model))
        {
            return;
        }

        var Player = (PPlayer)player;
        await using var db = await dbContext.CreateDbContextAsync().ConfigureAwait(false);
        var vehicle = await db.StockVehicles.FirstOrDefaultAsync(a => a.Model == model).ConfigureAwait(false);
        var dbUser = await db.Users.FirstOrDefaultAsync(x => x.Id == Player.Id).ConfigureAwait(false);

        if (vehicle != null && dbUser != null)
        {
            if (dbUser.Money >= vehicle.Price)
            {
                dbUser.Money -= vehicle.Price;
            }
            else
            {
                Player.Emit("shop:vehicle:purchase", (int)ShopStatus.NO_MONEY);
                return;
            }

            dbUser.Vehicles.Add(
                new PlayerVehicle
                {
                    Model = vehicle.Model,
                    Price = vehicle.Price,
                    VehicleId = vehicle.Id,
                    AltVColor = Color,
                    Category = vehicle.Category,
                    DisplayName = vehicle.DisplayName,
                }
            );

            db.Users.Update(dbUser);
            await db.SaveChangesAsync().ConfigureAwait(false);
            Player.Emit("shop:vehicle:purchase", (int)ShopStatus.OK);
        }
        else
        {
            Player.Emit("shop:vehicle:purchase", (int)ShopStatus.ITEM_NOT_FOUND);
        }
    }

    public async Task GetAllItems(IPlayer player)
    {
        var Player = (PPlayer)player;
        var db = dbContext.CreateDbContext();
        var user = await db
            .Users.Where(x => x.Id == Player.ProtonId)
            .Select(a => new { a.Vehicles })
            .Include(x => x.Vehicles)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        var query = db.StockVehicles.AsQueryable();
        if (user is not null)
        {
            var models = user.Vehicles.Select(a => a.Model).ToArray();
            query = query.Where(a => !models.Contains(a.Model));
        }
        var vehicles = await query.ToListAsync().ConfigureAwait(false);

        Alt.LogInfo("Sending Vehicles: " + vehicles.Count);
        Player.Emit("shop:vehicle:all", (int)ShopStatus.OK, vehicles.ToShopItems());
    }

    public async Task GetOwnedItems(IPlayer _Player)
    {
        var Player = (PPlayer)_Player;
        var db = dbContext.CreateDbContext();
        var vehicles = await db.PlayerVehicles.Where(x => x.Id == Player.ProtonId).ToListAsync().ConfigureAwait(false);

        if (vehicles.Count == 0)
        {
            Player.Emit("shop:vehicle:owned", (int)ShopStatus.ITEM_NOT_FOUND, new List<SharedShopItem>());
        }
        else
        {
            Player.Emit("shop:vehicle:owned", (int)ShopStatus.OK, vehicles.Select(x => x.ToShopItem()).ToList());
        }
    }
}
