using AltV.Community.MValueAdapters.Generators.Abstractions;
using AltV.Community.MValueAdapters.Generators;

namespace Proton.Shared.Dtos;
[MValueAdapter(NamingConvention = NamingConvention.CamelCase)]
public class TuningDto
{
    public List<TuningWrapperDto> TuningWrappers { get; set; } = new List<TuningWrapperDto>();
    //public int MaxTune { get; set; }
    //public int CurrentTune { get; set; }
}
