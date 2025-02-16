using AltV.Net.Client;
using AltV.Net.Client.Elements.Data;
using AltV.Net.Data;
using AltV.Net.Shared.Enums;
using Proton.Client.Resource.Commons;
using Proton.Client.Resource.Features.UiViews.Abstractions;
using Proton.Client.Resource.Notifications.Abstractions;
using Proton.Shared.Constants;
using Proton.Shared.Dtos;
using Proton.Shared.Helpers;

namespace Proton.Client.Resource.Features.Shop.Scripts;

public sealed class ClothShopScript(IUiView uiView, INotificationService notification) : HostedService
{
    bool isUiOpen;
    Position purchasePosition = new Position(547.337f, 5515.422f, -90.64761f);

    public override Task StartAsync(CancellationToken ct)
    {
        Alt.OnServer<ClothShopMountDto>("cloth-shop.mount", OnServerMount);
        Alt.OnServer<int>("shop:cloth:purchase", (state) => PurchaseResponse((ShopStatus)state));

        uiView.On<long>("shop:cloth:buyItem", BuyCloth);
        uiView.On<long>("shop:cloth:showcase", (id) => Alt.EmitServer("shop:cloth:showroom", id));
        uiView.On<bool, long>("shop:cloth:equip", (state, id) => Alt.EmitServer("shop:cloth:equip", state, id));

        Alt.OnKeyUp += OnKeyUp;
        Alt.CreateMarker(MarkerType.MarkerMoney, purchasePosition, new Rgba(255, 0, 0, 128), true, 64);
        uiView.OnMount(Route.ClothShop, OnUiMount);
        uiView.OnUnmount(Route.ClothShop, OnUiUnmount);
        return Task.CompletedTask;
    }

    void OnServerMount(ClothShopMountDto dto)
    {
        uiView.Mount(Route.ClothShop, dto);
    }

    void OnUiMount()
    {
        Alt.OnKeyUp -= OnKeyUp;
        Alt.OnKeyUp += UiMountedOnKeyUp;
        uiView.Focus();
        Alt.GameControlsEnabled = false;
        Alt.ShowCursor(true);
        Alt.SetConfigFlag("DISABLE_IDLE_CAMERA", isUiOpen);
    }

    void OnUiUnmount()
    {
        Alt.OnKeyUp += OnKeyUp;
        Alt.OnKeyUp -= UiMountedOnKeyUp;
        uiView.Emit("shop:cloth:menuStatus", false);
        uiView.Unfocus();
        Alt.GameControlsEnabled = true;
        Alt.ShowCursor(false);
        Alt.SetConfigFlag("DISABLE_IDLE_CAMERA", isUiOpen);
    }

    void UiMountedOnKeyUp(Key key)
    {
        if (Alt.IsConsoleOpen)
        {
            return;
        }

        if (key == Key.Escape)
        {
            uiView.Unmount(Route.ClothShop);
        }
    }

    void OnKeyUp(Key key)
    {
        if (Alt.IsConsoleOpen)
        {
            return;
        }

        if (Alt.LocalPlayer.Position.GetDistanceSquaredTo(purchasePosition) <= 16f)
        {
            Alt.EmitServer("cloth-shop.mount");
        }
    }

    void BuyCloth(long Id)
    {
        Alt.EmitServer("shop:cloth:purchase", Id);
    }

    void PurchaseResponse(ShopStatus state)
    {
        switch (state)
        {
            case ShopStatus.OK:
                uiView.Unmount(Route.ClothShop);
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
}
