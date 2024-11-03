using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Microsoft.EntityFrameworkCore;
using Proton.Server.Core.Interfaces;
using Proton.Server.Infrastructure.Factorys;
using Proton.Server.Resource.Features.Vehicles.Abstractions;
using Proton.Server.Resource.SharedKernel;

namespace Proton.Server.Resource.Features.Vehicles.Scripts;

public sealed class VehicleMenuScript(IDbContextFactory dbFactory, IGarageService garageService) : HostedService
{
    public override Task StartAsync(CancellationToken ct)
    {
        AltAsync.OnClient<IPlayer, long, Task>(
            "vehicle-menu.spawn",
            (player, id) =>
            {
                if (player is not PPlayer pplayer)
                {
                    return Task.CompletedTask;
                }

                return OnSpawnAsync(pplayer, id);
            }
        );
        Alt.OnClient<IPlayer, long>("vehicle-menu.despawn", OnDespawn);
        return Task.CompletedTask;
    }

    private async Task OnSpawnAsync(PPlayer player, long id)
    {
        var hasSpawnedVehicles = garageService.SpawnedVehicles.TryGetValue(player, out var vehicles);
        if (hasSpawnedVehicles && vehicles!.Count > 0)
        {
            foreach (var v in vehicles)
            {
                v.Destroy();
            }
            vehicles.Clear();
        }

        await using var db = await dbFactory.CreateDbContextAsync().ConfigureAwait(false);
        var garage = await db
            .Garages.Where(a => a.OwnerId == player.ProtonId && a.Id == id)
            .Select(a => new { Id = a.Id, Model = a.VehicleItem.AltVHash })
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
        if (garage is null || !Enum.TryParse<VehicleModel>(garage.Model, out var model))
        {
            return;
        }

        if (!hasSpawnedVehicles)
        {
            vehicles = [];
            garageService.SpawnedVehicles[player] = vehicles;
        }
        else if (vehicles.Any(a => a is IProtonVehicle protonVehicle && protonVehicle.GarageId == id))
        {
            return;
        }

        var vehicle = (IProtonVehicle)
            Alt.CreateVehicle(model, player.Position, new Rotation(0, 0, player.Rotation.Yaw));
        vehicle.GarageId = garage.Id;
        vehicles.Add(vehicle);
        player.SetIntoVehicle(vehicle, 0);
        player.Emit("vehicle-menu.spawn", id);
    }

    private void OnDespawn(IPlayer player, long id)
    {
        if (!garageService.SpawnedVehicles.TryGetValue(player, out var vehicles))
        {
            return;
        }

        var index = vehicles.FindIndex(a => a is ProtonVehicle protonVehicle && protonVehicle.GarageId == id);
        if (index == -1)
        {
            return;
        }

        var vehicle = vehicles[index];
        vehicle.Destroy();
        vehicles.RemoveAt(index);

        if (vehicles.Count == 0)
        {
            garageService.SpawnedVehicles.Remove(player);
        }

        player.Emit("vehicle-menu.despawn", id);
    }
}
