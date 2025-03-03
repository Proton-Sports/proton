using AltV.Net.Client;
using AltV.Net.Client.Elements.Data;
using Proton.Client.Resource.Commons;
using Proton.Client.Resource.Features.UiViews.Abstractions;
using Proton.Shared.Constants;
using Proton.Shared.Dtos;

namespace Proton.Client.Resource.Features.Vehicles.Scripts;

public sealed class VehicleMenuScript(IUiView ui) : HostedService
{
    public override Task StartAsync(CancellationToken ct)
    {
        Alt.OnServer<VehicleMenuMountDto>("vehicle-menu.mount", OnServerMount);
        Alt.OnServer<long>("vehicle-menu.spawn", OnServerSpawn);
        Alt.OnServer<long>("vehicle-menu.despawn", OnServerDespawn);
        ui.On<long>("vehicle-menu.spawn", OnUiSpawn);
        ui.On<long>("vehicle-menu.despawn", OnUiDespawn);
        ui.OnMount(Route.VehicleMenu, OnUiMount);
        ui.OnUnmount(Route.VehicleMenu, OnUiUnmount);
        return Task.CompletedTask;
    }

    private void OnServerMount(VehicleMenuMountDto dto)
    {
        ui.Mount(Route.VehicleMenu, dto);
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
        ui.Unfocus();
        Alt.GameControlsEnabled = true;
        Alt.OnKeyUp -= OnKeyUp;
        ui.Mount(Route.RaceMenu, new RaceMenuMountDto { InitialActivePage = "collection" });
    }

    private void OnUiSpawn(long id)
    {
        Alt.EmitServer("vehicle-menu.spawn", id);
    }

    private void OnUiDespawn(long id)
    {
        Alt.EmitServer("vehicle-menu.despawn", id);
    }

    private void OnServerSpawn(long id)
    {
        ui.Emit("vehicle-menu.spawn", id);
    }

    private void OnServerDespawn(long id)
    {
        ui.Emit("vehicle-menu.despawn", id);
    }

    private void OnKeyUp(Key key)
    {
        if (key != Key.Escape || !ui.IsMounted(Route.VehicleMenu))
        {
            return;
        }
        ui.Unmount(Route.VehicleMenu);
    }
}
