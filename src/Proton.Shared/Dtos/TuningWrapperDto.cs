
using AltV.Community.MValueAdapters.Generators.Abstractions;
using AltV.Community.MValueAdapters.Generators;

namespace Proton.Shared.Dtos;
[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public class TuningWrapperDto
{
    public List<TuningDetailDto> Tunings { get; set; } = new List<TuningDetailDto>();
    public string Name { get; set; } = string.Empty;
}
