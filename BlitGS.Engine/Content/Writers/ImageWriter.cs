using System.IO;
using StbImageWriteSharp;

namespace BlitGS.Engine;
using StbImageWriter = StbImageWriteSharp.ImageWriter;

internal static unsafe class ImageWriter
{
    private static StbImageWriter? _writer;
    
    public static void Save(void* pixels, int width, int height, Stream dest)
    {
        _writer ??= new StbImageWriter();
        
        _writer.WritePng(pixels, width, height, ColorComponents.RedGreenBlueAlpha, dest);
    }
}