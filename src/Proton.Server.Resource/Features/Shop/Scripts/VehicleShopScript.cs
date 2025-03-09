using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Microsoft.EntityFrameworkCore;
using Proton.Server.Core.Interfaces;
using Proton.Server.Core.Models.Shop;
using Proton.Server.Infrastructure.Factorys;
using Proton.Server.Resource.SharedKernel;
using Proton.Shared.Dtos;
using Proton.Shared.Helpers;

namespace Proton.Server.Resource.Features.Shop.Scripts;

public class VehicleShopScript(IDbContextFactory dbContext) : HostedService
{
    public override Task StartAsync(CancellationToken ct)
    {
        AltAsync.OnClient<PPlayer, string, int, Task>("shop:vehicle:purchase", BuyItemAsync);
        AltAsync.OnClient<PPlayer, Task>("vehicle-shop.mount", OnMount);
        return Task.CompletedTask;
    }

    async Task OnMount(PPlayer player)
    {
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        await using var db = await dbContext.CreateDbContextAsync().ConfigureAwait(false);
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
        var vehicles = await db.StockVehicles.ToListAsync().ConfigureAwait(false);

        player.Emit(
            "vehicle-shop.mount",
            new VehicleShopMountDto
            {
                Vehicles =
                [
                    .. vehicles.Select(a => new VehicleShopVehicleDto
                    {
                        Id = a.Id,
                        DisplayName = a.DisplayName,
                        ItemName = Enum.GetName(a.Model) ?? "N/A",
                        Category = a.Category,
                        Price = a.Price,
                    }),
                ],
            }
        );
    }

    public async Task BuyItemAsync(IPlayer player, string Name, int Color)
    {
        if (!Enum.TryParse(Name, true, out VehicleModel model))
        {
            return;
        }

        var Player = (PPlayer)player;
        await using var db = await dbContext.CreateDbContextAsync().ConfigureAwait(false);
        var vehicle = await db.StockVehicles.FirstOrDefaultAsync(a => a.Model == model).ConfigureAwait(false);
        var dbUser = await db.Users.FirstOrDefaultAsync(x => x.Id == Player.ProtonId).ConfigureAwait(false);

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
                    PrimaryColor = Color switch
                    {
                        111 => new Rgba(255, 255, 246, 255),
                        150 => new Rgba(188, 25, 23, 255),
                        80 => new Rgba(66, 113, 225, 255),
                        126 => new Rgba(241, 204, 64, 255),
                        125 => new Rgba(131, 197, 102, 255),
                        _ => Rgba.Zero,
                    },
                    SecondaryColor = Color switch
                    {
                        111 => new Rgba(255, 255, 246, 255),
                        150 => new Rgba(188, 25, 23, 255),
                        80 => new Rgba(66, 113, 225, 255),
                        126 => new Rgba(241, 204, 64, 255),
                        125 => new Rgba(131, 197, 102, 255),
                        _ => Rgba.Zero,
                    },
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
}
