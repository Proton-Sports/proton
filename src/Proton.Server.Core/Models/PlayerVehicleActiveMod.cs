using Proton.Server.Core.Interfaces;

namespace Proton.Server.Core.Models;

public sealed record PlayerVehicleActiveMod : IAggregateRoot
{
    public long PlayerVehicleModId { get; init; }
    public PlayerVehicleMod PlayerVehicleMod { get; init; } = null!;
}
