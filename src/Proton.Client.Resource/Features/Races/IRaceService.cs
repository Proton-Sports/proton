using AltV.Net.Client.Elements.Interfaces;
using AltV.Net.Elements.Entities;
using Proton.Client.Resource.Features.Races.Abstractions;
using Proton.Shared.Constants;
using Proton.Shared.Models;

namespace Proton.Client.Resource.Features.Races;

public interface IRaceService
{
    event Action<object> RacePointHit;
    event Action Started;
    event Action Stopped;

    bool Ghosting { get; set; }
    RaceType RaceType { get; set; }
    int Dimension { get; set; }
    IReadOnlyList<RacePointDto> RacePoints { get; }
    string? IplName { get; set; }
    RaceStatus Status { get; set; }

    void ClearRacePoints();
    int EnsureRacePointsCapacity(int capacity);
    void AddRacePoint(RacePointDto point);
    void AddRacePoints(List<RacePointDto> points);
    ICheckpoint LoadRacePoint(CheckpointType checkpointType, int index, int? nextIndex);
    void UnloadRacePoint();
    bool TryGetPointResolver(out IRacePointResolver resolver);
    void Start();
    void Stop();
}
