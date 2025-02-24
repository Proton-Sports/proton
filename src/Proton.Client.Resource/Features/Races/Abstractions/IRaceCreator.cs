using AltV.Net.Client.Elements.Interfaces;
using AltV.Net.Data;
using Proton.Client.Resource.Features.Races.Abstractions;
using Proton.Shared.Models;

namespace Proton.Client.Core.Interfaces;

public interface IRaceCreator
{
    IEnumerable<SharedRacePoint> RacePoints { get; }
    IEnumerable<SharedRaceStartPoint> StartPoints { get; }
    string Name { get; set; }

    void ClearStartPoints();
    void ImportStartPoints(IEnumerable<SharedRaceStartPoint> points);
    void AddStartPoint(Position position, Rotation rotation);
    bool TryRemoveStartPoint(Position position, out StartPositionData removed);
    void ClearRacePoints();
    void ImportRacePoints(IEnumerable<SharedRacePoint> points);
    void AddRacePoint(Position position, float radius);
    bool TryRemoveRacePoint(Position position, out RacePointData removed);
    bool TryGetClosestRaceCheckpointTo(Position position, out ICheckpoint checkpoint);
    bool UpdateRacePointPosition(ICheckpoint checkpoint, Position position);
    // https://github.com/FabianTerhorst/coreclr-module/issues/840
    // bool TrySetLastRacePointRadius(Func<float, float> setRadius);
}
