using AltV.Net.Enums;
using Proton.Server.Core.Interfaces;
using Proton.Shared.Constants;

namespace Proton.Server.Core.Models;

public sealed record WheelVariation : IAggregateRoot
{
    public long Id { get; init; }
    public VehicleModel? Model { get; init; }
    public WheelType Type { get; init; }
    public string Name { get; init; } = string.Empty;
    public int Value { get; init; }
    public int Price { get; init; }
}
