namespace BlitGS.Engine;

public readonly struct GameConfig()
{
    public int WindowWidth { get; init; } = 480*2;

    public int WindowHeight { get; init; } = 270*2;

    public int CanvasWidth { get; init; } = 480;

    public int CanvasHeight { get; init; } = 270;

    public bool StartFullscreen { get; init; } = false;

    public StretchMode StretchMode { get; init; } = StretchMode.Integer;
    
    public required string Title { get; init; } = "BlitGS Game";

    public bool EnableGamepad { get; init; } = true;
}