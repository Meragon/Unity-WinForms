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
            CanSelect = true;
            TextAlign = HorizontalAlignment.Left;
            ForeColor = Color.Black;
            Padding = new Forms.Padding(2, 0, 2, 0);
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

            if (Enabled)
            {
                if (Focused)
                {
                    if (_shouldFocus)
                        UnityEngine.GUI.SetNextControlName(Name);

                    var _tempText = "";
                    if (!Multiline)
                        _tempText = g.DrawTextField(Text, Font, new SolidBrush(ForeColor), Padding.Left, Padding.Top, Width - Padding.Right - Padding.Left, Height - Padding.Bottom - Padding.Top, TextAlign);
                    else
                        _tempText = g.DrawTextArea(Text, Font, new SolidBrush(ForeColor), Padding.Left, Padding.Top, Width - Padding.Right - Padding.Left, Height - Padding.Bottom - Padding.Top);
                    if (ReadOnly == false && Text != _tempText)
                    {
                        Text = _tempText;

                        OnTextChanged(new EventArgs());
                        if (TextChangedCallback != null && TextChangedCallbackInfo != null)
                            TextChangedCallback.Invoke(TextChangedCallbackInfo);
                    }
                }
                else
                {
                    if (Multiline)
                        g.DrawString(Text, Font, new SolidBrush(ForeColor), Padding.Left + 2, Padding.Top, Width - Padding.Right - Padding.Left, Height - Padding.Bottom - Padding.Top, ContentAlignment.TopLeft);
                    else
                        g.DrawString(Text, Font, new SolidBrush(ForeColor), Padding.Left + 2, Padding.Top, Width - Padding.Right - Padding.Left, Height - Padding.Bottom - Padding.Top, TextAlign);
                }

                if (_shouldFocus)
                {
                    UnityEngine.GUI.FocusControl(Name);
                    _shouldFocus = false;
                }
            }
            else
            {
                if (Multiline)
                    g.DrawString(Text, Font, new SolidBrush(ForeColor), Padding.Left, Padding.Top, Width - Padding.Left - Padding.Right, Height - Padding.Bottom - Padding.Top, ContentAlignment.TopLeft);
                else
                    g.DrawString(Text, Font, new SolidBrush(ForeColor), Padding.Left, Padding.Top, Width - Padding.Left - Padding.Right, Height - Padding.Bottom - Padding.Top, TextAlign);
            }

            g.DrawRectangle(new Pen(_hovered ? BorderHoverColor : BorderColor), 0, 0, Width, Height);
        }

        public delegate void CallbackDelegate(object data);
    }
}
