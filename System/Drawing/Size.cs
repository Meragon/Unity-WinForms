using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Drawing
{
    [Serializable]
    public struct Size
    {
        private int _width;
        private int _height;

        public static Size Empty { get { return new Size(0, 0); } }

        public static Size operator +(Size sz1, Size sz2)
        {
            return new Size(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
        }
        public static Size operator +(Size sz1, int add)
        {
            return new Size(sz1.Width + add, sz1.Height + add);
        }
        public static Size operator -(Size sz1, Size sz2)
        {
            return new Size(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
        }
        public static bool operator ==(Size sz1, Size sz2)
        {
            if (sz1.Width == sz2.Width && sz1.Height == sz2.Height)
                return true;
            return false;
        }
        public static bool operator !=(Size sz1, Size sz2)
        {
            if (sz1.Width == sz2.Width && sz1.Height == sz2.Height)
                return false;
            return true;
        }

        public int Height { get { return _height; } set { _height = value; } }
        public int Width { get { return _width; } set { _width = value; } }

        public Size(int width, int height)
        {
            _width = 0;
            _height = 0;

            Width = width;
            Height = height;
        }

        public override bool Equals(object obj)
        {
            if (obj is Size == false) return false;
            return (((Size)obj).Width == Width && ((Size)obj).Height == Height);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return "{ W: " + _width.ToString() + "; H: " + _height.ToString() + " }";
        }
    }
}
