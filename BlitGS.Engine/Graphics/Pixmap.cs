using System.Collections.Generic;

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
    
    public Pixmap(IReadOnlyList<byte> data, int width, int height)
    {
        PixelBuffer = new uint[width * height];

        var sourceBufferLength = data.Count;

        var targetBufferIdx = 0;
        
        for (var i = 0; i < sourceBufferLength; i+=4)
        {
            byte r = data[i];
            byte g = data[i + 1];
            byte b = data[i + 2];

            PixelBuffer[targetBufferIdx++] = (uint)(0xFF000000 | (b << 16) | (uint)(g << 8) | r);
        }
        
        Width = width;
        Height = height;
        PixelCount = width * height;
        Pitch = width * sizeof(uint);
    }

    public uint[] PixelBuffer { get; }

    public int Width { get; }

    public int Height { get; }

    public int PixelCount { get; }

    public int Pitch { get; }
}