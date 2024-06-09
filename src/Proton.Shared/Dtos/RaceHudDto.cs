using AltV.Community.MValueAdapters.Generators;
using AltV.Community.MValueAdapters.Generators.Abstractions;

namespace Proton.Shared.Dtos;

[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public sealed class RaceHudDto
{
    public uint LocalId { get; set; }
    public int MaxLaps { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public List<RaceHudParticipantDto> Participants { get; set; } = null!;
}
