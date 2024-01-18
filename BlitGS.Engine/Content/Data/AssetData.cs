namespace BlitGS.Engine;

internal abstract class AssetData : IDefinitionData
{
    public required string Id { get; init; }

    public abstract bool IsValid();
}