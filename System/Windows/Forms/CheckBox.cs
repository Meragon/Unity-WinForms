using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public class CheckBox : Button
    {
        private bool _hovered;

        public bool Checked { get; set; }

        public CheckBox()
        {
            ForeColor = Color.Black;
            Size = new Drawing.Size(128, 17);
            TextAlign = ContentAlignment.MiddleLeft;

            Click += CheckBox_Click;
        }

        void CheckBox_Click(object sender, EventArgs e)
        {
            Checked = !Checked;
            CheckedChanged(this, new EventArgs());
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseHover(e);
            _hovered = true;
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hovered = false;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.FillRectangle(new SolidBrush(_hovered || Focused ? Color.FromArgb(243, 249, 255) : Color.White), Padding.Left, Padding.Top + Height / 2 - 6, 12, 12);
            g.DrawRectangle(new Pen(_hovered || Focused ? Color.FromArgb(51, 153, 255) : Color.FromArgb(172, 172, 172)), Padding.Left, Padding.Top + Height / 2 - 6, 12, 12);
            if (Checked)
                g.DrawTexture(ApplicationBehaviour.Resources.Images.Checked, Padding.Left, Padding.Top + Height / 2 - 6, 12, 12);

            g.DrawString(Text, Font, new SolidBrush(ForeColor), Padding.Left + 18, Padding.Top + 0, Width - 20, Height, TextAlign);
        }

        public event EventHandler CheckedChanged = delegate { };
    }
}
