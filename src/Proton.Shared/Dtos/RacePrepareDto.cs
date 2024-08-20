using AltV.Community.MValueAdapters.Generators;
using AltV.Community.MValueAdapters.Generators.Abstractions;
using Proton.Shared.Models;

namespace Proton.Shared.Dtos;

[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public sealed class RacePrepareDto
{
    public int Dimension { get; set; }
    public long EndTime { get; set; }
    public byte RaceType { get; set; }
    public List<RacePointDto> RacePoints { get; set; } = [];
    public string? IplName { get; set; }
    public bool DisableLoadingCheckpoint { get; set; }
}
