using AltV.Net.Client;
using AltV.Net.Client.Elements.Data;
using AltV.Net.Client.Elements.Interfaces;
using AltV.Net.Data;
using AltV.Net.Shared.Enums;
using Proton.Client.Resource.Commons;
using Proton.Client.Resource.Features.UiViews.Abstractions;
using Proton.Client.Resource.Notifications.Abstractions;
using Proton.Shared.Constants;
using Proton.Shared.Dtos;
using Proton.Shared.Helpers;

namespace Proton.Client.Resource.Features.Shop.Scripts;

public sealed class VehicleShopScript(IUiView uiView, INotificationService notification) : HostedService
{
    ILocalVehicle? previewVehicle;
    Position purchasePosition = new(443.94177f, 5605.972f, -96.5f);

    public override Task StartAsync(CancellationToken ct)
    {
        Alt.OnServer<int>("shop:vehicle:purchase", (state) => PurchaseResponse((ShopStatus)state));

        uiView.On<string>("shop:select:vehicle", CreatePreviewVehicle);
        uiView.On<int>("shop:vehicles:choosenColor", SetPreviewVehicleColor);
        uiView.On<string, int>("shop:vehicles:buyVehicle", BuyVehicle);

        Alt.OnKeyUp += Alt_OnKeyUp;
        Alt.CreateMarker(MarkerType.MarkerMoney, purchasePosition, new Rgba(255, 0, 0, 255), true, 128);
        Alt.OnServer<VehicleShopMountDto>("vehicle-shop.mount", OnServerMount);
        uiView.OnMount(Route.VehicleShop, OnUiMount);
        uiView.OnUnmount(Route.VehicleShop, OnUiUnmount);
        return Task.CompletedTask;
    }

    void OnServerMount(VehicleShopMountDto dto)
    {
        uiView.Mount(Route.VehicleShop, dto);
    }

    void OnUiMount()
    {
        Alt.GameControlsEnabled = false;
        Alt.ShowCursor(true);
        uiView.Focus();
        Alt.SetConfigFlag("DISABLE_IDLE_CAMERA", true);
    }

    void OnUiUnmount()
    {
        Alt.GameControlsEnabled = true;
        Alt.ShowCursor(false);
        uiView.Unfocus();
        RemovePreview();
        Alt.SetConfigFlag("DISABLE_IDLE_CAMERA", false);
    }

    void Alt_OnKeyUp(Key key)
    {
        if (Alt.IsConsoleOpen)
        {
            return;
        }
        switch (key)
        {
            case Key.E:
                if (
                    !uiView.IsMounted(Route.VehicleShop)
                    && Alt.LocalPlayer.Position.GetDistanceSquaredTo(purchasePosition) < 9
                )
                {
                    Alt.EmitServer("vehicle-shop.mount");
                }
                break;

            case Key.Escape:
                if (uiView.IsMounted(Route.VehicleShop))
                {
                    uiView.Emit("shop:vehicles:menuStatus", false);
                    uiView.Unmount(Route.VehicleShop);
                }
                break;
        }
    }

    void BuyVehicle(string Name, int color)
    {
        Alt.EmitServer("shop:vehicle:purchase", Name, color);
    }

    void PurchaseResponse(ShopStatus state)
    {
        switch (state)
        {
            case ShopStatus.OK:
                uiView.Unmount(Route.VehicleShop);

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
    void CreatePreviewVehicle(string Name)
    {
        if (previewVehicle != null)
        {
            RemovePreview();
        }

        var pos = new Position(461.0691f, 5611.657f, -95.80398f);
        var rot = new Rotation(0, 0, -2f);

        previewVehicle = Alt.CreateLocalVehicle(Alt.Hash(Name), Alt.LocalPlayer.Dimension, pos, rot, true, 20);
    }

    void SetPreviewVehicleColor(int Color)
    {
        if (previewVehicle != null)
        {
            Alt.Natives.SetVehicleColours(previewVehicle, Color, Color);
        }
    }

    void RemovePreview()
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
