using System.Runtime.CompilerServices;

namespace BlitGS.Engine;

public static class MathUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Swap(ref int v1, ref int v2) => (v1, v2) = (v2, v1);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Clamp(float value, float min, float max)
    {
        return (value > max) ? max : (value < min) ? min : value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Clamp(int value, int min, int max)
    {
        return (value > max) ? max : (value < min) ? min : value;
    }
}