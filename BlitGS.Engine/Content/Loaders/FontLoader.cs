using System;
using System.IO;
using System.Text.Json;
using StbImageSharp;

namespace BlitGS.Engine;

internal class FontLoader : AssetLoader<Font>
{
    public override Asset Load(string id, AssetsDefinition assetsDefinition)
    {
        if (assetsDefinition.Fonts?.TryGetValue(id, out var fontDef) == true)
        {
            IDefinitionData.ThrowIfInValid(fontDef, "PixmapLoader::Load");

            var assetFullPath = Path.Combine(ContentProperties.AssetsFolder, fontDef.Path);

            try
            {
                using var stream = File.OpenRead(assetFullPath);
                var font = LoadFromStream(stream, Path.GetDirectoryName(assetFullPath)!);
                font.Id = id;
                return font;
            }
            catch (Exception e)
            {
                BlitException.Throw("Failed to load Font Asset.", e);
            }
        }
        
        BlitException.Throw($"There's no asset definition with the Id: {id}");
        return default!;
    }

    protected override Font LoadFromStream(Stream stream, string assetDirectory)
    {
        var fontInfo = JsonSerializer.Deserialize<FontAssetInfo>(stream)!;

        var fontImagePath = Path.Combine(assetDirectory, fontInfo.Image);

        using var fontImageStream = File.OpenRead(fontImagePath);
        
        var imageInfo = ImageResult.FromStream(fontImageStream);

        if (imageInfo == null)
        {
            throw new Exception("ImageIO::Load: Failed to load image.");
        }

        var font = new Font(
            imageInfo.Data,
            imageInfo.Width,
            imageInfo.Height,
            fontInfo);

        return font;
    }
}