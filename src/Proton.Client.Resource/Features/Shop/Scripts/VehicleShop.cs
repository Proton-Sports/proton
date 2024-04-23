using AltV.Net.Client;
using AltV.Net.Client.Elements.Data;
using AltV.Net.Client.Elements.Interfaces;
using Proton.Client.Infrastructure.Interfaces;
using Proton.Client.Infrastructure.Services;
using Proton.Shared.Contants;
using Proton.Shared.Helpers;
using Proton.Shared.Interfaces;
using Proton.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Proton.Client.Resource.Features.Shop
{
    internal class VehicleShop : IStartup
    {
        private readonly IUiView uiView;
        private readonly NotificationService notification;
        private ILocalVehicle? previewVehicle;
        private bool IsUiOpen = false;

        private Dictionary<string, List<SharedShopItem>> shopItemsSorted = new Dictionary<string, List<SharedShopItem>>();
        private Dictionary<string, List<SharedShopItem>> ownedItemsSorted = new Dictionary<string, List<SharedShopItem>>();
        private List<SharedShopItem> shopItems = new List<SharedShopItem>();
        private List<SharedShopItem> ownedItems = new List<SharedShopItem>();

        public VehicleShop(IUiView uiView, NotificationService notification)
        {
            this.uiView = uiView;
            this.notification = notification;
            Alt.OnClient("authentication:done", RequestShopData);

            Alt.OnServer<int, List<SharedShopItem>>("shop:vehicle:all", (state, items)
                => ReceiveShopData(state, items, ref shopItems, "notOwnedVehicles", ref shopItemsSorted));
            Alt.OnServer<int, List<SharedShopItem>>("shop:vehicle:owned", (state, items)
                => ReceiveShopData(state, items, ref ownedItems, "ownedVehicles", ref ownedItemsSorted));
            Alt.OnServer<int>("shop:vehicle:purchase", (state) => PurchaseResponse((ShopStatus)state));

            this.uiView.On<string>("shop:select:vehicle", (displayName) => CreatePreviewVehicle(displayName));
            this.uiView.On<int>("shop:vehicles:choosenColor", (color) => SetPreviewVehicleColor(color));
            this.uiView.On<string, int>("shop:vehicles:buyVehicle", (displayName, color) => BuyVehicle(displayName, color));
            this.uiView.On("shop:vehicles:ready", UiReady);

            Alt.OnKeyUp += Alt_OnKeyUp;
        }

        private void Alt_OnKeyUp(Key key)
        {
            //If the ui is Open and Escape is pressed close the shop
            if (IsUiOpen && key == Key.Escape)
            {
                RemovePreview();
                ToggleUi();
            }

            if(key == Key.O)
            {
                ToggleUi();
            }
        }

        private void ToggleUi()
        {
            IsUiOpen = !IsUiOpen;
            if (IsUiOpen)
            {
                RequestShopData();
                uiView.Mount(Route.VehicleShop);
            }
            else
            {
                uiView.Emit("shop:vehicles:menuStatus", false);
                uiView.Unmount(Route.VehicleShop);
                RemovePreview();
            }

            uiView.Focus();
            Alt.GameControlsEnabled = !IsUiOpen;
            Alt.ShowCursor(IsUiOpen);
            Alt.SetConfigFlag("DISABLE_IDLE_CAMERA", IsUiOpen);
        }

        private void UiReady()
        {
            uiView.Emit("shop:vehicles:notOwnedVehicles", JsonSerializer.Serialize(shopItemsSorted));
            uiView.Emit("shop:vehicles:ownedVehicles", JsonSerializer.Serialize(ownedItemsSorted));
        }

        #region Shop data
        private void RequestShopData()
        {
            Alt.EmitServer("shop:vehicle:all");
            Alt.EmitServer("shop:vehicle:owned");
        }

        private void ReceiveShopData(int state,
            List<SharedShopItem> items,
            ref List<SharedShopItem> list,
            string type,
            ref Dictionary<string, List<SharedShopItem>> sortList)
        {
            Alt.Log(type + " " + state.ToString());
            list.Clear();
            list.AddRange(items);

            SortItems(items, ref sortList);
        }

        private void SortItems(List<SharedShopItem> items, ref Dictionary<string, List<SharedShopItem>> shop)
        {
            var uniqe = items.GroupBy(x => x.Category).ToList().Distinct();
            shop.Clear();

            foreach (var u in uniqe)
                shop.Add(u.Key, u.ToList());
        }
        #endregion

        private void BuyVehicle(string Name, int color)
        {
            Alt.EmitServer("shop:vehicle:purchase", Name, color);
        }

        private void PurchaseResponse(ShopStatus state)
        {
            Alt.Log(state.ToString());
            switch (state)
            {
                case ShopStatus.OK:
                    Alt.EmitServer("shop:items");
                    Alt.EmitServer("shop:items:owned");
                    ToggleUi();
                    RemovePreview();

                    notification.DrawNotification(Image: "CHAR_LS_CUSTOMS", Header: "Vehicle Shop", Details: "OK", Message: "Vehicle bought!");
                    break;
                case ShopStatus.NO_MONEY:
                    notification.DrawNotification(Image: "CHAR_LS_CUSTOMS", Header: "Vehicle Shop", Details: "Error", Message: "You dont have enough money to buy this Vehicle!");
                    break;
                case ShopStatus.ITEM_NOT_FOUND:
                default:
                    notification.DrawNotification(Image: "CHAR_LS_CUSTOMS", Header: "Vehicle Shop", Details: "Error", Message:"The Requested vehicle was not found!");
                    break;
            }
        }

        #region Preview vehicle handlers
        private void CreatePreviewVehicle(string Name)
        {
            if (previewVehicle != null)
                RemovePreview();

            var pos = Alt.LocalPlayer.Position;
            pos.X += 2;
            pos.Y += 2;
            pos.Z -= 0.5f;

            previewVehicle = Alt.CreateLocalVehicle(Alt.Hash(Name),
                Alt.LocalPlayer.Dimension, pos, Alt.LocalPlayer.Rotation, true, 20);
        }

        private void SetPreviewVehicleColor(int Color)
        {
            if (previewVehicle != null)
                Alt.Natives.SetVehicleColours(previewVehicle, Color, Color);
        }

        private void RemovePreview()
        {
            if (previewVehicle == null) return;

            previewVehicle.Destroy();
            previewVehicle = null;
        }
        #endregion
    }
}
