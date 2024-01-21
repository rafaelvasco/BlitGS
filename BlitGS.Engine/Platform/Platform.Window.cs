using System;
using System.Runtime.InteropServices;
using bottlenoselabs.C2CS.Runtime;
using static bottlenoselabs.SDL;

namespace BlitGS.Engine;

internal static unsafe partial class Platform
{
    public static Action<(int Width, int Height)> OnWindowResized = null!;
    public static Action OnWindowMinimized = null!;
    public static Action OnWindowRestored = null!;
    public static Action OnWindowMouseEntered = null!;
    public static Action OnWindowMouseExited = null!;
    
    private struct PlatformState
    {
        public SDL_Window* Window;
    }

    private static PlatformState _state;
    
    internal static SDL_Window* WindowPtr => _state.Window;
    
    private static void CreateWindow(GameConfig config)
    {
        var windowFlags = 
            SDL_WindowFlags.SDL_WINDOW_HIDDEN | 
            SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS |
            SDL_WindowFlags.SDL_WINDOW_MOUSE_FOCUS;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            windowFlags |= SDL_WindowFlags.SDL_WINDOW_METAL;
        }

        if (config.StartFullscreen)
        {
            windowFlags |= SDL_WindowFlags.SDL_WINDOW_FULLSCREEN;
        }
        
        _state.Window = SDL_CreateWindow(
            CString.FromString(config.Title),
            config.WindowWidth,
            config.WindowHeight,
            (uint)windowFlags
        );

        if (_state.Window == null)
        {
            CheckSDLError();
        }

        PrintFlags();
    }

    internal static void ShowWindow()
    {
        _ = SDL_ShowWindow(_state.Window);
    }

    public static bool IsFullscreen()
    {
        return ((SDL_WindowFlags)SDL_GetWindowFlags(_state.Window) &
               SDL_WindowFlags.SDL_WINDOW_FULLSCREEN) == SDL_WindowFlags.SDL_WINDOW_FULLSCREEN;
    }

    public static void SetFullscreen(bool fullscreen)
    {
        if (IsFullscreen() != fullscreen)
        {
            _ = SDL_SetWindowFullscreen(_state.Window, CBool.FromBoolean(fullscreen));
        }
    }

    public static void SetWindowSize(int w, int h)
    {
        if (IsFullscreen()) 
        {
            return;
        }

        _ = SDL_SetWindowSize(_state.Window, w, h);
        _ = SDL_SetWindowPosition(_state.Window, (int)SDL_WINDOWPOS_CENTERED_MASK, (int)SDL_WINDOWPOS_CENTERED_MASK);
    }

    public static (int Width, int Height) GetWindowSize()
    {
        var width = 0;
        var height = 0;
        _ = SDL_GetWindowSize(_state.Window, &width, &height);

        return (width, height);
    }

    public static void SetWindowTitle(string title)
    {
        _ = SDL_SetWindowTitle(_state.Window, CString.FromString(title));
    }

    public static string GetWindowTitle()
    {
        var title = SDL_GetWindowTitle(_state.Window);
        return title.ToString();
    }

    public static void ShowCursor(bool show)
    {
        _ = show ? SDL_ShowCursor() : SDL_HideCursor();
    }

    private static void ProcessWindowEvent(SDL_Event e)
    {
        switch (e.window.type)
        {
            case (uint)SDL_EventType.SDL_EVENT_WINDOW_RESIZED:
                var newW = e.window.data1;
                var newH = e.window.data2;
                OnWindowResized.Invoke((newW, newH));
                break;
            case (uint)SDL_EventType.SDL_EVENT_WINDOW_MINIMIZED:
                OnWindowMinimized.Invoke();
                break;
            case (uint)SDL_EventType.SDL_EVENT_WINDOW_RESTORED:
                OnWindowRestored.Invoke();
                break;
            case (uint)SDL_EventType.SDL_EVENT_WINDOW_MOUSE_ENTER:
                OnWindowMouseEntered.Invoke();
                break;
            case (uint)SDL_EventType.SDL_EVENT_WINDOW_MOUSE_LEAVE:
                OnWindowMouseExited.Invoke();
                break;
        }
    }
}