using AltV.Net;
using AltV.Net.Elements.Args;
using Proton.Shared.Adapters;

namespace Proton.Shared.Dtos;

public sealed record class RaceHostSubmitDto : IMValueConvertible
{
    public long MapId { get; set; }
    public string VehicleName { get; set; } = string.Empty;
    public int Racers { get; set; }
    public int Duration { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool Ghosting { get; set; }
    public string Type { get; set; } = string.Empty;
    public int? Laps { get; set; }
    public string Time { get; set; } = string.Empty;
    public string? ExactTime { get; set; }
    public string Weather { get; set; } = string.Empty;

    public IMValueBaseAdapter GetAdapter() => Adapter.Instance;

    public class Adapter : BaseMValueAdapter<RaceHostSubmitDto>
    {
        public static readonly Adapter Instance = new();

        public override RaceHostSubmitDto FromMValue(IMValueReader reader)
        {
            if (reader.Peek() == MValueReaderToken.Nil) return default!;

            var dto = new RaceHostSubmitDto();
            reader.BeginObject();
            while (reader.HasNext())
            {
                switch (reader.NextName())
                {
                    case "mapId":
                        dto.MapId = (long)reader.NextDouble();
                        break;
                    case "vehicleName":
                        dto.VehicleName = reader.NextString();
                        break;
                    case "racers":
                        dto.Racers = (int)reader.NextDouble();
                        break;
                    case "duration":
                        dto.Duration = (int)reader.NextDouble();
                        break;
                    case "description":
                        dto.Description = reader.NextString();
                        break;
                    case "ghosting":
                        dto.Ghosting = reader.NextBool();
                        break;
                    case "type":
                        dto.Type = reader.NextString();
                        break;
                    case "laps":
                        dto.Laps = (int)reader.NextDouble();
                        // else reader.SkipValue();
                        break;
                    case "time":
                        dto.Time = reader.NextString();
                        break;
                    case "exactTime":
                        dto.ExactTime = reader.NextString();
                        break;
                    case "weather":
                        dto.Weather = reader.NextString();
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }
            reader.EndObject();
            return dto;
        }

        public override void ToMValue(RaceHostSubmitDto value, IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("mapId");
            writer.Value(value.MapId);
            writer.Name("vehicleName");
            writer.Value(value.VehicleName);
            writer.Name("racers");
            writer.Value(value.Racers);
            writer.Name("duration");
            writer.Value(value.Duration);
            writer.Name("description");
            writer.Value(value.Description);
            writer.Name("ghosting");
            writer.Value(value.Ghosting);
            writer.Name("type");
            writer.Value(value.Type);
            if (value.Laps is not null)
            {
                writer.Name("laps");
                writer.Value((int)value.Laps);
            }
            writer.Name("time");
            writer.Value(value.Time);
            if (value.ExactTime is not null)
            {
                writer.Name("exactTime");
                writer.Value(value.ExactTime);
            }
            writer.Name("weather");
            writer.Value(value.Weather);
            writer.EndObject();
        }
    }
}
