using AltV.Community.MValueAdapters.Generators;
using AltV.Community.MValueAdapters.Generators.Abstractions;

namespace Proton.Shared.Dtos;

[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public sealed record TuningShopGenerateDto
{
    public List<TuningShopGenerateWheelDto> Wheels { get; set; } = null!;
}
