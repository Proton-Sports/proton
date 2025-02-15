using AltV.Community.MValueAdapters.Generators;
using AltV.Community.MValueAdapters.Generators.Abstractions;

namespace Proton.Shared.Dtos;

[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public sealed record TuningShopRequestModDataDto
{
    public List<TuningShopModDto> Mods { get; set; } = [];
    public List<TuningShopOwnedModDto> OwnedMods { get; set; } = [];
}
