namespace Proton.Server.Resource.Features.Races.Abstractions;

public sealed class RacePointResolverOutput
{
    public int Index { get; set; }
    public int Lap { get; set; }
    public bool Finished { get; set; }
    public int? NextIndex { get; set; }
}
