using System.Numerics;
using Proton.Client.Core.Models;

namespace Proton.Client.Core.Interfaces;

public interface IRaycastService
{
    Task<RaycastData?> RaycastAsync(Vector3 startPosition, Vector3 endPosition, CancellationToken ct = default);
    void StartAsyncRaycastBatch(
        Func<(Vector3 StartPosition, Vector3 EndPosition)?> produce,
        Action<RaycastData> onFinished
    );
    void StopAsyncRaycastBatch();
}
