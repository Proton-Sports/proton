using AltV.Net.Client;
using AltV.Net.Client.Elements.Interfaces;
using Proton.Client.Infrastructure.Interfaces;
using Proton.Shared.Contants;
using Proton.Shared.Interfaces;
using Proton.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Proton.Client.Resource.Shop.Scripts
{
    public class VehicleScript : IStartup
    {
        private readonly IUiView uiView;
        private List<SharedShopItem> shopItems = new List<SharedShopItem>();
        private Dictionary<string, List<SharedShopItem>> shopItemsSorted = new Dictionary<string, List<SharedShopItem>>();
        private Dictionary<string, List<SharedShopItem>> ownedItemsSorted = new Dictionary<string, List<SharedShopItem>>();
        private List<SharedShopItem> ownedItems = new List<SharedShopItem>();

        private ILocalVehicle? previewVehicle;

        public VehicleScript(IUiView uiView)
        {
            this.uiView = uiView;

            this.uiView.On<string>("shop:select:vehicle", 
                (displayName) => SetVehiclePreview(displayName));
            Alt.OnServer<int, List<SharedShopItem>>("shop:items",
                (state, items) => GetShopItems(state, items, ref shopItems, "notOwned", ref shopItemsSorted));
            Alt.OnServer<int , List<SharedShopItem>>("shop:items:owned",
                (state, items) => GetShopItems(state, items, ref ownedItems, "owned", ref ownedItemsSorted));

            Alt.EmitServer("shop:items");
            Alt.EmitServer("shop:items:owned");

            Alt.OnConsoleCommand += Alt_OnConsoleCommand;

            uiView.Mount(Route.VehicleShop);
        }

        private void Alt_OnConsoleCommand(string name, string[] args)
        {
            Console.WriteLine(name);
            if (name != "vehtest") return;

            Console.WriteLine($"NotOwned: {shopItems.Count}");
            Console.WriteLine($"Owned: {ownedItems.Count}");
            foreach (var item in shopItems)
            {
                Console.WriteLine(item.Displayname);
            }
            MountUi();
        }

        private void GetShopItems(int state, List<SharedShopItem> items, 
            ref List<SharedShopItem> list,
            string type, ref Dictionary<string, List<SharedShopItem>> sortList)
        {
            
            list.Clear();
            list.AddRange(items);
            Sort(items, ref sortList);

            Console.WriteLine($"State: {state}, count: {items.Count}, type: {type}, path: 'shop:vehicles:{type}', Data: {JsonSerializer.Serialize(sortList)}");

            uiView.Emit($"shop:vehicles:{type}", JsonSerializer.Serialize(sortList));
        }

        private void Sort(List<SharedShopItem> items, ref Dictionary<string, List<SharedShopItem>> shop)
        {
            var uniqe = items.GroupBy(x => x.Category).ToList().Distinct();
            shop.Clear();

            foreach(var u in uniqe)
                shop.Add(u.Key, u.ToList());
        }

        private void MountUi()
        {   
            uiView.Focus();
            Alt.GameControlsEnabled = false;
            Alt.ShowCursor(true);
            uiView.Emit("shop:vehicles:menuStatus", false);
            uiView.Emit($"shop:vehicles:owned", JsonSerializer.Serialize(shopItemsSorted));
            uiView.Emit($"shop:vehicles:notOwned", JsonSerializer.Serialize(ownedItemsSorted));
            uiView.Visible = true;
        }

        private void SetVehiclePreview(string name)
        {
            var vehicle = shopItems.Where(x => x.Displayname == name).FirstOrDefault();
            if(vehicle == null)
            {
                //Todo: Print Error
                return;
            }

            if(previewVehicle != null)
            {
                previewVehicle.Destroy();
                previewVehicle = null;
            }

            previewVehicle = Alt.CreateLocalVehicle(Alt.Hash(vehicle.Vehiclename),
                Alt.LocalPlayer.Dimension, 
                Alt.LocalPlayer.AimPosition, 
                new AltV.Net.Data.Rotation(0,0,0),
                true, 10);

            Alt.Natives.SetVehicleEngineOn(previewVehicle, true, false, false);
        }

        private void ChangeVehicleColor(string color)
        {
            if (previewVehicle == null)
            {
                //Todo: Print Error
                return;
            }

        }
    }
}
