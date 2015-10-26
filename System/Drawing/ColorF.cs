using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Drawing
{
    public struct ColorF 
    {
        private float _a, _r, _g, _b;

        public float A { get { return _a; } }
        public float R { get { return _r; } }
        public float G { get { return _g; } }
        public float B { get { return _b; } }

        public static implicit operator Color(ColorF self)
        {
            return Color.FromArgb((int)self.A, (int)self.R, (int)self.G, (int)self.B);
        }

        public static ColorF FromArgb(float r, float g, float b)
        {
            return new ColorF() { _a = 255, _r = r, _g = g, _b = b };
        }
        public static ColorF FromArgb(float a, float r, float g, float b)
        {
            return new ColorF() { _a = a, _r = r, _g = g, _b = b };
        }
    }
}
