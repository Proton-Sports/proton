using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Discord;
using Microsoft.EntityFrameworkCore;
using Proton.Server.Core.Extentions;
using Proton.Server.Core.Factorys;
using Proton.Server.Core.Models;
using Proton.Server.Infrastructure.Interfaces;
using Proton.Server.Infrastructure.Persistence;
using Proton.Shared;
using Proton.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Proton.Server.Infrastructure.Services
{
    public class VehicleShopService : ShopAbstract
    {
        private readonly IDbContextFactory<DefaultDbContext> defaultDbFactory;

        public VehicleShopService(IDbContextFactory<DefaultDbContext> defaultDbFactory)
        {
            Console.WriteLine("Loading ShopService");
            this.defaultDbFactory = defaultDbFactory;

            Alt.OnClient<string, int>(ShopPurchase,
                (p, id, color) => BuyItem(p, id, color).GetAwaiter());
            Alt.OnClient(ShopGetData, GetAllItems);
            Alt.OnClient(ShopGetOwnData, GetOwnedItems);
            Alt.OnPlayerConnect += Alt_OnPlayerConnect;
        }

        private void Alt_OnPlayerConnect(IPlayer player, string reason) //TODO: Replace
        {
            player.Spawn(new AltV.Net.Data.Position(-365.425f, -131.809f, 37.873f));
            player.Model = (uint)PedModel.FreemodeMale01;
        }

        /// <summary>
        /// Buys an Vehicle
        /// </summary>
        /// <param name="_Player"></param>
        /// <param name="Id"></param>
        /// <param name="Color"></param>
        /// <returns>True - Purchase ok, False - Purchase Failed</returns>
        public override async Task BuyItem(IPlayer _Player, string Name, int Color)
        {
            var Player = (PPlayer)_Player;
            var db = defaultDbFactory.CreateDbContext();
            var vehicle = db.Vehicles.Where(x => x.AltVHash == Name).FirstOrDefault();
            var dbUser = db.Users.Where(x => x.Id == Player.Id).FirstOrDefault();
            if (vehicle != null && dbUser != null)
            {
                if (dbUser.Money >= vehicle.Price)
                    dbUser.Money -= vehicle.Price;
                else
                {
                    await Console.Out.WriteLineAsync($"Player has not enough money Player: {dbUser.Money} Price: {vehicle.Price}");
                    Player.Emit(ShopPurchase, (int)ShopStatus.NO_MONEY);
                    return;
                }

                dbUser.Garage.Add(new Core.Models.OwnedVehicle
                {
                    Price = vehicle.Price,
                    DisplayName = vehicle.DisplayName,
                    AltVHash = vehicle.AltVHash,
                    AltVColor = Color.ToString(),
                    Category = vehicle.Category,
                });

                db.Users.Update(dbUser);
                await db.SaveChangesAsync();
                Player.Emit(ShopPurchase, (int)ShopStatus.OK);
            }
            else
            {
                Player.Emit(ShopPurchase, (int)ShopStatus.ITEM_NOT_FOUND);
            }
        }

        public override Task<List<SharedShopItem>> GetAllItems(IPlayer _Player)
        {
            var Player = (PPlayer)_Player;
            var db = defaultDbFactory.CreateDbContext();
            var user = db.Users.Where(x => x.Id == Player.ProtonId)
                .Include(x => x.Garage)
                .FirstOrDefault();

            var vehicles = db.Vehicles.ToList();

            if (user != null)
            {
                var garageVehicles = user.Garage.ToShopItems();
                foreach(var gv in garageVehicles)
                {
                    var alreadyOwned = vehicles.Where(x => x.AltVHash == gv.ItemName).FirstOrDefault();
                    if (alreadyOwned != null)
                    {
                        vehicles.Remove(alreadyOwned);
                        Console.WriteLine("Removed Already owned vehicle ID: " + gv.ItemName);
                    }
                }
            }

            Console.WriteLine("Sending Vehicles: " + vehicles.Count);
            _Player.Emit(ShopGetData, (int)ShopStatus.OK, vehicles.ToShopItems());
            return Task.FromResult(vehicles.ToShopItems());
        }

        public override Task GetOwnedItems(IPlayer _Player)
        {
            var Player = (PPlayer)_Player;
            Console.WriteLine(Player.ProtonId);
            var db = defaultDbFactory.CreateDbContext();
            var user = db.Users.Where(x => x.Id == Player.ProtonId)
                .Include(x => x.Garage)
                .FirstOrDefault();

            if (user != null)
            {
                var vehicles = user.Garage.ToShopItems();
                Player.Emit(ShopGetOwnData, (int)ShopStatus.OK, vehicles);
                return Task.CompletedTask;
            }

            Player.Emit(ShopGetOwnData, (int)ShopStatus.ITEM_NOT_FOUND, new List<SharedShopItem>());

            return Task.CompletedTask;
        }
    }
}
