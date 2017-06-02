using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public class TextBox : Control
    {
        private readonly Pen _borderPen;
        private string _text;

        public Color BorderColor { get; set; }
        public Color BorderHoverColor { get; set; }
        public bool Multiline { get; set; }
        public bool ReadOnly { get; set; }
        public override string Text
        {
            get { return _text; }
            set
            {
                var changed = _text != value;
                _text = value;
                if (_text == null)
                    _text = "";

                if (changed)
                    OnTextChanged(EventArgs.Empty);
            }
        }
        public HorizontalAlignment TextAlign { get; set; }

        public TextBox()
        {
            _borderPen = new Pen(Color.White);
            _text = "";

            BackColor = Color.FromArgb(250, 250, 250);
            BorderColor = Color.LightGray;
            BorderHoverColor = Color.FromArgb(126, 180, 234);
            TextAlign = HorizontalAlignment.Left;
            ForeColor = Color.Black;
            Padding = new Padding(2, 0, 2, 0);
            Size = new Size(128, 24);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            _borderPen.Color = Hovered ? BorderHoverColor : BorderColor;

            var g = e.Graphics;
            var textX = Padding.Left;
            var textY = Padding.Top;
            var textW = Width - Padding.Horizontal;
            var textH = Height - Padding.Vertical;

            g.uwfFillRectangle(BackColor, 0, 0, Width, Height);

            if (Enabled && Focused)
            {
                var _tempText = "";
                if (!Multiline)
                    _tempText = g.uwfDrawTextField(Text, Font, ForeColor, textX, textY, textW, textH, TextAlign);
                else
                    _tempText = g.uwfDrawTextArea(Text, Font, ForeColor, textX, textY, textW, textH);

                if (ReadOnly == false && string.Equals(Text, _tempText) == false)
                    Text = _tempText;
            }
            else
            {
                if (Multiline)
                    g.uwfDrawString(Text, Font, ForeColor, textX, textY, textW, textH, ContentAlignment.TopLeft);
                else
                    g.uwfDrawString(Text, Font, ForeColor, textX, textY, textW, textH, TextAlign);
            }

            g.DrawRectangle(_borderPen, 0, 0, Width, Height);
        }
    }
}
