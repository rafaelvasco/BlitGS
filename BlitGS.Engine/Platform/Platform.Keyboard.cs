using System.Collections.Generic;
using static bottlenoselabs.SDL;

namespace BlitGS.Engine;

internal static partial class Platform
{
    public static KeyEvent? KeyDown;
    public static KeyEvent? KeyUp;
    public static TextInputEvent? TextInput;

    private static readonly HashSet<Key> KeyPressBuffer = new();

    private static Dictionary<int, Key> KeyMap = null!;

    private static void InitKeyboard()
    {
        KeyMap = new Dictionary<int, Key>()
        {
            { 8, Key.Back },
            { 9, Key.Tab },
            { 13, Key.Enter },
            { 27, Key.Escape },
            { 32, Key.Space },
            { 39, Key.OemQuotes },
            { 43, Key.Add },
            { 44, Key.OemComma },
            { 45, Key.OemMinus },
            { 46, Key.OemPeriod },
            { 47, Key.OemQuestion },
            { 48, Key.D0 },
            { 49, Key.D1 },
            { 50, Key.D2 },
            { 51, Key.D3 },
            { 52, Key.D4 },
            { 53, Key.D5 },
            { 54, Key.D6 },
            { 55, Key.D7 },
            { 56, Key.D8 },
            { 57, Key.D9 },
            { 59, Key.OemSemicolon },
            { 60, Key.OemBackslash },
            { 61, Key.OemPlus },
            { 91, Key.OemOpenBrackets },
            { 92, Key.OemPipe },
            { 93, Key.OemCloseBrackets },
            { 96, Key.OemTilde },
            { 97, Key.A },
            { 98, Key.B },
            { 99, Key.C },
            { 100, Key.D },
            { 101, Key.E },
            { 102, Key.F },
            { 103, Key.G },
            { 104, Key.H },
            { 105, Key.I },
            { 106, Key.J },
            { 107, Key.K },
            { 108, Key.L },
            { 109, Key.M },
            { 110, Key.N },
            { 111, Key.O },
            { 112, Key.P },
            { 113, Key.Q },
            { 114, Key.R },
            { 115, Key.S },
            { 116, Key.T },
            { 117, Key.U },
            { 118, Key.V },
            { 119, Key.W },
            { 120, Key.X },
            { 121, Key.Y },
            { 122, Key.Z },
            { 127, Key.Delete },
            { 1073741881, Key.CapsLock },
            { 1073741882, Key.F1 },
            { 1073741883, Key.F2 },
            { 1073741884, Key.F3 },
            { 1073741885, Key.F4 },
            { 1073741886, Key.F5 },
            { 1073741887, Key.F6 },
            { 1073741888, Key.F7 },
            { 1073741889, Key.F8 },
            { 1073741890, Key.F9 },
            { 1073741891, Key.F10 },
            { 1073741892, Key.F11 },
            { 1073741893, Key.F12 },
            { 1073741894, Key.PrintScreen },
            { 1073741895, Key.Scroll },
            { 1073741896, Key.Pause },
            { 1073741897, Key.Insert },
            { 1073741898, Key.Home },
            { 1073741899, Key.PageUp },
            { 1073741901, Key.End },
            { 1073741902, Key.PageDown },
            { 1073741903, Key.Right },
            { 1073741904, Key.Left },
            { 1073741905, Key.Down },
            { 1073741906, Key.Up },
            { 1073741907, Key.NumLock },
            { 1073741908, Key.Divide },
            { 1073741909, Key.Multiply },
            { 1073741910, Key.Subtract },
            { 1073741911, Key.Add },
            { 1073741912, Key.Enter },
            { 1073741913, Key.NumPad1 },
            { 1073741914, Key.NumPad2 },
            { 1073741915, Key.NumPad3 },
            { 1073741916, Key.NumPad4 },
            { 1073741917, Key.NumPad5 },
            { 1073741918, Key.NumPad6 },
            { 1073741919, Key.NumPad7 },
            { 1073741920, Key.NumPad8 },
            { 1073741921, Key.NumPad9 },
            { 1073741922, Key.NumPad0 },
            { 1073741923, Key.Decimal },
            { 1073741925, Key.Apps },
            { 1073741928, Key.F13 },
            { 1073741929, Key.F14 },
            { 1073741930, Key.F15 },
            { 1073741931, Key.F16 },
            { 1073741932, Key.F17 },
            { 1073741933, Key.F18 },
            { 1073741934, Key.F19 },
            { 1073741935, Key.F20 },
            { 1073741936, Key.F21 },
            { 1073741937, Key.F22 },
            { 1073741938, Key.F23 },
            { 1073741939, Key.F24 },
            { 1073741951, Key.VolumeMute },
            { 1073741952, Key.VolumeUp },
            { 1073741953, Key.VolumeDown },
            { 1073742040, Key.OemClear },
            { 1073742044, Key.Decimal },
            { 1073742048, Key.LeftControl },
            { 1073742049, Key.LeftShift },
            { 1073742050, Key.LeftAlt },
            { 1073742051, Key.LeftWindows },
            { 1073742052, Key.RightControl },
            { 1073742053, Key.RightShift },
            { 1073742054, Key.RightAlt },
            { 1073742055, Key.RightWindows },
            { 1073742082, Key.MediaNextTrack },
            { 1073742083, Key.MediaPreviousTrack },
            { 1073742084, Key.MediaStop },
            { 1073742085, Key.MediaPlayPause },
            { 1073742086, Key.VolumeMute },
            { 1073742087, Key.SelectMedia },
            { 1073742089, Key.LaunchMail },
            { 1073742092, Key.BrowserSearch },
            { 1073742093, Key.BrowserHome },
            { 1073742094, Key.BrowserBack },
            { 1073742095, Key.BrowserForward },
            { 1073742096, Key.BrowserStop },
            { 1073742097, Key.BrowserRefresh },
            { 1073742098, Key.BrowserFavorites },
            { 1073742106, Key.Sleep }
        };
    }
    

