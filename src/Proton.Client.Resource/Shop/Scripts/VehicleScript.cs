using AltV.Net.Client;
using AltV.Net.Client.Elements.Data;
using AltV.Net.Client.Elements.Entities;
using AltV.Net.Client.Elements.Interfaces;
using Proton.Client.Infrastructure.Interfaces;
using Proton.Client.Infrastructure.Services;
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
        //private readonly NotificationService notificationService;
        private List<SharedShopItem> shopItems = new List<SharedShopItem>();
        private List<SharedShopItem> ownedItems = new List<SharedShopItem>();

        private Dictionary<string, List<SharedShopItem>> shopItemsSorted = new Dictionary<string, List<SharedShopItem>>();
        private Dictionary<string, List<SharedShopItem>> ownedItemsSorted = new Dictionary<string, List<SharedShopItem>>();

        public VehicleScript(IUiView uiView/*, NotificationService notificationService*/)
        {
            this.uiView = uiView;
            //this.notificationService = notificationService;
            this.uiView.On<string>("shop:select:vehicle", 
                (displayName) => SetVehiclePreview(displayName));
            this.uiView.On<int>("shop:vehicles:choosenColor",
                (color) => Alt.EmitServer("shop:vehicle:changeColor", color)); 

            this.uiView.On<string, int>("shop:vehicles:buyVehicle",
                (name, color) => Alt.EmitServer("shop:purchase", name, color));

            Alt.OnServer<int, List<SharedShopItem>>("shop:items",
                (state, items) => SetShopItems(state, items, ref shopItems, "notOwnedVehicles", ref shopItemsSorted));
            Alt.OnServer<int , List<SharedShopItem>>("shop:items:owned",
                (state, items) => SetShopItems(state, items, ref ownedItems, "ownedVehicles", ref ownedItemsSorted));

            Alt.OnServer<int>("shop:purchase", (state) => PurchaseResponse((ShopStatus)state));
            Alt.OnClient("authentication:done", Init);

            Alt.OnKeyDown += Alt_OnKeyDown;
            Alt.OnConsoleCommand += Alt_OnConsoleCommand;
            
            uiView.Mount(Route.VehicleShop);
        }

        private void Init()
        {
            Alt.EmitServer("shop:items:owned");
            Alt.EmitServer("shop:items");            
        }

        private void Alt_OnKeyDown(Key key)
        {
            if(key == Key.Escape)
            {
                ToggleUi(false);
            }
        }

        private void PurchaseResponse(ShopStatus state)
        {
            Console.WriteLine("Received State: " + state);
            switch (state)
            {
                case ShopStatus.OK:
                    Alt.EmitServer("shop:items");
                    Alt.EmitServer("shop:items:owned");
                    Alt.EmitServer("shop:vehicle:showroom:termiante");
                    ToggleUi(false);

                    //notificationService.DrawNotification(Header: "Vehicle Shop", Details: "OK", Message: "Vehicle bought!");
                    break;
                case ShopStatus.NO_MONEY:
                    //notificationService.DrawNotification(Header: "Vehicle Shop", Details: "Error", Message: "You dont have enough money to buy this Vehicle!");
                    break;
                case ShopStatus.ITEM_NOT_FOUND:
                default:
                    //notificationService.DrawNotification(Header: "Vehicle Shop", Details: "Error", Message:"The Requested vehicle was not found!");
                    break;
            }
        }

        private void Alt_OnConsoleCommand(string name, string[] args)
        {
            if (name == "vehtest"){
                Console.WriteLine($"NotOwned: {shopItems.Count}");
                Console.WriteLine($"Owned: {ownedItems.Count}");
                foreach (var item in shopItems)
                {
                    Console.WriteLine(item.Displayname);
                }
                ToggleUi(To: true);
            }

            if(name == "veh")
            {
                Alt.EmitServer("veh", "bati2");
                Alt.OnServer("vehid", (int id) =>
                {
                    Console.WriteLine(id);
                    var veh = Alt.GetAllVehicles().Where(x => x.ScriptId == id).FirstOrDefault();
                    
                    try
                    {
                        Console.WriteLine(veh.Model);
                        var output = Alt.CreateAudioOutputAttached(Alt.Hash("frontend_radio"), Alt.LocalPlayer);
                        var audio = Alt.CreateAudio("https://upload.wikimedia.org/wikipedia/commons/c/c8/Example.ogg", 100);
                        audio.AddOutput(output);
                        audio.Play();
                    }catch(Exception ex)
                    {
                        Console.WriteLine(ex);
                    }

                    

                    Alt.Natives.SetVehicleModColor1(veh, 1, 76, 0);
                    Alt.Natives.SetVehicleModColor2(veh, 1, 5);

                    Console.WriteLine(Alt.Natives.GetVehicleLiveryCount(veh));
                    Console.WriteLine(Alt.Natives.GetVehicleLivery2Count(veh));
                });
                
            }

            
        }

        private void SetShopItems(int state, List<SharedShopItem> items, 
            ref List<SharedShopItem> list,
            string type, ref Dictionary<string, List<SharedShopItem>> sortList)
        {
            
            list.Clear();
            list.AddRange(items);

            Sort(items, ref sortList);

            Console.WriteLine($"State: {state}, count: {items.Count}, type: {type}, path: 'shop:vehicles:{type}', Data: {JsonSerializer.Serialize(sortList)}");
            
        }

        private void Sort(List<SharedShopItem> items, ref Dictionary<string, List<SharedShopItem>> shop)
        {
            var uniqe = items.GroupBy(x => x.Category).ToList().Distinct();
            shop.Clear();

            foreach(var u in uniqe)
                shop.Add(u.Key, u.ToList());
        }

        /// <param name="To">TRUE = Show UI, FALSE = Hide UI</param>
        private void ToggleUi(bool To)
        {
            //uiView.Mount(Route.VehicleShop);
            //Init();
            uiView.Focus();
            Alt.GameControlsEnabled = !To;
            Alt.ShowCursor(To);
            uiView.Emit("shop:vehicles:menuStatus", To);
            uiView.Emit("shop:vehicles:notOwnedVehicles", JsonSerializer.Serialize(shopItemsSorted));
            uiView.Emit("shop:vehicles:ownedVehicles", JsonSerializer.Serialize(ownedItemsSorted));
            //uiView.Visible = To;
            Alt.SetConfigFlag("DISABLE_IDLE_CAMERA", To);

            if (!To)
            {
                Alt.EmitServer("shop:vehicle:showroom:termiante");
            }
        }

        private void SetVehiclePreview(string name)
        {
            Console.WriteLine(name);
            Alt.EmitServer("shop:vehicle:showroom", name);
        }
    }
}
