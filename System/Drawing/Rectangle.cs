namespace System.Drawing
{
    using System.Globalization;

    [Serializable]
    public struct Rectangle : IEquatable<Rectangle>
    {
        public static readonly Rectangle Empty = new Rectangle();

        public Rectangle(int x, int y, int width, int height) : this()
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
        public Rectangle(Point location, Size size) : this()
        {
            X = location.X;
            Y = location.Y;
            Width = size.Width;
            Height = size.Height;
        }

        public int Bottom
        {
            get { return Y + Height; }
        }
        public int Height { get; set; }
        public bool IsEmpty
        {
            get { return X == 0 && Y == 0 && Width == 0 && Height == 0; }
        }
        public int Left
        {
            get { return X; }
        }
        public Point Location
        {
            get { return new Point(X, Y); }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }
        public int Right
        {
            get { return X + Width; }
        }
        public Size Size
        {
            get { return new Size(Width, Height); }
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }
        public int Top
        {
            get { return Y; }
        }
        public int Width { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public static bool operator !=(Rectangle left, Rectangle right)
        {
            return !(left == right);
        }
        public static bool operator ==(Rectangle left, Rectangle right)
        {
            if (left.X == right.X && left.Y == right.Y && left.Width == right.Width && left.Height == right.Height)
                return true;
            return false;
        }

        public static Rectangle FromLTRB(int left, int top, int right, int bottom)
        {
            return new Rectangle(left,
                top,
                right - left,
                bottom - top);
        }
        public static Rectangle Inflate(Rectangle rect, int x, int y)
        {
            var r = rect;
            r.Inflate(x, y);
            return r;
        }
        public static Rectangle Intersect(Rectangle a, Rectangle b)
        {
            int x1 = Math.Max(a.X, b.X);
            int x2 = Math.Min(a.X + a.Width, b.X + b.Width);
            int y1 = Math.Max(a.Y, b.Y);
            int y2 = Math.Min(a.Y + a.Height, b.Y + b.Height);

            if (x2 >= x1 && y2 >= y1)
                return new Rectangle(x1, y1, x2 - x1, y2 - y1);

            return Empty;
        }
        public static Rectangle Union(Rectangle a, Rectangle b)
        {
            int x1 = Math.Min(a.X, b.X);
            int x2 = Math.Max(a.X + a.Width, b.X + b.Width);
            int y1 = Math.Min(a.Y, b.Y);
            int y2 = Math.Max(a.Y + a.Height, b.Y + b.Height);

            return new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }

        public bool Contains(int x, int y)
        {
            return X <= x && x < X + Width && Y <= y && y < Y + Height;
        }
        public bool Contains(Point pt)
        {
            return Contains(pt.X, pt.Y);
        }
        public bool Contains(Rectangle rect)
        {
            return X <= rect.X && rect.X + rect.Width <= X + Width && Y <= rect.Y && rect.Y + rect.Height <= Y + Height;
        }
        public bool Equals(Rectangle other)
        {
            return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Rectangle)) return false;
            var rect = (Rectangle) obj;
            return rect.X == X && rect.Y == Y && rect.Width == Width && rect.Height == Height;
        }
        public override int GetHashCode()
        {
            return unchecked((int) ((uint) X ^
                                    (((uint) Y << 13) | ((uint) Y >> 19)) ^
                                    (((uint) Width << 26) | ((uint) Width >> 6)) ^
                                    (((uint) Height << 7) | ((uint) Height >> 25))));
        }
        public void Inflate(int width, int height)
        {
            X -= width;
            Y -= height;
            Width += 2 * width;
            Height += 2 * height;
        }
        public void Inflate(Size size)
        {
            Inflate(size.Width, size.Height);
        }
        public void Intersect(Rectangle rect)
        {
            var rectangle = Intersect(rect, this);
            X = rectangle.X;
            Y = rectangle.Y;
            Width = rectangle.Width;
            Height = rectangle.Height;
        }
        public bool IntersectsWith(Rectangle rect)
        {
            return rect.X < X + Width && X < rect.X + rect.Width && rect.Y < Y + Height && Y < rect.Y + rect.Height;
        }
        public void Offset(Point pos)
        {
            Offset(pos.X, pos.Y);
        }
        public void Offset(int x, int y)
        {
            X += x;
            Y += y;
        }
        public override string ToString()
        {
            return "{X=" + X.ToString(CultureInfo.CurrentCulture) + ",Y=" + Y.ToString(CultureInfo.CurrentCulture) +
                   ",Width=" + Width.ToString(CultureInfo.CurrentCulture) +
                   ",Height=" + Height.ToString(CultureInfo.CurrentCulture) + "}";
        }
    }
}