using System;
using System.Collections.Generic;
using System.Numerics;
using bottlenoselabs.C2CS.Runtime;
using static bottlenoselabs.SDL;

namespace BlitGS.Engine;

internal static unsafe partial class Platform
{
    public static int ConnectedGamePads { get; private set; }
    
    public static int FlippedFaceButtons { get; private set; }
    
    private static void InitGamepad()
    {
        var hint = SDL_GetHint((CString)SDL_HINT_JOYSTICK_ALLOW_BACKGROUND_EVENTS);

        if (string.IsNullOrEmpty((string?)hint))
        {
            SDL_SetHint((CString)SDL_HINT_JOYSTICK_ALLOW_BACKGROUND_EVENTS, (CString)"1");
        }

        _ = SDL_InitSubSystem((uint)SDL_InitFlags.SDL_INIT_GAMEPAD);
    }
    
    
    public static GamePadCapabilities GetGamePadCapabilities(int index)
    {
        return GamepadDevices[index] == IntPtr.Zero ? new GamePadCapabilities() : GamepadCaps[index];
    }
    
    public static string GetGamePadName(int index)
    {
        index = MathUtils.Clamp(index, 0, GamepadDevices.Length - 1);

        return (string)SDL_GetGamepadName((SDL_Gamepad*)GamepadDevices[index]);
    }
    
    public static GamePadState GetGamePadState(int index, GamePadDeadZone deadZoneMode)
    { 
        var device = (SDL_Gamepad*)GamepadDevices[index];
        if (device == null)
        {
            return new GamePadState();
        }

        GamePadButtons gcButtonState = 0;
        

        // Sticks
        var stickLeft = new Vector2(
            SDL_GetGamepadAxis(
                device,
                SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFTX
            ) / JoystickAxisMax,
            SDL_GetGamepadAxis(
                device,
                SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFTY
            ) / -JoystickAxisMax
        );
        var stickRight = new Vector2(
            SDL_GetGamepadAxis(
                device,
                SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHTX
            ) / JoystickAxisMax,
            SDL_GetGamepadAxis(
                device,
                SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHTY
            ) / -JoystickAxisMax
        );

        gcButtonState |= ConvertStickValuesToButtons(
            stickLeft,
            GamePadButtons.LeftThumbstickLeft,
            GamePadButtons.LeftThumbstickRight,
            GamePadButtons.LeftThumbstickUp,
            GamePadButtons.LeftThumbstickDown,
            Gamepad.LeftDeadZone
        );
        gcButtonState |= ConvertStickValuesToButtons(
            stickRight,
            GamePadButtons.RightThumbstickLeft,
            GamePadButtons.RightThumbstickRight,
            GamePadButtons.RightThumbstickUp,
            GamePadButtons.RightThumbstickDown,
            Gamepad.RightDeadZone
        );

        // Triggers
        float triggerLeft = SDL_GetGamepadAxis(
            device,
            SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFT_TRIGGER
        ) / JoystickAxisMax;
        float triggerRight = SDL_GetGamepadAxis(
            device,
            SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHT_TRIGGER
        ) / JoystickAxisMax;
        
        if (triggerLeft > Gamepad.TriggerThreshold)
        {
            gcButtonState |= GamePadButtons.LeftTrigger;
        }

        if (triggerRight > Gamepad.TriggerThreshold)
        {
            gcButtonState |= GamePadButtons.RightTrigger;
        }

        // Buttons
        if (SDL_GetGamepadButton(device, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_SOUTH) != 0)
        {
            gcButtonState |= GamePadButtons.South;
        }

        if (SDL_GetGamepadButton(device, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_EAST) != 0)
        {
            gcButtonState |= GamePadButtons.East;
        }

        if (SDL_GetGamepadButton(device, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_WEST) != 0)
        {
            gcButtonState |= GamePadButtons.West;
        }

        if (SDL_GetGamepadButton(device, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_NORTH) != 0)
        {
            gcButtonState |= GamePadButtons.North;
        }

        if (SDL_GetGamepadButton(device, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_BACK) != 0)
        {
            gcButtonState |= GamePadButtons.Back;
        }

        if (SDL_GetGamepadButton(device, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_START) != 0)
        {
            gcButtonState |= GamePadButtons.Start;
        }

        if (SDL_GetGamepadButton(device, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_LEFT_STICK) != 0)
        {
            gcButtonState |= GamePadButtons.LeftStick;
        }

        if (SDL_GetGamepadButton(device, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_RIGHT_STICK) != 0)
        {
            gcButtonState |= GamePadButtons.RightStick;
        }

        if (SDL_GetGamepadButton(device, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_LEFT_SHOULDER) != 0)
        {
            gcButtonState |= GamePadButtons.LeftShoulder;
        }

        if (SDL_GetGamepadButton(device, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_RIGHT_SHOULDER) != 0)
        {
            gcButtonState |= GamePadButtons.RightShoulder;
        }

        if (SDL_GetGamepadButton(device, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_UP) != 0)
        {
            gcButtonState |= GamePadButtons.DPadUp;
        }

        if (SDL_GetGamepadButton(device, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_DOWN) != 0)
        {
            gcButtonState |= GamePadButtons.DPadDown;
        }

        if (SDL_GetGamepadButton(device, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_LEFT) != 0)
        {
            gcButtonState |= GamePadButtons.DPadLeft;
        }

        if (SDL_GetGamepadButton(device, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_RIGHT) != 0)
        {
            gcButtonState |= GamePadButtons.DPadRight;
        }

        // Build the GamePadState, increment PacketNumber if state changed.

        var gcBuiltState = new GamePadState(
            new GamePadThumbSticks(stickLeft, stickRight, deadZoneMode),
            new GamePadTriggers(triggerLeft, triggerRight, deadZoneMode),
            gcButtonState
        )
        {
            IsConnected = true,
            PacketNumber = GamepadStates[index].PacketNumber
        };
        
        if (gcBuiltState != GamepadStates[index])
        {
            gcBuiltState.PacketNumber += 1;
            GamepadStates[index] = gcBuiltState;
        }

        return gcBuiltState;
    }
    
