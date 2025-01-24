using AltV.Community.MValueAdapters.Generators;
using AltV.Community.MValueAdapters.Generators.Abstractions;
using AltV.Net.Data;
using Proton.Shared.Constants;

namespace Proton.Shared.Dtos;

[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public sealed record TuningShopMountDto
{
    public List<TuningShopModDto> Mods { get; set; } = [];
    public List<TuningShopWheelVariationDto> WheelVariations { get; set; } = [];
    public List<TuningShopOwnedModDto> OwnedMods { get; set; } = [];
    public List<TuningShopOwnedWheelVariationDto> OwnedWheelVariations { get; set; } = [];
    public WheelType WheelType { get; set; }
    public Rgba PrimaryColor { get; set; }
    public Rgba SecondaryColor { get; set; }
}
