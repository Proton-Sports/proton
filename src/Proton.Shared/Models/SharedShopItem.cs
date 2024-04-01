using AltV.Community.MValueAdapters.Generators;
using AltV.Net;
using Proton.Shared.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Proton.Shared.Models
{
    [MValueAdapter]
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
}
