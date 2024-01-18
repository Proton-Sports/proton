using System.Numerics;

namespace Proton.Client.Core.Models;

public class RaycastData
{
    public bool IsHit { get; }
    public uint HitEntity { get; }
    public Vector3 EndPosition { get; }
    public Vector3 SurfaceNormal { get; }

    public RaycastData(bool isHit, uint hitEntity, Vector3 endPosition, Vector3 surfaceNormal)
    {
        IsHit = isHit;
        HitEntity = hitEntity;
        EndPosition = endPosition;
        SurfaceNormal = surfaceNormal;
    }
}
