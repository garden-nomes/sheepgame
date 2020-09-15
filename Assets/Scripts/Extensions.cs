using UnityEngine;

public static class Vector2Extensions
{
    public static Vector2 Rotate(this Vector2 vector, float degrees)
    {
        return Quaternion.Euler(0, 0, degrees) * vector;
    }

    public static bool LessThan(this Vector2 vector, float radius)
    {
        return vector.sqrMagnitude < radius * radius;
    }
}

public static class Vector3Extensions
{
    public static bool LessThan(this Vector3 vector, float radius)
    {
        return vector.sqrMagnitude < radius * radius;
    }
}

public static class FloatExtensions
{
    public static float Sq(this float value)
    {
        return value * value;
    }

    public static float Map(this float x, float a0, float b0, float a1, float b1)
    {
        return (x - a0) / (b0 - a0) * (b1 - a1) + a1;
    }
}
