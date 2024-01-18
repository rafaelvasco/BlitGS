using System;
using System.IO;
using System.Runtime.CompilerServices;
using StbImageSharp;
using StbImageWriteSharp;
using ColorComponents = StbImageWriteSharp.ColorComponents;

namespace BlitGS.Engine;

public static unsafe class ImageIO
{
    private static ImageWriter? _writer;

    public static Pixmap Load(Stream fileStream)
    {
        var imageInfo = ImageResult.FromStream(fileStream);

        if (imageInfo == null)
        {
            throw new Exception("ImageIO::Load: Failed to load image.");
        }

        var pixmap = new Pixmap(imageInfo.Data, imageInfo.Width, imageInfo.Height);

        return pixmap;
    } 
    
    public static void Save(Pixmap pixmap, string path)
    {
        using var stream = File.OpenWrite(path);
        
        Save(Unsafe.AsPointer(ref pixmap.PixelBuffer[0]), pixmap.Width, pixmap.Height, stream);
    }

    public static void Save(void* pixels, int width, int height, Stream dest)
    {
        _writer ??= new ImageWriter();
        
        _writer.WritePng(pixels, width, height, ColorComponents.RedGreenBlueAlpha, dest);
    }
}