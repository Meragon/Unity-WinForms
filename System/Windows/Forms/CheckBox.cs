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

            g.FillRectangle(new SolidBrush(_hovered ? Color.FromArgb(243, 249, 255) : Color.White), 0, Height / 2 - 6, 12, 12);
            g.DrawRectangle(new Pen(_hovered ? Color.FromArgb(51, 153, 255) : Color.FromArgb(172, 172, 172)), 0, Height / 2 - 6, 12, 12);
            if (Checked)
                g.DrawTexture(Application.Resources.Reserved.Checked, 0, Height / 2 - 6, 12, 12);

            g.DrawString(Text, Font, new SolidBrush(ForeColor), 18, 0, Width - 20, Height, TextAlign);
        }

        public event EventHandler CheckedChanged = delegate { };
    }
}
