using AltV.Community.MValueAdapters.Generators;
using AltV.Community.MValueAdapters.Generators.Abstractions;

namespace Proton.Shared.Dtos;

[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public sealed record TuningShopOwnedModDto
{
    public long ModId { get; set; }
    public int Category { get; set; }
    public bool IsActive { get; set; }
}
