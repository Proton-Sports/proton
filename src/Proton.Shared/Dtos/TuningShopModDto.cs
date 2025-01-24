using AltV.Community.MValueAdapters.Generators;
using AltV.Community.MValueAdapters.Generators.Abstractions;

namespace Proton.Shared.Dtos;

[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public sealed record TuningShopModDto
{
    public long Id { get; set; }
    public int Category { get; set; }
    public uint? Model { get; set; }
    public int Value { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Price { get; set; }
}
