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
        protected readonly Pen borderPen = new Pen(Color.White);

        public Color BorderColor
        {
            get { return borderPen.Color; }
            set { borderPen.Color = value; }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.uwfFillRectangle(BackColor, 0, 0, Width, Height);
            e.Graphics.DrawRectangle(borderPen, 0, 0, Width, Height);
        }
    }
}