    public static KeyboardState GetKeyboardState()
    {
        return new KeyboardState(KeyPressBuffer);
    }

    internal static void ActivateTextInput(bool active)
    {
        if (active)
        {
            SDL_StartTextInput();
        }
        else
        {
            SDL_StopTextInput();
        }
    }

    // private static void ProcessTextInputEvent(SDL_Event evt)
    // {
    //     if (evt.type == (uint)SDL_EventType.SDL_EVENT_TEXT_INPUT)
    //     {
    //         var eventText = evt.text.text;
    //
    //         foreach (var t in eventText)
    //         {
    //             TextInput?.Invoke(new TextInputEventArgs(t, ConvertKey(t)));
    //         }
    //     }
    // }   

    private static Key ConvertKey(int keyChar)
    {
        return KeyMap.GetValueOrDefault(keyChar, Key.None);
    }

    private static void ProcessKeyEvent(SDL_Event evt)
    {
        switch (evt.type)
        {
            case (uint)SDL_EventType.SDL_EVENT_KEY_DOWN:
                {
                    Key key = ConvertKey(evt.key.keysym.sym);

                    KeyPressBuffer.Add(key);

                    if (TextInput != null)
                    {
                        char character = (char)evt.key.keysym.sym;

                        if (char.IsControl(character))
                        {
                            TextInput.Invoke(new TextInputEventArgs(character, key));
                        }
                    }

                    KeyDown?.Invoke(key);

                    break;
                }
            case (uint)SDL_EventType.SDL_EVENT_KEY_UP:
                {
                    Key key = ConvertKey(evt.key.keysym.sym);

                    KeyPressBuffer.Remove(key);

                    KeyUp?.Invoke(key);
                    break;
                }
        }
    }
}