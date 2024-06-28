using AltV.Community.MValueAdapters.Generators;
using AltV.Community.MValueAdapters.Generators.Abstractions;

namespace Proton.Shared.Dtos;

[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public sealed class RaceHudTickDto
{
    public List<RaceHudParticipantTickDto> Participants { get; set; } = null!;
}
