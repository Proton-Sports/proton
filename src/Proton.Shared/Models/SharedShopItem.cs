using System.Text.Json.Serialization;
using AltV.Community.MValueAdapters.Generators;
using AltV.Community.MValueAdapters.Generators.Abstractions;

namespace Proton.Shared.Models;

[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public class SharedShopItem
{
    [JsonPropertyName("displayname")]
    public string Displayname { get; set; } = string.Empty;

    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("itemname")]
    public string ItemName { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public int Price { get; set; }
    public string Category { get; set; } = string.Empty;

    public SharedShopItem() { }

    public SharedShopItem(long id, string displayname, string vehicleName, int price, string category)
    {
        Displayname = displayname;
        Id = id;
        ItemName = vehicleName;
        Price = price;
        Category = category;
    }
}
