using System.Numerics;

namespace Proton.Client.Core.Interfaces;

public interface IGameplayCamera
{
    bool IsRendering { get; }
    Vector3 Position { get; }
    Vector3 Rotation { get; }
    Vector3 ForwardVector { get; }
}
