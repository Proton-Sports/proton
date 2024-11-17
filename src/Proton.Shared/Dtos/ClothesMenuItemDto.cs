using AltV.Community.MValueAdapters.Generators;
using AltV.Community.MValueAdapters.Generators.Abstractions;

namespace Proton.Shared.Dtos;

[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public sealed record ClothesMenuItemDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