    public static bool SetGamePadVibration(int index, float leftMotor, float rightMotor)
    {
        if (!GamepadCaps[index].HasVibration)
        {
            return false;
        }
        
        var device = (SDL_Gamepad*)GamepadDevices[index];
        if (device == null)
        {
            return false;
        }

        return SDL_RumbleGamepad(
            device,
            (ushort)(MathUtils.Clamp(leftMotor, 0.0f, 1.0f) * 0xFFFF),
            (ushort)(MathUtils.Clamp(rightMotor, 0.0f, 1.0f) * 0xFFFF),
            0
        ) == 0;
    }
    
    private static void AddGamePadInstance(SDL_JoystickID deviceId)
    {
        if (ConnectedGamePads == Gamepad.MaxCount)
        {
            return;
        }

        SDL_ClearError();

        int which = ConnectedGamePads++;
        
        // Open the device!
        var device = SDL_OpenGamepad(deviceId);

        var joystick = SDL_GetGamepadJoystick(device);

        var joystickInstanceId = SDL_GetJoystickInstanceID(joystick);
        
        Console.WriteLine($"Adding GamePad Instance: {joystickInstanceId.Data}");

        GamepadDeviceMap[joystickInstanceId] = which;

        GamepadDevices[which] = (IntPtr)device;
            
        // Start with a fresh state.
        GamepadStates[which] = new GamePadState
        {
            IsConnected = true
        };

        FillGamepadCaps(device, which);

        Console.WriteLine($"GamePad Added: {GetGamePadName(which)}");
    }

