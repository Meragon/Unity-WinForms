using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Forms
{
    public enum ButtonState
    {
        Normal = 0,
        Inactive = 256,
        Pushed = 512,
        Checked = 1024,
        Flat = 16384,
        All = 18176,
    }
}
