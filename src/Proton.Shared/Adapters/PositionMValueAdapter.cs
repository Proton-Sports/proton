using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Args;

namespace Proton.Shared.Adapters;

public sealed class PositionMValueAdapter : IMValueAdapter<Position>
{
    public static readonly PositionMValueAdapter Instance = new();

    public Position FromMValue(IMValueReader reader)
    {
        var value = new Position();
        if (reader.Peek() == MValueReaderToken.Nil) return value;

        reader.BeginObject();
        while (reader.HasNext())
        {
            var name = reader.NextName();
            switch (name)
            {
                case "x":
                    {
                        value.X = (float)reader.NextDouble();
                        break;
                    }
                case "y":
                    {
                        value.Y = (float)reader.NextDouble();
                        break;
                    }
                case "z":
                    {
                        value.Z = (float)reader.NextDouble();
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
        return value;
    }

    public void ToMValue(Position value, IMValueWriter writer)
    {
        writer.BeginObject();
        writer.Name("x");
        writer.Value(value.X);
        writer.Name("y");
        writer.Value(value.Y);
        writer.Name("z");
        writer.Value(value.Z);
        writer.EndObject();
    }

    public void ToMValue(object obj, IMValueWriter writer)
    {
        if (obj is Position position)
        {
            ToMValue(position, writer);
        }
    }

    object IMValueBaseAdapter.FromMValue(IMValueReader reader)
    {
        return FromMValue(reader);
    }
}
