using AltV.Community.MValueAdapters.Generators;
using AltV.Community.MValueAdapters.Generators.Abstractions;

namespace Proton.Shared.Dtos;

[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public sealed record RaceMenuMountDto
{
    public string? InitialActivePage { get; set; }
    public List<string>? InitialDisabledPages { get; set; }
}
