using AltV.Net.Enums;

namespace Proton.Server.Core.Models.Shop;

public sealed record StockVehicle
{
    public long Id { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public VehicleModel Model { get; init; }
    public int Price { get; init; }
    public string Category { get; init; } = string.Empty;
}
