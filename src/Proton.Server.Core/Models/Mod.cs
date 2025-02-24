using AltV.Net.Enums;
using Proton.Server.Core.Interfaces;

namespace Proton.Server.Core.Models;

public sealed record Mod : IAggregateRoot
{
    public long Id { get; init; }
    public int Category { get; init; }
    public string Name { get; init; } = string.Empty;
    public VehicleModel? Model { get; init; }
    public int Value { get; init; }
    public int Price { get; init; }
}
