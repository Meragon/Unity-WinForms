using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Drawing
{
    public struct SizeF
    {
        private float _width;
        private float _height;

        public static SizeF Empty { get { return new SizeF(0, 0); } }

        public static SizeF operator +(SizeF sz1, SizeF sz2)
        {
            return new SizeF(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
        }
        public static SizeF operator +(SizeF sz1, float add)
        {
            return new SizeF(sz1.Width + add, sz1.Height + add);
        }
        public static SizeF operator -(SizeF sz1, SizeF sz2)
        {
            return new SizeF(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
        }
        public static bool operator ==(SizeF sz1, SizeF sz2)
        {
            if (sz1.Width == sz2.Width && sz1.Height == sz2.Height)
                return true;
            return false;
        }
        public static bool operator !=(SizeF sz1, SizeF sz2)
        {
            if (sz1.Width == sz2.Width && sz1.Height == sz2.Height)
                return false;
            return true;
        }

        public float Height { get { return _height; } set { _height = value; } }
        public float Width { get { return _width; } set { _width = value; } }

        public SizeF(float width, float height)
        {
            _width = 0;
            _height = 0;

            Width = width;
            Height = height;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is SizeF && ((SizeF)obj)._height == _height && ((SizeF)obj)._width == _width)
                return true;
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
