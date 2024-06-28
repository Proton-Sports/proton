using AltV.Community.MValueAdapters.Generators;
using AltV.Community.MValueAdapters.Generators.Abstractions;

namespace Proton.Shared.Dtos;

[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public sealed class RaceCreatorDto
{
    public List<RaceCreatorMapDto> Maps { get; set; } = new();
    public string[] Ipls { get; set; } = Array.Empty<string>();
}
