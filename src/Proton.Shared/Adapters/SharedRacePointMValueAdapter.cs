using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Args;
using Proton.Shared.Models;

namespace Proton.Shared.Adapters;

public sealed class SharedRacePointMValueAdapter : IMValueAdapter<SharedRacePoint>
{
    public static readonly SharedRacePointMValueAdapter Instance = new();

    public SharedRacePoint FromMValue(IMValueReader reader)
    {
        if (reader.Peek() == MValueReaderToken.Nil) return default!;

        Position position = default!;
        float radius = default;
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
                case "radius":
                    {
                        radius = (float)reader.NextDouble();
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
        return new SharedRacePoint(position, radius);
    }

    public void ToMValue(SharedRacePoint value, IMValueWriter writer)
    {
        writer.BeginObject();
        writer.Name("position");
        PositionMValueAdapter.Instance.ToMValue(value.Position, writer);
        writer.Name("radius");
        writer.Value(value.Radius);
        writer.EndObject();
    }

    public void ToMValue(object obj, IMValueWriter writer)
    {
        if (obj is SharedRacePoint point)
        {
            ToMValue(point, writer);
        }
    }

    object IMValueBaseAdapter.FromMValue(IMValueReader reader)
    {
        return FromMValue(reader);
    }
}
