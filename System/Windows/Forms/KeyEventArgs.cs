using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Forms
{
    public class KeyEventArgs : EventArgs
    {
        public UnityEngine.KeyCode KeyCode { get; set; }
        public UnityEngine.EventModifiers Modifiers { get; set; }
    }
}
