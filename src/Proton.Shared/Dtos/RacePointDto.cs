using AltV.Community.MValueAdapters.Generators;
using AltV.Community.MValueAdapters.Generators.Abstractions;
using AltV.Net.Data;

namespace Proton.Shared.Models;

[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public class RacePointDto
{
    public Position Position { get; set; }
    public float Radius { get; set; }
}
