namespace System.Drawing
{
    using System.Globalization;
    
    public struct Point : IEquatable<Point>
    {
        public static readonly Point Empty = new Point();

        public Point(int x, int y) : this()
        {
            X = x;
            Y = y;
        }
        public Point(Size sz) : this()
        {
            X = sz.Width;
            Y = sz.Height;
        }
        public Point(int dw) : this()
        {
            unchecked
            {
                X = (short)LOWORD(dw);
                Y = (short)HIWORD(dw);
            }
        }

        public bool IsEmpty
        {
            get { return X == 0 && Y == 0; }
        }
        public int X { get; set; }
        public int Y { get; set; }

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

        public bool Equals(Point other)
        {
            return X == other.X && Y == other.Y;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Point && Equals((Point)obj);
        }
        public override int GetHashCode()
        {
            return X ^ Y;
        }
        public void Offset(int dx, int dy)
        {
            X += dx;
            Y += dy;
        }
        public void Offset(Point point)
        {
            X += point.X;
            Y += point.Y;
        }
        public override string ToString()
        {
            return "{X=" + X.ToString(CultureInfo.CurrentCulture) + ",Y= " + Y.ToString(CultureInfo.CurrentCulture) + " }";
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
