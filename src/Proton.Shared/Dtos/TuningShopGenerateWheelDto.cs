using AltV.Community.MValueAdapters.Generators;
using AltV.Community.MValueAdapters.Generators.Abstractions;
using Proton.Shared.Constants;

namespace Proton.Shared.Dtos;

[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public sealed record TuningShopGenerateWheelDto
{
    public WheelType Type { get; set; }
    public int Count { get; set; }
}
