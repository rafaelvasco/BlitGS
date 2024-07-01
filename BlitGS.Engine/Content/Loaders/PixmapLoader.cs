

using System;
using System.IO;
using StbImageSharp;

namespace BlitGS.Engine;

internal class PixmapLoader : AssetLoader<Pixmap>
{
    public override Asset Load(string id, AssetsDefinition assetsDefinition)
    {
        if (assetsDefinition.Images?.TryGetValue(id, out var imageDef) == true)
        {
            IDefinitionData.ThrowIfInValid(imageDef, "PixmapLoader::Load");

            var assetFullPath = Path.Combine(ContentProperties.AssetsFolder, imageDef.Path);

            try
            {
                using var stream = File.OpenRead(assetFullPath);
                var pixmap = LoadFromStream(stream, Path.GetDirectoryName(assetFullPath)!);
                pixmap.Id = id;
                return pixmap;
            }
            catch (Exception e)
            {
                BlitException.Throw("Failed to load Pixmap Asset.", e);
            }
        }
        
        BlitException.Throw($"There's no asset definition with the Id: {id}");
        return default!;
    }

    protected override Pixmap LoadFromStream(Stream stream, string assetDirectory)
    {
        var imageInfo = ImageResult.FromStream(stream);

        if (imageInfo == null)
        {
            throw new Exception("ImageIO::Load: Failed to load image.");
        }

        var pixmap = new Pixmap(imageInfo.Data, imageInfo.Width, imageInfo.Height);

        return pixmap;
    }
}