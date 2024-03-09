using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Microsoft.EntityFrameworkCore;
using Proton.Server.Core.Extentions;
using Proton.Server.Core.Factorys;
using Proton.Server.Core.Models;
using Proton.Server.Infrastructure.Interfaces;
using Proton.Server.Infrastructure.Persistence;
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

            Alt.OnClient<int, string>(ShopPurchase,
                (p, id, color) => BuyItem(p, id, color).GetAwaiter());
            Alt.OnClient(ShopGetData, GetAllItems);
            Alt.OnClient(ShopGetOwnData, GetOwnedItems);
            Alt.OnPlayerConnect += Alt_OnPlayerConnect;
        }

        private void Alt_OnPlayerConnect(IPlayer player, string reason)
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
        public override async Task BuyItem(IPlayer _Player, int Id, string Color)
        {
            var Player = (PPlayer)_Player;
            var db = defaultDbFactory.CreateDbContext();
            var vehicle = db.Vehicles.Where(x => x.Id == Id).FirstOrDefault();
            var dbUser = db.Users.Where(x => x.Id == Player.Id).FirstOrDefault();
            if (vehicle != null && dbUser != null)
            {
                if (dbUser.Money >= vehicle.Price)
                    dbUser.Money -= vehicle.Price;
                else
                {
                    Player.Emit(ShopPurchase, (int)ShopStatus.NO_MONEY);
                    return;
                }

                dbUser.Garage.Add(new Core.Models.OwnedVehicle
                {
                    Price = vehicle.Price,
                    DisplayName = vehicle.DisplayName,
                    AltVHash = vehicle.AltVHash,
                    AltVColor = Color
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

        public override Task<List<SharedShopItem>> GetAllItems(IPlayer player)
        {
            var db = defaultDbFactory.CreateDbContext();
            var vehicles = db.Vehicles.ToList();
            Console.WriteLine("Sending Vehicles: " + vehicles.Count);
            player.Emit(ShopGetData, (int)ShopStatus.OK, vehicles.ToShopItems());
            return Task.FromResult(vehicles.ToShopItems());
        }

        public override Task GetOwnedItems(IPlayer _Player)
        {
            var Player = (PPlayer)_Player;
            var db = defaultDbFactory.CreateDbContext();
            var user = db.Users.Where(x => x.Id == Player.ProtonId).FirstOrDefault();

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
