using AltV.Community.MValueAdapters.Generators;
using AltV.Community.MValueAdapters.Generators.Abstractions;

namespace Proton.Shared.Dtos;

[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public sealed record AdminPanelVehicleDto
{
    public uint Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
