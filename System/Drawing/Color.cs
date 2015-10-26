namespace System.Drawing
{
    public struct Color
    {
        private byte _a, _r, _g, _b;

        public byte A { get { return _a; } }
        public byte R { get { return _r; } }
        public byte G { get { return _g; } }
        public byte B { get { return _b; } }

        public static Color operator +(Color left, Color right)
        {
            int a = left.A + right.A; a = a > 255 ? 255 : a;
            int r = left.R + right.R; r = r > 255 ? 255 : r;
            int g = left.G + right.G; g = g > 255 ? 255 : g;
            int b = left.B + right.B; b = b > 255 ? 255 : b;
            return Color.FromArgb(a, r, g, b);
        }
        public static Color operator -(Color left, Color right)
        {
            int a = left.A - right.A; a = a < 0 ? 0 : a;
            int r = left.R - right.R; r = r < 0 ? 0 : r;
            int g = left.G - right.G; g = g < 0 ? 0 : g;
            int b = left.B - right.B; b = b < 0 ? 0 : b;
            return Color.FromArgb(a, r, g, b);
        }
        public static bool operator !=(Color left, Color right)
        {
            if (left._a != right._a || left._r != right._r || left._g != right._g || left._b != right._b)
                return true;
            return false;
        }
        public static bool operator ==(Color left, Color right)
        {
            if (left._a == right._a && left._r == right._r && left._g == right._g && left._b == right._b)
                return true;
            return false;
        }
        public static implicit operator ColorF(Color self)
        {
            return ColorF.FromArgb(self.A, self.R, self.G, self.B);
        }

        public static Color AliceBlue { get { return Color.FromArgb(0xF0, 0xF8, 0xFF); } }
        public static Color AntiqueWhie { get { return Color.FromArgb(0xFA, 0xEB, 0xD7); } }
        public static Color Aqua { get { return Color.FromArgb(0x00, 0xFF, 0xFF); } }
        public static Color Aquamarine { get { return Color.FromArgb(0x7F, 0xFF, 0xD4); } }
        public static Color Azure { get { return Color.FromArgb(0xF0, 0xFF, 0xFF); } }
        public static Color Beige { get { return Color.FromArgb(0xF5, 0xF5, 0xDC); } }
        public static Color Bisque { get { return Color.FromArgb(0xFF, 0xE4, 0xC4); } }
        public static Color Black { get { return Color.FromArgb(0x00, 0x00, 0x00); } }
        public static Color BlanchedAlmond { get { return Color.FromArgb(0xFF, 0xEB, 0xCD); } }
        public static Color Blue { get { return Color.FromArgb(0x00, 0x00, 0xFF); } }
        public static Color BlueViolet { get { return Color.FromArgb(0x8A, 0x2B, 0xE2); } }
        public static Color Brown { get { return Color.FromArgb(0xA5, 0x2A, 0x2A); } }
        public static Color BurlyWood { get { return Color.FromArgb(0xDE, 0xB8, 0x87); } }
        public static Color CadetBlue { get { return Color.FromArgb(0x5F, 0x9E, 0xA0); } }
        public static Color Chartreuse { get { return Color.FromArgb(0x7F, 0xFF, 0x00); } }
        public static Color Chocolate { get { return Color.FromArgb(0xD2, 0x69, 0x1E); } }
        public static Color Coral { get { return Color.FromArgb(0xFF, 0x7F, 0x50); } }
        public static Color CornflowerBlue { get { return Color.FromArgb(0x64, 0x95, 0xED); } }
        public static Color Cornsilk { get { return Color.FromArgb(0xFF, 0xF8, 0xDC); } }
        public static Color Crimson { get { return Color.FromArgb(0xDC, 0x14, 0x3C); } }
        public static Color Cyan { get { return Color.FromArgb(0x00, 0xFF, 0xFF); } }
        public static Color DarkBlue { get { return Color.FromArgb(0x00, 0x00, 0x8B); } }
        public static Color DarkCyan { get { return Color.FromArgb(0x00, 0x8B, 0x8B); } }
        public static Color DarkGoldenrod { get { return Color.FromArgb(0xB8, 0x86, 0x0B); } }
        public static Color DarkGray { get { return Color.FromArgb(0xA9, 0xA9, 0xA9); } }
        public static Color DarkGreen { get { return Color.FromArgb(0x00, 0x64, 0x00); } }
        public static Color DarkKhaki { get { return Color.FromArgb(0xBD, 0xB7, 0x6B); } }
        public static Color DarkMagenta { get { return Color.FromArgb(0x8B, 0x00, 0x8B); } }
        public static Color DarkOliveGreen { get { return Color.FromArgb(0x55, 0x6B, 0x2F); } }
        public static Color DarkOrange { get { return Color.FromArgb(0xFF, 0x8C, 0x00); } }
        public static Color DarkOrchid { get { return Color.FromArgb(0x99, 0x32, 0xCC); } }
        public static Color DarkRed { get { return Color.FromArgb(0x8B, 0x00, 0x00); } }
        public static Color DarkSalmon { get { return Color.FromArgb(0xE9, 0x96, 0x7A); } }
        public static Color DarkSeaGreen { get { return Color.FromArgb(0x8F, 0xBC, 0x8F); } }
        public static Color DarkSlateBlue { get { return Color.FromArgb(0x48, 0x3D, 0x8B); } }
        public static Color DarkSlateGray { get { return Color.FromArgb(0x2F, 0x4F, 0x4F); } }
        public static Color DarkTurquoise { get { return Color.FromArgb(0x00, 0xCE, 0xD1); } }
        public static Color DarkViolet { get { return Color.FromArgb(0x94, 0x00, 0xD3); } }
        public static Color DeepPink { get { return Color.FromArgb(0xFF, 0x14, 0x93); } }
        public static Color DeepSkyBlue { get { return Color.FromArgb(0x00, 0xBF, 0xFF); } }
        public static Color DimGray { get { return Color.FromArgb(0x69, 0x69, 0x69); } }
        public static Color DodgerBlue { get { return Color.FromArgb(0x1E, 0x90, 0xFF); } }
        public static Color Firebrick { get { return Color.FromArgb(0xB2, 0x22, 0x22); } }
        public static Color FloralWhite { get { return Color.FromArgb(0xFF, 0xFA, 0xF0); } }
        public static Color ForestGreen { get { return Color.FromArgb(0x22, 0x8B, 0x22); } }
        public static Color Fuchsia { get { return Color.FromArgb(0xFF, 0x00, 0xFF); } }
        public static Color Gainsboro { get { return Color.FromArgb(0xDC, 0xDC, 0xDC); } }
        public static Color GhostWhite { get { return Color.FromArgb(0xF8, 0xF8, 0xFF); } }
        public static Color Gold { get { return Color.FromArgb(0xFF, 0xD7, 0x00); } }
        public static Color Goldenrod { get { return Color.FromArgb(0xDA, 0xA5, 0x20); } }
        public static Color Gray { get { return Color.FromArgb(0x80, 0x80, 0x80); } }
        public static Color Green { get { return Color.FromArgb(0x00, 0x80, 0x00); } }
        public static Color GreenYellow { get { return Color.FromArgb(0xAD, 0xFF, 0x2F); } }
        public static Color Honeydew { get { return Color.FromArgb(0xF0, 0xFF, 0xF0); } }
        public static Color HotPink { get { return Color.FromArgb(0xFF, 0x69, 0xB4); } }
        public static Color IndianRed { get { return Color.FromArgb(0xCD, 0x5C, 0x5C); } }
        public static Color Indigo { get { return Color.FromArgb(0x4B, 0x00, 0x82); } }
        public static Color Ivory { get { return Color.FromArgb(0xFF, 0xFF, 0xF0); } }
        public static Color Khaki { get { return Color.FromArgb(0xF0, 0xE6, 0x8C); } }
        public static Color Lavender { get { return Color.FromArgb(0xE6, 0xE6, 0xFA); } }
        public static Color LavenderBlush { get { return Color.FromArgb(0xFF, 0xF0, 0xF5); } }
        public static Color LawnGreen { get { return Color.FromArgb(0x7C, 0xFC, 0x00); } }
        public static Color LemonChiffon { get { return Color.FromArgb(0xFF, 0xFA, 0xCD); } }
        public static Color LightBlue { get { return Color.FromArgb(0xAD, 0xD8, 0xE6); } }
        public static Color LightCoral { get { return Color.FromArgb(0xF0, 0x80, 0x80); } }
        public static Color LightCyan { get { return Color.FromArgb(0xE0, 0xFF, 0xFF); } }
        public static Color LightGoldenrodYellow { get { return Color.FromArgb(0xFA, 0xFA, 0xD2); } }
        public static Color LightGray { get { return Color.FromArgb(0xD3, 0xD3, 0xD3); } }
        public static Color LightGreen { get { return Color.FromArgb(0x90, 0xEE, 0x90); } }
        public static Color LightPink { get { return Color.FromArgb(0xFF, 0xB6, 0xC1); } }
        public static Color LightSalmon { get { return Color.FromArgb(0xFF, 0xA0, 0x7A); } }
        public static Color LightSeaGreen { get { return Color.FromArgb(0x20, 0xB2, 0xAA); } }
        public static Color LightSkyBlue { get { return Color.FromArgb(0x87, 0xCE, 0xFA); } }
        public static Color LightSlateGray { get { return Color.FromArgb(0x77, 0x88, 0x99); } }
        public static Color LightSteelBlue { get { return Color.FromArgb(0xB0, 0xC4, 0xDE); } }
        public static Color LightYellow { get { return Color.FromArgb(0xFF, 0xFF, 0xE0); } }
        public static Color Lime { get { return Color.FromArgb(0x00, 0xFF, 0x00); } }
        public static Color LimeGreen { get { return Color.FromArgb(0x32, 0xCD, 0x32); } }
        public static Color Linen { get { return Color.FromArgb(0xFA, 0xF0, 0xE6); } }
        public static Color Magenta { get { return Color.FromArgb(0xFF, 0x00, 0xFF); } }
        public static Color Maroon { get { return Color.FromArgb(0x80, 0x00, 0x00); } }
        public static Color MediumAquamarine { get { return Color.FromArgb(0x66, 0xCD, 0xAA); } }
        public static Color MediumBlue { get { return Color.FromArgb(0x00, 0x00, 0xCD); } }
        public static Color MediumOrchid { get { return Color.FromArgb(0xBA, 0x55, 0xD3); } }
        public static Color MediumPurple { get { return Color.FromArgb(0x93, 0x70, 0xDB); } }
        public static Color MediumSeaGreen { get { return Color.FromArgb(0x3C, 0xB3, 0x71); } }
        public static Color MediumSlateBlue { get { return Color.FromArgb(0x7B, 0x68, 0xEE); } }
        public static Color MediumSpringGreen { get { return Color.FromArgb(0x00, 0xFA, 0x9A); } }
        public static Color MediumTurquoise { get { return Color.FromArgb(0x48, 0xD1, 0xCC); } }
        public static Color MediumVioletRed { get { return Color.FromArgb(0xC7, 0x15, 0x85); } }
        public static Color MidnightBlue { get { return Color.FromArgb(0x19, 0x19, 0x70); } }
        public static Color MintCream { get { return Color.FromArgb(0xF5, 0xFF, 0xFA); } }
        public static Color MistyRose { get { return Color.FromArgb(0xFF, 0xE4, 0xE1); } }
        public static Color Moccasin { get { return Color.FromArgb(0xFF, 0xE4, 0xB5); } }
        public static Color NavajoWhite { get { return Color.FromArgb(0xFF, 0xDE, 0xAD); } }
        public static Color Navy { get { return Color.FromArgb(0x00, 0x00, 0x80); } }
        public static Color OldLace { get { return Color.FromArgb(0xFD, 0xF5, 0xE6); } }
        public static Color Olive { get { return Color.FromArgb(0x80, 0x80, 0x00); } }
        public static Color OliveDrab { get { return Color.FromArgb(0x6B, 0x8E, 0x23); } }
        public static Color Orange { get { return Color.FromArgb(0xFF, 0xA5, 0x00); } }
        public static Color OrangeRed { get { return Color.FromArgb(0xFF, 0x45, 0x00); } }
        public static Color Orchid { get { return Color.FromArgb(0xDA, 0x70, 0xD6); } }
        public static Color PaleGoldenrod { get { return Color.FromArgb(0xEE, 0xE8, 0xAA); } }
        public static Color PaleGreen { get { return Color.FromArgb(0x98, 0xFB, 0x98); } }
        public static Color PaleTurquoise { get { return Color.FromArgb(0xAF, 0xEE, 0xEE); } }
        public static Color PaleVioletRed { get { return Color.FromArgb(0xDB, 0x70, 0x93); } }
        public static Color PapayaWhip { get { return Color.FromArgb(0xFF, 0xEF, 0xD5); } }
        public static Color PeachPuff { get { return Color.FromArgb(0xFF, 0xDA, 0xB9); } }
        public static Color Peru { get { return Color.FromArgb(0xCD, 0x85, 0x3F); } }
        public static Color Pink { get { return Color.FromArgb(0xFF, 0xC0, 0xCB); } }
        public static Color Plum { get { return Color.FromArgb(0xDD, 0xA0, 0xDD); } }
        public static Color PowderBlue { get { return Color.FromArgb(0xB0, 0xE0, 0xE6); } }
        public static Color Purple { get { return Color.FromArgb(0x80, 0x00, 0x80); } }
        public static Color Red { get { return Color.FromArgb(0xFF, 0x00, 0x00); } }
        public static Color RosyBrown { get { return Color.FromArgb(0xBC, 0x8F, 0x8F); } }
        public static Color RoyalBlue { get { return Color.FromArgb(0x41, 0x69, 0xE1); } }
        public static Color SaddleBrown { get { return Color.FromArgb(0x8B, 0x45, 0x13); } }
        public static Color Salmon { get { return Color.FromArgb(0xFA, 0x80, 0x72); } }
        public static Color SandyBrown { get { return Color.FromArgb(0xF4, 0xA4, 0x60); } }
        public static Color SeaGreen { get { return Color.FromArgb(0x2E, 0x8B, 0x57); } }
        public static Color SeaShell { get { return Color.FromArgb(0xFF, 0xF5, 0xEE); } }
        public static Color Sienna { get { return Color.FromArgb(0xA0, 0x52, 0x2D); } }
        public static Color Silver { get { return Color.FromArgb(0xC0, 0xC0, 0xC0); } }
        public static Color SkyBlue { get { return Color.FromArgb(0x87, 0xCE, 0xEB); } }
        public static Color SlateBlue { get { return Color.FromArgb(0x6A, 0x5A, 0xCD); } }
        public static Color SlateGray { get { return Color.FromArgb(0x70, 0x80, 0x90); } }
        public static Color Snow { get { return Color.FromArgb(0xFF, 0xFA, 0xFA); } }
        public static Color SpringGreen { get { return Color.FromArgb(0x00, 0xFF, 0x7F); } }
        public static Color SteelBlue { get { return Color.FromArgb(0x46, 0x82, 0xB4); } }
        public static Color Tan { get { return Color.FromArgb(0xD2, 0xB4, 0x8C); } }
        public static Color Teal { get { return Color.FromArgb(0x00, 0x80, 0x80); } }
        public static Color Thistle { get { return Color.FromArgb(0xD8, 0xBF, 0xD8); } }
        public static Color Tomato { get { return Color.FromArgb(0xFF, 0x63, 0x47); } }
        public static Color Transparent { get { return Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF); } }
        public static Color Turquoise { get { return Color.FromArgb(0x40, 0xE0, 0xD0); } }
        public static Color Violet { get { return Color.FromArgb(0xEE, 0x82, 0xEE); } }
        public static Color Wheat { get { return Color.FromArgb(0xF5, 0xDE, 0xB3); } }
        public static Color White { get { return Color.FromArgb(0xFF, 0xFF, 0xFF); } }
        public static Color WhiteSmoke { get { return Color.FromArgb(0xF5, 0xF5, 0xF5); } }
        public static Color Yellow { get { return Color.FromArgb(0xFF, 0xFF, 0x00); } }
        public static Color YellowGreen { get { return Color.FromArgb(0x9A, 0xCD, 0x32); } }

        public static Color FromArgb(int alpha, Color baseColor)
        {
            return Color.FromArgb(alpha, baseColor.R, baseColor.G, baseColor.B);
        }
        public static Color FromArgb(int r, int g, int b)
        {
            return new Color() { _a = 255, _r = (byte)r, _g = (byte)g, _b = (byte)b };
        }
        public static Color FromArgb(int a, int r, int g, int b)
        {
            if (a < 0) a = 0;
            return new Color() { _a = (byte)a, _r = (byte)r, _g = (byte)g, _b = (byte)b };
        }
        public static Color FromUColor(UnityEngine.Color color)
        {
            return Color.FromArgb((int)(color.a * 255), (int)(color.r * 255), (int)(color.g * 255), (int)(color.b * 255));
        }

        public override bool Equals(object obj)
        {
            if (this == (Color)obj)
                return true;
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return "{ R: " + R.ToString() + "; G: " + G.ToString() + "; B: " + B.ToString() + "; A: " + A.ToString() + " }";
        }
        public UnityEngine.Color ToUColor()
        {
            return new UnityEngine.Color((float)_r / 255, (float)_g / 255, (float)_b / 255, (float)_a / 255);
        }
    }
}