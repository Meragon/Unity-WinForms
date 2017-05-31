using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    [Serializable]
    public class ScrollableControl : Control
    {
        public ScrollableControl()
        {
            this.SetStyle(ControlStyles.ContainerControl, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, false);
        }
    }
}
