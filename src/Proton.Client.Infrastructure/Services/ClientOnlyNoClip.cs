using System.Numerics;
using AltV.Net.Client;
using AltV.Net.Data;
using Proton.Client.Core.Interfaces;
using Proton.Client.Core.Models;
using Proton.Shared.Helpers;

namespace Proton.Client.Infrastructure.Services;

public class ClientOnlyNoClip : INoClip
{
    private const int ActionForward = 32;
    private const int ActionBackward = 33;
    private const int ActionLeft = 34;
    private const int ActionRight = 35;
    private const int ActionUp = 22;
    private const int ActionDown = 36;
    private const int ActionShift = 21;
    private const int ActionNextCamera = 0;
    private readonly IRaycastService raycastService;
    private readonly IGameplayCamera gameplayCamera;

    private RaycastData? raycastData = default;

    public bool IsStarted { get; protected set; }

    public ClientOnlyNoClip(IRaycastService raycastService, IGameplayCamera gameplayCamera)
    {
        this.raycastService = raycastService;
        this.gameplayCamera = gameplayCamera;
    }

    public void Start()
    {
        IsStarted = true;
        raycastService.StartAsyncRaycastBatch(() =>
        {
            var position = gameplayCamera.Position;
            return (position, position + gameplayCamera.ForwardVector * 1000);
        }, (raycastData) =>
        {
            this.raycastData = raycastData;
        });
        Alt.OnTick += HandleTick;
    }

    public void Stop()
    {
        IsStarted = false;
        raycastService.StopAsyncRaycastBatch();
        Alt.OnTick -= HandleTick;
    }

    public bool TryGetRaycastData(out RaycastData data)
    {
        data = raycastData!;
        return raycastData != default;
    }

    private void HandleTick()
    {
        Alt.Natives.DisableControlAction(0, ActionForward, true);
        Alt.Natives.DisableControlAction(0, ActionBackward, true);
        Alt.Natives.DisableControlAction(0, ActionLeft, true);
        Alt.Natives.DisableControlAction(0, ActionRight, true);
        Alt.Natives.DisableControlAction(0, ActionUp, true);
        Alt.Natives.DisableControlAction(0, ActionDown, true);
        Alt.Natives.DisableControlAction(0, ActionShift, true);
        Alt.Natives.DisableControlAction(0, ActionNextCamera, true);

        var player = Alt.LocalPlayer;
        Vector3 currentPosition = player.Position;
        Vector3 position = currentPosition;
        var speed = 1f;
        var camRotationInRadian = Alt.Natives.GetGameplayCamRot(2) * MathF.PI / 180;
        player.Rotation = new Rotation(0, 0, camRotationInRadian.Z);
        var (forward, right) = ToVectors(camRotationInRadian);
        forward.Z = 0;
        right.Z = 0;
        if (Alt.Natives.IsDisabledControlPressed(0, ActionShift)) speed *= 5;
        if (Alt.Natives.IsDisabledControlPressed(0, ActionForward)) position += forward * speed;
        if (Alt.Natives.IsDisabledControlPressed(0, ActionBackward)) position += forward * (-speed);
        if (Alt.Natives.IsDisabledControlPressed(0, ActionLeft)) position += right * (-speed);
        if (Alt.Natives.IsDisabledControlPressed(0, ActionRight)) position += right * speed;
        if (Alt.Natives.IsDisabledControlPressed(0, ActionUp)) position.Z += speed;
        if (Alt.Natives.IsDisabledControlPressed(0, ActionDown)) position.Z -= speed;

        if (raycastData is { IsHit: true })
        {
            Alt.Natives.DrawMarkerSphere(raycastData.EndPosition.X, raycastData.EndPosition.Y, raycastData.EndPosition.Z, 2, 255, 0, 0, 255);
        }
        if (currentPosition != position)
        {
            Alt.LocalPlayer.Position = position;
        }
    }

    private static (Vector3 Forward, Vector3 Right) ToVectors(Vector3 rotation)
    {
        var right = VectorHelper.ConvertRotationToRightVector2D(rotation);
        var forward = new Vector2(-right.Y, right.X);
        return (new Vector3(forward.X, forward.Y, 0), new Vector3(right.X, right.Y, 0));
    }
}
