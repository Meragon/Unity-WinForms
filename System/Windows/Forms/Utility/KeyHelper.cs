using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Forms
{
    public static class KeyHelper
    {
        public static UnityEngine.KeyCode ToKey(this UnityEngine.EventModifiers modifier, bool returnLeftButton = true)
        {
            switch (modifier)
            {
                case UnityEngine.EventModifiers.Alt:
                    return returnLeftButton ? UnityEngine.KeyCode.LeftAlt : UnityEngine.KeyCode.RightAlt;
                case UnityEngine.EventModifiers.CapsLock:
                    return UnityEngine.KeyCode.CapsLock;
                case UnityEngine.EventModifiers.Command:
                    return returnLeftButton ? UnityEngine.KeyCode.LeftCommand : UnityEngine.KeyCode.RightCommand;
                case UnityEngine.EventModifiers.Control:
                    return returnLeftButton ? UnityEngine.KeyCode.LeftControl : UnityEngine.KeyCode.RightControl;
                case UnityEngine.EventModifiers.Numeric:
                    return UnityEngine.KeyCode.Numlock;
                case UnityEngine.EventModifiers.Shift:
                    return returnLeftButton ? UnityEngine.KeyCode.LeftShift : UnityEngine.KeyCode.RightShift;
                    
                default:
                    return UnityEngine.KeyCode.None;
            }
        }
        public static UnityEngine.EventModifiers ToModifier(this UnityEngine.KeyCode key)
        {
            switch (key)
            {
                case UnityEngine.KeyCode.LeftAlt:
                case UnityEngine.KeyCode.RightAlt:
                    return UnityEngine.EventModifiers.Alt;

                case UnityEngine.KeyCode.CapsLock:
                    return UnityEngine.EventModifiers.CapsLock;

                case UnityEngine.KeyCode.LeftCommand:
                case UnityEngine.KeyCode.RightCommand:
                    return UnityEngine.EventModifiers.Command;

                case UnityEngine.KeyCode.LeftControl:
                case UnityEngine.KeyCode.RightControl:
                    return UnityEngine.EventModifiers.Control;

                case UnityEngine.KeyCode.Numlock:
                    return UnityEngine.EventModifiers.Numeric;

                case UnityEngine.KeyCode.LeftShift:
                case UnityEngine.KeyCode.RightShift:
                    return UnityEngine.EventModifiers.Shift;

                default:
                    return UnityEngine.EventModifiers.None;
            }
        }
    }
}
