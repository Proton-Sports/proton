using AltV.Net.Client;
using Proton.Client.Resource.Features.UiViews.Abstractions;
using Proton.Shared.Contants;
using Proton.Shared.Interfaces;

namespace Proton.Client.Resource.Utils.Scripts
{
    internal class SpeedometerScript : IStartup
    {
        private readonly IUiView uiView;
        private uint UpdateIntervalId = 0;

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
            UpdateIntervalId = Alt.SetInterval(UpdateVehicleUi, 50);
        }

        private void Alt_OnPlayerLeaveVehicle(AltV.Net.Client.Elements.Interfaces.IVehicle vehicle, byte seat)
        {
            uiView.Unmount(Route.Speedometer);
            Alt.ClearInterval(UpdateIntervalId);
        }

        private void UpdateVehicleUi()
        {
            if (Alt.LocalPlayer.Vehicle == null) return;

            var vehicle = Alt.LocalPlayer.Vehicle;

            uiView.Emit("vehicle:gear", vehicle.Gear);
            uiView.Emit("vehicle:speed", (int)Alt.Natives.GetEntitySpeed(vehicle) * 3, 6);
            uiView.Emit("vehicle:rpm", vehicle.Rpm * 10_000);

            //if holding breaks
            if (Alt.Natives.GetControlValue(2, 72) > 127 || Alt.Natives.GetControlValue(2, 76) > 127)
                uiView.Emit("vehicle:red", true);
            else
                uiView.Emit("vehicle:red", false);

            //if moving forward
            if (Alt.Natives.GetControlValue(2, 71) > 127)
                uiView.Emit("vehicle:green", true);
            else
                uiView.Emit("vehicle:green", false);
        }
    }
}
