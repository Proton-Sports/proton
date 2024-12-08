using AltV.Net.Client;
using AltV.Net.Client.Elements.Data;
using Proton.Client.Resource.Features.UiViews.Abstractions;
using Proton.Shared.Constants;
using Proton.Shared.Interfaces;

namespace Proton.Client.Resource.Utils.Scripts;

public sealed class SpeedometerScript : IStartup
{
    private const int INPUT_VEH_ACCELERATE = 71;
    private const int INPUT_VEH_HANDBRAKE = 76;

    private readonly IUiView uiView;
    private uint updateIntervalId;
    private int brakeCount;

    public SpeedometerScript(IUiView uiView)
    {
        Alt.OnPlayerEnterVehicle += Alt_OnPlayerEnterVehicle;
        Alt.OnPlayerLeaveVehicle += Alt_OnPlayerLeaveVehicle;
        this.uiView = uiView;

        Alt.Natives.HideHudComponentThisFrame(6);
        Alt.Natives.HideHudComponentThisFrame(7);
        Alt.Natives.HideHudComponentThisFrame(8);
        Alt.Natives.HideHudComponentThisFrame(9);
    }

    private void Alt_OnPlayerEnterVehicle(AltV.Net.Client.Elements.Interfaces.IVehicle vehicle, byte seat)
    {
        Alt.Natives.HideHudComponentThisFrame(6);
        Alt.Natives.HideHudComponentThisFrame(7);
        Alt.Natives.HideHudComponentThisFrame(8);
        Alt.Natives.HideHudComponentThisFrame(9);

        uiView.Mount(Route.Speedometer);
        updateIntervalId = Alt.SetInterval(UpdateVehicleUi, 50);

        if (Alt.Natives.IsUsingKeyboardAndMouse(0))
        {
            Alt.OnKeyDown += OnKeyDown;
            Alt.OnKeyUp += OnKeyUp;
        }
        else
        {
            Alt.OnTick += OnTick;
        }
    }

    private void Alt_OnPlayerLeaveVehicle(AltV.Net.Client.Elements.Interfaces.IVehicle vehicle, byte seat)
    {
        uiView.Unmount(Route.Speedometer);
        Alt.ClearInterval(updateIntervalId);
        if (Alt.Natives.IsUsingKeyboardAndMouse(0))
        {
            Alt.OnKeyDown -= OnKeyDown;
            Alt.OnKeyUp -= OnKeyUp;
        }
        else
        {
            Alt.OnTick -= OnTick;
        }
    }

    private void UpdateVehicleUi()
    {
        if (Alt.LocalPlayer.Vehicle == null)
        {
            return;
        }

        var vehicle = Alt.LocalPlayer.Vehicle;

        uiView.Emit("vehicle:gear", vehicle.Gear);
        uiView.Emit("vehicle:speed", (int)Alt.Natives.GetEntitySpeed(vehicle) * 3, 6);
        uiView.Emit("vehicle:rpm", vehicle.Rpm * 10_000);
    }

    private void OnKeyDown(Key key)
    {
        switch (key)
        {
            case Key.S:
            case Key.Space:
                if (++brakeCount > 0)
                {
                    uiView.Emit("vehicle:red", true);
                }
                break;
            case Key.W:
                uiView.Emit("vehicle:green", true);
                break;
        }
    }

    private void OnKeyUp(Key key)
    {
        switch (key)
        {
            case Key.S:
            case Key.Space:
                if (--brakeCount == 0)
                {
                    uiView.Emit("vehicle:red", false);
                }
                break;
            case Key.W:
                uiView.Emit("vehicle:green", false);
                break;
        }
    }

    private void OnTick()
    {
        if (Alt.Natives.IsControlJustPressed(0, INPUT_VEH_HANDBRAKE))
        {
            uiView.Emit("vehicle:red", true);
        }
        else if (Alt.Natives.IsControlJustReleased(0, INPUT_VEH_HANDBRAKE))
        {
            uiView.Emit("vehicle:red", false);
        }

        if (Alt.Natives.IsControlJustPressed(0, INPUT_VEH_ACCELERATE))
        {
            uiView.Emit("vehicle:green", true);
        }
        else if (Alt.Natives.IsControlJustReleased(0, INPUT_VEH_ACCELERATE))
        {
            uiView.Emit("vehicle:green", false);
        }
    }
}
