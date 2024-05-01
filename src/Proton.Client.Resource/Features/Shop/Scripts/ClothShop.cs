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
    internal class ClothShop : IStartup
    {
        private readonly IUiView uiView;
        private readonly NotificationService notification;
        private bool IsUiOpen = false;

        private Dictionary<string, List<SharedClothShopItem>> shopItemsSorted = new Dictionary<string, List<SharedClothShopItem>>();
        private Dictionary<string, List<SharedClothShopItem>> ownedItemsSorted = new Dictionary<string, List<SharedClothShopItem>>();
        private List<SharedClothShopItem> shopItems = new List<SharedClothShopItem>();
        private List<SharedClothShopItem> ownedItems = new List<SharedClothShopItem>();

        public ClothShop(IUiView uiView, NotificationService notification)
        {
            this.uiView = uiView;
            this.notification = notification;
            Alt.OnClient("authentication:done", RequestShopData);

            Alt.OnServer<int, List<SharedClothShopItem>>("shop:cloth:all", (state, items)
                => ReceiveShopData(state, items, ref shopItems, "notOwnedClothes", ref shopItemsSorted));
            Alt.OnServer<int, List<SharedClothShopItem>>("shop:cloth:owned", (state, items)
                => ReceiveShopData(state, items, ref ownedItems, "ownedClothes", ref ownedItemsSorted));
            Alt.OnServer<int>("shop:cloth:purchase", (state) => PurchaseResponse((ShopStatus)state));

            //this.uiView.On<long>("shop:cloth:wearItem", (id) => CreatePreviewVehicle(displayName));
            this.uiView.On<long>("shop:cloth:buyItem", (id) => BuyCloth(id));
            this.uiView.On("shop:cloth:ready", UiReady);
            this.uiView.On<long>("shop:cloth:showcase", (id) => Alt.EmitServer("shop:cloth:showroom", id));
            this.uiView.On<bool, long>("shop:cloth:equip", (state, id) => Alt.EmitServer("shop:cloth:equip",state, id));

            Alt.OnKeyUp += Alt_OnKeyUp;
        }

        private void Alt_OnKeyUp(Key key)
        {
            //If the ui is Open and Escape is pressed close the shop
            if (IsUiOpen && key == Key.Escape)
            {
                //RemovePreview();
                ToggleUi();
            }

            if(key == Key.N)
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
                uiView.Mount(Route.ClothShop);
            }
            else
            {
                uiView.Emit("shop:cloth:menuStatus", false);
                uiView.Unmount(Route.ClothShop);
                Alt.EmitServer("shop:cloth:clear");
                //RemovePreview();
            }

            uiView.Focus();
            Alt.GameControlsEnabled = !IsUiOpen;
            Alt.ShowCursor(IsUiOpen);
            Alt.SetConfigFlag("DISABLE_IDLE_CAMERA", IsUiOpen);
        }

        private void UiReady()
        {
            Alt.Log(JsonSerializer.Serialize(shopItemsSorted));
            Alt.Log(JsonSerializer.Serialize(ownedItemsSorted));
            uiView.Emit("shop:cloth:notOwnedClothes", JsonSerializer.Serialize(shopItemsSorted));
            uiView.Emit("shop:cloth:ownedClothes", JsonSerializer.Serialize(ownedItemsSorted));
        }

        #region Shop data
        private void RequestShopData()
        {
            Alt.EmitServer("shop:cloth:all");
            Alt.EmitServer("shop:cloth:owned");
        }

        private void ReceiveShopData(int state,
            List<SharedClothShopItem> items,
            ref List<SharedClothShopItem> list,
            string type,
            ref Dictionary<string, List<SharedClothShopItem>> sortList)
        {
            Alt.Log(type + " " + state.ToString());
            list.Clear();
            list.AddRange(items);

            SortItems(items, ref sortList);

            uiView.Emit($"shop:cloth:{type}", JsonSerializer.Serialize(sortList));
        }

        private void SortItems(List<SharedClothShopItem> items, ref Dictionary<string, List<SharedClothShopItem>> shop)
        {
            var uniqe = items.GroupBy(x => x.Category).ToList().Distinct();
            shop.Clear();

            foreach (var u in uniqe)
                shop.Add(u.Key, u.ToList());
        }
        #endregion

        private void BuyCloth(long Id)
        {
            Alt.EmitServer("shop:cloth:purchase", Id);
        }

        private void PurchaseResponse(ShopStatus state)
        {
            switch (state)
            {
                case ShopStatus.OK:
                    RequestShopData();
                    ToggleUi();
                    //RemovePreview();
                    //TODO: Replace CHAR_BOATSITE2

                    notification.DrawNotification(Image: "CHAR_BOATSITE2", Header: "Vehicle Cloth", Details: "OK", Message: "Cloth bought!");
                    break;
                case ShopStatus.NO_MONEY:
                    notification.DrawNotification(Image: "CHAR_BOATSITE2", Header: "Vehicle Cloth", Details: "Error", Message: "You dont have enough money to buy this Vehicle!");
                    break;
                case ShopStatus.ITEM_NOT_FOUND:
                default:
                    notification.DrawNotification(Image: "CHAR_BOATSITE2", Header: "Cloth Shop", Details: "Error", Message:"The Requested cloth was not found!");
                    break;
            }
        }

        #region Preview cloth handlers

        #endregion
    }
}
