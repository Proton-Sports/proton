using AltV.Net.Elements.Entities;

namespace Proton.Server.Resource.Features.Races.Abstractions;

public class RaceParticipant
{
    public required IPlayer Player { get; set; }
    public IVehicle? Vehicle { get; set; }
    public int Lap { get; set; }
    public float AccumulatedDistance { get; set; }
    public float PartialDistance { get; set; }
    public int? NextRacePointIndex { get; set; }
    public LinkedList<RacePointLog> PointLogs { get; set; } = new();
    public long FinishTime { get; set; }
}
