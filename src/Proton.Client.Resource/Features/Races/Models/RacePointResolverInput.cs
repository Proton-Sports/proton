namespace Proton.Client.Resource.Features.Races.Models;

public sealed class RacePointResolverInput
{
    public int Index { get; set; }
    public int Lap { get; set; }
    public int TotalPoints { get; set; }
    public int TotalLaps { get; set; }
}
