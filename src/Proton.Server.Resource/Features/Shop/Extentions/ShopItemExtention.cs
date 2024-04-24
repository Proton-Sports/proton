using Proton.Server.Core.Models.Shop;
using Proton.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Server.Resource.Features.Shop.Extentions
{
    internal static class ShopItemExtention
    {
        public static List<SharedShopItem> ToShopItems(this ICollection<Vehicle> v)
        {
            List<SharedShopItem> items = new List<SharedShopItem>();
            foreach (var vehicle in v)
                items.Add(ToShopItem(vehicle));

            return items;
        }

        public static List<SharedClothShopItem> ToShopItems(this ICollection<Cloth> v)
        {
            List<SharedClothShopItem> items = new List<SharedClothShopItem>();
            foreach (var c in v)
                items.Add(ToShopItem(c, false));

            return items;
        }

        //public static List<SharedShopItem> ToShopItems(this ICollection<OwnedVehicle> v)
        //{
        //    List<SharedShopItem> items = new List<SharedShopItem>();
        //    foreach (var vehicle in v)
        //        items.Add(ToShopItem(vehicle));

        //    return items;
        //}

        public static SharedShopItem ToShopItem(this Vehicle v)
        {
            return new SharedShopItem { Id = v.Id, Displayname = v.DisplayName, ItemName = v.AltVHash, Price = v.Price, Category = v.Category };
        }

        public static SharedClothShopItem ToShopItem(this Cloth v, bool Equiped)
        {
            return new SharedClothShopItem
            { 
                DisplayName = v.DisplayName,
                Price = v.Price,
                Id = v.Id,
                Equiped = Equiped,
                Category = v.Category
            };
        }

        //public static SharedShopItem ToShopItem(this OwnedVehicle v)
        //{
        //    return new SharedShopItem { Id = v.Id, Displayname = v.DisplayName, ItemName = v.AltVHash, Price = v.Price, Category = v.Category };
        //}
    }
}
