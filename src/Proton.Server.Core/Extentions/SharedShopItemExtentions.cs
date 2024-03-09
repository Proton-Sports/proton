using Proton.Server.Core.Models;
using Proton.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Server.Core.Extentions
{
    public static class SharedShopItemExtentions
    {
        public static List<SharedShopItem> ToShopItems(this ICollection<Vehicle> v)
        {
            List<SharedShopItem> items = new List<SharedShopItem>();
            foreach (var vehicle in v)
                items.Add(ToShopItem(vehicle));

            return items;
        }

        public static List<SharedShopItem> ToShopItems(this ICollection<OwnedVehicle> v)
        {
            List<SharedShopItem> items = new List<SharedShopItem>();
            foreach (var vehicle in v)
                items.Add(ToShopItem(vehicle));

            return items;
        }

        public static SharedShopItem ToShopItem(this Vehicle v)
        {
            return new SharedShopItem { Id= v.Id, Displayname= v.DisplayName, Vehiclename= v.AltVHash, Price= v.Price, Category = v.Category };
        }

        public static SharedShopItem ToShopItem(this OwnedVehicle v)
        {
            return new SharedShopItem { Id = v.Id, Displayname = v.DisplayName, Vehiclename = v.AltVHash, Price = v.Price, Category = v.Category };
        }
    }
}
