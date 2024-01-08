namespace BlitGS.Engine;

public struct Rectangle(int x, int y, int w, int h)
{
    public int X = x;
    public int Y = y;
    public int W = w;
    public int H = h;

    public static Rectangle Empty => new(0, 0, 0, 0);

    public static bool IsNullOrEmpty(Rectangle? rect)
    {
        return rect == null || rect.Value.IsEmpty;
    }
    
    public bool IsEmpty => W == 0 || H == 0;

    public int Left => X;
    public int Top => Y;
    public int Right => X + W;
    public int Bottom => Y + H;
}