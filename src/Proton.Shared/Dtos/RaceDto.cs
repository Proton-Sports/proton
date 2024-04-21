using AltV.Community.MValueAdapters.Generators;
using AltV.Community.MValueAdapters.Generators.Abstractions;

namespace Proton.Shared.Dtos;

[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public class RaceDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Participants { get; set; }
    public int MaxParticipants { get; set; }
    public byte Status { get; set; }
}
