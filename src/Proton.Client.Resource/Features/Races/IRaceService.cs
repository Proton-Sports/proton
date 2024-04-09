using AltV.Net.Client.Elements.Interfaces;
using AltV.Net.Elements.Entities;
using Proton.Shared.Constants;
using Proton.Shared.Models;

namespace Proton.Client.Resource.Features.Races;

public interface IRaceService
{
    event Action<object> RacePointHit;

    bool IsStarted { get; }
    RaceType RaceType { get; set; }
    int Dimension { get; set; }
    int Laps { get; set; }
    int CurrentLap { get; set; }
    IReadOnlyList<RacePointDto> RacePoints { get; }
    string? IplName { get; set; }

    void ClearRacePoints();
    int EnsureRacePointsCapacity(int capacity);
    void AddRacePoint(RacePointDto point);
    void AddRacePoints(List<RacePointDto> points);
    ICheckpoint LoadRacePoint(CheckpointType checkpointType, int index, int? nextIndex);
    bool UnloadRacePoint(int index);
    bool TryGetPointResolver(out IRacePointResolver resolver);
    void Start();
    void Stop();
}
