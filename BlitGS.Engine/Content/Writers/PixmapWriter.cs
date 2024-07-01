using System.IO;
using System.Runtime.CompilerServices;

namespace BlitGS.Engine;

internal unsafe class PixmapWriter : AssetWriter
{
    public override void WriteToFile(Asset asset, string outputPath)
    {
        var pixmap = (asset as Pixmap)!;
        
        using var stream = File.OpenWrite(outputPath);
        
        ImageWriter.Save(Unsafe.AsPointer(ref pixmap.PixelBuffer[0]), pixmap.Width, pixmap.Height, stream);
    }
}