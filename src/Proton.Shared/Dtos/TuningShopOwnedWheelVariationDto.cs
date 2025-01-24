using AltV.Community.MValueAdapters.Generators;
using AltV.Community.MValueAdapters.Generators.Abstractions;

namespace Proton.Shared.Dtos;

[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public sealed record TuningShopOwnedWheelVariationDto
{
    public long WheelVariationId { get; set; }
    public int Type { get; set; }
    public int Value { get; set; }
    public bool IsActive { get; set; }
}
