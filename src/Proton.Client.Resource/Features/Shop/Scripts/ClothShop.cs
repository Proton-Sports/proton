using System.Text.Json;
using AltV.Net.Client;
using AltV.Net.Client.Elements.Data;
using Proton.Client.Resource.Features.UiViews.Abstractions;
using Proton.Client.Resource.Notifications.Abstractions;
using Proton.Shared.Constants;
using Proton.Shared.Helpers;
using Proton.Shared.Interfaces;
using Proton.Shared.Models;

namespace Proton.Client.Resource.Features.Shop.Scripts;

internal class ClothShop : IStartup
{
    private readonly IUiView uiView;
    private readonly INotificationService notification;
    private bool isUiOpen;

    private Dictionary<string, List<SharedClothShopItem>> shopItemsSorted = [];
    private Dictionary<string, List<SharedClothShopItem>> ownedItemsSorted = [];
    private List<SharedClothShopItem> shopItems = [];
    private List<SharedClothShopItem> ownedItems = [];

    public ClothShop(IUiView uiView, INotificationService notification)
    {
        this.uiView = uiView;
        this.notification = notification;
        Alt.OnClient("authentication:done", RequestShopData);

        Alt.OnServer<int, List<SharedClothShopItem>>(
            "shop:cloth:all",
            (state, items) => ReceiveShopData(state, items, ref shopItems, "notOwnedClothes", ref shopItemsSorted)
        );
        Alt.OnServer<int, List<SharedClothShopItem>>(
            "shop:cloth:owned",
            (state, items) => ReceiveShopData(state, items, ref ownedItems, "ownedClothes", ref ownedItemsSorted)
        );
        Alt.OnServer<int>("shop:cloth:purchase", (state) => PurchaseResponse((ShopStatus)state));

        //this.uiView.On<long>("shop:cloth:wearItem", (id) => CreatePreviewVehicle(displayName));
        this.uiView.On<long>("shop:cloth:buyItem", BuyCloth);
        this.uiView.On("shop:cloth:ready", UiReady);
        this.uiView.On<long>("shop:cloth:showcase", (id) => Alt.EmitServer("shop:cloth:showroom", id));
        this.uiView.On<bool, long>("shop:cloth:equip", (state, id) => Alt.EmitServer("shop:cloth:equip", state, id));

        Alt.OnKeyUp += Alt_OnKeyUp;
    }

    private void Alt_OnKeyUp(Key key)
    {
        if (Alt.IsConsoleOpen)
        {
            return;
        }
    }

    private void ToggleUi()
    {
        isUiOpen = !isUiOpen;
        if (isUiOpen)
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
        Alt.GameControlsEnabled = !isUiOpen;
        Alt.ShowCursor(isUiOpen);
        Alt.SetConfigFlag("DISABLE_IDLE_CAMERA", isUiOpen);
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

    private void ReceiveShopData(
        int state,
        List<SharedClothShopItem> items,
        ref List<SharedClothShopItem> list,
        string type,
        ref Dictionary<string, List<SharedClothShopItem>> sortList
    )
    {
        Alt.Log(type + " " + state.ToString());
        list.Clear();
        list.AddRange(items);

        SortItems(items, ref sortList);

        uiView.Emit($"shop:cloth:{type}", JsonSerializer.Serialize(sortList));
    }

    private static void SortItems(
        List<SharedClothShopItem> items,
        ref Dictionary<string, List<SharedClothShopItem>> shop
    )
    {
        var uniqe = items.GroupBy(x => x.Category).ToList().Distinct();
        shop.Clear();

        foreach (var u in uniqe)
        {
            shop.Add(u.Key, [.. u]);
        }
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

                notification.DrawNotification(
                    image: "CHAR_BOATSITE2",
                    header: "Vehicle Cloth",
                    details: "OK",
                    message: "Cloth bought!"
                );
                break;
            case ShopStatus.NO_MONEY:
                notification.DrawNotification(
                    image: "CHAR_BOATSITE2",
                    header: "Vehicle Cloth",
                    details: "Error",
                    message: "You dont have enough money to buy this Vehicle!"
                );
                break;
            case ShopStatus.ITEM_NOT_FOUND:
            default:
                notification.DrawNotification(
                    image: "CHAR_BOATSITE2",
                    header: "Cloth Shop",
                    details: "Error",
                    message: "The Requested cloth was not found!"
                );
                break;
        }
    }

    #region Preview cloth handlers

    #endregion
}
