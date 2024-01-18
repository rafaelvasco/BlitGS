using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace BlitGS.Engine;

public static class Content
{
    private static AssetsDefinition _assetsDefinition = null!;

    internal static void Init()
    {
        _assetLoaders[typeof(Pixmap)] = new PixmapLoader();
        _assetsDefinition = LoadAssetsDefinition();
    }

    // internal static void Clear()
    // {
    //     _assetLoaders.Clear();
    //     GC.Collect();
    // }
     
    public static T Get<T>(string id) where T : Asset
    {
        if (_loadedAssets.TryGetValue(id, out var asset))
        {
            return (asset as T)!;
        }

        if (_assetLoaders.TryGetValue(typeof(T), out var loader))
        {
            var loadedAsset = loader.Load(id, _assetsDefinition);

            _loadedAssets.Add(loadedAsset.Id, loadedAsset);
            
            return (loadedAsset as T)!;
        }
        
        BlitException.Throw($"No Loader Registered for this type: {typeof(T)}");
        return default!;
    }

    private static AssetsDefinition LoadAssetsDefinition()
    {
        var filePath = Path.Combine(ContentProperties.AssetsFolder, ContentProperties.AssetsDefinitionFile);

        var data = LoadDefinitionData<AssetsDefinition>(filePath);

        data ??= new AssetsDefinition();

        return data;
    }

    public static T? LoadDefinitionData<T>(string filePath) where T : class, IDefinitionData
    {
        try
        {
            using var stream = File.OpenRead(filePath);
            var data = JsonSerializer.Deserialize<T>(stream, ContentProperties.SerializationOptions);
            IDefinitionData.ThrowIfInValid(data, "Content::LoadDefinitionData");
            return data!;
        }
        catch (Exception)
        {
            Console.WriteLine($"Failed To Load Definition At: {filePath}");
        }

        return null;
    }
    
    private static readonly Dictionary<string, Asset> _loadedAssets = new();

    private static readonly Dictionary<Type, AssetLoader> _assetLoaders = new();
}