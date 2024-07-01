using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BlitGS.Engine;

[StructLayout(LayoutKind.Sequential)]
public struct ColorRGB(byte r, byte g, byte b) : IEquatable<ColorRGB>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte ExtractR(uint color) => (byte)color;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte ExtractG(uint color)
    {
        unchecked
        {
            return (byte)(color >> 8);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte ExtractB(uint color)
    {
        unchecked
        {
            return (byte)(color >> 16);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Build(byte r, byte g, byte b)
    {
        unchecked
        {
            return (uint)(0xFF000000 | (b << 16) | (uint)(g << 8) | r);
        }
    }

    public static uint Build(float r, float g, float b)
    {
        unchecked
        {
            var byteR = (byte)(r * 255f);
            var byteG = (byte)(g * 255f);
            var byteB = (byte)(b * 255f);

            return (uint)(0xFF000000 | (byteB << 16) | (uint)(byteG << 8) | byteR);
        }
    }

    public const uint Empty = 4294902015;
    public const uint Black = 4278190080U;
    public const uint White = 4294967295U;
    public const uint DarkBlue = 4283640605U;
    public const uint Wine = 4283639166U;
    public const uint MiddleGreen = 4283533056U;
    public const uint Brown = 4281750187U;
    public const uint DarkGray = 4283389791U;
    public const uint LightGray = 4291281858U;
    public const uint Silver = 4293456383U;
    public const uint DarkPink = 4283236607U;
    public const uint Orange = 4278232063U;
    public const uint Yellow = 4280806655U;
    public const uint Green = 4281787392U;
    public const uint SkyBlue = 4294946089U;
    public const uint Lavander = 4288444035U;
    public const uint Pink = 4289230847U;
    public const uint Sand = 4289383679U;
    
    public byte R
    {
        get => ExtractR(_abgr);
        set => _abgr = (_abgr & 0xffffff00) | value;
    }
    
    public byte G
    {
        get => ExtractG(_abgr);
        set => _abgr = (_abgr & 0xffff00ff) | (uint)(value << 8);
    }
    
    public byte B
    {
        get => ExtractB(_abgr);
        set => _abgr = (_abgr & 0xff00ffff) | (uint)(value << 16);
    }
    
    private uint _abgr = (uint)(0xFF000000 | (b << 16) | (uint)(g << 8) | r);

    public bool Equals(ColorRGB other)
    {
        return _abgr == other._abgr;
    }

    public override bool Equals(object? obj)
    {
        return obj is ColorRGB other && Equals(other);
    }

    public override int GetHashCode()
    {
        return (int)_abgr;
    }

    public static bool operator ==(ColorRGB left, ColorRGB right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ColorRGB left, ColorRGB right)
    {
        return !(left == right);
    }
    
    public static implicit operator uint(ColorRGB color)
    {
        return color._abgr;
    }

    public static implicit operator ColorRGB(uint value)
    {
        byte r = (byte)value;
        byte g = (byte)(value >> 8);
        byte b = (byte)(value >> 16);
        
        return new ColorRGB(r, g, b);
    }
}