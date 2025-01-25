using Proton.Server.Core.Models.Shop;
using Proton.Shared.Models;

namespace Proton.Server.Resource.Features.Shop.Extensions;

internal static class ShopItemExtention
{
    public static List<SharedShopItem> ToShopItems(this ICollection<StockVehicle> v)
    {
        var items = new List<SharedShopItem>();
        foreach (var vehicle in v)
        {
            items.Add(ToShopItem(vehicle));
        }

        return items;
    }

    public static List<SharedClothShopItem> ToShopItems(this ICollection<Cloth> v)
    {
        var items = new List<SharedClothShopItem>();
        foreach (var c in v)
        {
            items.Add(ToShopItem(c, false));
        }

        return items;
    }

    public static SharedShopItem ToShopItem(this StockVehicle v)
    {
        return new SharedShopItem
        {
            Id = v.Id,
            Displayname = v.DisplayName,
            ItemName = Enum.GetName(v.Model) ?? "None",
            Price = v.Price,
            Category = v.Category
        };
    }

    public static SharedShopItem ToShopItem(this PlayerVehicle v)
    {
        return new SharedShopItem
        {
            Id = v.Id,
            Displayname = v.DisplayName,
            ItemName = Enum.GetName(v.Model) ?? "None",
            Price = v.Price,
            Category = v.Category
        };
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
}
