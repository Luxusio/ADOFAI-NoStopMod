using System;
using System.Collections.Generic;
using SharpHook.Native;

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
            new(UnityEngine.KeyCode.None,           (ushort) KeyCode.VcUndefined ),
            new(UnityEngine.KeyCode.Escape,         (ushort) KeyCode.VcEscape ),
            new(UnityEngine.KeyCode.Alpha1,         (ushort) KeyCode.Vc1 ),
            new(UnityEngine.KeyCode.Alpha2,         (ushort) KeyCode.Vc2 ),
            new(UnityEngine.KeyCode.Alpha3,         (ushort) KeyCode.Vc3 ),
            new(UnityEngine.KeyCode.Alpha4,         (ushort) KeyCode.Vc4 ),
            new(UnityEngine.KeyCode.Alpha5,         (ushort) KeyCode.Vc5 ),
            new(UnityEngine.KeyCode.Alpha6,         (ushort) KeyCode.Vc6 ),
            new(UnityEngine.KeyCode.Alpha7,         (ushort) KeyCode.Vc7 ),
            new(UnityEngine.KeyCode.Alpha8,         (ushort) KeyCode.Vc8 ),
            new(UnityEngine.KeyCode.Alpha9,         (ushort) KeyCode.Vc9 ),
            new(UnityEngine.KeyCode.Alpha0,         (ushort) KeyCode.Vc0 ),
            new(UnityEngine.KeyCode.Minus,          (ushort) KeyCode.VcMinus ),
            new(UnityEngine.KeyCode.Equals,         (ushort) KeyCode.VcEquals ),
            new(UnityEngine.KeyCode.Backspace,      (ushort) KeyCode.VcBackspace ),
            new(UnityEngine.KeyCode.Tab,            (ushort) KeyCode.VcTab ),
            new(UnityEngine.KeyCode.Q,              (ushort) KeyCode.VcQ ),
            new(UnityEngine.KeyCode.W,              (ushort) KeyCode.VcW ),
            new(UnityEngine.KeyCode.E,              (ushort) KeyCode.VcE ),
            new(UnityEngine.KeyCode.R,              (ushort) KeyCode.VcR ),
            new(UnityEngine.KeyCode.T,              (ushort) KeyCode.VcT ),
            new(UnityEngine.KeyCode.Y,              (ushort) KeyCode.VcY ),
            new(UnityEngine.KeyCode.U,              (ushort) KeyCode.VcU ),
            new(UnityEngine.KeyCode.I,              (ushort) KeyCode.VcI ),
            new(UnityEngine.KeyCode.O,              (ushort) KeyCode.VcO ),
            new(UnityEngine.KeyCode.P,              (ushort) KeyCode.VcP ),
            new(UnityEngine.KeyCode.LeftBracket,    (ushort) KeyCode.VcOpenBracket ),
            new(UnityEngine.KeyCode.RightBracket,   (ushort) KeyCode.VcCloseBracket ),
            new(UnityEngine.KeyCode.Return,         (ushort) KeyCode.VcEnter ),
            new(UnityEngine.KeyCode.LeftControl,    (ushort) KeyCode.VcLeftControl ),
            new(UnityEngine.KeyCode.A,              (ushort) KeyCode.VcA ),
            new(UnityEngine.KeyCode.S,              (ushort) KeyCode.VcS ),
            new(UnityEngine.KeyCode.D,              (ushort) KeyCode.VcD ),
            new(UnityEngine.KeyCode.F,              (ushort) KeyCode.VcF ),
            new(UnityEngine.KeyCode.G,              (ushort) KeyCode.VcG ),
            new(UnityEngine.KeyCode.H,              (ushort) KeyCode.VcH ),
            new(UnityEngine.KeyCode.J,              (ushort) KeyCode.VcJ ),
            new(UnityEngine.KeyCode.K,              (ushort) KeyCode.VcK ),
            new(UnityEngine.KeyCode.L,              (ushort) KeyCode.VcL ),
            new(UnityEngine.KeyCode.Semicolon,      (ushort) KeyCode.VcSemicolon ),
            new(UnityEngine.KeyCode.Quote,          (ushort) KeyCode.VcQuote ),
            new(UnityEngine.KeyCode.BackQuote,      (ushort) KeyCode.VcBackquote ),
            new(UnityEngine.KeyCode.LeftShift,      (ushort) KeyCode.VcLeftShift ),
            new(UnityEngine.KeyCode.Backslash,      (ushort) KeyCode.VcBackSlash ),
            new(UnityEngine.KeyCode.Z,              (ushort) KeyCode.VcZ ),
            new(UnityEngine.KeyCode.X,              (ushort) KeyCode.VcX ),
            new(UnityEngine.KeyCode.C,              (ushort) KeyCode.VcC ),
            new(UnityEngine.KeyCode.V,              (ushort) KeyCode.VcV ),
            new(UnityEngine.KeyCode.B,              (ushort) KeyCode.VcB ),
            new(UnityEngine.KeyCode.N,              (ushort) KeyCode.VcN ),
            new(UnityEngine.KeyCode.M,              (ushort) KeyCode.VcM ),
            new(UnityEngine.KeyCode.Comma,          (ushort) KeyCode.VcComma ),
            new(UnityEngine.KeyCode.Period,         (ushort) KeyCode.VcPeriod ),
            new(UnityEngine.KeyCode.Slash,          (ushort) KeyCode.VcSlash ),
            new(UnityEngine.KeyCode.RightShift,     (ushort) KeyCode.VcRightShift ),
            new(UnityEngine.KeyCode.KeypadMultiply, (ushort) KeyCode.VcNumPadMultiply ),
            new(UnityEngine.KeyCode.LeftAlt,        (ushort) KeyCode.VcLeftAlt ),
            new(UnityEngine.KeyCode.Space,          (ushort) KeyCode.VcSpace ),
            new(UnityEngine.KeyCode.CapsLock,       (ushort) KeyCode.VcCapsLock ),
            new(UnityEngine.KeyCode.F1,             (ushort) KeyCode.VcF1 ),
            new(UnityEngine.KeyCode.F2,             (ushort) KeyCode.VcF2 ),
            new(UnityEngine.KeyCode.F3,             (ushort) KeyCode.VcF3 ),
            new(UnityEngine.KeyCode.F4,             (ushort) KeyCode.VcF4 ),
            new(UnityEngine.KeyCode.F5,             (ushort) KeyCode.VcF5 ),
            new(UnityEngine.KeyCode.F6,             (ushort) KeyCode.VcF6 ),
            new(UnityEngine.KeyCode.F7,             (ushort) KeyCode.VcF7 ),
            new(UnityEngine.KeyCode.F8,             (ushort) KeyCode.VcF8 ),
            new(UnityEngine.KeyCode.F9,             (ushort) KeyCode.VcF9 ),
            new(UnityEngine.KeyCode.F10,            (ushort) KeyCode.VcF10 ),
            new(UnityEngine.KeyCode.Numlock,        (ushort) KeyCode.VcNumLock ),
            new(UnityEngine.KeyCode.ScrollLock,     (ushort) KeyCode.VcScrollLock ),
            new(UnityEngine.KeyCode.Keypad7,        (ushort) KeyCode.VcNumPad7 ),
            new(UnityEngine.KeyCode.Keypad8,        (ushort) KeyCode.VcNumPad8 ),
            new(UnityEngine.KeyCode.Keypad9,        (ushort) KeyCode.VcNumPad9 ),
            new(UnityEngine.KeyCode.KeypadMinus,    (ushort) KeyCode.VcNumPadSubtract ),
            new(UnityEngine.KeyCode.Keypad4,        (ushort) KeyCode.VcNumPad4 ),
            new(UnityEngine.KeyCode.Keypad5,        (ushort) KeyCode.VcNumPad5 ),
            new(UnityEngine.KeyCode.Keypad6,        (ushort) KeyCode.VcNumPad6 ),
            new(UnityEngine.KeyCode.KeypadPlus,     (ushort) KeyCode.VcNumPadAdd ),
            new(UnityEngine.KeyCode.Keypad1,        (ushort) KeyCode.VcNumPad1 ),
            new(UnityEngine.KeyCode.Keypad2,        (ushort) KeyCode.VcNumPad2 ),
            new(UnityEngine.KeyCode.Keypad3,        (ushort) KeyCode.VcNumPad3 ),
            new(UnityEngine.KeyCode.Keypad0,        (ushort) KeyCode.VcNumPad0 ),
            new(UnityEngine.KeyCode.KeypadPeriod,   (ushort) KeyCode.VcNumPadSeparator ),
            new(UnityEngine.KeyCode.F11,            (ushort) KeyCode.VcF11 ),
            new(UnityEngine.KeyCode.F12,            (ushort) KeyCode.VcF12 ),
            new(UnityEngine.KeyCode.F13,            (ushort) KeyCode.VcF13 ),
            new(UnityEngine.KeyCode.F14,            (ushort) KeyCode.VcF14 ),
            new(UnityEngine.KeyCode.F15,            (ushort) KeyCode.VcF15 ),
            new(UnityEngine.KeyCode.Underscore,     (ushort) KeyCode.VcUnderscore ),
            new(UnityEngine.KeyCode.KeypadEquals,   (ushort) KeyCode.VcNumPadEquals ),
            new(UnityEngine.KeyCode.RightControl,   (ushort) KeyCode.VcRightControl ),
            new(UnityEngine.KeyCode.KeypadDivide,   (ushort) KeyCode.VcNumPadDivide ),
            new(UnityEngine.KeyCode.Print,          (ushort) KeyCode.VcPrintscreen ),
            new(UnityEngine.KeyCode.RightAlt,       (ushort) KeyCode.VcRightAlt ),
            new(UnityEngine.KeyCode.Pause,          (ushort) KeyCode.VcPause ),
            new(UnityEngine.KeyCode.LeftMeta,       (ushort) KeyCode.VcLeftMeta ),
            new(UnityEngine.KeyCode.RightMeta,      (ushort) KeyCode.VcRightMeta ),
            new(UnityEngine.KeyCode.Menu,           (ushort) KeyCode.VcContextMenu ),
            new(UnityEngine.KeyCode.Clear,          (ushort) KeyCode.VcEnter ),
            new(UnityEngine.KeyCode.KeypadEnter,    (ushort) KeyCode.VcNumPadEnter ),
            new(UnityEngine.KeyCode.LeftApple,      (ushort) KeyCode.VcLeftMeta ),
            new(UnityEngine.KeyCode.UpArrow,        (ushort) KeyCode.VcNumPadUp ),
            new(UnityEngine.KeyCode.DownArrow,      (ushort) KeyCode.VcNumPadDown ),
            new(UnityEngine.KeyCode.LeftArrow,      (ushort) KeyCode.VcNumPadLeft ),
            new(UnityEngine.KeyCode.RightArrow,     (ushort) KeyCode.VcNumPadRight ),
            new(UnityEngine.KeyCode.Insert,         (ushort) KeyCode.VcNumPadInsert ),
            new(UnityEngine.KeyCode.Home,           (ushort) KeyCode.VcNumPadHome ),
            new(UnityEngine.KeyCode.PageUp,         (ushort) KeyCode.VcNumPadPageUp ),
            new(UnityEngine.KeyCode.Delete,         (ushort) KeyCode.VcNumPadDelete ),
            new(UnityEngine.KeyCode.End,            (ushort) KeyCode.VcNumPadEnd ),
            new(UnityEngine.KeyCode.PageDown,       (ushort) KeyCode.VcNumPadPageDown ),
            new(UnityEngine.KeyCode.Mouse0,         (ushort) (1000 + MouseButton.Button1) ),
            new(UnityEngine.KeyCode.Mouse1,         (ushort) (1000 + MouseButton.Button2) ),
            new(UnityEngine.KeyCode.Mouse2,         (ushort) (1000 + MouseButton.Button3) ),
            new(UnityEngine.KeyCode.Mouse3,         (ushort) (1000 + MouseButton.Button4) ),
            new(UnityEngine.KeyCode.Mouse4,         (ushort) (1000 + MouseButton.Button5) ),
        };

        private static Dictionary<ushort, ushort> InitNativeKeyCodeMapper() => new()
        {
            [(ushort) KeyCode.VcDelete] =     (ushort) KeyCode.VcNumPadDelete,      // original position : KeyCode.VcNumPadSeparator
            [(ushort) KeyCode.VcInsert] =     (ushort) KeyCode.VcNumPadInsert,      // original position : KeyCode.VcNumPad0
            [(ushort) KeyCode.VcEnd] =        (ushort) KeyCode.VcNumPadEnd,         // original position : KeyCode.VcNumPad1
            [(ushort) KeyCode.VcDown] =       (ushort) KeyCode.VcNumPadDown,        // original position : KeyCode.VcNumPad2
            [(ushort) KeyCode.VcPageDown] =   (ushort) KeyCode.VcNumPadPageDown,    // original position : KeyCode.VcNumPad3
            [(ushort) KeyCode.VcLeft] =       (ushort) KeyCode.VcNumPadLeft,        // original position : KeyCode.VcNumPad4
            [(ushort) KeyCode.VcRight] =      (ushort) KeyCode.VcNumPadRight,       // original position : KeyCode.VcNumPad6
            [(ushort) KeyCode.VcHome] =       (ushort) KeyCode.VcNumPadHome,        // original position : KeyCode.VcNumPad7
            [(ushort) KeyCode.VcUp] =         (ushort) KeyCode.VcNumPadUp,          // original position : KeyCode.VcNumPad8
            [(ushort) KeyCode.VcPageUp] =     (ushort) KeyCode.VcNumPadPageUp,      // original position : KeyCode.VcNumPad9
        };
        

        public static ushort ConvertNativeKeyCode(ushort keyCode)
        {
            return NativeKeyCodeMapper.TryGetValue(keyCode, out var newKeyCode) ? newKeyCode 
                : keyCode;
        }

    }
    
}
