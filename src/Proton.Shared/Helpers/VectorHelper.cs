using System.Numerics;
using AltV.Net.Data;

namespace Proton.Shared.Helpers;

public static class VectorHelper
{
    public static Vector2 ConvertRotationToForwardVector2D(Vector3 rotation, bool isRadian = true)
    {
        var rotationInRadian = isRadian ? rotation : rotation * MathF.PI / 180f;
        rotationInRadian.Z += MathF.PI / 2;
        return Vector2.Normalize(new Vector2(MathF.Cos(rotationInRadian.Z), MathF.Sin(rotationInRadian.Z)));
    }

    public static Vector2 ConvertRotationToRightVector2D(Vector3 rotation, bool isRadian = true)
    {
        var rotationInRadian = isRadian ? rotation : rotation * MathF.PI / 180f;
        return Vector2.Normalize(new Vector2(MathF.Cos(rotationInRadian.Z), MathF.Sin(rotationInRadian.Z)));
    }

    public static Vector3 ConvertRotationToForwardVector(Vector3 rotation, bool isRadian = true)
    {
        rotation = isRadian ? rotation : rotation * MathF.PI / 180f;
        var cosX = MathF.Abs(MathF.Cos(rotation.X));
        var x = cosX * -MathF.Sin(rotation.Z);
        var y = cosX * MathF.Cos(rotation.Z);
        var z = MathF.Sin(rotation.X);
        return Vector3.Normalize(new Vector3(x, y, z));
    }

    public static Vector3 ConvertRotationToRightVector(Vector3 rotation, bool isRadian = true)
    {
        var rotationInRadian = isRadian ? rotation : rotation * MathF.PI / 180f;
        return Vector3.Normalize(new Vector3(MathF.Cos(rotationInRadian.Z), MathF.Sin(rotationInRadian.Z), MathF.Sin(rotationInRadian.X)));
    }
}
