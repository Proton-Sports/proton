using AltV.Net;
using AltV.Net.Data;
using Proton.Shared.Adapters;

namespace Proton.Shared.Models;

public class SharedRaceStartPoint : IMValueConvertible
{
    public Position Position { get; }
    public Rotation Rotation { get; }

    public SharedRaceStartPoint(Position position, Rotation rotation)
    {
        Position = position;
        Rotation = rotation;
    }

    public IMValueBaseAdapter GetAdapter()
    {
        return SharedRaceStartPointMValueAdapter.Instance;
    }
}
