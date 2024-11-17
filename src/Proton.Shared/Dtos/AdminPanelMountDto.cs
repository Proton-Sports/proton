using AltV.Community.MValueAdapters.Generators;
using AltV.Community.MValueAdapters.Generators.Abstractions;

namespace Proton.Shared.Dtos;

[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public sealed record AdminPanelMountDto
{
    public int? Tab { get; set; }
    public List<AdminPanelPlayerDto>? Players { get; set; }
    public List<AdminPanelVehicleDto>? Vehicles { get; set; }
}
