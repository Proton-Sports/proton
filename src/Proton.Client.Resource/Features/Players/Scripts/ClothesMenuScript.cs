using AltV.Net.Client;
using AltV.Net.Client.Elements.Data;
using Proton.Client.Resource.Commons;
using Proton.Client.Resource.Features.UiViews.Abstractions;
using Proton.Shared.Contants;
using Proton.Shared.Dtos;

namespace Proton.Client.Resource.Features.Players.Scripts;

public sealed class ClothesMenuScript(IUiView ui) : HostedService
{
    public override Task StartAsync(CancellationToken ct)
    {
        Alt.OnServer<ClothesMenuMountDto>("clothes-menu.mount", OnServerMount);
        ui.OnMount(Route.ClothesMenu, OnUiMount);
        ui.OnUnmount(Route.ClothesMenu, OnUiUnmount);
        ui.On<long, string>("clothes-menu.option.change", OnUiOptionChange);
        Alt.OnServer<long, string>("clothes-menu.option.change", OnServerOptionChange);
        return Task.CompletedTask;
    }

    private void OnServerMount(ClothesMenuMountDto dto)
    {
        ui.Mount(Route.ClothesMenu, dto);
    }

    private void OnUiMount()
    {
        Alt.ShowCursor(true);
        Alt.GameControlsEnabled = false;
        ui.Focus();
        Alt.OnKeyUp += OnKeyUp;
    }

    private void OnUiUnmount()
    {
        Alt.ShowCursor(false);
        Alt.GameControlsEnabled = true;
        ui.Unfocus();
        Alt.OnKeyUp -= OnKeyUp;
    }

    private void OnUiOptionChange(long closestId, string value)
    {
        if (!value.Equals("equip") && !value.Equals("unequip"))
        {
            return;
        }

        Alt.EmitServer("clothes-menu.option.change", closestId, value);
    }

    private void OnServerOptionChange(long closestId, string value)
    {
        ui.Emit("clothes-menu.option.change", closestId, value);
    }

    private void OnKeyUp(Key key)
    {
        if (key == Key.Escape)
        {
            ui.Unmount(Route.ClothesMenu);
            ui.Mount(Route.RaceMainMenuList);
            return;
        }
    }
}
