using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Drawing
{
    public struct RectangleF
    {
        private float _x;
        private float _y;
        private float _width;
        private float _height;

        public float X { get { return _x; } set { _x = value; } }
        public float Y { get { return _y; } set { _y = value; } }
        public float Height { get { return _height; } set { _height = value; } }
        public float Width { get { return _width; } set { _width = value; } }

        public static implicit operator Rectangle(RectangleF rect)
        {
            return new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
        }

        public RectangleF(float x, float y, float width, float height)
        {
            _x = 0;
            _y = 0;
            _width = 0;
            _height = 0;

            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public bool Contains(Point pt)
        {
            if (pt.X >= X && pt.X <= X + Width && pt.Y >= Y && pt.Y <= Y + Height)
                return true;
            return false;
        }
        public bool Contains(PointF pt)
        {
            if (pt.X >= X && pt.X <= X + Width && pt.Y >= Y && pt.Y <= Y + Height)
                return true;
            return false;
        }

        public override string ToString()
        {
            return "{ " + string.Format("{0}, {1}, {2}, {3}", X, Y, Width, Height) + " }";
        }
    }
}
