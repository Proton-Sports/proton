namespace Proton.Server.Resource.Features.Players.Abstractions;

public readonly record struct ClosetClothes
{
    public long Id { get; init; }
    public readonly uint Dlc { get; init; }
    public readonly ushort Drawable { get; init; }
    public readonly byte Texture { get; init; }
    public readonly byte Palette { get; init; }
}
