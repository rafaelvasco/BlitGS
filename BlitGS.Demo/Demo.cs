using BlitGS.Engine;

namespace BlitGS.Demo;

public class Demo(GameConfig config) : Game(config)
{
    protected override void Frame()
    {
        Canvas.Color(ColorRGB.Black);
        Canvas.Fill();
        Canvas.Color(ColorRGB.DarkPink);
        Canvas.FillRect(Canvas.Width / 2 - 100, Canvas.Height / 2 - 100, 200, 200);
        
    }
}