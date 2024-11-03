using System.Collections.Concurrent;
using AltV.Net.Elements.Entities;
using Proton.Server.Resource.Features.Vehicles.Abstractions;

namespace Proton.Server.Resource.Features.Vehicles;

public sealed class GarageService : IGarageService
{
    private readonly ConcurrentDictionary<IPlayer, List<IVehicle>> spawnedVehicles = [];

    public IDictionary<IPlayer, List<IVehicle>> SpawnedVehicles => spawnedVehicles;
}
