using Proton.Server.Core.Interfaces;

namespace Proton.Server.Core.Models;

public class RaceMap : IAggregateRoot
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<RacePoint> RacePoints { get; set; } = null!;
    public ICollection<RaceStartPoint> StartPoints { get; set; } = null!;
}
