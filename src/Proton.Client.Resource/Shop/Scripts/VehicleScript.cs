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
using System.Threading.Tasks;

namespace Proton.Client.Resource.Shop.Scripts
{
    public class VehicleScript : IStartup
    {
        private readonly IUiView uiView;
        private List<SharedShopItem> shopItems = new List<SharedShopItem>();
        private List<SharedShopItem> ownedItems = new List<SharedShopItem>();

        private ILocalVehicle? previewVehicle;

        public VehicleScript(IUiView uiView)
        {
            this.uiView = uiView;

            this.uiView.On<string>("shop:select:vehicle", 
                (displayName) => SetVehiclePreview(displayName));
            Alt.OnServer<List<SharedShopItem>>("shop:items",
                (items) => GetShopItems(items, ref shopItems, "notOwned"));
            Alt.OnServer<List<SharedShopItem>>("shop:items:owned",
                (items) => GetShopItems(items, ref ownedItems, "owned"));

            Alt.EmitServer("shop:items");
            Alt.EmitServer("shop:items:owned");

            Alt.OnConsoleCommand += Alt_OnConsoleCommand;
        }

        private void Alt_OnConsoleCommand(string name, string[] args)
        {
            Console.WriteLine(name);
            if (name != "vehtest") return;

            uiView.Emit("shop:vehicles:menuStatus", false);
            uiView.Mount(Route.VehicleShop);
            Alt.GameControlsEnabled = false;
            Alt.ShowCursor(true);
        }

        private void GetShopItems(List<SharedShopItem> items, 
            ref List<SharedShopItem> list,
            string type)
        {
            Console.WriteLine(items.Count);
            list.Clear();
            list.AddRange(items);

            uiView.Emit($"shop:vehicles:{type}", list);
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
