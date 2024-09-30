using AltV.Community.MValueAdapters.Generators;
using AltV.Community.MValueAdapters.Generators.Abstractions;

namespace Proton.Shared.Dtos;

[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public class RaceCountdownDto
{
    public string MapName { get; set; } = string.Empty;
    public uint Id { get; set; }
    public int DurationSeconds { get; set; }
    public string[] Vehicles { get; set; } = [];
    public RaceCountdownParticipantDto[] Participants { get; set; } = [];
    public int MaxParticipants { get; set; }
}
