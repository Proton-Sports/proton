using System.Numerics;
using Proton.Server.Core.Interfaces;

namespace Proton.Server.Core.Models;

public class RaceStartPoint : IAggregateRoot
{
    public long MapId { get; set; }
    public RaceMap Map { get; set; } = null!;
    public int Index { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Rotation { get; set; }
}
