using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public interface IResizableControl
    {
        ControlResizeTypes GetResizeAt(Point mclient);
    }

    public enum ControlResizeTypes
    {
        None,

        Right,
        Down,
        Left,
        Up,

        RightDown,
        LeftDown,
        LeftUp,
        RightUp
    }
}
