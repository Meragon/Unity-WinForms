using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    [Serializable]
    public class Label : Control
    {
        public ContentAlignment TextAlign { get; set; }

        public Label()
        {
            BackColor = Color.Transparent;
            Padding = new Forms.Padding(4, 0, 8, 0);
            Size = new Drawing.Size(128, 20);
            TabStop = false;
            TextAlign = ContentAlignment.TopLeft;

            this.SetStyle(ControlStyles.FixedHeight | ControlStyles.Selectable, false);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;

            g.uwfFillRectangle(BackColor, 0, 0, Width, Height);
            g.uwfDrawString(Text, Font, ForeColor, Padding.Left, Padding.Top, Width - Padding.Right - Padding.Left, Height - Padding.Bottom - Padding.Top, TextAlign);
        }
    }
}
