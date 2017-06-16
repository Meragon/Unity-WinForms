using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace System.Drawing
{
    public struct SizeF : IEquatable<SizeF>
    {
        private float width;
        private float height;

        public static readonly SizeF Empty = new SizeF();

        public static explicit operator PointF(SizeF size)
        {
            return new PointF(size.Width, size.Height);
        }

        public static SizeF operator +(SizeF sz1, SizeF sz2)
        {
            return Add(sz1, sz2);
        }
        public static SizeF operator -(SizeF sz1, SizeF sz2)
        {
            return Subtract(sz1, sz2);
        }
        public static bool operator ==(SizeF sz1, SizeF sz2)
        {
            return sz1.Width == sz2.Width && sz1.Height == sz2.Height;
        }
        public static bool operator !=(SizeF sz1, SizeF sz2)
        {
            return !(sz1 == sz2);
        }

        public float Height
        {
            get { return height; }
            set { height = value; }
        }
        public bool IsEmpty
        {
            get { return width == 0 && height == 0; }
        }
        public float Width
        {
            get { return width; }
            set { width = value; }
        }

        public SizeF(SizeF size)
        {
            width = size.width;
            height = size.height;
        }
        public SizeF(PointF pt)
        {
            width = pt.X;
            height = pt.Y;
        }
        public SizeF(float width, float height)
        {
            this.width = width;
            this.height = height;
        }

        public bool Equals(SizeF other)
        {
            return width.Equals(other.width) && height.Equals(other.height);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is SizeF && Equals((SizeF) obj);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return (width.GetHashCode() * 397) ^ height.GetHashCode();
            }
        }
        public PointF ToPointF()
        {
            return (PointF)this;
        }
        public Size ToSize()
        {
            return Size.Truncate(this);
        }
        public override string ToString()
        {
            return "{Width=" + width.ToString(CultureInfo.CurrentCulture) + ", Height=" + height.ToString(CultureInfo.CurrentCulture) + "}";
        }

        public static SizeF Add(SizeF sz1, SizeF sz2)
        {
            return new SizeF(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
        }   
        public static SizeF Subtract(SizeF sz1, SizeF sz2)
        {
            return new SizeF(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
        }
    }
}
