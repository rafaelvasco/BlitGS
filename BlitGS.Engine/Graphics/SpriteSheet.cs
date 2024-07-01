using System;
using System.Collections.Generic;

namespace BlitGS.Engine;

public class SpriteSheet : Pixmap
{
    public ref readonly Rectangle this[int index] => 
        ref _tiles[MathUtils.Clamp(index, 0, _tiles.Length - 1)];

    private static readonly Rectangle _emptyRect = new ();

    public ref readonly Rectangle this[string name]
    {
        get
        {
            if (_spriteMap.TryGetValue(name, out var index))
            {
                return ref _tiles[index];
            }

            return ref _emptyRect;
        }
    }
    
    public int TileSize { get; }
    
    internal SpriteSheet(int width, int height, int tileSize) : base(width, height)
    {
        TileSize = tileSize;

        _tiles = BuildTiles(width, height, tileSize);

        _spriteMap = new Dictionary<string, int>();
    }

    internal SpriteSheet(ReadOnlySpan<byte> data, int width, int height, int tileSize) : base(data, width, height)
    {
        TileSize = tileSize;

        _tiles = BuildTiles(width, height, tileSize);

        _spriteMap = new Dictionary<string, int>();
    }

    public void MapNamedSprite(string name, int index)
    {
        _spriteMap[name] = index;
    }

    private static Rectangle[] BuildTiles(int width, int height, int tileSize)
    {
        var tilesHoriz = width / tileSize;
        var tilesVert = height / tileSize;

        var idx = 0;

        var tiles = new Rectangle[tilesHoriz * tilesVert];

        for (var j = 0; j < tilesVert; ++j)
        {
            for (var i = 0; i < tilesHoriz; ++i)
            {
                tiles[idx++] = new Rectangle(i * tileSize, j * tileSize, tileSize, tileSize);
            }
        }

        return tiles;
    }

    private readonly Rectangle[] _tiles;
    private readonly Dictionary<string, int> _spriteMap;
    
}