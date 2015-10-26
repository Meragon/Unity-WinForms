using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Drawing
{
    public struct Rectangle
    {
        private int _x;
        private int _y;
        private int _width;
        private int _height;

        public static readonly Rectangle Empty = default(Rectangle);

        public int Left { get { return _x; } }
        public int Right { get { return _x + _width; } }
        public int Top { get { return _y; } }
        public int Bottom { get { return _y + _height; } }
        public int X { get { return _x; } set { _x = value; } }
        public int Y { get { return _y; } set { _y = value; } }
        public Point Location { get { return new Point(_x, _y); } }
        public int Height { get { return _height; } set { _height = value; } }
        public int Width { get { return _width; } set { _width = value; } }

        public static implicit operator RectangleF(Rectangle rect)
        {
            return new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
        }
        public static Rectangle operator +(Rectangle left, Rectangle right)
        {
            return new Rectangle(left.X + right.X, left.Y + right.Y, left.Width + right.Width, left.Height + right.Height);
        }
        public static Rectangle operator +(Rectangle left, int right)
        {
            return new Rectangle(left.X - right, left.Y - right, left.Width + right * 2, left.Height + right * 2);
        }
        public static Rectangle operator -(Rectangle left, int right)
        {
            return new Rectangle(left.X + right, left.Y + right, left.Width - right * 2, left.Height - right * 2);
        }
        public static bool operator !=(Rectangle left, Rectangle right)
        {
            if (left.X != right.X || left.Y != right.Y || left.Width != right.Width || left.Height != right.Height)
                return true;
            return false;
        }
        public static bool operator ==(Rectangle left, Rectangle right)
        {
            if (left.X == right.X && left.Y == right.Y && left.Width == right.Width && left.Height == right.Height)
                return true;
            return false;
        }

        public Rectangle(int x, int y, int width, int height)
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

        public bool Contains(int x, int y)
        {
            return this.X <= x && x < this.X + this.Width && this.Y <= y && y < this.Y + this.Height;
        }
        public bool Contains(Point pt)
        {
            return this.Contains(pt.X, pt.Y);
        }
        public bool Contains(Rectangle rect)
        {
            return this.X <= rect.X && rect.X + rect.Width <= this.X + this.Width && this.Y <= rect.Y && rect.Y + rect.Height <= this.Y + this.Height;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Rectangle))
            {
                return false;
            }
            Rectangle rectangle = (Rectangle)obj;
            return rectangle.X == this.X && rectangle.Y == this.Y && rectangle.Width == this.Width && rectangle.Height == this.Height;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public void Inflate(int width, int height)
        {
            this.X -= width;
            this.Y -= height;
            this.Width += 2 * width;
            this.Height += 2 * height;
        }
        public void Intersect(Rectangle rect)
        {
            Rectangle rectangle = Rectangle.Intersect(rect, this);
            this.X = rectangle.X;
            this.Y = rectangle.Y;
            this.Width = rectangle.Width;
            this.Height = rectangle.Height;
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
        public bool IntersectsWith(Rectangle rect)
        {
            return rect.X < this.X + this.Width && this.X < rect.X + rect.Width && rect.Y < this.Y + this.Height && this.Y < rect.Y + rect.Height;
        }
        public override string ToString()
        {
            return String.Concat(new string[] {
                "{ X: ", X.ToString(), 
                ", Y: ", Y.ToString(),
                ", W: ", Width.ToString(),
                ", H: ", Height.ToString(),
                " }"
            });
        }
    }
}
