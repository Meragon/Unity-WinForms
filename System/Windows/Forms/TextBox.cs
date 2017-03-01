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
            CanSelect = true;
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

            g.FillRectangle(BackColor, 0, 0, Width, Height);

            if (Enabled && Focused)
            {
                var _tempText = "";
                if (!Multiline)
                    _tempText = g.DrawTextField(Text, Font, ForeColor, textX, textY, textW, textH, TextAlign);
                else
                    _tempText = g.DrawTextArea(Text, Font, ForeColor, textX, textY, textW, textH);

                if (ReadOnly == false && string.Equals(Text, _tempText) == false)
                    Text = _tempText;
            }
            else
            {
                if (Multiline)
                    g.DrawString(Text, Font, ForeColor, textX, textY, textW, textH, ContentAlignment.TopLeft);
                else
                    g.DrawString(Text, Font, ForeColor, textX, textY, textW, textH, TextAlign);
            }

            g.DrawRectangle(_borderPen, 0, 0, Width, Height);
        }
        protected override object OnPaintEditor(float width)
        {
            var control = base.OnPaintEditor(width);

#if UNITY_EDITOR

            Editor.NewLine(1);
            Editor.ColorField("BorderColor", BorderColor, (c) => { BorderColor = c; });
            Editor.ColorField("BorderHoverColor", BorderHoverColor, (c) => { BorderHoverColor = c; });

            var editorMultiline = Editor.BooleanField("Multiline", Multiline);
            if (editorMultiline.Changed)
                Multiline = editorMultiline.Value;

            var editorReadonly = Editor.BooleanField("ReadOnly", ReadOnly);
            if (editorReadonly.Changed)
                ReadOnly = editorReadonly.Value;

            Editor.Label("TextAlign", TextAlign);
#endif

            return control;
        }
    }
}
