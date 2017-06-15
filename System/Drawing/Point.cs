
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace System.Drawing
{
    [Serializable]
    public struct Point : IEquatable<Point>
    {
        private int x, y;

        public static readonly Point Empty = new Point();

        public static implicit operator PointF(Point p)
        {
            return new PointF(p.X, p.Y);
        }
        public static explicit operator Size(Point p)
        {
            return new Size(p.X, p.Y);
        }

        public static Point operator +(Point pt, Size sz)
        {
            return Add(pt, sz);
        }    
        public static Point operator -(Point pt, Size sz)
        {
            return Subtract(pt, sz);
        }
        public static bool operator ==(Point left, Point right)
        {
            return left.X == right.X && left.Y == right.Y;
        }
        public static bool operator !=(Point left, Point right)
        {
            return !(left == right);
        }

        public bool IsEmpty
        {
            get { return x == 0 && y == 0; }
        }
        public int X { get { return x; } set { x = value; } }
        public int Y { get { return y; } set { y = value; } }

        public Point(int x, int y)
        {
            this.x = 0;
            this.y = 0;

            X = x;
            Y = y;
        }
        public Point(Size sz)
        {
            this.x = sz.Width;
            this.y = sz.Height;
        }
        public Point(int dw)
        {
            unchecked
            {
                this.x = (short)LOWORD(dw);
                this.y = (short)HIWORD(dw);
            }
        }

        public bool Equals(Point other)
        {
            return x == other.x && y == other.y;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Point && Equals((Point)obj);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return (x * 397) ^ y;
            }
        }
        public void Offset(int dx, int dy)
        {
            x += dx;
            y += dy;
        }
        public void Offset(Point point)
        {
            x += point.X;
            y += point.Y;
        }
        public override string ToString()
        {
            return "{X=" + X.ToString(CultureInfo.CurrentCulture) + ",Y= " + Y.ToString(CultureInfo.CurrentCulture) + " }";
        }

        public static Point Add(Point pt, Size sz)
        {
            return new Point(pt.X + sz.Width, pt.Y + sz.Height);
        }
        public static Point Ceiling(PointF value)
        {
            return new Point((int)Math.Ceiling(value.X), (int)Math.Ceiling(value.Y));
        }
        public static Point Round(PointF value)
        {
            return new Point((int)Math.Round(value.X), (int)Math.Round(value.Y));
        }
        public static Point Subtract(Point pt, Size sz)
        {
            return new Point(pt.X - sz.Width, pt.Y - sz.Height);
        }
        public static Point Truncate(PointF value)
        {
            return new Point((int)value.X, (int)value.Y);
        }

        private static int HIWORD(int n)
        {
            return (n >> 16) & 0xffff;
        }
        private static int LOWORD(int n)
        {
            return n & 0xffff;
        }
    }
}
