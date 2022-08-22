using System;
using System.Collections.Generic;
using SharpHook.Native;
using KeyCode = SharpHook.Native.NativeKeyCode;

namespace NoStopMod.Helper
{
    
    /*
     Normal Keys, Numpad keys is reversed
     
     Normal Keys
     UpArrow ->     VcNumPadUp;
     DownArrow ->   VcNumPadDown;
     LeftArrow ->   VcNumPadLeft;
     RightArrow ->  VcNumPadRight;
     Insert ->      VcNumPadInsert;
     Home ->        VcNumPadHome;
     PageUp ->      VcNumPadPageUp;
     Delete ->      VcNumPadDelete;
     End ->         VcNumPadEnd;
     PageDown ->    VcNumPadPageDown;

     
     
     Numpad Keys

     UpArrow ->     VcUp
     DownArrow ->   VcDown
     LeftArrow ->   VcLeft
     RightArrow ->  VcRight
     Insert ->      VcInsert
     Home ->        VcHome
     PageUp ->      VcPageUp
     Delete ->      VcDelete
     End ->         VcEnd
     PageDown ->    VcPageDown
     */
    
    public class KeyCodeHelper
    {
        
        private static readonly IReadOnlyDictionary<ushort, ushort> NativeKeyCodeMapper;
        public static readonly IReadOnlyList<Tuple<UnityEngine.KeyCode, ushort>> UnityNativeKeyCodeList;
        
        static KeyCodeHelper()
        {
            NativeKeyCodeMapper = InitNativeKeyCodeMapper();
            UnityNativeKeyCodeList = InitUnityNativeKeyCodeList();
        }

