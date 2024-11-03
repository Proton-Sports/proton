using AltV.Net.Elements.Entities;

namespace Proton.Server.Resource.Features.Vehicles.Abstractions;

public interface IGarageService
{
    IDictionary<IPlayer, List<IVehicle>> SpawnedVehicles { get; }
}
