using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public class MouseEventArgs : EventArgs
    {
        private MouseButtons _button;
        private Point _location;
        private int _x;
        private int _y;
        private int _delta;

        public MouseButtons Button { get { return _button; } }
        public Point Location { get { return _location; } }
        public int X { get { return _x; } }
        public int Y { get { return _y; } }
        public int Delta { get { return _delta; } }

        public MouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta)
        {
            _button = button;
            _x = x;
            _y = y;
            _location = new Point(_x, _y);
            _delta = delta;
        }

        
    }

    public enum MouseButtons
    {
        None = 0,
        Left = 1048576,
        Right = 2097152,
        Middle = 4194304,
        XButton1 = 8388608,
        XButton2 = 16777216,
    }
}
