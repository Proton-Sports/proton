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
        AltAsync.OnClient<PPlayer, long, Task>("vehicle-menu.spawn", OnSpawnAsync);
        Alt.OnClient<IPlayer, long>("vehicle-menu.despawn", OnDespawn);
        return Task.CompletedTask;
    }

    private async Task OnSpawnAsync(PPlayer player, long id)
    {
        if (garageService.SpawnedVehicles.TryGetValue(player, out var vehicles) && vehicles.Count > 0)
        {
            foreach (var v in vehicles.Where(a => a is IProtonVehicle).Cast<IProtonVehicle>())
            {
                v.Destroy();
                player.Emit("vehicle-menu.despawn", v.GarageId);
            }
            vehicles.Clear();
        }

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        await using var db = await dbFactory.CreateDbContextAsync().ConfigureAwait(false);
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
        var playerVehicle = await db
            .PlayerVehicles.Where(a => a.Id == id)
            .Select(a => new
            {
                a.Id,
                a.Model,
                a.PrimaryColor,
                a.SecondaryColor,
                Mods = a
                    .Mods.Where(b => b.PlayerVehicleActiveMod != null)
                    .Select(a => new { a.Mod.Category, a.Mod.Value }),
                WheelVariations = a
                    .WheelVariations.Where(b => b.PlayerVehicleActiveWheelVariation != null)
                    .Select(a => new { a.WheelVariation.Type, a.WheelVariation.Value })
            })
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
        if (playerVehicle is null)
        {
            return;
        }

        if (!garageService.SpawnedVehicles.TryGetValue(player, out vehicles))
        {
            vehicles = [];
            garageService.SpawnedVehicles[player] = vehicles;
        }
        else if (vehicles.Any(a => a is IProtonVehicle protonVehicle && protonVehicle.GarageId == id))
        {
            return;
        }

        var vehicle = (IProtonVehicle)(
            await AltAsync.CreateVehicle(playerVehicle.Model, player.Position, player.Rotation).ConfigureAwait(false)
        );
        vehicles.Add(vehicle);
        vehicle.GarageId = playerVehicle.Id;
        vehicle.Dimension = player.Dimension;
        vehicle.ModKit = 1;
        var wheel = playerVehicle.WheelVariations.FirstOrDefault();
        if (wheel is not null)
        {
            vehicle.SetWheels((byte)wheel.Type, (byte)wheel.Value);
        }
        foreach (var mod in playerVehicle.Mods)
        {
            vehicle.SetMod((byte)mod.Category, (byte)mod.Value);
        }
        vehicle.PrimaryColorRgb = playerVehicle.PrimaryColor;
        vehicle.SecondaryColorRgb = playerVehicle.SecondaryColor;
        player.SetIntoVehicle(vehicle, 1);
        player.Emit("vehicle-menu.spawn", id);
    }

    private void OnDespawn(IPlayer player, long id)
    {
        if (!garageService.SpawnedVehicles.TryGetValue(player, out var vehicles))
        {
            return;
        }

        var index = vehicles.FindIndex(a => a is IProtonVehicle protonVehicle && protonVehicle.GarageId == id);
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
