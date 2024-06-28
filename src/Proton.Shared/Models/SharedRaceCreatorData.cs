using AltV.Net;
using Proton.Shared.Adapters;

namespace Proton.Shared.Models;

public class SharedRaceCreatorData : IMValueConvertible
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? IplName { get; set; }
    public List<SharedRaceStartPoint> StartPoints { get; set; } = new();
    public List<SharedRacePoint> RacePoints { get; set; } = new();

    public IMValueBaseAdapter GetAdapter() => SharedRaceCreatorDataMValueAdapter.Instance;
}
