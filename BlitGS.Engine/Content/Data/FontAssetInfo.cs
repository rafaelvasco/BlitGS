namespace BlitGS.Engine;

public class FontAssetInfo
{
    public required string Image { get; init; }

    // public required int GlyphColumnCount { get; init; }
    //
    // public required int GlyphRowCount { get; init; }

    public required int GlyphWidth { get; init; }
    
    public required int GlyphHeight { get; init; }
    
    public required int GlyphSpacing { get; init; }
    
    public int LineSpacing { get; init; }
    
    public int? CharRangeStart { get; init; }
    
    public int? CharRangeEnd { get; init; }

}