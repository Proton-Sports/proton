using AltV.Net;
using AltV.Net.Elements.Args;
using Proton.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Shared.Adapters
{
    public sealed class SharedShopItemAdapter : IMValueAdapter<SharedShopItem>
    {
        public static readonly SharedShopItemAdapter Instance = new();

        public SharedShopItem FromMValue(IMValueReader reader)
        {
            if (reader.Peek() == MValueReaderToken.Nil) return null!;

            long id = default;
            string displayname = string.Empty;
            string vehiclename = string.Empty;
            string category = string.Empty;
            int price = default;
            reader.BeginObject();
            while (reader.HasNext())
            {
                switch (reader.NextName())
                {
                    case "id":
                        {
                            id = reader.NextLong();
                            break;
                        }
                    case "displayname":
                        {
                            displayname = reader.NextString();
                            break;
                        }
                    case "vehiclename":
                        {
                            vehiclename = reader.NextString();
                            break;
                        }
                    case "price":
                        {
                            price = reader.NextInt();
                            break;
                        }
                    case "category":
                        {
                            category = reader.NextString();
                            break;
                        }
                    default:
                        {
                            reader.SkipValue();
                            break;
                        }
                }
            }
            reader.EndObject();
            return new(id, displayname, vehiclename, price, category);
        }

        public void ToMValue(SharedShopItem value, IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("id");
            writer.Value(value.Id);
            writer.Name("displayname");
            writer.Value(value.Displayname);
            writer.Name("vehiclename");
            writer.Value(value.Vehiclename);
            writer.Name("price");
            writer.Value(value.Price);
            writer.Name("category");
            writer.Value(value.Category);
            writer.EndObject();
        }

        public void ToMValue(object obj, IMValueWriter writer)
        {
            if (obj is SharedShopItem value)
            {
                ToMValue(value, writer);
            }
        }

        object IMValueBaseAdapter.FromMValue(IMValueReader reader)
        {
            return FromMValue(reader);
        }
    }
}
