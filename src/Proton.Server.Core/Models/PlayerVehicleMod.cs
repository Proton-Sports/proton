using Proton.Server.Core.Models.Shop;

namespace Proton.Server.Core.Models;

public sealed record PlayerVehicleMod
{
    public long Id { get; init; }
    public long ModId { get; init; }
    public Mod Mod { get; init; } = null!;
    public long PlayerVehicleId { get; init; }
    public PlayerVehicle PlayerVehicle { get; init; } = null!;
}
