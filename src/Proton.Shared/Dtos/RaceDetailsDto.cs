using AltV.Community.MValueAdapters.Generators;
using AltV.Community.MValueAdapters.Generators.Abstractions;

namespace Proton.Shared.Dtos;

[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public class RaceDetailsDto
{
    public long Id { get; set; }
    public string Host { get; set; } = string.Empty;
    public long MapId { get; set; }
    public string[] VehicleModels { get; set; } = [];
    public int Duration { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool Ghosting { get; set; }
    public byte Type { get; set; }
    public int? Laps { get; set; }
    public string Time { get; set; } = string.Empty;
    public string Weather { get; set; } = string.Empty;
    public List<RaceParticipantDto> Participants { get; set; } = [];
}
