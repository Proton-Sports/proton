using AltV.Net;
using AltV.Net.Elements.Args;
using Proton.Shared.Models;

namespace Proton.Shared.Adapters;

public sealed class SharedRaceCreatorDataMValueAdapter : IMValueAdapter<SharedRaceCreatorData>
{
    public static readonly SharedRaceCreatorDataMValueAdapter Instance = new();

    public SharedRaceCreatorData FromMValue(IMValueReader reader)
    {
        if (reader.Peek() == MValueReaderToken.Nil) return null!;

        long id = default;
        string name = string.Empty;
        List<SharedRaceStartPoint> startPoints = default!;
        List<SharedRacePoint> racePoints = default!;
        reader.BeginObject();
        while (reader.HasNext())
        {
            switch (reader.NextName())
            {
                case "id":
                    {
                        id = reader.NextLong();
                        break;
                    }
                case "name":
                    {
                        name = reader.NextString();
                        break;
                    }
                case "startPoints":
                    {
                        startPoints = DefaultMValueAdapters.GetArrayAdapter(SharedRaceStartPointMValueAdapter.Instance).FromMValue(reader);
                        break;
                    }
                case "racePoints":
                    {
                        racePoints = DefaultMValueAdapters.GetArrayAdapter(SharedRacePointMValueAdapter.Instance).FromMValue(reader);
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
        return new SharedRaceCreatorData(id, name, startPoints, racePoints);
    }

    public void ToMValue(SharedRaceCreatorData value, IMValueWriter writer)
    {
        writer.BeginObject();
        writer.Name("id");
        writer.Value(value.Id);
        writer.Name("name");
        writer.Value(value.Name);
        writer.Name("startPoints");
        DefaultMValueAdapters.GetArrayAdapter(SharedRaceStartPointMValueAdapter.Instance).ToMValue(value.StartPoints, writer);
        writer.Name("racePoints");
        DefaultMValueAdapters.GetArrayAdapter(SharedRacePointMValueAdapter.Instance).ToMValue(value.RacePoints, writer);
        writer.EndObject();
    }

    public void ToMValue(object obj, IMValueWriter writer)
    {
        if (obj is SharedRaceCreatorData value)
        {
            ToMValue(value, writer);
        }
    }

    object IMValueBaseAdapter.FromMValue(IMValueReader reader)
    {
        return FromMValue(reader);
    }
}
