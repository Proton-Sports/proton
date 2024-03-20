namespace AltV.Net.Data;

public static class PositionExtensions
{
    public static float GetDistanceSquaredTo(this Position a, Position b)
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        var dz = a.Z - b.Z;
        return dx * dx + dy * dy + dz * dz;
    }
}
