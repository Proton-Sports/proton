using AltV.Community.MValueAdapters.Generators;
using AltV.Net;
using AltV.Net.Data;
using Proton.Shared.Adapters;

namespace Proton.Shared.Models;

public class SharedRacePoint : IMValueConvertible
{
    public Position Position { get; }
    public float Radius { get; }

    public SharedRacePoint(Position position, float radius)
    {
        Position = position;
        Radius = radius;
    }

    public IMValueBaseAdapter GetAdapter()
    {
        return SharedRacePointMValueAdapter.Instance;
    }
}
