using MemoryPack;

namespace BlitGS.Engine;

[MemoryPackable]
internal partial class ImageData : AssetData
{
    public required byte[] Data { get; init; }
 
    public int Width { get; init; }
    
    public int Height { get; init; }

    public override string ToString()
    {
        return $"Id: {Id}\nData: {Data.Length}\nWidth:{Width}\nHeight:{Height}";
    }

    public override bool IsValid()
    {
        return Data is { Length: > 0 } && Width > 0 && Height > 0;
    }
}