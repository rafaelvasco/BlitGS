using System;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace BlitGS.Engine;

public static class MathUtils
{
    public const float Pi = 3.141592653589793239f;
    public const float HalfPi = 1.570796326794896619f;
    public const float QuarterPi = 0.785398163397448310f;
    public const float TwoPi = 6.283185307179586f;
    public const float InvTwoPi = (float)(1.0 / 6.283185307179586);
    public const float PiTimesPi = (float)(3.141592653589793239 * 3.141592653589793239);

    public static readonly float MachineEpsilonFloat = GetMachineEpsilonFloat();

    private static float GetMachineEpsilonFloat()
    {
        float machineEpsilon = 1.0f;
        float comparison;

        /* Keep halving the working value of machineEpsilon until we get a number that
         * when added to 1.0f will still evaluate as equal to 1.0f.
         */
        do
        {
            machineEpsilon *= 0.5f;
            comparison = 1.0f + machineEpsilon;
        } while (comparison > 1.0f);

        return machineEpsilon;
    }

    #region Base

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Abs(int n)
    {
        var mask = n >> 31;
        return (n ^ mask) - mask;
    }

    /// <summary>
    /// Calculate the SquareRoot of an Integer only.
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SqrtI(int val)
    {
        if (val == 0)
        {
            return 0;
        }

        int n = (val / 2) + 1;
        int n1 = (n + (val / n)) / 2;

        while (n1 < n)
        {
            n = n1;
            n1 = (n + (val / n)) / 2;
        }

        return n;
    }

    /// <summary>
    /// Returns an approximation of the inverse square root of left number.
    /// </summary>
    /// <param name="x">A number.</param>
    /// <returns>An approximation of the inverse square root of the specified number, with an upper error bound of 0.001.</returns>
    /// <remarks>
    /// This is an improved implementation of the the method known as Carmack's inverse square rootMath
    /// which is found in the Quake III source code. This implementation comes from
    /// http://www.codemaestro.com/reviews/review00000105.html. For the history of this method, see
    /// http://www.beyond3d.com/content/articles/8/.
    /// </remarks>
    [Pure]
    public static float InverseSqrtFast(float x)
    {
        unsafe
        {
            var xhalf = 0.5f * x;
            var i = *(int*)&x; // Read bits as integer.
            i = 0x5f375a86 - (i >> 1); // Make an initial guess for Newton-Raphson approximation
            x = *(float*)&i; // Convert bits back to float
            x *= (1.5f - (xhalf * x * x)); // Perform left single Newton-Raphson step.
            return x;
        }
    }

