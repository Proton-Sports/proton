using AltV.Net;
using AltV.Net.Elements.Entities;
using Proton.Server.Resource.Features.Vehicles.Abstractions;
using Proton.Server.Resource.SharedKernel;

namespace Proton.Server.Resource.Features.Vehicles.Scripts;

public sealed class GarageScript(IGarageService garageService) : HostedService
{
    public override Task StartAsync(CancellationToken ct)
    {
        Alt.OnPlayerDisconnect += OnPlayerDisconnect;
        return Task.CompletedTask;
    }

    private void OnPlayerDisconnect(IPlayer player, string reason)
    {
        if (garageService.SpawnedVehicles.Remove(player, out var vehicles))
        {
            foreach (var vehicle in vehicles)
            {
                vehicle.Destroy();
            }
        }
    }
}
