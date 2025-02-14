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
        var position = vehicle?.Position ?? Alt.LocalPlayer.Position;
        var vehicles = Alt
            .GetAllVehicles()
            .Where(a => a != vehicle
                && a.ScriptId != 0
                && a.Position.GetDistanceSquaredTo(position) <= distanceSquared)
            .ToList();
        if (vehicle is not null)
        {
            vehicles.Add(vehicle);
        }
        for (var i = 0; i != vehicles.Count; ++i)
        {
            for (var j = i + 1; i != vehicles.Count; ++j)
            {
                Alt.Natives.SetEntityNoCollisionEntity(vehicles[i], vehicles[j], true);
            }
        }
    }
}
