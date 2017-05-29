
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace System.Drawing
{
    public struct PointF : IEquatable<PointF>
    {
        private float x, y;

        public static readonly Point Empty = new Point();

        public static PointF operator +(PointF pt, Size sz)
        {
            return Add(pt, sz);
        }
        public static PointF operator -(PointF pt, Size sz)
        {
            return Subtract(pt, sz);
        }
        public static PointF operator +(PointF pt, SizeF sz)
        {
            return Add(pt, sz);
        }
        public static PointF operator -(PointF pt, SizeF sz)
        {
            return Subtract(pt, sz);
        }
        public static bool operator ==(PointF left, PointF right)
        {
            return left.X == right.X && left.Y == right.Y;
        }
        public static bool operator !=(PointF left, PointF right)
        {
            return !(left == right);
        }

        public bool IsEmpty
        {
            get { return x == 0 && y == 0; }
        }
        public float X { get { return x; } set { x = value; } }
        public float Y { get { return y; } set { y = value; } }

        public PointF(float x, float y)
        {
            this.x = 0;
            this.y = 0;

            X = x;
            Y = y;
        }

        public bool Equals(PointF other)
        {
            return x == other.x && y == other.y;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is PointF)) return false;
            PointF comp = (PointF)obj;
            return comp.X == this.X && comp.Y == this.Y && comp.GetType() == this.GetType();
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public void Offset(int dx, int dy)
        {
            X += dx;
            Y += dy;
        }
        public void Offset(Point point)
        {
            x += point.X;
            y += point.Y;
        }

        public static PointF Add(PointF pt, Size sz)
        {
            return new PointF(pt.X + sz.Width, pt.Y + sz.Height);
        }
        public static PointF Subtract(PointF pt, Size sz)
        {
            return new PointF(pt.X - sz.Width, pt.Y - sz.Height);
        }
        public static PointF Add(PointF pt, SizeF sz)
        {
            return new PointF(pt.X + sz.Width, pt.Y + sz.Height);
        }
        public static PointF Subtract(PointF pt, SizeF sz)
        {
            return new PointF(pt.X - sz.Width, pt.Y - sz.Height);
        }
    }
}
