namespace BlitGS.Engine;

public delegate void GameEvent();

public abstract class Game : Disposable
{
    public static event GameEvent? OnMouseEnter;
    public static event GameEvent? OnMouseExit;
    
    public static bool HideMouse
    {
        get => _instance._hideMouse;
        set
        {
            if (value != _instance._hideMouse)
            {
                _instance._hideMouse = value;
                Platform.ShowCursor(value);
            }
        }
    }

    public static (int Width, int Height) WindowSize
    {
        get => Platform.GetWindowSize();
        set => Platform.SetWindowSize(value.Width, value.Height);
    }

    public static string Title
    {
        get => Platform.GetWindowTitle();
        set => Platform.SetWindowTitle(value);
    }

    internal static ref GameConfig Config => ref _config;

    public static bool Fullscreen
    {
        get => Platform.IsFullscreen();
        set
        {
            if (Platform.IsFullscreen() == value)
            {
                return;
            }
            
            Platform.SetFullscreen(value);
        }
    }

    protected static GameConfig _config;
    
    private static Game _instance = null!;
    
    protected Game(GameConfig config)
    {
        _instance = this;

        _config = config;
        
        Platform.Init(config);
        
        Content.Init();
        Canvas.Init(config);
        GameLoop.Init();
        
        Keyboard.Init();
        Mouse.Init();

        if (config.EnableGamepad)
        {
            Gamepad.Init();
        }
        
        Platform.OnQuit = PlatformOnOnQuit;
        Platform.OnWindowMinimized = OnWindowMinimized;
        Platform.OnWindowRestored = OnWindowRestored;
        Platform.OnWindowResized = OnWindowResized;
        Platform.OnWindowMouseEntered = OnWindowMouseEntered;
        Platform.OnWindowMouseExited = OnWindowMouseExited;
    }

    public void Start()
    {
        Load();
        GameLoop.Start(this);
        Platform.ShowWindow();
        Tick();
    }

    public static void Quit()
    {
        GameLoop.Terminate();
    }

    internal void TickUpdate(float dt)
    {
        Update(dt);
    }

    internal void TickFixedUpdate(float dt)
    {
        FixedUpdate(dt);
    }

    internal void TickFrame(float dt)
    {
        Frame(dt);
    }

    protected abstract void Update(float dt);

    protected abstract void FixedUpdate(float dt);

    protected abstract void Frame(float dt);

    protected abstract void Load();

    private void Tick()
    {
        while (true)
        {
            GameLoop.Tick(this);
            
            if (!GameLoop.Running)
            {
                break;
            }
        }
    }
    
    protected override void Free()
    {
        Canvas.Terminate();
        Platform.Terminate();
    }
    
    private static void OnWindowMouseExited()
    {
        OnMouseExit?.Invoke();
    }

    private static void OnWindowMouseEntered()
    {
        OnMouseEnter?.Invoke();
    }

    private static void OnWindowResized((int Width, int Height) size)
    {
    }

    private static void OnWindowRestored()
    {
        GameLoop.IsActive = true;
    }

    private static void OnWindowMinimized()
    {
        GameLoop.IsActive = false;
    }

    private static void PlatformOnOnQuit()
    {
        Quit();
    }

    private bool _hideMouse;
}