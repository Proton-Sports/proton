using AltV.Net.Enums;
using Proton.Server.Core.Interfaces;

namespace Proton.Server.Core.Models;

public sealed record UserRaceRestoration : IAggregateRoot
{
    public long UserId { get; init; }
    public User User { get; init; } = null!;
    public Guid RaceId { get; init; }
    public int Lap { get; set; }
    public float AccumulatedDistance { get; set; }
    public float PartialDistance { get; set; }
    public int? NextRacePointIndex { get; set; }
    public ICollection<UserRacePointRestoration> Points { get; set; } = null!;
    public long FinishTime { get; set; }
    public float X { get; init; }
    public float Y { get; init; }
    public float Z { get; init; }
    public float Roll { get; init; }
    public float Pitch { get; init; }
    public float Yaw { get; init; }
    public VehicleModel VehicleModel { get; init; }
}
