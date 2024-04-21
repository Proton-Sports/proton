using AltV.Community.MValueAdapters.Generators;
using AltV.Community.MValueAdapters.Generators.Abstractions;

namespace Proton.Shared.Dtos;

[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public class RaceCountdownDataDto
{
    public string MapName { get; set; } = string.Empty;
    public long EndTime { get; set; }
    public int Participants { get; set; }
    public int MaxParticipants { get; set; }
}
