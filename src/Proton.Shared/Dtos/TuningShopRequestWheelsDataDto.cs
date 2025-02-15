using AltV.Community.MValueAdapters.Generators;
using AltV.Community.MValueAdapters.Generators.Abstractions;
using Proton.Shared.Constants;

namespace Proton.Shared.Dtos;

[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public sealed record TuningShopRequestWheelsDataDto
{
    public List<TuningShopWheelVariationDto> WheelVariations { get; set; } = [];
    public List<TuningShopOwnedWheelVariationDto> OwnedWheelVariations { get; set; } = [];
    public WheelType WheelType { get; set; }
}
