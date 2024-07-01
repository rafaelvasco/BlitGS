using BlitGS.Engine;

namespace BlitGS.Demo;

public class MathShape(string name) : Scene(name)
{
    private const float Pi_8 = MathUtils.Pi / 8;
    private const float Pi_2 = MathUtils.TwoPi;
    private const int Size = 250;
    private int offsetX;
    private int offsetY;
    
    private float t;
    public override void Load()
    {
        offsetX = (Canvas.Width / 2) - (Size/2);
        offsetY = (Canvas.Height / 2) - (Size/2);
    }

    public override void Update(float dt)
    {
    }

    public override void FixedUpdate(float dt)
    {
        t += 0.5f;
    }

    public override void Frame(float dt)
    {
        Canvas.Color(ColorRGB.DarkBlue);
        Canvas.FillRect();
     
        // Lines
        for (float i = t % 8; i < Size; i += 8)
        {
            Canvas.Color(ColorRGB.Lavander);
            Canvas.Line((int)i + offsetX, offsetY, offsetX, (int)(offsetY + (Size - i)));

            Canvas.Color(ColorRGB.Pink);
            Canvas.Line((int)i + offsetX,   offsetY + Size, offsetX + Size, (int)(offsetY +(Size - i)));
        }
        
        // Prism
        
        Canvas.Color(ColorRGB.Orange);
        
        for (float i = (t / 64) % Pi_8; i < Pi_2; i += Pi_8)
        {
            var x = offsetX + ((Size / 2f) + (Size / 4f) * Math.Cos(i));
            var y = offsetY + ((Size / 2f) + (Size / 4f) * Math.Cos(i));
            Canvas.Line(offsetX + Size, offsetY, (int)x, (int)y);
            Canvas.Line(offsetX, offsetY + Size, (int)x, (int)y);
        }
        
        // Border
        
        Canvas.Color(ColorRGB.Green);
        Canvas.Line(offsetX, offsetY,  offsetX + Size, offsetY);
        Canvas.Line(offsetX, offsetY, offsetX, offsetY + Size);

        Canvas.Color(ColorRGB.DarkPink);
        Canvas.Line( offsetX + Size, offsetY, offsetX + Size, offsetY + Size);
        Canvas.Line(offsetX, offsetY + Size, offsetX + Size, offsetY + Size);
    }
}