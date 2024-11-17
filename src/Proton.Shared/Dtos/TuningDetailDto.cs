using AltV.Community.MValueAdapters.Generators.Abstractions;
using AltV.Community.MValueAdapters.Generators;

namespace Proton.Shared.Dtos;
[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public class TuningDetailDto
{
    public string Name { get; set; } = string.Empty;
    public int Price { get; set; } = 999999;
    public int CategoryId { get; set; }
    public byte Value { get; set; }
}
