using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlitGS.Engine;

public static class ContentProperties
{
    public const string AssetsFolder = "Assets";
    public const string AssetsDefinitionFile = "assets.json";

    public static readonly JsonSerializerOptions SerializationOptions = new()
    {
        WriteIndented = true,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };
}