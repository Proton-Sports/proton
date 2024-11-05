using AltV.Net.Client;
using AltV.Net.Client.Elements.Data;
using Proton.Client.Resource.Features.UiViews.Abstractions;
using Proton.Shared.Constants;
using Proton.Shared.Interfaces;

namespace Proton.Client.Resource.Utils.Scripts
{
    internal class SpeedometerScript : IStartup
    {
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

            Alt.OnKeyDown += OnKeyDown;
            Alt.OnKeyUp += OnKeyUp;
        }

        private void Alt_OnPlayerLeaveVehicle(AltV.Net.Client.Elements.Interfaces.IVehicle vehicle, byte seat)
        {
            uiView.Unmount(Route.Speedometer);
            Alt.ClearInterval(updateIntervalId);
            Alt.OnKeyDown -= OnKeyDown;
            Alt.OnKeyUp -= OnKeyUp;
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
    }
}
