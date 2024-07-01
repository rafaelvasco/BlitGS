using System;

namespace BlitGS.Engine;

public class Pixmap : Asset
{
    public Pixmap(int width, int height)
    {
        PixelBuffer = new uint[width * height];
        Width = width;
        Height = height;
        PixelCount = width * height;
        Pitch = width * sizeof(uint);
    }
    
    public Pixmap(ReadOnlySpan<byte> data, int width, int height)
    {
        PixelBuffer = new uint[width * height];

        SetData(data);
        
        Width = width;
        Height = height;
        PixelCount = width * height;
        Pitch = width * sizeof(uint);
    }

    public void SetData(ReadOnlySpan<byte> pixels)
    {
        if (pixels.Length > PixelBuffer.Length * 4)
        {
            BlitException.Throw("Pixmap::SetData : Pixel buffer bigger than internal buffer");
            return;
        }
        
        var sourceBufferLength = pixels.Length;

        var targetBufferIdx = 0;

        unchecked
        {
            for (var i = 0; i < sourceBufferLength; i+=4)
            {
                byte r = pixels[i];
                byte g = pixels[i + 1];
                byte b = pixels[i + 2];

                PixelBuffer[targetBufferIdx++] = (uint)(0xFF000000 | (b << 16) | (uint)(g << 8) | r);
            }
        }
    }
    
    public uint[] PixelBuffer { get; }

    public int Width { get; }

    public int Height { get; }

    public int PixelCount { get; }

    public int Pitch { get; }

   
}