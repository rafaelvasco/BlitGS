using System;
using static bottlenoselabs.SDL;

namespace BlitGS.Engine;

internal static unsafe partial class Platform
{
    public static Action OnQuit = null!;
    
    internal static void Init(GameConfig gameConfig)
    {
        const uint initFlags = (uint)(SDL_InitFlags.SDL_INIT_VIDEO);
        
        var errorCode = SDL_Init(initFlags);
        CheckSDLError(errorCode);
        CreateWindow(gameConfig);
        
        InitKeyboard();
        InitGamepad();
    }

    internal static void Terminate()
    {
        SDL_QuitSubSystem((uint)SDL_InitFlags.SDL_INIT_GAMEPAD);
        SDL_DestroyWindow(_state.Window);
        SDL_Quit();
    }

    internal static void ProcessEvents()
    {
        SDL_Event e;
        if (!SDL_PollEvent(&e)) return;
        switch (e.type)
        {
            case 
                (uint) SDL_EventType.SDL_EVENT_KEY_DOWN or 
                (uint) SDL_EventType.SDL_EVENT_KEY_UP:
                ProcessKeyEvent(e);
                break;
            
            case  
                (uint) SDL_EventType.SDL_EVENT_GAMEPAD_ADDED or
                (uint) SDL_EventType.SDL_EVENT_GAMEPAD_REMOVED:
                ProcessGamePadEvent(e);
                break;
            
            case 
                (uint) SDL_EventType.SDL_EVENT_WINDOW_RESIZED or 
                (uint) SDL_EventType.SDL_EVENT_WINDOW_RESTORED or 
                (uint) SDL_EventType.SDL_EVENT_WINDOW_MINIMIZED or 
                (uint) SDL_EventType.SDL_EVENT_WINDOW_MOUSE_ENTER or 
                (uint) SDL_EventType.SDL_EVENT_WINDOW_MOUSE_LEAVE:
                ProcessWindowEvent(e);
                break;
            case (uint) SDL_EventType.SDL_EVENT_QUIT:
                TriggerOnQuit();
                break;
        }
    }
    
    private static void PrintFlags()
    {
        var windowFlags = SDL_GetWindowFlags(_state.Window);

        SDL_version version;

        _ = SDL_GetVersion(&version);
        
        Console.WriteLine("::::::::::::");
        Console.WriteLine(".::BlitGS::.");
        Console.WriteLine("::::::::::::");
        
        Console.WriteLine($"SDL Version: {version.major}.{version.minor}");
        
        Console.WriteLine($"""Window: "Fullscreen" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_FULLSCREEN) != 0}""");
        Console.WriteLine($"""Window: "OpenGL" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_OPENGL) != 0}""");
        Console.WriteLine($"""Window: "Hidden" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_HIDDEN) != 0}""");
        Console.WriteLine($"""Window: "Resizeable" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_RESIZABLE) != 0}""");
        Console.WriteLine($"""Window: "Mouse grabbed" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_MOUSE_GRABBED) != 0}""");
        Console.WriteLine($"""Window: "Input focus" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS) != 0}""");
        Console.WriteLine($"""Window: "Mouse focus" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_MOUSE_FOCUS) != 0}""");
        Console.WriteLine($"""Window: "Mouse capture" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_MOUSE_CAPTURE) != 0}""");
        Console.WriteLine($"""Window: "Always on top" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_ALWAYS_ON_TOP) != 0}""");
        Console.WriteLine($"""Window: "Utility" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_UTILITY) != 0}""");
        Console.WriteLine($"""Window: "Vulkan" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_VULKAN) != 0}""");
        Console.WriteLine($"""Window: "Metal" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_METAL) != 0}""");
    }

    private static void TriggerOnQuit()
    {
        OnQuit.Invoke();
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

    internal static double GetPerfFreq()
    {
        return SDL_GetPerformanceFrequency();
    }

    public static double GetPerfCounter()
    {
        return SDL_GetPerformanceCounter();
    }
}