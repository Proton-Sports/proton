using Proton.Server.Core.Interfaces;
using Proton.Server.Core.Models.Shop;

namespace Proton.Server.Core.Models;

public sealed record PlayerVehicleMod : IAggregateRoot
{
    public long Id { get; init; }
    public long ModId { get; init; }
    public Mod Mod { get; init; } = null!;
    public long PlayerVehicleId { get; init; }
    public PlayerVehicle PlayerVehicle { get; init; } = null!;
    public PlayerVehicleActiveMod? PlayerVehicleActiveMod { get; init; }
}