        private static List<Tuple<UnityEngine.KeyCode, ushort>> InitUnityNativeKeyCodeList() => new()
        {
            new(UnityEngine.KeyCode.None,           (ushort) KeyCode.Undefined ),
            new(UnityEngine.KeyCode.Escape,         (ushort) KeyCode.Escape ),
            new(UnityEngine.KeyCode.Alpha1,         (ushort) KeyCode.Alpha1 ),
            new(UnityEngine.KeyCode.Alpha2,         (ushort) KeyCode.Alpha2 ),
            new(UnityEngine.KeyCode.Alpha3,         (ushort) KeyCode.Alpha3 ),
            new(UnityEngine.KeyCode.Alpha4,         (ushort) KeyCode.Alpha4 ),
            new(UnityEngine.KeyCode.Alpha5,         (ushort) KeyCode.Alpha5 ),
            new(UnityEngine.KeyCode.Alpha6,         (ushort) KeyCode.Alpha6 ),
            new(UnityEngine.KeyCode.Alpha7,         (ushort) KeyCode.Alpha7 ),
            new(UnityEngine.KeyCode.Alpha8,         (ushort) KeyCode.Alpha8 ),
            new(UnityEngine.KeyCode.Alpha9,         (ushort) KeyCode.Alpha9 ),
            new(UnityEngine.KeyCode.Alpha0,         (ushort) KeyCode.Alpha0 ),
            new(UnityEngine.KeyCode.Minus,          (ushort) KeyCode.Minus ),
            new(UnityEngine.KeyCode.Equals,         (ushort) KeyCode.Equals ),
            new(UnityEngine.KeyCode.Backspace,      (ushort) KeyCode.Backspace ),
            new(UnityEngine.KeyCode.Tab,            (ushort) KeyCode.Tab ),
            new(UnityEngine.KeyCode.Q,              (ushort) KeyCode.Q ),
            new(UnityEngine.KeyCode.W,              (ushort) KeyCode.W ),
            new(UnityEngine.KeyCode.E,              (ushort) KeyCode.E ),
            new(UnityEngine.KeyCode.R,              (ushort) KeyCode.R ),
            new(UnityEngine.KeyCode.T,              (ushort) KeyCode.T ),
            new(UnityEngine.KeyCode.Y,              (ushort) KeyCode.Y ),
            new(UnityEngine.KeyCode.U,              (ushort) KeyCode.U ),
            new(UnityEngine.KeyCode.I,              (ushort) KeyCode.I ),
            new(UnityEngine.KeyCode.O,              (ushort) KeyCode.O ),
            new(UnityEngine.KeyCode.P,              (ushort) KeyCode.P ),
            new(UnityEngine.KeyCode.LeftBracket,    (ushort) KeyCode.OpenBracket ),
            new(UnityEngine.KeyCode.RightBracket,   (ushort) KeyCode.CloseBracket ),
            new(UnityEngine.KeyCode.Return,         (ushort) KeyCode.Enter ),
            new(UnityEngine.KeyCode.LeftControl,    (ushort) KeyCode.LeftControl ),
            new(UnityEngine.KeyCode.A,              (ushort) KeyCode.A ),
            new(UnityEngine.KeyCode.S,              (ushort) KeyCode.S ),
            new(UnityEngine.KeyCode.D,              (ushort) KeyCode.D ),
            new(UnityEngine.KeyCode.F,              (ushort) KeyCode.F ),
            new(UnityEngine.KeyCode.G,              (ushort) KeyCode.G ),
            new(UnityEngine.KeyCode.H,              (ushort) KeyCode.H ),
            new(UnityEngine.KeyCode.J,              (ushort) KeyCode.J ),
            new(UnityEngine.KeyCode.K,              (ushort) KeyCode.K ),
            new(UnityEngine.KeyCode.L,              (ushort) KeyCode.L ),
            new(UnityEngine.KeyCode.Semicolon,      (ushort) KeyCode.Semicolon ),
            new(UnityEngine.KeyCode.Quote,          (ushort) KeyCode.Quote ),
            new(UnityEngine.KeyCode.BackQuote,      (ushort) KeyCode.Backquote ),
            new(UnityEngine.KeyCode.LeftShift,      (ushort) KeyCode.LeftShift ),
            new(UnityEngine.KeyCode.Backslash,      (ushort) KeyCode.BackSlash ),
            new(UnityEngine.KeyCode.Z,              (ushort) KeyCode.Z ),
            new(UnityEngine.KeyCode.X,              (ushort) KeyCode.X ),
            new(UnityEngine.KeyCode.C,              (ushort) KeyCode.C ),
            new(UnityEngine.KeyCode.V,              (ushort) KeyCode.V ),
            new(UnityEngine.KeyCode.B,              (ushort) KeyCode.B ),
            new(UnityEngine.KeyCode.N,              (ushort) KeyCode.N ),
            new(UnityEngine.KeyCode.M,              (ushort) KeyCode.M ),
            new(UnityEngine.KeyCode.Comma,          (ushort) KeyCode.Comma ),
            new(UnityEngine.KeyCode.Period,         (ushort) KeyCode.Period ),
            new(UnityEngine.KeyCode.Slash,          (ushort) KeyCode.Slash ),
            new(UnityEngine.KeyCode.RightShift,     (ushort) KeyCode.RightShift ),
            new(UnityEngine.KeyCode.KeypadMultiply, (ushort) KeyCode.NumPadMultiply ),
            new(UnityEngine.KeyCode.LeftAlt,        (ushort) KeyCode.LeftAlt ),
            new(UnityEngine.KeyCode.Space,          (ushort) KeyCode.Space ),
            new(UnityEngine.KeyCode.CapsLock,       (ushort) KeyCode.CapsLock ),
            new(UnityEngine.KeyCode.F1,             (ushort) KeyCode.F1 ),
            new(UnityEngine.KeyCode.F2,             (ushort) KeyCode.F2 ),
            new(UnityEngine.KeyCode.F3,             (ushort) KeyCode.F3 ),
            new(UnityEngine.KeyCode.F4,             (ushort) KeyCode.F4 ),
            new(UnityEngine.KeyCode.F5,             (ushort) KeyCode.F5 ),
            new(UnityEngine.KeyCode.F6,             (ushort) KeyCode.F6 ),
            new(UnityEngine.KeyCode.F7,             (ushort) KeyCode.F7 ),
            new(UnityEngine.KeyCode.F8,             (ushort) KeyCode.F8 ),
            new(UnityEngine.KeyCode.F9,             (ushort) KeyCode.F9 ),
            new(UnityEngine.KeyCode.F10,            (ushort) KeyCode.F10 ),
            new(UnityEngine.KeyCode.Numlock,        (ushort) KeyCode.NumLock ),
            new(UnityEngine.KeyCode.ScrollLock,     (ushort) KeyCode.ScrollLock ),
            new(UnityEngine.KeyCode.Keypad7,        (ushort) KeyCode.NumPad7 ),
            new(UnityEngine.KeyCode.Keypad8,        (ushort) KeyCode.NumPad8 ),
            new(UnityEngine.KeyCode.Keypad9,        (ushort) KeyCode.NumPad9 ),
            new(UnityEngine.KeyCode.KeypadMinus,    (ushort) KeyCode.NumPadSubtract ),
            new(UnityEngine.KeyCode.Keypad4,        (ushort) KeyCode.NumPad4 ),
            new(UnityEngine.KeyCode.Keypad5,        (ushort) KeyCode.NumPad5 ),
            new(UnityEngine.KeyCode.Keypad6,        (ushort) KeyCode.NumPad6 ),
            new(UnityEngine.KeyCode.KeypadPlus,     (ushort) KeyCode.NumPadAdd ),
            new(UnityEngine.KeyCode.Keypad1,        (ushort) KeyCode.NumPad1 ),
            new(UnityEngine.KeyCode.Keypad2,        (ushort) KeyCode.NumPad2 ),
            new(UnityEngine.KeyCode.Keypad3,        (ushort) KeyCode.NumPad3 ),
            new(UnityEngine.KeyCode.Keypad0,        (ushort) KeyCode.NumPad0 ),
            new(UnityEngine.KeyCode.KeypadPeriod,   (ushort) KeyCode.NumPadSeparator ),
            new(UnityEngine.KeyCode.F11,            (ushort) KeyCode.F11 ),
            new(UnityEngine.KeyCode.F12,            (ushort) KeyCode.F12 ),
            new(UnityEngine.KeyCode.F13,            (ushort) KeyCode.F13 ),
            new(UnityEngine.KeyCode.F14,            (ushort) KeyCode.F14 ),
            new(UnityEngine.KeyCode.F15,            (ushort) KeyCode.F15 ),
            new(UnityEngine.KeyCode.Underscore,     (ushort) KeyCode.Underscore ),
            new(UnityEngine.KeyCode.KeypadEquals,   (ushort) KeyCode.NumPadEquals ),
            new(UnityEngine.KeyCode.RightControl,   (ushort) KeyCode.RightControl ),
            new(UnityEngine.KeyCode.KeypadDivide,   (ushort) KeyCode.NumPadDivide ),
            new(UnityEngine.KeyCode.Print,          (ushort) KeyCode.Printscreen ),
            new(UnityEngine.KeyCode.RightAlt,       (ushort) KeyCode.RightAlt ),
            new(UnityEngine.KeyCode.Pause,          (ushort) KeyCode.Pause ),
            new(UnityEngine.KeyCode.LeftMeta,       (ushort) KeyCode.LeftMeta ),
            new(UnityEngine.KeyCode.RightMeta,      (ushort) KeyCode.RightMeta ),
            new(UnityEngine.KeyCode.Menu,           (ushort) KeyCode.ContextMenu ),
            new(UnityEngine.KeyCode.Clear,          (ushort) KeyCode.Enter ),
            new(UnityEngine.KeyCode.KeypadEnter,    (ushort) KeyCode.NumPadEnter ),
            new(UnityEngine.KeyCode.LeftApple,      (ushort) KeyCode.LeftMeta ),
            new(UnityEngine.KeyCode.UpArrow,        (ushort) KeyCode.NumPadUp ),
            new(UnityEngine.KeyCode.DownArrow,      (ushort) KeyCode.NumPadDown ),
            new(UnityEngine.KeyCode.LeftArrow,      (ushort) KeyCode.NumPadLeft ),
            new(UnityEngine.KeyCode.RightArrow,     (ushort) KeyCode.NumPadRight ),
            new(UnityEngine.KeyCode.Insert,         (ushort) KeyCode.NumPadInsert ),
            new(UnityEngine.KeyCode.Home,           (ushort) KeyCode.NumPadHome ),
            new(UnityEngine.KeyCode.PageUp,         (ushort) KeyCode.NumPadPageUp ),
            new(UnityEngine.KeyCode.Delete,         (ushort) KeyCode.NumPadDelete ),
            new(UnityEngine.KeyCode.End,            (ushort) KeyCode.NumPadEnd ),
            new(UnityEngine.KeyCode.PageDown,       (ushort) KeyCode.NumPadPageDown ),
            new(UnityEngine.KeyCode.Mouse0,         (ushort) (1000 + MouseButton.Button1) ),
            new(UnityEngine.KeyCode.Mouse1,         (ushort) (1000 + MouseButton.Button2) ),
            new(UnityEngine.KeyCode.Mouse2,         (ushort) (1000 + MouseButton.Button3) ),
            new(UnityEngine.KeyCode.Mouse3,         (ushort) (1000 + MouseButton.Button4) ),
            new(UnityEngine.KeyCode.Mouse4,         (ushort) (1000 + MouseButton.Button5) ),
        };

