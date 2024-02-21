using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Proton.Server.Infrastructure.Constants;

namespace Proton.Server.Infrastructure.Models;

public record class Race
{
    public IPlayer Host { get; set; } = null!;
    public long MapId { get; set; }
    public VehicleModel VehicleModel { get; set; }
    public int Racers { get; set; }
    public int Duration { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool Ghosting { get; set; }
    public RaceType Type { get; set; }
    public int? Laps { get; set; }
    public TimeOnly Time { get; set; }
    public string Weather { get; set; } = string.Empty;
}
