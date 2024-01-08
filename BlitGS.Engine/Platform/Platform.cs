using System;
using System.Runtime.InteropServices;
using bottlenoselabs.C2CS.Runtime;
using static bottlenoselabs.SDL;

namespace BlitGS.Engine;

public delegate void PlatformEvent();

internal static unsafe class Platform
{
    public static event PlatformEvent? OnQuit;
    
    internal static SDL_Window* WindowPtr => _state.Window;
    
    private struct PlatformState
    {
        public SDL_Window* Window;
    }

    private static PlatformState _state;

    internal static void Init(GameConfig gameConfig)
    {
        const uint initFlags = (uint)(SDL_InitFlags.SDL_INIT_VIDEO | SDL_InitFlags.SDL_INIT_GAMEPAD);
        
        var errorCode = SDL_Init(initFlags);
        CheckSDLError(errorCode);
        
        CreateWindow(gameConfig.WindowWidth, gameConfig.WindowHeight, gameConfig.Title);
    }

    internal static void Terminate()
    {
        SDL_DestroyWindow(_state.Window);
        SDL_Quit();
    }

    internal static void ProcessEvents()
    {
        SDL_Event e;
        if (!SDL_PollEvent(&e)) return;
        if (e.type == (ulong)SDL_EventType.SDL_EVENT_QUIT)
        {
            TriggerOnQuit();
        }
    }
    
    private static void CreateWindow(int width, int height, string title)
    {
        var windowFlags = SDL_WindowFlags.SDL_WINDOW_HIDDEN;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            windowFlags |= SDL_WindowFlags.SDL_WINDOW_METAL;
        }
        
        _state.Window = SDL_CreateWindow(
            CString.FromString(title),
            width,
            height,
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
    
    private static void PrintFlags()
    {
        var windowFlags = SDL_GetWindowFlags(_state.Window);

        SDL_version version;

        _ = SDL_GetVersion(&version);
        
        Console.WriteLine($"SDL Version: {version.major}.{version.minor}");
        
        Console.WriteLine($"""Window: "Fullscreen" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_FULLSCREEN) != 0}""");
        Console.WriteLine($"""WindowL "OpenGL" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_OPENGL) != 0}""");
        Console.WriteLine($"""Window: "Hidden" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_HIDDEN) != 0}""");
        Console.WriteLine($"""Window: "Borderless" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_BORDERLESS) != 0}""");
        Console.WriteLine($"""Window: "Resizeable" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_RESIZABLE) != 0}""");
        Console.WriteLine($"""Window: "Minimized" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_MINIMIZED) != 0}""");
        Console.WriteLine($"""Window: "Maximized" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_MAXIMIZED) != 0}""");
        Console.WriteLine($"""Window: "Mouse grabbed" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_MOUSE_GRABBED) != 0}""");
        Console.WriteLine($"""Window: "Input focus" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS) != 0}""");
        Console.WriteLine($"""Window: "Mouse focus" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_MOUSE_FOCUS) != 0}""");
        Console.WriteLine($"""Window: "Mouse capture" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_MOUSE_CAPTURE) != 0}""");
        Console.WriteLine($"""Window: "Always on top" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_ALWAYS_ON_TOP) != 0}""");
        Console.WriteLine($"""Window: "Utility" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_UTILITY) != 0}""");
        Console.WriteLine($"""Window: "Tooltip" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_TOOLTIP) != 0}""");
        Console.WriteLine($"""Window: "Popup menu" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_POPUP_MENU) != 0}""");
        Console.WriteLine($"""Window: "Keyboard grabbed" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_KEYBOARD_GRABBED) != 0}""");
        Console.WriteLine($"""Window: "Vulkan" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_VULKAN) != 0}""");
        Console.WriteLine($"""Window: "Metal" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_METAL) != 0}""");
    }

    private static void TriggerOnQuit()
    {
        OnQuit?.Invoke();
    }
    
    public static void CheckSDLError(int? errorCode = -1)
    {
        if (errorCode >= 0)
        {
            return;
        }

        var error = SDL_GetError();
        Console.Error.WriteLine($"SDL2 Error: {error.ToString()}");
        Environment.Exit(1);
    }
}