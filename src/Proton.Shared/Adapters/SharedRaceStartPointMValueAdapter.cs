using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Args;
using Proton.Shared.Models;

namespace Proton.Shared.Adapters;

public sealed class SharedRaceStartPointMValueAdapter : IMValueAdapter<SharedRaceStartPoint>
{
    public static readonly SharedRaceStartPointMValueAdapter Instance = new();

    public SharedRaceStartPoint FromMValue(IMValueReader reader)
    {
        if (reader.Peek() == MValueReaderToken.Nil) return default!;

        Position position = default!;
        Rotation rotation = default!;
        reader.BeginObject();
        while (reader.HasNext())
        {
            switch (reader.NextName())
            {
                case "position":
                    {
                        position = PositionMValueAdapter.Instance.FromMValue(reader);
                        break;
                    }
                case "rotation":
                    {
                        rotation = RotationMValueAdapter.Instance.FromMValue(reader);
                        break;
                    }
                default:
                    {
                        reader.SkipValue();
                        break;
                    }
            }
        }
        reader.EndObject();
        return new SharedRaceStartPoint(position, rotation);
    }

    public void ToMValue(SharedRaceStartPoint value, IMValueWriter writer)
    {
        writer.BeginObject();
        writer.Name("position");
        PositionMValueAdapter.Instance.ToMValue(value.Position, writer);
        writer.Name("rotation");
        RotationMValueAdapter.Instance.ToMValue(value.Rotation, writer);
        writer.EndObject();
    }

    public void ToMValue(object obj, IMValueWriter writer)
    {
        if (obj is SharedRaceStartPoint point)
        {
            ToMValue(point, writer);
        }
    }

    object IMValueBaseAdapter.FromMValue(IMValueReader reader)
    {
        return FromMValue(reader);
    }
}
