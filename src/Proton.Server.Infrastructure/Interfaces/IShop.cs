using AltV.Net.Elements.Entities;
using Proton.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Server.Infrastructure.Interfaces
{
    public interface IShop
    {
        Task BuyItem(IPlayer Player, int Id, string Color);
        Task<List<SharedShopItem>> GetAllItems(IPlayer player);
        Task GetOwnedItems(IPlayer Player);
    }

    public abstract class ShopAbstract : IShop
    {
        public const string ShopPurchase = "shop:purchase";
        public const string ShopGetData = "shop:items";
        public const string ShopGetOwnData = "shop:items:owned";

        public abstract Task BuyItem(IPlayer Player, int Id, string Color);
        public abstract Task<List<SharedShopItem>> GetAllItems(IPlayer player);
        public abstract Task GetOwnedItems(IPlayer Player);
    }
}
