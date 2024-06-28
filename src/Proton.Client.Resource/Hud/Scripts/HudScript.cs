using AltV.Net.Client;
using Proton.Shared.Interfaces;

namespace Proton.Client.Resource.Hud.Scripts;

public class HudScript : IStartup
{
    private uint everyTickEvent;
    
    public HudScript()
    {
        DisableHudElements();
        SetInfiniteStamina();
    }

    private void SetInfiniteStamina()
    {
        Alt.SetStat("stamina", 100);
    }
    
    private void DisableHudElements()
    {
        everyTickEvent = Alt.EveryTick(() =>
        {
            Alt.Natives.HudSuppressWeaponWheelResultsThisFrame();
            Alt.Natives.HideHudComponentThisFrame(6);
            Alt.Natives.HideHudComponentThisFrame(7);
            Alt.Natives.HideHudComponentThisFrame(8);
            Alt.Natives.HideHudComponentThisFrame(9);
            Alt.Natives.DisableControlAction(2, 37, true);
            
            Alt.Natives.DisablePlayerFiring(Alt.LocalPlayer, true);
            Alt.Natives.DisableControlAction(0, 106, true);
            Alt.Natives.DisableControlAction(0, 140, true);

            var playerVehicle = Alt.LocalPlayer.Vehicle;
            if (playerVehicle == null) return;
            
            Alt.Natives.SetUserRadioControlEnabled(false);
            Alt.Natives.SetVehRadioStation(playerVehicle, "OFF");
            Alt.Natives.SetVehicleRadioEnabled(playerVehicle, false);

            var vehicleClass = Alt.Natives.GetVehicleClass(playerVehicle);
            if (vehicleClass is not (13 or 8)) return;
            
            Alt.Natives.DisableControlAction(2, 345, true);
        });
    }
}