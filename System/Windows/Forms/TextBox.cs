using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public class TextBox : Control
    {
        private bool _shouldFocus;
        private bool _hovered;
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
                _text = value;
                if (_text == null)
                    _text = "";
            }
        }
        public HorizontalAlignment TextAlign { get; set; }
        public CallbackDelegate TextChangedCallback { get; set; }
        public object TextChangedCallbackInfo { get; set; }

        public TextBox()
        {
            _text = "";
            BackColor = Color.FromArgb(250, 250, 250);
            BorderColor = Color.LightGray;
            BorderHoverColor = Color.FromArgb(126, 180, 234);
            TextAlign = HorizontalAlignment.Left;
            ForeColor = Color.Black;
            Padding = new Forms.Padding(1, 0, 2, 0);
            Size = new Size(128, 24);
        }

        public override void Focus()
        {
            base.Focus();
            _shouldFocus = true;
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _hovered = true;
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hovered = false;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            g.FillRectangle(new SolidBrush(BackColor), 0, 0, Width, Height);

            if (Enabled && ReadOnly == false)
            {
                if (Focused)
                {
                    if (_shouldFocus)
                        UnityEngine.GUI.SetNextControlName(Name);

                    var _tempText = Text;
                    if (!Multiline)
                        Text = g.DrawTextField(Text, Font, new SolidBrush(ForeColor), Padding.Left, Padding.Top, Width - Padding.Right - Padding.Left, Height - Padding.Bottom - Padding.Top, TextAlign);
                    else
                        Text = g.DrawTextArea(Text, Font, new SolidBrush(ForeColor), 1, 0, Width - 2, Height);
                    if (_text != _tempText)
                    {
                        OnTextChanged(new EventArgs());
                        if (TextChangedCallback != null && TextChangedCallbackInfo != null)
                            TextChangedCallback.Invoke(TextChangedCallbackInfo);
                    }
                }
                else
                    g.DrawString(Text, Font, new SolidBrush(ForeColor), 4, 0, Width - 8, Height);

                if (_shouldFocus)
                {
                    UnityEngine.GUI.FocusControl(Name);
                    _shouldFocus = false;
                }
            }
            else
                g.DrawString(Text, Font, new SolidBrush(ForeColor), new RectangleF(4, 0, Width - 8, Height));

            g.DrawRectangle(new Pen(_hovered ? BorderHoverColor : BorderColor), 0, 0, Width, Height);
        }

        public delegate void CallbackDelegate(object data);
    }
}
