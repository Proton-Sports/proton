using System.Numerics;
using AltV.Net.Client;
using Proton.Client.Core.Interfaces;
using Proton.Shared.Helpers;

namespace Proton.Client.Infrastructure.Services;

public class DefaultGameplayCamera : IGameplayCamera
{
    private const int RotationOrder = 2;

    public bool IsRendering => Alt.Natives.IsGameplayCamRendering();

    public Vector3 Position => Alt.Natives.GetGameplayCamCoord();
    public Vector3 Rotation => Alt.Natives.GetGameplayCamRot(RotationOrder);
    public Vector3 ForwardVector => VectorHelper.ConvertRotationToForwardVector(Rotation, isRadian: false);
}
