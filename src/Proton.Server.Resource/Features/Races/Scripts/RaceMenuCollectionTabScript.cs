using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using Microsoft.EntityFrameworkCore;
using Proton.Server.Core.Interfaces;
using Proton.Server.Infrastructure.Factorys;
using Proton.Server.Resource.Features.Vehicles.Abstractions;
using Proton.Server.Resource.SharedKernel;
using Proton.Shared.Dtos;

namespace Proton.Server.Resource.Features.Races.Scripts;

public sealed class RaceMenuCollectionTabScript(IDbContextFactory dbFactory, IGarageService garageService)
    : HostedService
{
    public override Task StartAsync(CancellationToken ct)
    {
        AltAsync.OnClient<IPlayer, string, Task>("race-menu-collection.option.change", OnOptionChangeAsync);
        return Task.CompletedTask;
    }

    private Task OnOptionChangeAsync(IPlayer player, string option)
    {
        if (player is not PPlayer pplayer)
        {
            return Task.CompletedTask;
        }

        switch (option)
        {
            case "cars":
                return LoadCarsAsync(pplayer);
            case "clothes":
                break;
        }
        return Task.CompletedTask;
    }

    private async Task LoadCarsAsync(PPlayer player)
    {
        // TODO: move this to IVehicleMenuService
        await using var db = await dbFactory.CreateDbContextAsync().ConfigureAwait(false);
        var items = await db
            .Garages.Where(a => a.OwnerId == player.ProtonId)
            .Select(a => new VehicleMenuItemDto { Id = a.Id, Name = a.VehicleItem.DisplayName })
            .ToListAsync()
            .ConfigureAwait(false);

        List<long>? spawnedIds = null;
        if (garageService.SpawnedVehicles.TryGetValue(player, out var vehicles))
        {
            spawnedIds = vehicles
                .Where(a => a is IProtonVehicle)
                .Cast<IProtonVehicle>()
                .Select(a => a.GarageId)
                .ToList();
        }
        player.Emit("vehicle-menu.mount", new VehicleMenuMountDto { Items = items, SpawnedIds = spawnedIds });
    }
}
