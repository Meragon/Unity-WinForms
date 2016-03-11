using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public class ToolTip : Component
    {
        private static ToolTip _instance;
        private static StringFormat _formatCenter = new StringFormat()
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };
        private static float _alphaF;
        private static float _alphaWait;
        private static float _waitToShow;

        private Control _control;
        private int _initialDelay = 1000;
        private Point _location;
        private string _text;

        public int InitialDelay
        {
            get { return _initialDelay; }
            set
            {
                _initialDelay = UnityEngine.Mathf.Clamp(value, 0, 32767);
            }
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

        public void Show(string text, Point point)
        {
            _location = point;
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
                //e.Graphics.Control = null;
                var size = e.Graphics.MeasureString(_instance._text, new Font("Arial", 12)) + new SizeF(16, 8);

                if (_instance._location.X + size.Width + 2 > Screen.PrimaryScreen.WorkingArea.Width)
                    _instance._location = new Point(Screen.PrimaryScreen.WorkingArea.Width - (int)size.Width - 2, _instance._location.Y);
                if (_instance._location.Y + size.Height + 2 > Screen.PrimaryScreen.WorkingArea.Height)
                    _instance._location = new Point(_instance._location.X, Screen.PrimaryScreen.WorkingArea.Height - (int)size.Height - 2);

                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(12 - 255 + (int)_alphaF, 64, 64, 64)), _instance._location.X + 6 + 6, _instance._location.Y + 6 + 6, size.Width - 12, size.Height - 12);
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(12 - 255 + (int)_alphaF, 64, 64, 64)), _instance._location.X + 6 + 5, _instance._location.Y + 6 + 5, size.Width - 10, size.Height - 10);
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(12 - 255 + (int)_alphaF, 64, 64, 64)), _instance._location.X + 6 + 4, _instance._location.Y + 6 + 4, size.Width - 8, size.Height - 8);
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(12 - 255 + (int)_alphaF, 64, 64, 64)), _instance._location.X + 6 + 3, _instance._location.Y + 6 + 3, size.Width - 6, size.Height - 6);
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(12 - 255 + (int)_alphaF, 64, 64, 64)), _instance._location.X + 6 + 2, _instance._location.Y + 6 + 2, size.Width - 4, size.Height - 4);

                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb((int)_alphaF, 255, 255, 255)), _instance._location.X, _instance._location.Y, size.Width, 20);
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb((int)_alphaF, 118, 118, 118)), _instance._location.X, _instance._location.Y, size.Width, 20);
                e.Graphics.DrawString(_instance._text, new Font("Arial", 12), new SolidBrush(Color.FromArgb((int)_alphaF, 118, 118, 118)), _instance._location.X, _instance._location.Y, size.Width, 20, _formatCenter);

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
