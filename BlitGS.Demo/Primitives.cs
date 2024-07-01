using BlitGS.Engine;

namespace BlitGS.Demo;

public struct DemoRect
{
    public int X;
    public int Y;
    public int W;
    public int H;
    public int Sx;
    public int Sy;
}

public class Primitives(string name) : Scene(name)
{
    private const int RECTS_COUNT = 5;
    
    private readonly DemoRect[] _rects = new DemoRect[RECTS_COUNT];
    
    private const int RECT_SIZE = 20;
    
    public override void Load()
    {
        var random = new Random();
        
        for (int i = 0; i < RECTS_COUNT; ++i)
        {
            _rects[i] = new DemoRect()
            {
                X = random.Next(0, Canvas.Width - RECT_SIZE * 2),
                Y = random.Next(0, Canvas.Height - RECT_SIZE * 2),
                W = RECT_SIZE,
                H = RECT_SIZE,
                Sx = random.Next(50,200),
                Sy = random.Next(50,200)
            };
        }
    }

    public override void Update(float dt)
    {
    }

    public override void FixedUpdate(float dt)
    {
        if (Keyboard.KeyPressed(Key.F))
        {
            Game.Fullscreen = !Game.Fullscreen;
        }

        if (Keyboard.KeyPressed(Key.Escape))
        {
            Game.Quit();
        }
        
        for (int i = 0; i < RECTS_COUNT; ++i)
        {
            ref var rect = ref _rects[i];
            rect.X += (int)(rect.Sx * dt);
            rect.Y += (int)(rect.Sy * dt);
        
            if (rect.X > Canvas.Width - RECT_SIZE)
            {
                rect.X = Canvas.Width - RECT_SIZE;
                rect.Sx *= -1;
            }
            else if (rect.X < 0)
            {
                rect.X = 0;
                rect.Sx *= -1;
            }
            
            if (rect.Y > Canvas.Height - RECT_SIZE)
            {
                rect.Y = Canvas.Height - RECT_SIZE;
                rect.Sy *= -1;
            }
            else if (rect.Y < 0)
            {
                rect.Y = 0;
                rect.Sy *= -1;
            }
        }
    }

    public override void Frame(float dt)
    {
        Canvas.Color(ColorRGB.DarkBlue);
        Canvas.FillRect();
        
        Canvas.Color(ColorRGB.Pink);
        
        for (int i = 0; i < RECTS_COUNT; ++i)
        {
            ref var rect = ref _rects[i];
            Canvas.FillRect(rect.X, rect.Y, rect.W, rect.H);
        }
        
        Canvas.Color(ColorRGB.Orange);
        Canvas.Circle(100,100, 80);
        
        Canvas.FillCircle(Canvas.Width - 100, 100, 80);
        
        Canvas.Color(ColorRGB.Lavander);
        Canvas.FillCircle(50, 150, 80);
        
        Canvas.Triangle(100, Canvas.Height - 50, 150, Canvas.Height - 150, 200, Canvas.Height - 50);
    }
}