using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public class ToolTip : Component
    {
        private static ToolTip _instance;
        private static float _alphaF;
        private static float _alphaWait;
        private static float _waitToShow;

        private Control _control;
        private int _initialDelay = 1000;
        private Point _location;
        private string _text;

        public Color BackColor { get; set; }
        public Color BorderColor { get; set; }
        public Font Font { get; set; }
        public Color ForeColor { get; set; }
        public int InitialDelay
        {
            get { return _initialDelay; }
            set
            {
                _initialDelay = UnityEngine.Mathf.Clamp(value, 0, 32767);
            }
        }
        public Padding Padding { get; set; }
        public HorizontalAlignment TextAlign { get; set; }

        public ToolTip()
        {
            BackColor = Color.White;
            BorderColor = Color.FromArgb(118, 118, 118);
            Font = new Font("Arial", 12);
            ForeColor = Color.FromArgb(118, 118, 118);
            Padding = new Padding(4);
            TextAlign = HorizontalAlignment.Center;
        }

        public void SetToolTip(Control control, string caption)
        {
            _control = control;
            control.MouseEnter += control_MouseEnter;
            control.MouseLeave += control_MouseLeave;
            control.OnDisposing += control_OnDisposing;
            control.VisibleChanged += control_OnDisposing;
            _text = caption;
        }

        void control_MouseLeave(object sender, EventArgs e)
        {
            if (_instance == this)
                _instance = null;
        }
        void control_MouseEnter(object sender, EventArgs e)
        {
            if (_control != null && _control.Visible && !_control.Disposing)
            {
                var position = Control.MousePosition + new Point(0, 18);
                Show(_text, position);
            }
        }
        void control_OnDisposing(object sender, EventArgs e)
        {
            if (_instance != null && _instance == this)
                _instance = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="point">Useless right now (will use Control.MousePosition)</param>
        public void Show(string text, Point point)
        {
            _location = Control.MousePosition;
            _location = new Point(_location.X, _location.Y + 24); // Cursor offset.
            _text = text;
            _instance = this;
            _alphaF = 255;
            _alphaWait = 1024;
            _waitToShow = _initialDelay;
        }

        internal static void OnPaint(PaintEventArgs e)
        {
            if (_instance != null)
            {
                if (_waitToShow > 0)
                {
                    _waitToShow -= (1000 * UnityEngine.Time.deltaTime);
                    return;
                }
                
                var size = e.Graphics.MeasureString(_instance._text, _instance.Font) + new SizeF(16, 4);

                Point loc = _instance._location;

                if (loc.X + size.Width + 2 > Screen.PrimaryScreen.WorkingArea.Width)
                    loc = new Point(Screen.PrimaryScreen.WorkingArea.Width - (int)size.Width - 2, loc.Y);
                if (loc.Y + size.Height + 2 > Screen.PrimaryScreen.WorkingArea.Height)
                    loc = new Point(loc.X, Screen.PrimaryScreen.WorkingArea.Height - (int)size.Height - 2);

                int shadowAlpha = 12 - 255 + (int)_alphaF;
                var shadowColor = Color.FromArgb(shadowAlpha, 64, 64, 64);

                int stringHeight = (int)size.Height;

                e.Graphics.uwfFillRectangle(shadowColor, loc.X + 1, loc.Y + 1, size.Width + 3, stringHeight + 3);
                e.Graphics.uwfFillRectangle(shadowColor, loc.X + 2, loc.Y + 2, size.Width + 1, stringHeight + 1);
                e.Graphics.uwfFillRectangle(shadowColor, loc.X + 3, loc.Y + 3, size.Width - 1, stringHeight - 1);
                
                var borderColor = Color.FromArgb((int)_alphaF, _instance.BorderColor);
                var textColor = Color.FromArgb((int)_alphaF, _instance.ForeColor);
                var textFont = _instance.Font;

                e.Graphics.uwfFillRectangle(Color.FromArgb((int)_alphaF, _instance.BackColor), loc.X, loc.Y, size.Width, stringHeight);
                e.Graphics.DrawRectangle(new Pen(borderColor), loc.X, loc.Y, size.Width, stringHeight);
                e.Graphics.uwfDrawString(_instance._text, textFont, textColor, 
                    loc.X + _instance.Padding.Left, 
                    loc.Y + _instance.Padding.Top, 
                    size.Width - _instance.Padding.Bottom, 
                    stringHeight - _instance.Padding.Right, 
                    _instance.TextAlign);

                if (_alphaWait > 0)
                    _alphaWait -= 1;
                else
                {
                    if (_alphaF > 0)
                        _alphaF -= 1;
                    else
                        _instance = null;
                }
            }
        }
    }
}
