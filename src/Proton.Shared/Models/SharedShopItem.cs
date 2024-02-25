using AltV.Net;
using Proton.Shared.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Shared.Models
{
    public class SharedShopItem : IMValueConvertible
    {
        public IMValueBaseAdapter GetAdapter() => SharedShopItemAdapter.Instance;

        public string Displayname { get; set; }
        public long Id { get; set; }
        public string Vehiclename { get; set; }
        public int Price { get; set; }

        public SharedShopItem(long id, string displayname, string vehicleName, int price)
        {
            Displayname = displayname;
            Id = id;
            Vehiclename = vehicleName;
            Price = price;
        }
    }
}
