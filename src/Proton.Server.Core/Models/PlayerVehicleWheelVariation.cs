using Proton.Server.Core.Interfaces;
using Proton.Server.Core.Models.Shop;

namespace Proton.Server.Core.Models;

public sealed record PlayerVehicleWheelVariation : IAggregateRoot
{
    public long Id { get; init; }
    public long WheelVariationId { get; init; }
    public WheelVariation WheelVariation { get; init; } = null!;
    public long PlayerVehicleId { get; init; }
    public PlayerVehicle PlayerVehicle { get; init; } = null!;
    public PlayerVehicleActiveWheelVariation? PlayerVehicleActiveWheelVariation { get; init; }
}
