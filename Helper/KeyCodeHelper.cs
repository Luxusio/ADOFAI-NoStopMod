using System.Collections.Generic;
using SharpHook.Native;

namespace NoStopMod.Helper
{
    public class KeyCodeHelper
    {
        
        private static readonly Dictionary<UnityEngine.KeyCode, ushort> UnityToNativeKeyCodeDictionary = new();

        static KeyCodeHelper()
        {
            InitUnityToNativeKeyCodeDictionary();
        }

        private static void InitUnityToNativeKeyCodeDictionary()
        {
            var d = UnityToNativeKeyCodeDictionary;
            d[UnityEngine.KeyCode.None] = (ushort) KeyCode.VcUndefined;
            d[UnityEngine.KeyCode.Escape] = (ushort) KeyCode.VcEscape;
            d[UnityEngine.KeyCode.Alpha1] = (ushort) KeyCode.Vc1;
            d[UnityEngine.KeyCode.Alpha2] = (ushort) KeyCode.Vc2;
            d[UnityEngine.KeyCode.Alpha3] = (ushort) KeyCode.Vc3;
            d[UnityEngine.KeyCode.Alpha4] = (ushort) KeyCode.Vc4;
            d[UnityEngine.KeyCode.Alpha5] = (ushort) KeyCode.Vc5;
            d[UnityEngine.KeyCode.Alpha6] = (ushort) KeyCode.Vc6;
            d[UnityEngine.KeyCode.Alpha7] = (ushort) KeyCode.Vc7;
            d[UnityEngine.KeyCode.Alpha8] = (ushort) KeyCode.Vc8;
            d[UnityEngine.KeyCode.Alpha9] = (ushort) KeyCode.Vc9;
            d[UnityEngine.KeyCode.Alpha0] = (ushort) KeyCode.Vc0;
            d[UnityEngine.KeyCode.Minus] = (ushort) KeyCode.VcMinus;
            d[UnityEngine.KeyCode.Equals] = (ushort) KeyCode.VcEquals;
            d[UnityEngine.KeyCode.Backspace] = (ushort) KeyCode.VcBackspace;
            d[UnityEngine.KeyCode.Tab] = (ushort) KeyCode.VcTab;
            d[UnityEngine.KeyCode.Q] = (ushort) KeyCode.VcQ;
            d[UnityEngine.KeyCode.W] = (ushort) KeyCode.VcW;
            d[UnityEngine.KeyCode.E] = (ushort) KeyCode.VcE;
            d[UnityEngine.KeyCode.R] = (ushort) KeyCode.VcR;
            d[UnityEngine.KeyCode.T] = (ushort) KeyCode.VcT;
            d[UnityEngine.KeyCode.Y] = (ushort) KeyCode.VcY;
            d[UnityEngine.KeyCode.U] = (ushort) KeyCode.VcU;
            d[UnityEngine.KeyCode.I] = (ushort) KeyCode.VcI;
            d[UnityEngine.KeyCode.O] = (ushort) KeyCode.VcO;
            d[UnityEngine.KeyCode.P] = (ushort) KeyCode.VcP;
            d[UnityEngine.KeyCode.LeftBracket] = (ushort) KeyCode.VcOpenBracket;
            d[UnityEngine.KeyCode.RightBracket] = (ushort) KeyCode.VcCloseBracket;
            d[UnityEngine.KeyCode.Return] = (ushort) KeyCode.VcEnter;
            d[UnityEngine.KeyCode.LeftControl] = (ushort) KeyCode.VcLeftControl;
            d[UnityEngine.KeyCode.A] = (ushort) KeyCode.VcA;
            d[UnityEngine.KeyCode.S] = (ushort) KeyCode.VcS;
            d[UnityEngine.KeyCode.D] = (ushort) KeyCode.VcD;
            d[UnityEngine.KeyCode.F] = (ushort) KeyCode.VcF;
            d[UnityEngine.KeyCode.G] = (ushort) KeyCode.VcG;
            d[UnityEngine.KeyCode.H] = (ushort) KeyCode.VcH;
            d[UnityEngine.KeyCode.J] = (ushort) KeyCode.VcJ;
            d[UnityEngine.KeyCode.K] = (ushort) KeyCode.VcK;
            d[UnityEngine.KeyCode.L] = (ushort) KeyCode.VcL;
            d[UnityEngine.KeyCode.Semicolon] = (ushort) KeyCode.VcSemicolon;
            d[UnityEngine.KeyCode.Quote] = (ushort) KeyCode.VcQuote;
            d[UnityEngine.KeyCode.BackQuote] = (ushort) KeyCode.VcBackquote;
            d[UnityEngine.KeyCode.LeftShift] = (ushort) KeyCode.VcLeftShift;
            d[UnityEngine.KeyCode.Backslash] = (ushort) KeyCode.VcBackSlash;
            d[UnityEngine.KeyCode.Z] = (ushort) KeyCode.VcZ;
            d[UnityEngine.KeyCode.X] = (ushort) KeyCode.VcX;
            d[UnityEngine.KeyCode.C] = (ushort) KeyCode.VcC;
            d[UnityEngine.KeyCode.V] = (ushort) KeyCode.VcV;
            d[UnityEngine.KeyCode.B] = (ushort) KeyCode.VcB;
            d[UnityEngine.KeyCode.N] = (ushort) KeyCode.VcN;
            d[UnityEngine.KeyCode.M] = (ushort) KeyCode.VcM;
            d[UnityEngine.KeyCode.Comma] = (ushort) KeyCode.VcComma;
            d[UnityEngine.KeyCode.Period] = (ushort) KeyCode.VcPeriod;
            d[UnityEngine.KeyCode.Slash] = (ushort) KeyCode.VcSlash;
            d[UnityEngine.KeyCode.RightShift] = (ushort) KeyCode.VcRightShift;
            d[UnityEngine.KeyCode.KeypadMultiply] = (ushort) KeyCode.VcNumPadMultiply;
            d[UnityEngine.KeyCode.LeftAlt] = (ushort) KeyCode.VcLeftAlt;
            d[UnityEngine.KeyCode.Space] = (ushort) KeyCode.VcSpace;
            d[UnityEngine.KeyCode.CapsLock] = (ushort) KeyCode.VcCapsLock;
            d[UnityEngine.KeyCode.F1] = (ushort) KeyCode.VcF1;
            d[UnityEngine.KeyCode.F2] = (ushort) KeyCode.VcF2;
            d[UnityEngine.KeyCode.F3] = (ushort) KeyCode.VcF3;
            d[UnityEngine.KeyCode.F4] = (ushort) KeyCode.VcF4;
            d[UnityEngine.KeyCode.F5] = (ushort) KeyCode.VcF5;
            d[UnityEngine.KeyCode.F6] = (ushort) KeyCode.VcF6;
            d[UnityEngine.KeyCode.F7] = (ushort) KeyCode.VcF7;
            d[UnityEngine.KeyCode.F8] = (ushort) KeyCode.VcF8;
            d[UnityEngine.KeyCode.F9] = (ushort) KeyCode.VcF9;
            d[UnityEngine.KeyCode.F10] = (ushort) KeyCode.VcF10;
            d[UnityEngine.KeyCode.Numlock] = (ushort) KeyCode.VcNumLock;
            d[UnityEngine.KeyCode.ScrollLock] = (ushort) KeyCode.VcScrollLock;
            d[UnityEngine.KeyCode.Keypad7] = (ushort) KeyCode.VcNumPad7;
            d[UnityEngine.KeyCode.Keypad8] = (ushort) KeyCode.VcNumPad8;
            d[UnityEngine.KeyCode.Keypad9] = (ushort) KeyCode.VcNumPad9;
            d[UnityEngine.KeyCode.KeypadMinus] = (ushort) KeyCode.VcNumPadSubtract;
            d[UnityEngine.KeyCode.Keypad4] = (ushort) KeyCode.VcNumPad4;
            d[UnityEngine.KeyCode.Keypad5] = (ushort) KeyCode.VcNumPad5;
            d[UnityEngine.KeyCode.Keypad6] = (ushort) KeyCode.VcNumPad6;
            d[UnityEngine.KeyCode.KeypadPlus] = (ushort) KeyCode.VcNumPadAdd;
            d[UnityEngine.KeyCode.Keypad1] = (ushort) KeyCode.VcNumPad1;
            d[UnityEngine.KeyCode.Keypad2] = (ushort) KeyCode.VcNumPad2;
            d[UnityEngine.KeyCode.Keypad3] = (ushort) KeyCode.VcNumPad3;
            d[UnityEngine.KeyCode.Keypad0] = (ushort) KeyCode.VcNumPad0;
            d[UnityEngine.KeyCode.KeypadPeriod] = (ushort) KeyCode.VcNumPadSeparator;
            d[UnityEngine.KeyCode.F11] = (ushort) KeyCode.VcF11;
            d[UnityEngine.KeyCode.F12] = (ushort) KeyCode.VcF12;
            d[UnityEngine.KeyCode.F13] = (ushort) KeyCode.VcF13;
            d[UnityEngine.KeyCode.F14] = (ushort) KeyCode.VcF14;
            d[UnityEngine.KeyCode.F15] = (ushort) KeyCode.VcF15;
            d[UnityEngine.KeyCode.Underscore] = (ushort) KeyCode.VcUnderscore;
            d[UnityEngine.KeyCode.KeypadEquals] = (ushort) KeyCode.VcNumPadEquals;
            d[UnityEngine.KeyCode.RightControl] = (ushort) KeyCode.VcRightControl;
            d[UnityEngine.KeyCode.KeypadDivide] = (ushort) KeyCode.VcNumPadDivide;
            d[UnityEngine.KeyCode.Print] = (ushort) KeyCode.VcPrintscreen;
            d[UnityEngine.KeyCode.RightAlt] = (ushort) KeyCode.VcRightAlt;
            d[UnityEngine.KeyCode.Pause] = (ushort) KeyCode.VcPause;
            d[UnityEngine.KeyCode.Home] = (ushort) KeyCode.VcHome;
            d[UnityEngine.KeyCode.PageUp] = (ushort) KeyCode.VcPageUp;
            d[UnityEngine.KeyCode.End] = (ushort) KeyCode.VcEnd;
            d[UnityEngine.KeyCode.PageDown] = (ushort) KeyCode.VcPageDown;
            d[UnityEngine.KeyCode.Insert] = (ushort) KeyCode.VcInsert;
            d[UnityEngine.KeyCode.Delete] = (ushort) KeyCode.VcDelete;
            d[UnityEngine.KeyCode.LeftMeta] = (ushort) KeyCode.VcLeftMeta;
            d[UnityEngine.KeyCode.RightMeta] = (ushort) KeyCode.VcRightMeta;
            d[UnityEngine.KeyCode.Menu] = (ushort) KeyCode.VcContextMenu;
            d[UnityEngine.KeyCode.UpArrow] = (ushort) KeyCode.VcUp;
            d[UnityEngine.KeyCode.LeftArrow] = (ushort) KeyCode.VcLeft;
            d[UnityEngine.KeyCode.Clear] = (ushort) KeyCode.VcEnter;
            d[UnityEngine.KeyCode.RightArrow] = (ushort) KeyCode.VcRight;
            d[UnityEngine.KeyCode.DownArrow] = (ushort) KeyCode.VcDown;
            d[UnityEngine.KeyCode.KeypadEnter] = (ushort) KeyCode.VcNumPadEnter;
            d[UnityEngine.KeyCode.LeftApple] = (ushort) KeyCode.VcLeftMeta;
            //d[UnityEngine.KeyCode.None] = (ushort) KeyCode.CharUndefined;
            
            //d[UnityEngine.KeyCode.None] = (ushort) (1000 + MouseButton.NoButton);
            d[UnityEngine.KeyCode.Mouse0] = (ushort) (1000 + MouseButton.Button1);
            d[UnityEngine.KeyCode.Mouse1] = (ushort) (1000 + MouseButton.Button2);
            d[UnityEngine.KeyCode.Mouse2] = (ushort) (1000 + MouseButton.Button3);
            d[UnityEngine.KeyCode.Mouse3] = (ushort) (1000 + MouseButton.Button4);
            d[UnityEngine.KeyCode.Mouse4] = (ushort) (1000 + MouseButton.Button5);

        }
        
        public static bool TryToNativeKeyCode(UnityEngine.KeyCode keyCode, out ushort nativeKeyCode)
        {
            return UnityToNativeKeyCodeDictionary.TryGetValue(keyCode, out nativeKeyCode);
        }

    }
    
}
