using AltV.Net.Client;
using AltV.Net.Data;
using Proton.Shared.Interfaces;

namespace Proton.Client.Resource.Features.Races.Scripts;

public sealed class RacePhasingScript : IStartup
{
    private readonly IRaceService raceService;

    public RacePhasingScript(IRaceService raceService)
    {
        this.raceService = raceService;
        raceService.Started += HandleStarted;
        raceService.Stopped += HandleStopped;
    }

    private void HandleStarted()
    {
        if (raceService.Ghosting)
        {
            Alt.OnTick += HandleTick;
        }
    }

    private void HandleStopped()
    {
        if (raceService.Ghosting)
        {
            Alt.OnTick -= HandleTick;
        }
    }

    private void HandleTick()
    {
        const float distanceSquared = 20 * 20;
        var vehicle = Alt.LocalPlayer.Vehicle;
        if (vehicle is null) return;
        foreach (var other in Alt.GetAllVehicles().Where(x => x != vehicle
            && x.ScriptId != 0
            && x.Position.GetDistanceSquaredTo(vehicle.Position) <= distanceSquared))
        {
            Alt.Natives.SetEntityNoCollisionEntity(vehicle, other, true);
        }
    }
}
