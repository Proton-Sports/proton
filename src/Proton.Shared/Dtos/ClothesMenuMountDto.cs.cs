using AltV.Community.MValueAdapters.Generators;
using AltV.Community.MValueAdapters.Generators.Abstractions;

namespace Proton.Shared.Dtos;

[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public sealed record ClothesMenuMountDto
{
    public List<ClothesMenuItemDto> Items { get; set; } = null!;
    public List<long>? EquippedIds { get; set; }
}
