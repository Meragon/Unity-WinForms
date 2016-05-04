using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace System.Windows.Forms
{
    public class Panel : ScrollableControl
    {
        public Color BorderColor { get; set; }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(BackColor, 0, 0, Width, Height);
            e.Graphics.DrawRectangle(new Pen(BorderColor), 0, 0, Width, Height);
        }
    }
}
