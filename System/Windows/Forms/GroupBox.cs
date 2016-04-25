using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public class GroupBox : Control
    {
        public Color BorderColor { get; set; }

        public GroupBox()
        {
            BackColor = Color.FromArgb(240, 240, 240);
            BorderColor = Color.LightGray;
            ForeColor = Color.Gray;
            TabIndex = -1;
        }
        protected override void OnLatePaint(PaintEventArgs e)
        {
            base.OnLatePaint(e);
            e.Graphics.DrawRectangle(new Pen(BorderColor), 0, 0, Width, Height);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            g.FillRectangle(new SolidBrush(BackColor), 0, 0, Width, Height);
            g.DrawString(Text, Font, new SolidBrush(ForeColor), 8, 0, Width - 16, Height - 0);
        }
    }
}
