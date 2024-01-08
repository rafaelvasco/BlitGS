namespace BlitGS.Engine;

public class Pixmap(int width, int height)
{
    public uint[] PixelBuffer { get; } = new uint[width * height];

    public int Width { get; } = width;

    public int Height { get; } = height;
}