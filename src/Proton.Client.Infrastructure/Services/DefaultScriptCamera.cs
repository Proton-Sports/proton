using System.Numerics;
using AltV.Net.Client;
using Proton.Client.Core.Interfaces;
using Proton.Shared.Helpers;

namespace Proton.Client.Infrastructure.Services;

public class DefaultScriptCamera(int cameraId) : IScriptCamera
{
    private const int RotationOrder = 2;
    private bool disposed;

    public int Id { get; } = cameraId;

    public bool IsActive
    {
        get => Alt.Natives.IsCamActive(Id);
        set => Alt.Natives.SetCamActive(Id, value);
    }

    public bool IsRendering => Alt.Natives.GetRenderingCam() == Id;

    public Vector3 Position
    {
        get => Alt.Natives.GetCamCoord(Id);
        set => Alt.Natives.SetCamCoord(Id, value.X, value.Y, value.Z);
    }

    public Vector3 Rotation
    {
        get => Alt.Natives.GetCamRot(Id, RotationOrder);
        set => Alt.Natives.SetCamRot(Id, value.X, value.Y, value.Z, RotationOrder);
    }

    public Vector3 ForwardVector =>
        VectorHelper.ConvertRotationToForwardVector(Rotation * MathF.PI / 180);

    public float Fov
    {
        get => Alt.Natives.GetCamFov(Id);
        set => Alt.Natives.SetCamFov(Id, value);
    }

    public static DefaultScriptCamera From(int cameraId) => new(cameraId);

    public void SetActiveWithInterpolation(TimeSpan duration, bool easeLocation, bool easeRotation)
    {
        SetActiveWithInterpolation(
            Id,
            Alt.Natives.GetRenderingCam(),
            duration,
            easeLocation,
            easeRotation
        );
    }

    public void SetActiveWithInterpolation(
        IScriptCamera fromCamera,
        TimeSpan duration,
        bool easeLocation,
        bool easeRotation
    )
    {
        SetActiveWithInterpolation(Id, fromCamera.Id, duration, easeLocation, easeRotation);
    }

    public virtual void Render() => Render(true, false, TimeSpan.Zero);

    public virtual void Render(TimeSpan easeTime) => Render(true, true, easeTime);

    public void Unrender() => Render(false, false, TimeSpan.Zero);

    public void Unrender(TimeSpan easeTime) => Render(false, true, easeTime);

    private static void SetActiveWithInterpolation(
        int toCamera,
        int fromCamera,
        TimeSpan duration,
        bool easeLocation,
        bool easeRotation
    )
    {
        Alt.Natives.SetCamActiveWithInterp(
            toCamera,
            fromCamera,
            (int)duration.TotalMilliseconds,
            easeLocation ? 1 : 0,
            easeRotation ? 1 : 0
        );
    }

    private static void Render(bool render, bool ease, TimeSpan easeTime)
    {
        Alt.Natives.RenderScriptCams(
            render,
            ease,
            (int)easeTime.TotalMilliseconds,
            false,
            false,
            0
        );
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            // if (disposing) { }
            IsActive = false;
            if (IsRendering)
            {
                Unrender();
            }
            Alt.Natives.DestroyCam(Id, true);
            disposed = true;
        }
    }

    ~DefaultScriptCamera()
    {
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
