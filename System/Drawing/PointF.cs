namespace System.Drawing
{
    public struct PointF : IEquatable<PointF>
    {
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

        public PointF(float x, float y) : this()
        {
            this.X = 0;
            this.Y = 0;

            X = x;
            Y = y;
        }

        public bool IsEmpty
        {
            get { return X == 0 && Y == 0; }
        }
        public float X { get; set; }
        public float Y { get; set; }
        
        public bool Equals(PointF other)
        {
            return X == other.X && Y == other.Y;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is PointF)) return false;
            PointF comp = (PointF)obj;
            return comp.X == X && comp.Y == Y && comp.GetType() == GetType();
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
            X += point.X;
            Y += point.Y;
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
