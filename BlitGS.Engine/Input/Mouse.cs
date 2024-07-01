using System;
using System.Text;

namespace BlitGS.Engine;

[Flags]
public enum MouseButton
{
    None = 0,
    Left = 1,
    Middle = 2,
    Right = 3
}

public delegate void MouseButtonEvent(MouseButton button);

public delegate void MouseEvent(int x, int y);

/// <summary>
/// Represents a mouse state with cursor position and button press information.
/// </summary>
public readonly struct MouseState
{
    public bool this[MouseButton button] => (Buttons & button) == button;

    /// <summary>
    /// Gets horizontal position of the cursor.
    /// </summary>
    public int X { get; }

    /// <summary>
    /// Gets vertical position of the cursor.
    /// </summary>
    public int Y { get; }

    /// <summary>
    /// Gets state of the buttons.
    /// </summary>
    public MouseButton Buttons { get; }

    /// <summary>
    /// Returns cumulative scroll wheel value since the game start.
    /// </summary>
    public int ScrollWheelValue { get; }

    /// <summary>
    /// Initializes a new instance of the MouseState.
    /// </summary>
    /// <param name="x">Horizontal position of the mouse.</param>
    /// <param name="y">Vertical position of the mouse.</param>
    /// <param name="scrollWheel">Mouse scroll wheel's value.</param>
    /// <param name="left">Left mouse button's state.</param>
    /// <param name="middle">Middle mouse button's state.</param>
    /// <param name="right">Right mouse button's state.</param>
    public MouseState(
        int x,
        int y,
        int scrollWheel,
        bool left,
        bool middle,
        bool right
    ) : this()
    {
        X = x;
        Y = y;
        ScrollWheelValue = scrollWheel;

        if (left)
        {
            Buttons |= MouseButton.Left;
        }

        if (right)
        {
            Buttons |= MouseButton.Right;
        }

        if (middle)
        {
            Buttons |= MouseButton.Middle;
        }
    }

    /// <summary>
    /// Compares whether two MouseState instances are equal.
    /// </summary>
    /// <param name="left">MouseState instance on the left of the equal sign.</param>
    /// <param name="right">MouseState instance on the right of the equal sign.</param>
    /// <returns>true if the instances are equal; false otherwise.</returns>
    public static bool operator ==(MouseState left, MouseState right)
    {
        return (left.X == right.X &&
                left.Y == right.Y &&
                left.Buttons == right.Buttons &&
                left.ScrollWheelValue == right.ScrollWheelValue);
    }

    /// <summary>
    /// Compares whether two MouseState instances are not equal.
    /// </summary>
    /// <param name="left">MouseState instance on the left of the equal sign.</param>
    /// <param name="right">MouseState instance on the right of the equal sign.</param>
    /// <returns>true if the objects are not equal; false otherwise.</returns>
    public static bool operator !=(MouseState left, MouseState right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Compares whether current instance is equal to specified object.
    /// </summary>
    /// <param name="obj">The MouseState to compare.</param>
    /// <returns></returns>
    public override bool Equals(object? obj)
    {
        return (obj is MouseState state) && (this == state);
    }

    public bool Equals(MouseState other)
    {
        return X == other.X && Y == other.Y && Buttons == other.Buttons &&
               ScrollWheelValue == other.ScrollWheelValue;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Buttons, ScrollWheelValue);
    }

    /// <summary>
    /// Returns a string describing the mouse state.
    /// </summary>
    public override string ToString()
    {
        var buttons = new StringBuilder();

        if ((Buttons & MouseButton.Left) != 0)
        {
            buttons.Append("Left");
        }

        if ((Buttons & MouseButton.Right) != 0)
        {
            if (buttons.Length > 0)
            {
                buttons.Append(' ');
            }

            buttons.Append("Right");
        }

        if ((Buttons & MouseButton.Middle) != 0)
        {
            if (buttons.Length > 0)
            {
                buttons.Append(' ');
            }

            buttons.Append("Middle");
        }

        if (buttons.Length == 0)
        {
            buttons.Append("None");
        }

        return $"[MouseState X={X}, Y={Y}, Buttons={buttons}, Wheel={ScrollWheelValue}]";
    }
}

public static class Mouse
{
    public static event MouseButtonEvent? OnMouseDown;
    public static event MouseButtonEvent? OnMouseUp;
    public static event MouseEvent? OnMouseMove;
    public static event MouseEvent? OnMouseEntered;
    public static event MouseEvent? OnMouseExited;

    public static bool EnableMouse { get; set; } = true;

    public static (int X, int Y) MousePos
    {
        get
        {
            //(float transformedX, float transformedY) = Canvas.TransformPointToViewportTransform(_msState.X, _msState.Y);
            return ((int)_msState.X, (int)_msState.Y);
        }
    }

    public static int DeltaX => _msState.X - _prevMsState.X;
    public static int DeltaY => _msState.Y - _prevMsState.Y;

    public static bool ButtonDown(MouseButton button)
    {
        return _msState[button];
    }

    public static bool ButtonPressed(MouseButton button)
    {
        return _msState[button] && !_prevMsState[button];
    }

    public static bool ButtonReleased(MouseButton button)
    {
        return !_msState[button] && _prevMsState[button];
    }

    internal static void Init()
    {
        _msState = Platform.GetMouseState();

        Platform.MouseDown = button => { OnMouseDown?.Invoke(button); };

        Platform.MouseUp = button => { OnMouseUp?.Invoke(button); };

        Platform.MouseMove = (x, y) =>
        {
            //(float transformedX, float transformedY) = Canvas.TransformPointToViewportTransform(x, y);
            OnMouseMove?.Invoke((int)x, (int)y);
        };

        Platform.OnWindowMouseEntered = () =>
        {
            OnMouseEntered?.Invoke(MousePos.X, MousePos.Y);
        };

        Platform.OnWindowMouseExited = () =>
        {
            OnMouseExited?.Invoke(MousePos.X, MousePos.Y);
        };
    }

    internal static void UpdateState()
    {
        _prevMsState = _msState;
        _msState = Platform.GetMouseState();
    }

    private static MouseState _msState;
    private static MouseState _prevMsState;
}