using System.Collections.Generic;

namespace BlitGS.Engine;

public class AssetInfo : IDefinitionData
{
    public required string Id { get; init; }
    
    public required string Path { get; init; }
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(Id) &&
               !string.IsNullOrEmpty(Path);
    }
}

public class AssetsDefinition : IDefinitionData
{
    public Dictionary<string, AssetInfo>? Images { get; init; }
    
    public Dictionary<string, AssetInfo>? Fonts { get; init; }
    
    public bool IsValid()
    {
        return true;
    }
}