    private static void FillGamepadCaps(SDL_Gamepad* device, int index)
    {
        var gamepadCaps = new GamePadCapabilities()
        {
            IsConnected = true,
            HasAButton = SDL_GamepadHasButton(device, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_SOUTH),
            HasBButton = SDL_GamepadHasButton(device, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_EAST),
            HasXButton = SDL_GamepadHasButton(device, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_WEST),
            HasYButton = SDL_GamepadHasButton(device, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_NORTH),
            HasBackButton = SDL_GamepadHasButton(device, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_BACK),
            HasStartButton = SDL_GamepadHasButton(device, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_START),
            HasDPadLeftButton = SDL_GamepadHasButton(device, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_LEFT),
            HasDPadRightButton = SDL_GamepadHasButton(device, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_RIGHT),
            HasDPadUpButton = SDL_GamepadHasButton(device, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_UP),
            HasDPadDownButton = SDL_GamepadHasButton(device, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_DOWN),
            HasLeftXThumbStick = SDL_GamepadHasAxis(device, SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFTX),
            HasLeftYThumbStick = SDL_GamepadHasAxis(device, SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFTY),
            HasRightXThumbStick = SDL_GamepadHasAxis(device, SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHTX),
            HasRightYThumbStick = SDL_GamepadHasAxis(device, SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHTY),
            HasLeftTrigger = SDL_GamepadHasAxis(device, SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFT_TRIGGER),
            HasRightTrigger = SDL_GamepadHasAxis(device, SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHT_TRIGGER),
            HasLeftStickButton = SDL_GamepadHasButton(device, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_LEFT_STICK),
            HasRightStickButton = SDL_GamepadHasButton(device, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_RIGHT_STICK),
            HasLeftShoulderButton = SDL_GamepadHasButton(device, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_LEFT_SHOULDER),
            HasRightShoulderButton = SDL_GamepadHasButton(device, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_RIGHT_SHOULDER),
            HasVibration = SDL_GamepadHasRumble(device),
        };

        GamepadCaps[index] = gamepadCaps;
    }

    private static void RemoveGamePadInstance(SDL_JoystickID deviceId)
    {
        Console.WriteLine("GamePad Removed");
        if (!GamepadDeviceMap.TryGetValue(deviceId, out int deviceIndex))
        {
            return;
        }

        SDL_CloseGamepad((SDL_Gamepad*)GamepadDevices[deviceIndex]);
        
        GamepadDevices[deviceIndex] = IntPtr.Zero;
        GamepadStates[deviceIndex] = new GamePadState();

        SDL_ClearError();

        ConnectedGamePads--;
    }
    
    

    private static GamePadButtons ConvertStickValuesToButtons(Vector2 stick, GamePadButtons left, GamePadButtons right,
        GamePadButtons up, GamePadButtons down, float deadZoneSize)
    {
        GamePadButtons b = 0;

        var (x, y) = stick;
        if (x > deadZoneSize)
        {
            b |= right;
        }

        if (x < -deadZoneSize)
        {
            b |= left;
        }

        if (y > deadZoneSize)
        {
            b |= up;
        }

        if (y < -deadZoneSize)
        {
            b |= down;
        }

        return b;
    }

   
    private static void ProcessGamePadEvent(SDL_Event evt)
    {
        switch (evt.type)
        {
            case (uint) SDL_EventType.SDL_EVENT_GAMEPAD_ADDED:
                AddGamePadInstance(evt.gdevice.which);
                break;
            case (uint) SDL_EventType.SDL_EVENT_GAMEPAD_REMOVED:
                RemoveGamePadInstance(evt.gdevice.which);
                break;
        }
    }
    
 
    
    

    private const float JoystickAxisMax = SDL_JOYSTICK_AXIS_MAX;
    
    private static readonly IntPtr[] GamepadDevices = new IntPtr[Gamepad.MaxCount];
    private static readonly GamePadCapabilities[] GamepadCaps = new GamePadCapabilities[Gamepad.MaxCount];
    private static readonly Dictionary<uint, int> GamepadDeviceMap = new();
    private static readonly GamePadState[] GamepadStates = new GamePadState[Gamepad.MaxCount];
}