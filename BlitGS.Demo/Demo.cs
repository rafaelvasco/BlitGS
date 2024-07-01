using BlitGS.Engine;

namespace BlitGS.Demo;

public class Demo(GameConfig config) : Game(config)
{
    private Font? _font;
    
    private readonly Scene[] _scenes =
    [
        new Primitives("Primitives Demo"),
        new MathShape("Math Shape Demo"),
        new BlitSprite(name: "Blit Sprite Demo")
    ];

    private Scene? _currentScene;
    private int _sceneIndex;
    
    protected override void Load()
    {
        _font = Content.Get<Font>("font");
        
        foreach (var scene in _scenes)   
        {
            scene.Load();
        }

        Keyboard.OnKeyUp += key =>
        {
            switch (key)
            {
                case Key.Left: _sceneIndex--;
                    break;
                case Key.Right: _sceneIndex++;
                    break;
            }
            
            _sceneIndex = MathUtils.Clamp(_sceneIndex, 0, _scenes.Length - 1);

            _currentScene = _scenes[_sceneIndex];
        };
        _currentScene = _scenes[_sceneIndex];
    }
    
    protected override void Update(float dt)
    {
        _currentScene?.Update(dt);

    }

    protected override void FixedUpdate(float dt)
    {
       _currentScene?.FixedUpdate(dt);
    }

    protected override void Frame(float dt)
    {
        if (_currentScene == null)
        {
            return;
        }  
        
        _currentScene.Frame(dt);

        if (_font != null)
        {
            Canvas.Text(_font, 10, 10, $"Current Scene: {_currentScene.Name}");
        }
    }
}