using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Drawing
{
    public struct PointF
    {
        private float _x;
        private float _y;

        public float X { get { return _x; } set { _x = value; } }
        public float Y { get { return _y; } set { _y = value; } }

        public static implicit operator Point(PointF p)
        {
            return new Point((int)p.X, (int)p.Y);
        }

        public PointF(float x, float y)
        {
            _x = 0;
            _y = 0;

            X = x;
            Y = y;
        }
    }
}
