using Proton.Server.Core.Interfaces;

namespace Proton.Server.Core.Models;

public sealed record PlayerVehicleActiveWheelVariation : IAggregateRoot
{
    public long PlayerVehicleWheelVariationId { get; init; }
    public PlayerVehicleWheelVariation PlayerVehicleWheelVariation { get; init; } = null!;
}
