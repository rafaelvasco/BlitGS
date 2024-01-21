using System;
using System.Numerics;

namespace BlitGS.Engine;

public static class Vector2Ext
{
    public static void Normalize(ref this Vector2 vector)
    {
        float val = 1.0f / (float)Math.Sqrt((vector.X * vector.X) + (vector.Y * vector.Y));
        vector.X *= val;
        vector.Y *= val;
    }

    public static void Deconstruct(this Vector2 vector, out float x, out float y)
    {
        x = vector.X;
        y = vector.Y;
    }
}