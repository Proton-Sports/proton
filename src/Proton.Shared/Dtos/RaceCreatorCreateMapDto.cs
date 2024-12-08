using AltV.Community.MValueAdapters.Generators;
using AltV.Community.MValueAdapters.Generators.Abstractions;

namespace Proton.Shared.Dtos;

[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public sealed record RaceCreatorCreateMapDto
{
    public string MapName { get; set; } = string.Empty;
    public string IplName { get; set; } = string.Empty;
}
