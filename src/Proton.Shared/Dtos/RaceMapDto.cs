using AltV.Net;
using AltV.Net.Elements.Args;
using Proton.Shared.Adapters;
using Proton.Shared.Models;

namespace Proton.Shared.Dtos;

public sealed class RaceMapDto : IMValueConvertible
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<SharedRaceStartPoint> StartPoints { get; set; } = null!;
    public List<SharedRacePoint> RacePoints { get; set; } = null!;

    public IMValueBaseAdapter GetAdapter() => Adapter.Instance;

    public class Adapter : BaseMValueAdapter<RaceMapDto>
    {
        public static readonly Adapter Instance = new();

        public override RaceMapDto FromMValue(IMValueReader reader)
        {
            if (reader.Peek() == MValueReaderToken.Nil) return default!;

            var dto = new RaceMapDto();
            reader.BeginObject();
            while (reader.HasNext())
            {
                switch (reader.NextName())
                {
                    case "id":
                        dto.Id = reader.NextLong();
                        break;
                    case "name":
                        dto.Name = reader.NextString();
                        break;
                    case "startPoints":
                        dto.StartPoints = DefaultMValueAdapters.GetArrayAdapter(SharedRaceStartPointMValueAdapter.Instance).FromMValue(reader);
                        break;
                    case "racePoints":
                        dto.RacePoints = DefaultMValueAdapters.GetArrayAdapter(SharedRacePointMValueAdapter.Instance).FromMValue(reader);
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }
            reader.EndObject();
            return dto;
        }

        public override void ToMValue(RaceMapDto value, IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("id");
            writer.Value(value.Id);
            writer.Name("name");
            writer.Value(value.Name);
            writer.Name("startPoints");
            DefaultMValueAdapters.GetArrayAdapter(SharedRaceStartPointMValueAdapter.Instance).ToMValue(value.StartPoints ?? new List<SharedRaceStartPoint>(), writer);
            writer.Name("racePoints");
            DefaultMValueAdapters.GetArrayAdapter(SharedRacePointMValueAdapter.Instance).ToMValue(value.RacePoints ?? new List<SharedRacePoint>(), writer);
            writer.EndObject();
        }
    }
}
