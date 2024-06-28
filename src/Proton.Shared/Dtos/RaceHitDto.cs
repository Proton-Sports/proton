using AltV.Community.MValueAdapters.Generators;
using AltV.Community.MValueAdapters.Generators.Abstractions;

namespace Proton.Shared.Dtos;

[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public sealed class RaceHitDto
{
    public int Lap { get; set; }
    public int Index { get; set; }
    public int? NextIndex { get; set; }
    public bool Finished { get; set; }
}
