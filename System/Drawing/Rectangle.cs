namespace System.Drawing
{
    using System.Globalization;

    [Serializable]
    public struct Rectangle : IEquatable<Rectangle>
    {
        private int x;
        private int y;
        private int width;
        private int height;

        public static readonly Rectangle Empty = new Rectangle();

        public int Bottom { get { return y + height; } }
        public int Height { get { return height; } set { height = value; } }
        public bool IsEmpty
        {
            get { return x == 0 && y == 0 && width == 0 && height == 0; }
        }
        public int Left { get { return x; } }
        public Point Location
        {
            get { return new Point(x, y); }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }
        public int Right { get { return x + width; } }
        public Size Size
        {
            get { return new Size(Width, Height); }
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }
        public int Top { get { return y; } }
        public int Width { get { return width; } set { width = value; } }
        public int X { get { return x; } set { x = value; } }
        public int Y { get { return y; } set { y = value; } }
        
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

        public Rectangle(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
        public Rectangle(Point location, Size size)
        {
            x = location.X;
            y = location.Y;
            width = size.Width;
            height = size.Height;
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
            return x == other.x && y == other.y && width == other.width && height == other.height;
        }
        public override bool Equals(object obj)
        {
            if (obj is Rectangle == false) return false;
            var rect = (Rectangle)obj;
            return rect.x == x && rect.y == y && rect.width == width && rect.height == height;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = x;
                hashCode = (hashCode * 397) ^ y;
                hashCode = (hashCode * 397) ^ width;
                hashCode = (hashCode * 397) ^ height;
                return hashCode;
            }
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
            Rectangle rectangle = Rectangle.Intersect(rect, this);
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

        public static Rectangle FromLTRB(int left, int top, int right, int bottom)
        {
            return new Rectangle(left,
                                 top,
                                 right - left,
                                 bottom - top);
        }
        public static Rectangle Inflate(Rectangle rect, int x, int y)
        {
            Rectangle r = rect;
            r.Inflate(x, y);
            return r;
        }
        public static Rectangle Intersect(Rectangle a, Rectangle b)
        {
            int num = Math.Max(a.X, b.X);
            int num2 = Math.Min(a.X + a.Width, b.X + b.Width);
            int num3 = Math.Max(a.Y, b.Y);
            int num4 = Math.Min(a.Y + a.Height, b.Y + b.Height);
            if (num2 >= num && num4 >= num3)
            {
                return new Rectangle(num, num3, num2 - num, num4 - num3);
            }
            return Rectangle.Empty;
        }
        public static Rectangle Union(Rectangle a, Rectangle b)
        {
            int x1 = Math.Min(a.X, b.X);
            int x2 = Math.Max(a.X + a.Width, b.X + b.Width);
            int y1 = Math.Min(a.Y, b.Y);
            int y2 = Math.Max(a.Y + a.Height, b.Y + b.Height);

            return new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }
    }
}
