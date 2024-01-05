using System.Numerics;

namespace Proton.Server.Core.Models;

public class RaceStartPoint
{
    public long MapId { get; set; }
    public RaceMap Map { get; set; } = null!;
    public int Index { get; set; }
    public Vector3 Position { get; set; }
}
