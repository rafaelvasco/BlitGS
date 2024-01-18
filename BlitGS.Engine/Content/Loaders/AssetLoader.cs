namespace BlitGS.Engine;


internal abstract class AssetLoader
{
    public abstract Asset Load(string id, AssetsDefinition assetsDefinition);
}