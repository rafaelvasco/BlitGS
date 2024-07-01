namespace BlitGS.Engine;

internal abstract class AssetWriter
{
    public abstract void WriteToFile(Asset asset, string outputPath);
}