using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Args;

namespace Proton.Shared.Adapters;

public sealed class RotationMValueAdapter : IMValueAdapter<Rotation>
{
    public static readonly RotationMValueAdapter Instance = new();

    public Rotation FromMValue(IMValueReader reader)
    {
        var value = new Rotation();
        if (reader.Peek() == MValueReaderToken.Nil) return value;

        reader.BeginObject();
        while (reader.HasNext())
        {
            var name = reader.NextName();
            switch (name)
            {
                case "roll":
                    {
                        value.Roll = (float)reader.NextDouble();
                        break;
                    }
                case "pitch":
                    {
                        value.Pitch = (float)reader.NextDouble();
                        break;
                    }
                case "yaw":
                    {
                        value.Yaw = (float)reader.NextDouble();
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

    public void ToMValue(Rotation value, IMValueWriter writer)
    {
        writer.BeginObject();
        writer.Name("roll");
        writer.Value(value.Roll);
        writer.Name("pitch");
        writer.Value(value.Pitch);
        writer.Name("yaw");
        writer.Value(value.Yaw);
        writer.EndObject();
    }

    public void ToMValue(object obj, IMValueWriter writer)
    {
        if (obj is Rotation rotation)
        {
            ToMValue(rotation, writer);
        }
    }

    object IMValueBaseAdapter.FromMValue(IMValueReader reader)
    {
        return FromMValue(reader);
    }
}
