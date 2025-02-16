using AltV.Community.MValueAdapters.Generators;
using AltV.Community.MValueAdapters.Generators.Abstractions;

namespace Proton.Shared.Dtos;

[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public record ClothShopMountDto
{
    public List<ClothShopClothesDto> Clothes { get; set; } = null!;
    public List<ClothShopOwnedClothesDto> OwnedClothes { get; set; } = null!;
}
