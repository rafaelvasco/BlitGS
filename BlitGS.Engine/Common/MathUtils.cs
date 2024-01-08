using System.Runtime.CompilerServices;

namespace BlitGS.Engine;

public static class MathUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Swap(ref int v1, ref int v2) => (v1, v2) = (v2, v1);
}