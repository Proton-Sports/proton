using AltV.Community.MValueAdapters.Generators;
using AltV.Community.MValueAdapters.Generators.Abstractions;

namespace Proton.Shared.Dtos;

[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public class RaceCountdownParticipantDto
{
    public uint Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsHost { get; set; }
    public bool IsReady { get; set; }
    public string VehicleModel { get; set; } = string.Empty;
}
