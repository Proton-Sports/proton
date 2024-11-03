using AltV.Community.MValueAdapters.Generators;
using AltV.Community.MValueAdapters.Generators.Abstractions;

namespace Proton.Shared.Dtos;

[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public sealed record VehicleMenuMountDto
{
    public List<VehicleMenuItemDto> Items { get; set; } = null!;
    public List<long>? SpawnedIds { get; set; }
}
