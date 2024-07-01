using System;
using static bottlenoselabs.SDL;

namespace BlitGS.Engine;

internal static unsafe partial class Platform
{
    public static Action<MouseButton>? MouseUp;
    public static Action<MouseButton>? MouseDown;
    public static Action<float, float>? MouseMove;
    
    public static MouseState GetMouseState()
    {
        float x, y;
        var flags = SDL_GetMouseState(&x, &y);
        Canvas.ConvertWindowCoordinatesToCanvas(x, y, out var canvasX, out var canvasY);

        var left = (flags & SDL_BUTTON_LEFT) > 0;
        var middle = ((flags & SDL_BUTTON_MIDDLE) >> 1) > 0;
        var right = ((flags & SDL_BUTTON_RIGHT) >> 2) > 0;

        return new MouseState((int)canvasX, (int)canvasY, (int)_wheelValue, left, middle, right);
    }

    public static void SetMousePosition(int x, int y)
    {
        SDL_WarpMouseInWindow(_state.Window, x, y);
    }

    public static bool GetRelativeMouseMode()
    {
        return SDL_GetRelativeMouseMode() == true;
    }

    public static void SetRelativeMouseMode(bool enable)
    {
        _ = SDL_SetRelativeMouseMode(
            enable
        );
    }
    
    private static void InitMouse()
    {
        _supportsGlobalMouse =
            OperatingSystem.IsWindows() || OperatingSystem.IsMacOS() || OperatingSystem.IsLinux();
    }
    
    private static void ProcessMouseEvent(SDL_Event evt)
    {
        var button = TranslatePlatformMouseButton(evt.button.button);
        
        switch (evt.type)
        {
            case  (uint) SDL_EventType.SDL_EVENT_MOUSE_MOTION when MouseMove is not null:

                var x = evt.motion.x;
                var y = evt.motion.y;
                
                Canvas.ConvertWindowCoordinatesToCanvas(x, y, out var canvasX, out var canvasY);
                
                MouseMove(canvasX, canvasY);
                break;
            case (uint) SDL_EventType.SDL_EVENT_MOUSE_BUTTON_DOWN when MouseDown is not null:
                MouseDown(button);
                break;
            case (uint) SDL_EventType.SDL_EVENT_MOUSE_BUTTON_UP when MouseUp is not null:
                MouseUp(button);
                break;
        }

        if (evt.type == (uint) SDL_EventType.SDL_EVENT_MOUSE_WHEEL)
        {
            _wheelValue += evt.wheel.y * 120;
        }
    }

    private static MouseButton TranslatePlatformMouseButton(byte button)
    {
        return button switch
        {
            1 => MouseButton.Left,
            2 => MouseButton.Middle,
            3 => MouseButton.Right,
            _ => MouseButton.None
        };
    }
    
    private static bool _supportsGlobalMouse;
    private static float _wheelValue;
}