    /// <summary>
    /// checks to see if two values are approximately the same using the value of Epsilon
    /// </summary>
    /// <param name="value1">Value1.</param>
    /// <param name="value2">Value2.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ApproximatelyEqual(float value1, float value2)
    {
        return Math.Abs(value1 - value2) <= MachineEpsilonFloat;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int NextPowerOfTwo(int x)
    {
        x--;
        x |= x >> 1;
        x |= x >> 2;
        x |= x >> 4;
        x |= x >> 8;
        x |= x >> 16;
        x++;
        return x;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPowerOfTwo(int x)
    {
        return ((x & (x - 1)) == 0);
    }

    public static float IntBitsToFloat(int value)
    {
        var bytes = BitConverter.GetBytes(value & 0xFEFFFFFF);
        return BitConverter.ToSingle(bytes, 0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Swap(ref float v1, ref float v2) => (v1, v2) = (v2, v1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Swap(ref int v1, ref int v2) => (v1, v2) = (v2, v1);

    #endregion

    #region Angles

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float FasterSin(float radians)
    {
        BlitException.Assert(radians is >= (-MathUtils.Pi) and <= MathUtils.Pi);
        if (radians < 0)
            return radians * (1.27323954f + (0.405284735f * radians));
        else
            return radians * (1.27323954f - (0.405284735f * radians));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float FastSin(float radians)
    {
        const float negTwoPi = -MathUtils.TwoPi;
        var pre = MathF.Floor((radians + MathUtils.Pi) * MathUtils.InvTwoPi);
        var ranged = MathF.FusedMultiplyAdd(negTwoPi, pre, radians);

        var xa = (MathUtils.Pi - ranged) * ranged;
        var over = 16 * xa;
        var under = MathF.FusedMultiplyAdd(5, MathUtils.PiTimesPi, (xa * -4));
        return over / under;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float FastCos(float radians)
    {
        return FastSin(radians + MathUtils.HalfPi);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double DegreesToRadians(double degrees)
    {
        return 0.017453292519943295 * degrees;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float DegreesToRadians(float degrees)
    {
        return (float)(0.017453292519943295 * degrees);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float RadiansToDegrees(float radians)
    {
        return (float)(57.295779513082320876798154814105 * radians);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double RadiansToDegrees(double radians)
    {
        return 57.295779513082320876798154814105 * radians;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float AngleBetweenVectors(Vector2 from, Vector2 to)
    {
        return (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
    }

    /// <summary>
    /// Calculates the shortest difference between two given angles in degrees
    /// </summary>
    /// <returns>The angle.</returns>
    /// <param name="current">Current.</param>
    /// <param name="target">Target.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float DeltaAngle(float current, float target)
    {
        var num = Repeat(target - current, 360f);
        if (num > 180f)
            num -= 360f;

        return num;
    }

    /// <summary>
    /// Calculates the shortest difference between two given angles given in radians
    /// </summary>
    /// <returns>The angle.</returns>
    /// <param name="current">Current.</param>
    /// <param name="target">Target.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float DeltaAngleRadians(float current, float target)
    {
        var num = Repeat(target - current, TwoPi);
        if (num > Pi)
            num -= TwoPi;

        return num;
    }

    /// <summary>
    /// Clamps an angle to the range [0, 360).
    /// </summary>
    /// <param name="angle">The angle to clamp in degrees.</param>
    /// <returns>The clamped angle in the range [0, 360).</returns>
    public static float ClampAngle(float angle)
    {
        // mod angle so it's in the range (-360, 360)
        angle %= 360f;

        // abs angle so it's in the range [0, 360)
        angle = MathF.Abs(angle);

        return angle;
    }

    /// <summary>
    /// Clamps an angle to the range [0, 2π).
    /// </summary>
    /// <param name="angle">The angle to clamp in radians.</param>
    /// <returns>The clamped angle in the range [0, 2π).</returns>
    public static float ClampRadians(float angle)
    {
        // mod angle so it's in the range (-2π,2π)
        angle %= 2 * Pi;

        // abs angle so it's in the range [0,2π)
        angle = MathF.Abs(angle);

        return angle;
    }

    #endregion

    #region Clamping

    /// <summary>
    /// Ceils the float to the nearest int value above y. note that this only works for values in the range of short
    /// </summary>
    /// <returns>The ceil to int.</returns>
    /// <param name="value">F.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int FastCeilToInt(float value)
    {
        return 32768 - (int)(32768f - value);
    }

    [Pure]
    public static float Floor(float n) => (float)Math.Floor(n);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Max(int x, int y)
    {
        int z = x - y;
        return x - ((z >> 31) & z);
    }
    
    public static int Max(int v1, int v2, int v3)
    {
        int max = v1;
        if (v2 > max) max = v2;
        if (v3 > max) max = v3;
        return max;
    }
    
    public static int Max(int v1, int v2, int v3, int v4)
    {
        int max = v1;
        if (v2 > max) max = v2;
        if (v3 > max) max = v3;
        if (v4 > max) max = v4;
        return max;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Max(float a, float b)
    {
        return a > b ? a : b;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Max(float a, float b, float c)
    {
        return a > b ? (a > c ? a : c) : (b > c ? b : c);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Max(double a, double b)
    {
        return a > b ? a : b;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Min(int a, int b)
    {
        return a < b ? a : b;
    }
    
    public static int Min(int v1, int v2, int v3)
    {
        int min = v1;
        if (v2 < min) min = v2;
        if (v3 < min) min = v3;
        return min;
    }
    
    public static int Min(int v1, int v2, int v3, int v4)
    {
        int min = v1;
        if (v2 < min) min = v2;
        if (v3 < min) min = v3;
        if (v4 < min) min = v4;
        return min;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Min(uint a, uint b)
    {
        return a < b ? a : b;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Min(float a, float b)
    {
        return a < b ? a : b;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Min(double a, double b)
    {
        return a < b ? a : b;
    }

    /// <summary>
    /// Clamps float between 0f and 1f
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Normalize(float value)
    {
        if (value < 0f)
            return 0f;

        return value > 1f ? 1f : value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Normalize(float var, float min, float max)
    {
        if (var >= min && var < max) return var;

        if (var < min)
            var = max + ((var - min) % max);
        else
            var = min + (var % (max - min));

        return var;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Clamp(double value, double min, double max)
    {
        return (value > max) ? max : (value < min) ? min : value;
    }

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

    /// <summary>
    /// Restricts a value to be multiple of increment.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="increment"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Snap(float value, float increment)
    {
        return MathF.Round(value / increment) * increment;
    }

    public static float CeilSnap(float value, float increment)
    {
        return (MathF.Ceiling(value / increment) * increment);
    }

    #endregion

    #region Animation and Movement

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float SmoothStep(float value1, float value2, float amount)
    {
        float num = MathUtils.Clamp(amount, 0f, 1f);
        return Interpolate(value1, value2, (num * num) * (3f - (2f * num)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double SmoothStep(double value1, double value2, double amount)
    {
        var num = Clamp(amount, 0.0, 1.0);
        return Interpolate(value1, value2, (num * num) * (3f - (2f * num)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float SmoothStep(float amount)
    {
        float num = MathUtils.Clamp(amount, 0f, 1f);
        return (num * num) * (3f - (2f * num));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double SmoothStep(double amount)
    {
        var num = Clamp(amount, 0.0, 1.0);
        return (num * num) * (3.0 - (2.0 * num));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double SmootherStep(double amount)
    {
        var num = Clamp(amount, 0.0, 1.0);
        return num * num * num * ((num * ((num * 6.0) - 15.0)) + 10.0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Interpolate(float a, float b, float t)
    {
        return MathF.FusedMultiplyAdd(a, (1 - t), (b * t));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte Interpolate(byte a, byte b, float t)
    {
        return (byte)((a * (1 - t)) + (b * t));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Interpolate(double a, double b, double t)
    {
        return (a * (1 - t)) + (b * t);
    }

    /// <summary>
    /// Loops t so that it is never larger than length and never smaller than 0
    /// </summary>
    /// <param name="t">T.</param>
    /// <param name="length">Length.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Repeat(float t, float length)
    {
        return t - (Floor(t / length) * length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Wrap(float var, float min, float max)
    {
        if (var < min)
        {
            var += max;
        }

        var %= max;

        return var;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Wrap(int var, int min, int max)
    {
        if (var < min)
        {
            var += max;
        }

        var %= max;

        return var;
    }

    /// <summary>
    /// ping-pongs t so that it is never larger than length and never smaller than 0
    /// </summary>
    /// <returns>The pong.</returns>
    /// <param name="t">T.</param>
    /// <param name="length">Length.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float PingPong(float t, float length)
    {
        t = Repeat(t, length * 2f);
        return length - MathF.Abs(t - length);
    }

    /// <summary>
    /// Moves start towards end by shift amount clamping the result. start can be less than or greater than end.
    /// example: start is 2, end is 10, shift is 4 results in 6
    /// </summary>
    /// <param name="start">Start.</param>
    /// <param name="end">End.</param>
    /// <param name="shift">Shift.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Approach(float start, float end, float shift)
    {
        return start < end ? Math.Min(start + shift, end) : Math.Max(start - shift, end);
    }

    public static float Accelerate(float velocity, float minSpeed, float maxSpeed, float acceleration, float dt)
    {
        float min = minSpeed * dt;
        float max = maxSpeed * dt;

        return Clamp((velocity * dt) + (0.5f * acceleration * dt * dt), min, max);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 RotateAround(Vector2 point, Vector2 center, float angleRadians)
    {
        var cos = FastCos(angleRadians);
        var sin = FastSin(angleRadians);
        var rotatedX = (cos * (point.X - center.X)) - (sin * (point.Y - center.Y)) + center.X;
        var rotatedY = (sin * (point.X - center.X)) + (cos * (point.Y - center.Y)) + center.Y;

        return new Vector2(rotatedX, rotatedY);
    }

    #endregion

    #region Base Geometry

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Distance(Vector2 vec1, Vector2 vec2)
    {
        return (float)Math.Sqrt(((vec2.X - vec1.X) * (vec2.X - vec1.X)) + ((vec2.Y - vec1.Y) * (vec2.Y - vec1.Y)));
    }



 

    /// <summary>
    /// Gets a point on the circumference of the circle given its center, radius and angle. 0 degrees is 3 o'clock.
    /// </summary>
    /// <returns>The on circle.</returns>
    /// <param name="circleCenter">Circle center.</param>
    /// <param name="radius">Radius.</param>
    /// <param name="angleInDegrees">Angle in degrees.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 PointOnCircle(Vector2 circleCenter, float radius, float angleInDegrees)
    {
        var radians = DegreesToRadians(angleInDegrees);
        return new Vector2
        {
            X = (FastCos(radians) * radius) + circleCenter.X,
            Y = (FastSin(radians) * radius) + circleCenter.Y
        };
    }

    public static bool PointInRect(Vector2 point, float rx, float ry, float rw, float rh)
    {
        if (point.X <= rx) return false;
        if (point.X >= rx + rw) return false;
        if (point.Y <= ry) return false;
        if (point.Y >= ry + rh) return false;

        return true;
    }

    public static float DistanceRectPoint(Vector2 point, float rx, float ry, float rw, float rh)
    {
        if (point.X >= rx && point.X <= rx + rw)
        {
            if (point.Y >= ry && point.Y <= ry + rh) return 0;

            if (point.Y > ry) return point.Y - (ry + rh);

            return ry - point.Y;
        }

        if (point.Y >= ry && point.Y <= ry + rh)
        {
            if (point.X > rx) return point.X - (rx + rw);

            return rx - point.X;
        }

        if (point.X > rx)
        {
            if (point.Y > ry) return Distance(point, new Vector2(rx + rw, ry + rh));

            return Distance(point, new Vector2(rx + rw, ry));
        }

        return Distance(point, point.Y > ry ? new Vector2(rx, ry + rh) : new Vector2(rx, ry));
    }

    #endregion

    #region Packing

    /// <summary>
    /// Pack 3 ints into one. Each can have a max value of 1023.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    public static int Pack3(int a, int b, int c)
    {
        if (a > 1023)
        {
            throw new ArgumentOutOfRangeException(nameof(a));
        }

        if (b > 1023)
        {
            throw new ArgumentOutOfRangeException(nameof(b));
        }

        if (c > 1023)
        {
            throw new ArgumentOutOfRangeException(nameof(c));
        }

        return (a << 20) | (b << 10) | c;
    }

    public static (int a, int b, int c) Unpack(int packed)
    {
        return ((packed >> 20) & 0x3FF, (packed >> 10) & 0x3FF, (packed) & 0x3FF);
    }

    #endregion

    public static float[] ConvertByteArrayToFloat(byte[] array)
    {
        float[] floatArr = new float[array.Length / 4];
        for (int i = 0; i < floatArr.Length; i++)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(array, i * 4, 4);
            floatArr[i] = BitConverter.ToSingle(array, i * 4) / 0x80000000;
        }

        return floatArr;
    }

    public static byte[] ConvertFloatArrayToByte(float[] array)
    {
        byte[] byteArr = new byte[array.Length * 4];
        for (int i = 0; i < array.Length; i++)
        {
            var bytes = BitConverter.GetBytes(array[i] * 0x80000000);
            Array.Copy(bytes, 0, byteArr, i * 4, bytes.Length);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(byteArr, i * 4, 4);
        }

        return byteArr;
    }
}