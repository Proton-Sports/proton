using AltV.Community.MValueAdapters.Generators;
using AltV.Community.MValueAdapters.Generators.Abstractions;

namespace Proton.Shared.Dtos;

[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public sealed record MountScoreboardDto
{
    public string Name { get; set; } = string.Empty;
    public List<ScoreboardParticipantDto> Participants { get; set; } = null!;
}

[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public sealed record ScoreboardParticipantDto
{
    public string Name { get; set; } = string.Empty;
    public string Team { get; set; } = string.Empty;
    public long TimeMs { get; set; }
}
