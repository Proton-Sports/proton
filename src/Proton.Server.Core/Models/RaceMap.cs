using Proton.Server.Core.Interfaces;

namespace Proton.Server.Core.Models;

public class RaceMap : IAggregateRoot
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public IList<RacePoint> RacePoints { get; set; } = null!;
    public IList<RaceStartPoint> StartPoints { get; set; } = null!;
    public string? IplName { get; set; }
}
