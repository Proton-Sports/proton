using AltV.Community.MValueAdapters.Generators;
using AltV.Community.MValueAdapters.Generators.Abstractions;

namespace Proton.Shared.Dtos;

[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public sealed class RaceHudParticipantDto
{
    public uint Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Lap { get; set; }
    public float Distance { get; set; }
    public float PartialDistance { get; set; }
    public float SpeedPerHour { get; set; }
}
