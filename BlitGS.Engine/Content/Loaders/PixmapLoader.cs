

using System;
using System.IO;

namespace BlitGS.Engine;

internal class PixmapLoader : AssetLoader
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
                var pixmap = ImageIO.Load(stream);
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
}