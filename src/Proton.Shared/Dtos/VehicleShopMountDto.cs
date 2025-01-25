using AltV.Community.MValueAdapters.Generators;
using AltV.Community.MValueAdapters.Generators.Abstractions;

namespace Proton.Shared.Dtos;

[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public record VehicleShopMountDto
{
    public List<VehicleShopVehicleDto> Vehicles { get; set; } = null!;
}
