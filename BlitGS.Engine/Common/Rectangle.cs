namespace BlitGS.Engine;

public struct Rectangle(int x, int y, int width, int height)
{
    public int X = x;
    public int Y = y;
    public int Width = width;
    public int Height = height;

    public static Rectangle Empty => new(0, 0, 0, 0);

    public static bool IsNullOrEmpty(Rectangle? rect)
    {
        return rect == null || rect.Value.IsEmpty;
    }
    
    public bool IsEmpty => Width == 0 || Height == 0;

    public int Left => X;
    public int Top => Y;
    public int Right => X + Width;
    public int Bottom => Y + Height;
}