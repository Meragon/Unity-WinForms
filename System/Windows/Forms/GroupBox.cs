using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public class GroupBox : Control
    {
        private Pen borderPen = new Pen(Color.Transparent);

        public Color BorderColor
        {
            get { return borderPen.Color; }
            set { borderPen.Color = value; }
        }

        public GroupBox()
        {
            BackColor = Color.FromArgb(240, 240, 240);
            BorderColor = Color.LightGray;
            ForeColor = Color.Gray;
            Size = new Size(168, 286);
            TabIndex = -1;
        }
        protected override void OnLatePaint(PaintEventArgs e)
        {
            base.OnLatePaint(e);
            e.Graphics.DrawRectangle(borderPen, 0, 0, Width, Height);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            g.uwfFillRectangle(BackColor, 0, 0, Width, Height);
            g.uwfDrawString(Text, Font, ForeColor, 8, 0, Width - 16, Height - 0);
        }
    }
}
