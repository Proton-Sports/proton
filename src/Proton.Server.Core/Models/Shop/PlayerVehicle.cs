using AltV.Net.Data;
using AltV.Net.Enums;
using Proton.Server.Core.Interfaces;

namespace Proton.Server.Core.Models.Shop;

public class PlayerVehicle : IAggregateRoot
{
    public long Id { get; init; }
    public long PlayerId { get; init; }
    public User Player { get; init; } = null!;
    public long VehicleId { get; init; }
    public VehicleModel Model { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public int Price { get; init; }
    public int AltVColor { get; init; }
    public string Category { get; init; } = string.Empty;
    public DateTime PurchasedDate { get; init; } = DateTime.UtcNow;
    public Rgba PrimaryColor { get; init; }
    public Rgba SecondaryColor { get; init; }
    public ICollection<PlayerVehicleMod> Mods { get; init; } = null!;
    public ICollection<PlayerVehicleWheelVariation> WheelVariations { get; init; } = null!;
}
