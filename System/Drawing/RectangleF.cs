using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace System.Drawing
{
    public struct RectangleF : IEquatable<RectangleF>
    {
        public static readonly RectangleF Empty = new RectangleF();

        public static implicit operator RectangleF(Rectangle r)
        {
            return new RectangleF(r.X, r.Y, r.Width, r.Height);
        }

        public static bool operator ==(RectangleF left, RectangleF right)
        {
            return left.X == right.X
                   && left.Y == right.Y
                   && left.Width == right.Width
                   && left.Height == right.Height;
        }
        public static bool operator !=(RectangleF left, RectangleF right)
        {
            return !(left == right);
        }

        private float x;
        private float y;
        private float width;
        private float height;

        public float Bottom
        {
            get { return Y + Height; }
        }
        public float Height
        {
            get { return height; }
            set { height = value; }
        }
        public bool IsEmpty
        {
            get { return Width <= 0 || Height <= 0; }
        }
        public float Left
        {
            get { return X; }
        }
        public PointF Location
        {
            get { return new PointF(X, Y); }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }
        public float Right
        {
            get { return X + Width; }
        }
        public SizeF Size
        {
            get { return new SizeF(Width, Height); }
            set
            {
                this.Width = value.Width;
                this.Height = value.Height;
            }
        }
        public float Top
        {
            get { return Y; }
        }
        public float Width
        {
            get { return width; }
            set { width = value; }
        }
        public float X
        {
            get { return x; }
            set { x = value; }
        }
        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        public RectangleF(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
        public RectangleF(PointF location, SizeF size)
        {
            this.x = location.X;
            this.y = location.Y;
            this.width = size.Width;
            this.height = size.Height;
        }

        public bool Equals(RectangleF other)
        {
            return x.Equals(other.x) && y.Equals(other.y) && width.Equals(other.width) && height.Equals(other.height);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is RectangleF && Equals((RectangleF) obj);
        }
        public bool Contains(float x, float y)
        {
            return this.X <= x &&
            x < this.X + this.Width &&
            this.Y <= y &&
            y < this.Y + this.Height;
        }
        public bool Contains(PointF pt)
        {
            return Contains(pt.X, pt.Y);
        }
        public bool Contains(RectangleF rect)
        {
            return this.X <= rect.X &&
                   rect.X + rect.Width <= this.X + this.Width &&
                   this.Y <= rect.Y &&
                   rect.Y + rect.Height <= this.Y + this.Height;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = x.GetHashCode();
                hashCode = (hashCode * 397) ^ y.GetHashCode();
                hashCode = (hashCode * 397) ^ width.GetHashCode();
                hashCode = (hashCode * 397) ^ height.GetHashCode();
                return hashCode;
            }
        }
        public void Inflate(float x, float y)
        {
            this.X -= x;
            this.Y -= y;
            this.Width += 2 * x;
            this.Height += 2 * y;
        }
        public void Inflate(SizeF size)
        {
            Inflate(size.Width, size.Height);
        }        
        public void Intersect(RectangleF rect)
        {
            RectangleF result = RectangleF.Intersect(rect, this);

            this.X = result.X;
            this.Y = result.Y;
            this.Width = result.Width;
            this.Height = result.Height;
        }
        public bool IntersectsWith(RectangleF rect)
        {
            return rect.X < this.X + this.Width &&
                   this.X < rect.X + rect.Width &&
                   rect.Y < this.Y + this.Height &&
                   this.Y < rect.Y + rect.Height;
        }
        public void Offset(PointF pos)
        {
            Offset(pos.X, pos.Y);
        }
        public void Offset(float x, float y)
        {
            this.X += x;
            this.Y += y;
        }
        public override string ToString()
        {
            return "{X=" + X.ToString(CultureInfo.CurrentCulture) + ",Y=" + Y.ToString(CultureInfo.CurrentCulture) +
            ",Width=" + Width.ToString(CultureInfo.CurrentCulture) +
            ",Height=" + Height.ToString(CultureInfo.CurrentCulture) + "}";
        }

        public static RectangleF FromLTRB(float left, float top, float right, float bottom)
        {
            return new RectangleF(left, top, right - left, bottom - top);
        }
        public static RectangleF Inflate(RectangleF rect, float x, float y)
        {
            RectangleF r = rect;
            r.Inflate(x, y);
            return r;
        }
        public static RectangleF Intersect(RectangleF a, RectangleF b)
        {
            float x1 = Math.Max(a.X, b.X);
            float x2 = Math.Min(a.X + a.Width, b.X + b.Width);
            float y1 = Math.Max(a.Y, b.Y);
            float y2 = Math.Min(a.Y + a.Height, b.Y + b.Height);

            if (x2 >= x1
                && y2 >= y1)
            {

                return new RectangleF(x1, y1, x2 - x1, y2 - y1);
            }
            return RectangleF.Empty;
        }
        public static RectangleF Union(RectangleF a, RectangleF b)
        {
            float x1 = Math.Min(a.X, b.X);
            float x2 = Math.Max(a.X + a.Width, b.X + b.Width);
            float y1 = Math.Min(a.Y, b.Y);
            float y2 = Math.Max(a.Y + a.Height, b.Y + b.Height);

            return new RectangleF(x1, y1, x2 - x1, y2 - y1);
        }
    }
}
