using AltV.Community.MValueAdapters.Generators;
using AltV.Community.MValueAdapters.Generators.Abstractions;
using AltV.Net.Data;

namespace Proton.Shared.Dtos;

[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public sealed record TuningShopMountDto
{
    public Rgba PrimaryColor { get; set; }
    public Rgba SecondaryColor { get; set; }
}
