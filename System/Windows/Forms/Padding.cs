using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Forms
{
    public struct Padding
    {
        private int _left;
        private int _bottom;
        private int _right;
        private int _top;

        public int Bottom { get { return _bottom; } set { _bottom = value; } }
        public int Left { get { return _left; } set { _left = value; } }
        public int Horizontal { get { return Left + Right; } }
        public int Right { get { return _right; } set { _right = value; } }
        public int Top { get { return _top; } set { _top = value; } }
        public int Vertical { get { return Top + Bottom; } }

        public static readonly Padding Empty = new Padding(0);

        public Padding(int all)
        {
            _left = _bottom = _right = _top = 0;

            this.Left = all;
            this.Bottom = all;
            this.Right = all;
            this.Top = all;
        }
        public Padding(int left, int bottom, int right, int top)
        {
            _left = _bottom = _right = _top = 0;

            this.Left = left;
            this.Bottom = bottom;
            this.Right = right;
            this.Top = top;
        }

        public override string ToString()
        {
            return "{ L: " + _left.ToString() + "; B: " + _bottom.ToString() + "; R: " + _right.ToString() + "; T: " + _top + " }";
        }
    }
}
