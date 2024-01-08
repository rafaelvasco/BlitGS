namespace BlitGS.Engine;

public abstract class Game : Disposable
{
    protected Game(GameConfig config)
    {
        Platform.Init(config);
        Canvas.Init(config);
        
        Platform.OnQuit += PlatformOnOnQuit;
    }

    private void PlatformOnOnQuit()
    {
        Quit();
    }

    public void Start()
    {
        Load();
        Frame();
        Canvas.Flip();
        
        Platform.ShowWindow();
        
        Tick();
    }

    public void Quit()
    {
        _running = false;
    }

    protected abstract void Frame();

    protected virtual void Load(){}

    private void Tick()
    {
        while (true)
        {
            Platform.ProcessEvents();

            Frame();
            
            Canvas.Flip();
            
            if (!_running)
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

    private bool _running = true;
}