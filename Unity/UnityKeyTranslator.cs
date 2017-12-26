#define ALLOW_SPECIAL_KEYS

namespace Unity
{
    using System.Windows.Forms;

    using UE = UnityEngine;

    public static class UnityKeyTranslator
    {
        public static Keys ToKeyData(UE.EventModifiers umods, UE.KeyCode ukey)
        {
            Keys key;

            switch (ukey)
            {
                // oem + shift
#if ALLOW_SPECIAL_KEYS
                case UE.KeyCode.Ampersand: key = Keys.Shift | Keys.D7; break;
                case UE.KeyCode.At: key = Keys.Shift | Keys.D2; break;
                case UE.KeyCode.Asterisk: key = Keys.Shift | Keys.D8; break;
                case UE.KeyCode.Caret: key = Keys.Shift | Keys.D6; break;
                case UE.KeyCode.Colon: key = Keys.Shift | Keys.OemSemicolon; break;
                case UE.KeyCode.Dollar: key = Keys.Shift | Keys.D4; break;
                case UE.KeyCode.DoubleQuote: key = Keys.Shift | Keys.OemQuotes; break;
                case UE.KeyCode.Exclaim: key = Keys.Shift | Keys.D1; break;
                case UE.KeyCode.Greater: key = Keys.Shift | Keys.OemPeriod; break;
                case UE.KeyCode.Hash: key = Keys.Shift | Keys.D3; break;
                case UE.KeyCode.LeftParen: key = Keys.Shift | Keys.D9; break;
                case UE.KeyCode.Less: key = Keys.Shift | Keys.Oemcomma; break;
                case UE.KeyCode.Plus: key = Keys.Shift | Keys.Oemplus; break;
                case UE.KeyCode.Question: key = Keys.Shift | Keys.OemQuestion; break;
                case UE.KeyCode.RightParen: key = Keys.Shift | Keys.D0; break;
                case UE.KeyCode.Underscore: key = Keys.Shift | Keys.OemMinus; break;
#endif

                // oem
                case UE.KeyCode.BackQuote: key = Keys.Oemtilde; break;
                case UE.KeyCode.Backslash: key = Keys.OemPipe; break;
                case UE.KeyCode.Comma: key = Keys.Oemcomma; break;
                case UE.KeyCode.Equals: key = Keys.Oemplus; break;
                case UE.KeyCode.LeftBracket: key = Keys.OemOpenBrackets; break;
                case UE.KeyCode.Minus: key = Keys.OemMinus; break;
                case UE.KeyCode.Period: key = Keys.OemPeriod; break;
                case UE.KeyCode.Quote: key = Keys.OemQuotes; break;
                case UE.KeyCode.RightBracket: key = Keys.OemCloseBrackets; break;
                case UE.KeyCode.Semicolon: key = Keys.OemSemicolon; break;
                case UE.KeyCode.Slash: key = Keys.OemQuestion; break;

                // other
                case UE.KeyCode.CapsLock: key = Keys.CapsLock; break;
                case UE.KeyCode.DownArrow: key = Keys.Down; break;
                case UE.KeyCode.Delete: key = Keys.Delete; break;
                case UE.KeyCode.End: key = Keys.End; break;
                case UE.KeyCode.Help: key = Keys.Help; break;
                case UE.KeyCode.Home: key = Keys.Home; break;
                case UE.KeyCode.Insert: key = Keys.Insert; break;
                case UE.KeyCode.LeftArrow: key = Keys.Left; break;
                case UE.KeyCode.Numlock: key = Keys.NumLock; break;
                case UE.KeyCode.PageDown: key = Keys.PageDown; break;
                case UE.KeyCode.PageUp: key = Keys.PageUp; break;
                case UE.KeyCode.Print: key = Keys.Print; break;
                case UE.KeyCode.RightArrow: key = Keys.Right; break;
                case UE.KeyCode.ScrollLock: key = Keys.Scroll; break;
                case UE.KeyCode.Space: key = Keys.Space; break;
                case UE.KeyCode.UpArrow: key = Keys.Up; break;

                // keypad (numpad)
                case UE.KeyCode.Keypad0: key = Keys.NumPad0; break;
                case UE.KeyCode.Keypad1: key = Keys.NumPad1; break;
                case UE.KeyCode.Keypad2: key = Keys.NumPad2; break;
                case UE.KeyCode.Keypad3: key = Keys.NumPad3; break;
                case UE.KeyCode.Keypad4: key = Keys.NumPad4; break;
                case UE.KeyCode.Keypad5: key = Keys.NumPad5; break;
                case UE.KeyCode.Keypad6: key = Keys.NumPad6; break;
                case UE.KeyCode.Keypad7: key = Keys.NumPad7; break;
                case UE.KeyCode.Keypad8: key = Keys.NumPad8; break;
                case UE.KeyCode.Keypad9: key = Keys.NumPad9; break;
                case UE.KeyCode.KeypadPeriod: key = Keys.Decimal; break;
                case UE.KeyCode.KeypadDivide: key = Keys.Divide; break;
                case UE.KeyCode.KeypadMultiply: key = Keys.Multiply; break;
                case UE.KeyCode.KeypadMinus: key = Keys.Subtract; break;
                case UE.KeyCode.KeypadPlus: key = Keys.Add; break;
                case UE.KeyCode.KeypadEnter: key = Keys.Return; break;
                // KeyCode.KeypadEquals?

                // mods
                case UE.KeyCode.LeftAlt:
                case UE.KeyCode.RightAlt:
                    return Keys.Menu | Keys.Alt;
                case UE.KeyCode.LeftControl:
                case UE.KeyCode.RightControl:
                    return Keys.ControlKey | Keys.Control;
                case UE.KeyCode.LeftShift:
                case UE.KeyCode.RightShift:
                    return Keys.ShiftKey | Keys.Shift; // GetAsyncKeyState?

                // functional
                case UE.KeyCode.F1:
                case UE.KeyCode.F2:
                case UE.KeyCode.F3:
                case UE.KeyCode.F4:
                case UE.KeyCode.F5:
                case UE.KeyCode.F6:
                case UE.KeyCode.F7:
                case UE.KeyCode.F8:
                case UE.KeyCode.F9:
                case UE.KeyCode.F10:
                case UE.KeyCode.F11:
                case UE.KeyCode.F12:
                case UE.KeyCode.F13:
                case UE.KeyCode.F14:
                case UE.KeyCode.F15:
                    key = (Keys)(ukey - 170);
                    break;

                // leters
                case UE.KeyCode.A:
                case UE.KeyCode.B:
                case UE.KeyCode.C:
                case UE.KeyCode.D:
                case UE.KeyCode.E:
                case UE.KeyCode.F:
                case UE.KeyCode.G:
                case UE.KeyCode.H:
                case UE.KeyCode.I:
                case UE.KeyCode.J:
                case UE.KeyCode.K:
                case UE.KeyCode.L:
                case UE.KeyCode.M:
                case UE.KeyCode.N:
                case UE.KeyCode.O:
                case UE.KeyCode.P:
                case UE.KeyCode.Q:
                case UE.KeyCode.R:
                case UE.KeyCode.S:
                case UE.KeyCode.T:
                case UE.KeyCode.U:
                case UE.KeyCode.V:
                case UE.KeyCode.W:
                case UE.KeyCode.X:
                case UE.KeyCode.Y:
                case UE.KeyCode.Z:
                    key = (Keys)(ukey - 32);
                    break;

                default: // numbers, mouse, joystick, other. Modifiers can be fucked up.
                    key = (Keys)ukey;
                    break;
            }

            // Add mods.
            if ((umods & UE.EventModifiers.Alt) != 0) key |= Keys.Alt;
            if ((umods & UE.EventModifiers.Control) != 0) key |= Keys.Control;
            if ((umods & UE.EventModifiers.Shift) != 0) key |= Keys.Shift;
            //if ((umods & UE.EventModifiers.Command) != 0) key |= (Keys)UE.EventModifiers.Command;
            //if ((umods & UE.EventModifiers.Numeric) != 0) key |= (Keys)UE.EventModifiers.Numeric;
            //if ((umods & UE.EventModifiers.CapsLock) != 0) key |= (Keys)UE.EventModifiers.CapsLock;
            //if ((umods & UE.EventModifiers.FunctionKey) != 0) key |= (Keys)UE.EventModifiers.FunctionKey;

            return key;
        }
    }
}
