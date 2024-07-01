using BlitGS.Engine;

namespace BlitGS.Demo;

public class BlitSprite(string name) : Scene(name)
{
    private Pixmap? _sprite;

    private int _offsetX;
    private int _offsetY;
    
    public override void Load()
    {
        _sprite = Content.Get<Pixmap>("jump");
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

        if (Keyboard.KeyDown(Key.A))
        {
            _offsetX -= 1;
        }
        
        if (Keyboard.KeyDown(Key.D))
        {
            _offsetX += 1;
        }
        
        if (Keyboard.KeyDown(Key.W))
        {
            _offsetY -= 1;
        }
        
        if (Keyboard.KeyDown(Key.S))
        {
            _offsetY += 1;
        }
    }

    public override void Frame(float dt)
    {
        Canvas.Color(ColorRGB.DarkBlue);
        Canvas.FillRect();

        if (_sprite != null)
        {
            Canvas.BlitEx(_sprite, _offsetX, _offsetY, default, _sprite.Width / 2, _sprite.Height / 2);
        }
    }
}