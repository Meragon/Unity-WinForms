namespace System.Drawing
{
    using System.Globalization;
    
    [Serializable]
    public struct Size : IEquatable<Size>
    {
        public static readonly Size Empty = new Size();

        public Size(Point pt) : this()
        {
            Width = pt.X;
            Height = pt.Y;
        }
        public Size(int width, int height) : this()
        {
            Width = width;
            Height = height;
        }
        
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

        public int Height { get; set; }
        public bool IsEmpty
        {
            get { return Width == 0 && Height == 0; }
        }
        public int Width { get; set; }

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
        
        public bool Equals(Size other)
        {
            return Width == other.Width && Height == other.Height;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Size && Equals((Size) obj);
        }
        public override int GetHashCode()
        {
            return Width ^ Height;
        }
        public override string ToString()
        {
            return "{Width=" + Width.ToString(CultureInfo.CurrentCulture) + ", Height=" + Height.ToString(CultureInfo.CurrentCulture) + "}";
        }
    }
}
