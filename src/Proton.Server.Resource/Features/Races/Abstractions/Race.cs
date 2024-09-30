using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Proton.Shared.Constants;

namespace Proton.Server.Resource.Features.Races.Abstractions;

public record class Race
{
    public long Id { get; set; }
    public Guid Guid { get; set; }
    public IPlayer Host { get; set; } = null!;
    public long MapId { get; set; }
    public VehicleModel[] VehicleModels { get; set; } = [];
    public int MaxParticipants { get; set; }
    public int Duration { get; set; }
    public int CountdownSeconds { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool Ghosting { get; set; }
    public RaceType Type { get; set; }
    public int? Laps { get; set; }
    public TimeOnly Time { get; set; }
    public string Weather { get; set; } = string.Empty;
    public DateTimeOffset CreatedTime { get; set; }
    public RaceStatus Status { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public List<RaceParticipant> Participants { get; set; } = [];
    public int PrizePool { get; set; }
    public bool LobbyCountingDown { get; set; }
}
