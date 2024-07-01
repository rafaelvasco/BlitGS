using System.IO;

namespace BlitGS.Engine;

internal interface IAssetLoader
{
    public Asset Load(string id, AssetsDefinition assetsDefinition);
}

internal abstract class AssetLoader<T>: IAssetLoader where T : Asset
{
    public abstract Asset Load(string id, AssetsDefinition assetsDefinition);

    protected abstract T LoadFromStream(Stream stream, string assetDirectory);
}