using AltV.Community.MValueAdapters.Generators;
using AltV.Net;
using Proton.Shared.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Shared.Models
{
    //[MValueAdapter]
    public class SharedShopItem : IMValueConvertible
    {
        public IMValueBaseAdapter GetAdapter() => Adapters.SharedShopItemAdapter.Instance;

        public string Displayname { get; set; } = string.Empty;
        public long Id { get; set; }
        public string Vehiclename { get; set; } = string.Empty;
        public int Price { get; set; }
        public string Category { get; set; } = string.Empty;

        public SharedShopItem() { }
        public SharedShopItem(long id, string displayname, string vehicleName, int price, string category)
        {
            Displayname = displayname;
            Id = id;
            Vehiclename = vehicleName;
            Price = price;
            Category = category;
        }
    }
}
