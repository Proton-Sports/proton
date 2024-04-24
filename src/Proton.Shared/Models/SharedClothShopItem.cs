using AltV.Community.MValueAdapters.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Proton.Shared.Models
{
    [MValueAdapter]
    public class SharedClothShopItem
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("name")]
        public string DisplayName { get; set; } = string.Empty;
        [JsonPropertyName("selected")]
        public bool Equiped { get; set; } = false;
        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;
        [JsonPropertyName("price")]
        public int Price { get; set; }
    }
}
