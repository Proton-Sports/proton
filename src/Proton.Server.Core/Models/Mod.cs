using AltV.Net.Enums;

namespace Proton.Server.Core.Models;

public sealed record Mod
{
    public long Id { get; init; }
    public int Category { get; init; }
    public VehicleModel? Model { get; init; }
    public int Value { get; init; }
    public int Price { get; init; }
}