        private static Dictionary<ushort, ushort> InitNativeKeyCodeMapper() => new()
        {
            [(ushort) KeyCode.Delete] =     (ushort) KeyCode.NumPadDelete,      // original position : KeyCode.VcNumPadSeparator
            [(ushort) KeyCode.Insert] =     (ushort) KeyCode.NumPadInsert,      // original position : KeyCode.VcNumPad0
            [(ushort) KeyCode.End] =        (ushort) KeyCode.NumPadEnd,         // original position : KeyCode.VcNumPad1
            [(ushort) KeyCode.Down] =       (ushort) KeyCode.NumPadDown,        // original position : KeyCode.VcNumPad2
            [(ushort) KeyCode.PageDown] =   (ushort) KeyCode.NumPadPageDown,    // original position : KeyCode.VcNumPad3
            [(ushort) KeyCode.Left] =       (ushort) KeyCode.NumPadLeft,        // original position : KeyCode.VcNumPad4
            [(ushort) KeyCode.Right] =      (ushort) KeyCode.NumPadRight,       // original position : KeyCode.VcNumPad6
            [(ushort) KeyCode.Home] =       (ushort) KeyCode.NumPadHome,        // original position : KeyCode.VcNumPad7
            [(ushort) KeyCode.Up] =         (ushort) KeyCode.NumPadUp,          // original position : KeyCode.VcNumPad8
            [(ushort) KeyCode.PageUp] =     (ushort) KeyCode.NumPadPageUp,      // original position : KeyCode.VcNumPad9
        };
        

        public static ushort ConvertNativeKeyCode(ushort keyCode)
        {
            return NativeKeyCodeMapper.TryGetValue(keyCode, out var newKeyCode) ? newKeyCode 
                : keyCode;
        }

    }
    
}
