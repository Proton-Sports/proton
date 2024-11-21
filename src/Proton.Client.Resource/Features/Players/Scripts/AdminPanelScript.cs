using AltV.Net.Client;
using Proton.Client.Resource.Commons;
using Proton.Client.Resource.Features.UiViews.Abstractions;
using Proton.Shared.Dtos;
using Proton.Shared.Constants;
using AltV.Net.Client.Elements.Data;

namespace Proton.Client.Resource.Features.Players.Scripts;

public sealed class AdminPanelScript(IUiView ui) : HostedService
{
    public override Task StartAsync(CancellationToken ct)
    {
        Alt.OnServer<AdminPanelMountDto>("admin-panel.mount", OnServerMount);
        ui.OnMount(Route.AdminPanel, OnUiMount);
        ui.OnUnmount(Route.AdminPanel, OnUiUnmount);
        ui.On("admin-panel.players.get", OnUiPlayersGet);
        ui.On("admin-panel.vehicles.get", OnUiVehiclesGet);
        ui.On<uint, string>("admin-panel.players.action", OnUiPlayersAction);
        ui.On<string>("admin-panel.vehicles.create", OnUiVehiclesCreate);
        ui.On<uint>("admin-panel.vehicles.destroy", OnUiVehiclesDestroy);
        Alt.OnServer<List<AdminPanelPlayerDto>>("admin-panel.players.get", OnServerPlayersGet);
        Alt.OnServer<List<AdminPanelVehicleDto>>("admin-panel.vehicles.get", OnServerVehiclesGet);
        Alt.OnServer<AdminPanelVehicleDto>("admin-panel.vehicles.create", OnServerVehiclesCreate);
        Alt.OnServer<uint>("admin-panel.vehicles.destroy", OnServerVehiclesDestroy);
        return Task.CompletedTask;
    }

    private void OnServerMount(AdminPanelMountDto dto)
    {
        ui.Mount(Route.AdminPanel, dto);
    }

    private void OnUiMount()
    {
        Alt.ShowCursor(true);
        ui.Focus();
        Alt.GameControlsEnabled = false;
        Alt.OnKeyUp += OnKeyUp;
    }

    private void OnUiUnmount()
    {
        Alt.ShowCursor(false);
        ui.Unfocus();
        Alt.GameControlsEnabled = true;
        Alt.OnKeyUp -= OnKeyUp;
    }

    private void OnUiPlayersGet()
    {
        Alt.EmitServer("admin-panel.players.get");
    }

    private void OnUiVehiclesGet()
    {
        Alt.EmitServer("admin-panel.vehicles.get");
    }

    private void OnServerPlayersGet(List<AdminPanelPlayerDto> dtos)
    {
        ui.Emit("admin-panel.players.get", dtos);
    }

    private void OnServerVehiclesGet(List<AdminPanelVehicleDto> dtos)
    {
        ui.Emit("admin-panel.vehicles.get", dtos);
    }

    private void OnUiPlayersAction(uint id, string action)
    {
        var actions = new string[] { "kick", "ban", "tp", "bring" };
        if (!actions.Any(a => action.Equals(a, StringComparison.Ordinal)))
        {
            return;
        }

        Alt.EmitServer("admin-panel.players.action", id, action);
    }

    private void OnUiVehiclesCreate(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return;
        }

        Alt.EmitServer("admin-panel.vehicles.create", name);
    }

    private void OnUiVehiclesDestroy(uint id)
    {
        Alt.EmitServer("admin-panel.vehicles.destroy", id);
    }

    private void OnServerVehiclesCreate(AdminPanelVehicleDto dto)
    {
        ui.Emit("admin-panel.vehicles.create", dto);
    }

    private void OnServerVehiclesDestroy(uint id)
    {
        ui.Emit("admin-panel.vehicles.destroy", id);
    }

    private void OnKeyUp(Key key)
    {
        if (!Alt.IsConsoleOpen && key == Key.Escape)
        {
            ui.Unmount(Route.AdminPanel);
        }
    }
}
