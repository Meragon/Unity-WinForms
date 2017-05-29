using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace System.Drawing
{
    [Serializable]
    public struct Size : IEquatable<Size>
    {
        private int width;
        private int height;

        public static readonly Size Empty = new Size();

        public static implicit operator SizeF(Size p)
        {
            return new SizeF(p.Width, p.Height);
        }
        public static explicit operator Point(Size size)
        {
            return new Point(size.Width, size.Height);
        }

        public static Size operator +(Size sz1, Size sz2)
        {
            return Add(sz1, sz2);
        }
        public static Size operator -(Size sz1, Size sz2)
        {
            return Subtract(sz1, sz2);
        }
        public static bool operator ==(Size sz1, Size sz2)
        {
            return sz1.Width == sz2.Width && sz1.Height == sz2.Height;
        }
        public static bool operator !=(Size sz1, Size sz2)
        {
            return !(sz1 == sz2);
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }
        public bool IsEmpty
        {
            get { return width == 0 && height == 0; }
        }
        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public Size(Point pt)
        {
            width = pt.X;
            height = pt.Y;
        }
        public Size(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public bool Equals(Size other)
        {
            return width == other.width && height == other.height;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Size && Equals((Size) obj);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return (width * 397) ^ height;
            }
        }
        public override string ToString()
        {
            return "{Width=" + width.ToString(CultureInfo.CurrentCulture) + ", Height=" + height.ToString(CultureInfo.CurrentCulture) + "}";
        }

        public static Size Add(Size sz1, Size sz2)
        {
            return new Size(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
        }
        public static Size Ceiling(SizeF value)
        {
            return new Size((int)Math.Ceiling(value.Width), (int)Math.Ceiling(value.Height));
        }
        public static Size Subtract(Size sz1, Size sz2)
        {
            return new Size(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
        }
        public static Size Truncate(SizeF value)
        {
            return new Size((int)value.Width, (int)value.Height);
        }
        public static Size Round(SizeF value)
        {
            return new Size((int)Math.Round(value.Width), (int)Math.Round(value.Height));
        }
    }
}
