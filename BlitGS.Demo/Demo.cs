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

public class Demo(GameConfig config) : Game(config)
{
    private const int RECTS_COUNT = 5;
    
    private readonly DemoRect[] _rects = new DemoRect[RECTS_COUNT];
    
    private const int RECT_SIZE = 20;

    private Pixmap blitPixmap = null!;
    private Pixmap sonic = null!;

    private int blitX;
    private int blitY;

    private int srcX;
    private int srcY;

    private bool flip;
    
    protected override void Load()
    {
        blitX = Canvas.Width / 2 - 50;
        blitY = Canvas.Height / 2 - 50;
        
        sonic = Content.Get<Pixmap>("sonic");
        
        blitPixmap = new Pixmap(200, 200);
        
        Canvas.BeginTarget(blitPixmap);
        
        Canvas.Color(ColorRGB.Black);
        Canvas.FillRect(0, 0, 200, 200);
        
        Canvas.Color(ColorRGB.Pink);
        Canvas.FillRect(40, 40, 20, 20);
        
        Canvas.Color(ColorRGB.SkyBlue);
        Canvas.FillCircle(150, 50, 20);
        
        Canvas.Color(ColorRGB.MiddleGreen);
        Canvas.Rect(40, 140, 20, 20);
        
        Canvas.Color(ColorRGB.Orange);
        Canvas.Circle(150, 150, 20);
        
        Canvas.Line(0, 0, 0, 200);
        Canvas.Line(199, 0, 199, 200);
        
        
        Canvas.FillCircle(10, 10, 50);
        Canvas.Color(ColorRGB.Lavander);
        Canvas.FillCircle(190, 10, 50);
        Canvas.EndTarget();
        
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
    
    protected override void Update(float dt)
    {
      
    }

    protected override void FixedUpdate(float dt)
    {
        if (Gamepad.ButtonPressed(GamePadButtons.Start))
        {
            Fullscreen = !Fullscreen;
        }

        if (Gamepad.ButtonPressed(GamePadButtons.Back))
        {
            Quit();
        }

        if (Gamepad.ButtonDown(GamePadButtons.DPadLeft))
        {
            blitX -= 2;
            srcX -= 2;
        }

        if (Gamepad.ButtonDown(GamePadButtons.DPadRight))
        {
            blitX += 2;
            srcX += 2;
        }
        
        if (Gamepad.ButtonDown(GamePadButtons.DPadUp))
        {
            blitY -= 2;
            srcY -= 2;
        }
        
        if (Gamepad.ButtonDown(GamePadButtons.DPadDown))
        {
            blitY += 2;
            srcY += 2;
        }

        srcX = MathUtils.Wrap(srcX, 0, 100);
        srcY = MathUtils.Wrap(srcY, 0, 100);

        if (Gamepad.ButtonPressed(GamePadButtons.South))
        {
            flip = !flip;
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

    protected override void Frame(float dt)
    {
        Canvas.Clip(50, 50, Canvas.Width - 100, Canvas.Height - 100);
        
        Canvas.Color(ColorRGB.SkyBlue);
        Canvas.FillRect();

        Canvas.BlitEx(sonic, 0, 0, width: 128, height: 128, flip: true);
        Canvas.BlitEx(sonic, 130, 0, width: 64, height: 64);
        Canvas.BlitEx(sonic, 0, 130, width: 200, height: 50);
        Canvas.BlitEx(sonic, 220, 0, width: 50, height: 200);
        
        Canvas.Color(ColorRGB.Pink);
        
        for (int i = 0; i < RECTS_COUNT; ++i)
        {
            ref var rect = ref _rects[i];
            Canvas.FillRect(rect.X, rect.Y, rect.W, rect.H);
        }
        
        Canvas.BlitEx(blitPixmap, blitX , blitY, region: new Rectangle(srcX, srcY, 100, 100), flip: flip);
        
        //Canvas.ColorFilter(ColorRGB.Wine, Canvas.PixelColorOp.Set);
        Canvas.ColorFilter(1f, Canvas.PixelColorOp.Set, Canvas.ValueFilterChannel.Blue);
    }
}