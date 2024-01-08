namespace BlitGS.Engine;

public struct GameConfig
{
    public int WindowWidth { get; init; }
    
    public int WindowHeight { get; init; }
    
    public int CanvasWidth { get; init; }
    
    public int CanvasHeight { get; init; }
    
    public StretchMode StretchMode { get; init; }
    
    public required string Title { get; init; }
}