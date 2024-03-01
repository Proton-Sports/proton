namespace Proton.Client.Resource.Features.Races.Models;

public sealed class RacePointResolverOutput
{
    public int Index { get; set; }
    public int Lap { get; set; }
    public bool Finished { get; set; }
    public int? NextIndex { get; set; }
}
