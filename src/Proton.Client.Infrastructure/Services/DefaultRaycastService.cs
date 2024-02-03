using System.Numerics;
using AltV.Net.Client;
using AltV.Net.Client.Async;
using Proton.Client.Core.Interfaces;
using Proton.Client.Core.Models;

namespace Proton.Client.Infrastructure.Services;

public class DefaultRaycastService : IRaycastService
{
    private CancellationTokenSource? cts;
    private int shapeTestHandle = -1;

    private Action<RaycastData>? onFinished = default;
    private Func<(Vector3 StartPosition, Vector3 EndPosition)?>? produce = default;

    public async Task<RaycastData?> RaycastAsync(Vector3 startPosition, Vector3 endPosition, CancellationToken ct = default)
    {
        if (ct.IsCancellationRequested) return null;

        var handle = Alt.Natives.StartShapeTestLosProbe(startPosition.X, startPosition.Y, startPosition.Z, endPosition.X, endPosition.Y, endPosition.Z, 1, 0, 4);
        bool hit = default;
        Vector3 endCoords = default, surfaceNormal = default;
        uint entityHit = default;
        var result = Alt.Natives.GetShapeTestResult(handle, ref hit, ref endCoords, ref surfaceNormal, ref entityHit);
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(5));
        while (result != 0 && result != 2)
        {
            if (ct.IsCancellationRequested) return null;

            await timer.WaitForNextTickAsync(ct).ConfigureAwait(false);
            result = await AltAsync.Do(() => Alt.Natives.GetShapeTestResult(handle, ref hit, ref endCoords, ref surfaceNormal, ref entityHit)).ConfigureAwait(false);
        }
        return new RaycastData(hit, entityHit, endCoords, surfaceNormal);
    }

    public void StartAsyncRaycastBatch(Func<(Vector3 StartPosition, Vector3 EndPosition)?> produce, Action<RaycastData> onFinished)
    {
        cts = new();
        this.onFinished = onFinished;
        this.produce = produce;
        Alt.OnTick += ProduceRaycast;
        new Thread((state) =>
        {
            Work((CancellationToken)state!);
            cts.Dispose();
            shapeTestHandle = -1;
            this.onFinished = null;
            this.produce = null;
            cts = null;
        }).Start(cts.Token);
    }

    public void StopAsyncRaycastBatch()
    {
        Alt.OnTick -= ProduceRaycast;
        cts!.Cancel();
    }

    private void Work(CancellationToken ct)
    {
        bool hit = default;
        Vector3 endCoords = default, surfaceNormal = default;
        uint entityHit = default;
        while (!ct.IsCancellationRequested)
        {
            if (shapeTestHandle != -1)
            {
                var result = AltAsync.Do(() => Alt.Natives.GetShapeTestResult(shapeTestHandle, ref hit, ref endCoords, ref surfaceNormal, ref entityHit)).Result;
                if (ct.IsCancellationRequested) return;
                if (result != 1)
                {
                    shapeTestHandle = -1;
                }
                AltAsync.RunOnMainThread((state) => onFinished?.Invoke((RaycastData)state), new RaycastData(hit, entityHit, endCoords, surfaceNormal));
            }
            else
            {
                Thread.Sleep(10);
            }
        }
    }

    private void ProduceRaycast()
    {
        var tuple = produce!();
        if (!tuple.HasValue) return;

        var (start, end) = tuple.Value;
        if (shapeTestHandle == -1)
        {
            shapeTestHandle = Alt.Natives.StartShapeTestLosProbe(start.X, start.Y, start.Z, end.X, end.Y, end.Z, 1, 0, 4);
        }
    }
}
