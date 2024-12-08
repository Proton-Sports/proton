using System.Text.Json;
using AltV.Net.Client;
using AltV.Net.Client.Elements.Data;
using AltV.Net.Client.Elements.Interfaces;
using AltV.Net.Data;
using AltV.Net.Shared.Enums;
using Proton.Client.Resource.Features.UiViews.Abstractions;
using Proton.Client.Resource.Notifications.Abstractions;
using Proton.Shared.Constants;
using Proton.Shared.Helpers;
using Proton.Shared.Interfaces;
using Proton.Shared.Models;

namespace Proton.Client.Resource.Features.Shop.Scripts;

internal class VehicleShop : IStartup
{
    private readonly IUiView uiView;
    private readonly INotificationService notification;
    private ILocalVehicle? previewVehicle;
    private bool isUiOpen;

    private Dictionary<string, List<SharedShopItem>> shopItemsSorted = [];
    private Dictionary<string, List<SharedShopItem>> ownedItemsSorted = [];
    private List<SharedShopItem> shopItems = [];
    private List<SharedShopItem> ownedItems = [];

    public VehicleShop(IUiView uiView, INotificationService notification)
    {
        this.uiView = uiView;
        this.notification = notification;
        Alt.OnClient("authentication:done", RequestShopData);

        Alt.OnServer<int, List<SharedShopItem>>(
            "shop:vehicle:all",
            (state, items) => ReceiveShopData(state, items, ref shopItems, "notOwnedVehicles", ref shopItemsSorted)
        );
        Alt.OnServer<int, List<SharedShopItem>>(
            "shop:vehicle:owned",
            (state, items) => ReceiveShopData(state, items, ref ownedItems, "ownedVehicles", ref ownedItemsSorted)
        );
        Alt.OnServer<int>("shop:vehicle:purchase", (state) => PurchaseResponse((ShopStatus)state));

        this.uiView.On<string>("shop:select:vehicle", CreatePreviewVehicle);
        this.uiView.On<int>("shop:vehicles:choosenColor", SetPreviewVehicleColor);
        this.uiView.On<string, int>("shop:vehicles:buyVehicle", BuyVehicle);
        this.uiView.On("shop:vehicles:ready", UiReady);

        Alt.OnKeyUp += Alt_OnKeyUp;

        var c = Alt.CreateColShapeSphere(new Position(443.94177f, 5605.972f, -96.5f), 3f);
        var m = Alt.CreateMarker(
            MarkerType.MarkerMoney,
            new Position(443.94177f, 5605.972f, -96.5f),
            new Rgba(255, 0, 0, 255),
            true,
            128
        );
        m.Visible = true;

        Alt.OnColShape += (colShape, target, state) =>
        {
            Console.WriteLine($"OnColShape {target.Position.X} {target.Position.Y} {target.Position.Z}");
            if (!state && colShape != c || target != Alt.LocalPlayer)
            {
                return;
            }
            ToggleUi();
        };
    }

    private void Alt_OnKeyUp(Key key)
    {
        //If the ui is Open and Escape is pressed close the shop
        if (isUiOpen && !Alt.IsConsoleOpen && key == Key.Escape)
        {
            RemovePreview();
            ToggleUi();
        }
    }

    private void ToggleUi()
    {
        isUiOpen = !isUiOpen;
        if (isUiOpen)
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
        Alt.GameControlsEnabled = !isUiOpen;
        Alt.ShowCursor(isUiOpen);
        Alt.SetConfigFlag("DISABLE_IDLE_CAMERA", isUiOpen);
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

    private static void ReceiveShopData(
        int state,
        List<SharedShopItem> items,
        ref List<SharedShopItem> list,
        string type,
        ref Dictionary<string, List<SharedShopItem>> sortList
    )
    {
        Alt.Log(type + " " + state.ToString());
        list.Clear();
        list.AddRange(items);

        SortItems(items, ref sortList);
    }

    private static void SortItems(List<SharedShopItem> items, ref Dictionary<string, List<SharedShopItem>> shop)
    {
        var uniqe = items.GroupBy(x => x.Category).ToList().Distinct();
        shop.Clear();

        foreach (var u in uniqe)
        {
            shop.Add(u.Key, u.ToList());
        }
    }
    #endregion

    private void BuyVehicle(string Name, int color)
    {
        Alt.EmitServer("shop:vehicle:purchase", Name, color);
    }

    private void PurchaseResponse(ShopStatus state)
    {
        switch (state)
        {
            case ShopStatus.OK:
                RequestShopData();
                ToggleUi();
                RemovePreview();

                notification.DrawNotification(
                    image: "CHAR_LS_CUSTOMS",
                    header: "Vehicle Shop",
                    details: "OK",
                    message: "Vehicle bought!"
                );
                break;
            case ShopStatus.NO_MONEY:
                notification.DrawNotification(
                    image: "CHAR_LS_CUSTOMS",
                    header: "Vehicle Shop",
                    details: "Error",
                    message: "You dont have enough money to buy this Vehicle!"
                );
                break;
            case ShopStatus.ITEM_NOT_FOUND:
            default:
                notification.DrawNotification(
                    image: "CHAR_LS_CUSTOMS",
                    header: "Vehicle Shop",
                    details: "Error",
                    message: "The Requested vehicle was not found!"
                );
                break;
        }
    }

    #region Preview vehicle handlers
    private void CreatePreviewVehicle(string Name)
    {
        if (previewVehicle != null)
            RemovePreview();

        var pos = new AltV.Net.Data.Position(439f, 5611f, -80.04041f);
        var rot = new AltV.Net.Data.Rotation(0, 0, -2f);

        previewVehicle = Alt.CreateLocalVehicle(Alt.Hash(Name), Alt.LocalPlayer.Dimension, pos, rot, true, 20);
    }

    private void SetPreviewVehicleColor(int Color)
    {
        if (previewVehicle != null)
        {
            Alt.Natives.SetVehicleColours(previewVehicle, Color, Color);
        }
    }

    private void RemovePreview()
    {
        if (previewVehicle == null)
        {
            return;
        }

        previewVehicle.Destroy();
        previewVehicle = null;
    }
    #endregion
}
