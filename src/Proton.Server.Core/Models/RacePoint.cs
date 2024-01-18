using System.Numerics;
using Proton.Server.Core.Interfaces;

namespace Proton.Server.Core.Models;

public class RacePoint : IAggregateRoot
{
    public long MapId { get; set; }
    public RaceMap Map { get; set; } = null!;
    public int Index { get; set; }
    public Vector3 Position { get; set; }
    public float Radius { get; set; }
}
