using System;

namespace BlitGS.Engine;

public class Font : Pixmap
{
    private const int DefaultCharRangeStart = 32;
    private const int DefaultCharRangeEnd = 127;
    
    public int GlyphWidth { get; }
    
    public int GlyphHeight { get; }
    
    public int GlyphSpacing { get; set; }

    public int LineSpacing { get; set; }
    
    public int CharRangeStart { get; }
    
    public int CharRangeEnd { get; }

    internal Font(
        ReadOnlySpan<byte> data, 
        int width, 
        int height, 
        FontAssetInfo info) : base(data, width, height)
    {
        GlyphWidth = info.GlyphWidth;
        GlyphHeight = info.GlyphHeight;

        int glyphColumnCount = width / info.GlyphWidth;
        int glyphRowCount = height / info.GlyphHeight;

        _totalGlyphCount = glyphColumnCount * glyphRowCount;

        _glyphColumnCount = glyphColumnCount;

        _glyphRowCount = glyphRowCount;

        _regions = new Rectangle[_totalGlyphCount];

        CharRangeStart = info.CharRangeStart ?? DefaultCharRangeStart;

        CharRangeEnd = info.CharRangeEnd ?? DefaultCharRangeEnd;

        LineSpacing = info.LineSpacing;

        GlyphSpacing = info.GlyphSpacing;

        InitRegions();
    }

    public ref Rectangle GetGlyphRegion(char c)
    {
        var charIndex = c - CharRangeStart;

        if (charIndex > -1 && charIndex < _totalGlyphCount)
        {
            return ref _regions[charIndex];
        }

        return ref _regions[0];
    }

    private void InitRegions()
    {
        var index = 0;
        
        for (int line = 0; line < _glyphRowCount; ++line)
        {
            for (int col = 0; col < _glyphColumnCount; ++col)
            {
                _regions[index++] = new Rectangle(col * GlyphWidth, line * GlyphHeight, GlyphWidth, GlyphHeight);
            }
        }
    }

    private readonly Rectangle[] _regions;
    private readonly int _totalGlyphCount;
    private readonly int _glyphColumnCount;
    private readonly int _glyphRowCount;
}