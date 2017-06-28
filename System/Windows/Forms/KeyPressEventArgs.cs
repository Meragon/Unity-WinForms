using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Forms
{
    public class KeyPressEventArgs : EventArgs
    {
        public char KeyChar { get; set; }
        public bool Handled { get; set; }

        public KeyEventArgs uwfKeyArgs;

        public KeyPressEventArgs(char keyChar)
        {
            KeyChar = keyChar;
        }
    }
}
