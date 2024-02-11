using Proton.Server.Core.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Proton.Server.Core.Models;

public class RaceMap : IAggregateRoot
{
    [Key]
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<RacePoint> RacePoints { get; set; } = null!;
    public ICollection<RaceStartPoint> StartPoints { get; set; } = null!;
}
