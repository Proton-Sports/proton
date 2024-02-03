using AltV.Net;
using Proton.Shared.Adapters;

namespace Proton.Shared.Models;

public class SharedRaceCreatorData : IMValueConvertible
{
    public long Id { get; set; }
    public string Name { get; set; }
    public List<SharedRaceStartPoint> StartPoints { get; }
    public List<SharedRacePoint> RacePoints { get; }

    public SharedRaceCreatorData(long id, string name, List<SharedRaceStartPoint> startPoints, List<SharedRacePoint> racePoints)
    {
        Id = id;
        Name = name;
        StartPoints = startPoints;
        RacePoints = racePoints;
    }

    public IMValueBaseAdapter GetAdapter() => SharedRaceCreatorDataMValueAdapter.Instance;
}
