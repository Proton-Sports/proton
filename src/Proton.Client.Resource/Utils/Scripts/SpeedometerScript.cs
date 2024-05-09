using AltV.Net.Client;
using Proton.Client.Infrastructure.Interfaces;
using Proton.Shared.Contants;
using Proton.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            Alt.Log("mounting Ui");
            uiView.Mount(Route.Speedometer);
            UpdateIntervalId = Alt.SetInterval(UpdateVehicleUi, 50);            
        }

        private void Alt_OnPlayerLeaveVehicle(AltV.Net.Client.Elements.Interfaces.IVehicle vehicle, byte seat)
        {
            Alt.Log("unmounting Ui");
            uiView.Unmount(Route.Speedometer);
            Alt.ClearInterval(UpdateIntervalId);            
        }

        private bool IsEngineRunning = false;
        private void UpdateVehicleUi()
        {
            if (Alt.LocalPlayer.Vehicle == null) return;

            var vehicle = Alt.LocalPlayer.Vehicle;

            uiView.Emit("vehicle:gear", vehicle.Gear);
            uiView.Emit("vehicle:speed", (int)Alt.Natives.GetEntitySpeed(vehicle) * 3,6);
            uiView.Emit("vehicle:rpm", vehicle.Rpm * 10_000);            

            if(IsEngineRunning != Alt.Natives.GetIsVehicleEngineRunning(vehicle))
            {
                IsEngineRunning = !IsEngineRunning;
                uiView.Emit("vehicle:engine", IsEngineRunning);
            }
        }        
    }
}
