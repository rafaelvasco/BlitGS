using System;

namespace BlitGS.Engine;

public interface IDefinitionData
{
    public bool IsValid();
    
    public static void ThrowIfInValid(IDefinitionData? data, string location)
    {
        if (data == null || !data.IsValid())
        {
            BlitException.Throw($"Invalid Data: {data} at {location}");
        }
    }

    public static void Debug(IDefinitionData data, string message)
    {
        Console.WriteLine();
        Console.WriteLine(message);
        Console.WriteLine(data.ToString());
        Console.WriteLine();
    }
}