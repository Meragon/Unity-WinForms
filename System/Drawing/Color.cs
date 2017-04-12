namespace System.Drawing
{
    [Serializable]
    public struct Color
    {
        private bool _isEmpty;
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

        // TODO: as null.
        public static readonly Color Empty = new Color() { _a = 0, _r = 0, _g = 0, _b = 0, _isEmpty = true };

        static Color()
        {
            AliceBlue = Color.FromArgb(0xF0, 0xF8, 0xFF);
            AntiqueWhie = Color.FromArgb(0xFA, 0xEB, 0xD7);
            Aqua = Color.FromArgb(0x00, 0xFF, 0xFF);
            Aquamarine = Color.FromArgb(0x7F, 0xFF, 0xD4);
            Azure = Color.FromArgb(0xF0, 0xFF, 0xFF);
            Beige = Color.FromArgb(0xF5, 0xF5, 0xDC);
            Bisque = Color.FromArgb(0xFF, 0xE4, 0xC4);
            Black = Color.FromArgb(0x00, 0x00, 0x00);
            BlanchedAlmond = Color.FromArgb(0xFF, 0xEB, 0xCD);
            Blue = Color.FromArgb(0x00, 0x00, 0xFF);
            BlueViolet = Color.FromArgb(0x8A, 0x2B, 0xE2);
            Brown = Color.FromArgb(0xA5, 0x2A, 0x2A);
            BurlyWood = Color.FromArgb(0xDE, 0xB8, 0x87);
            CadetBlue = Color.FromArgb(0x5F, 0x9E, 0xA0);
            Chartreuse = Color.FromArgb(0x7F, 0xFF, 0x00);
            Chocolate = Color.FromArgb(0xD2, 0x69, 0x1E);
            Coral = Color.FromArgb(0xFF, 0x7F, 0x50);
            CornflowerBlue = Color.FromArgb(0x64, 0x95, 0xED);
            Cornsilk = Color.FromArgb(0xFF, 0xF8, 0xDC);
            Crimson = Color.FromArgb(0xDC, 0x14, 0x3C);
            Cyan = Color.FromArgb(0x00, 0xFF, 0xFF);
            DarkBlue = Color.FromArgb(0x00, 0x00, 0x8B);
            DarkCyan = Color.FromArgb(0x00, 0x8B, 0x8B);
            DarkGoldenrod = Color.FromArgb(0xB8, 0x86, 0x0B);
            DarkGray = Color.FromArgb(0xA9, 0xA9, 0xA9);
            DarkGreen = Color.FromArgb(0x00, 0x64, 0x00);
            DarkKhaki = Color.FromArgb(0xBD, 0xB7, 0x6B);
            DarkMagenta = Color.FromArgb(0x8B, 0x00, 0x8B);
            DarkOliveGreen = Color.FromArgb(0x55, 0x6B, 0x2F);
            DarkOrange = Color.FromArgb(0xFF, 0x8C, 0x00);
            DarkOrchid = Color.FromArgb(0x99, 0x32, 0xCC);
            DarkRed = Color.FromArgb(0x8B, 0x00, 0x00);
            DarkSalmon = Color.FromArgb(0xE9, 0x96, 0x7A);
            DarkSeaGreen = Color.FromArgb(0x8F, 0xBC, 0x8F);
            DarkSlateBlue = Color.FromArgb(0x48, 0x3D, 0x8B);
            DarkSlateGray = Color.FromArgb(0x2F, 0x4F, 0x4F);
            DarkTurquoise = Color.FromArgb(0x00, 0xCE, 0xD1);
            DarkViolet = Color.FromArgb(0x94, 0x00, 0xD3);
            DeepPink = Color.FromArgb(0xFF, 0x14, 0x93);
            DeepSkyBlue = Color.FromArgb(0x00, 0xBF, 0xFF);
            DimGray = Color.FromArgb(0x69, 0x69, 0x69);
            DodgerBlue = Color.FromArgb(0x1E, 0x90, 0xFF);
            Firebrick = Color.FromArgb(0xB2, 0x22, 0x22);
            FloralWhite = Color.FromArgb(0xFF, 0xFA, 0xF0);
            ForestGreen = Color.FromArgb(0x22, 0x8B, 0x22);
            Fuchsia = Color.FromArgb(0xFF, 0x00, 0xFF);
            Gainsboro = Color.FromArgb(0xDC, 0xDC, 0xDC);
            GhostWhite = Color.FromArgb(0xF8, 0xF8, 0xFF);
            Gold = Color.FromArgb(0xFF, 0xD7, 0x00);
            Goldenrod = Color.FromArgb(0xDA, 0xA5, 0x20);
            Gray = Color.FromArgb(0x80, 0x80, 0x80);
            Green = Color.FromArgb(0x00, 0x80, 0x00);
            GreenYellow = Color.FromArgb(0xAD, 0xFF, 0x2F);
            Honeydew = Color.FromArgb(0xF0, 0xFF, 0xF0);
            HotPink = Color.FromArgb(0xFF, 0x69, 0xB4);
            IndianRed = Color.FromArgb(0xCD, 0x5C, 0x5C);
            Indigo = Color.FromArgb(0x4B, 0x00, 0x82);
            Ivory = Color.FromArgb(0xFF, 0xFF, 0xF0);
            Khaki = Color.FromArgb(0xF0, 0xE6, 0x8C);
            Lavender = Color.FromArgb(0xE6, 0xE6, 0xFA);
            LavenderBlush = Color.FromArgb(0xFF, 0xF0, 0xF5);
            LawnGreen = Color.FromArgb(0x7C, 0xFC, 0x00);
            LemonChiffon = Color.FromArgb(0xFF, 0xFA, 0xCD);
            LightBlue = Color.FromArgb(0xAD, 0xD8, 0xE6);
            LightCoral = Color.FromArgb(0xF0, 0x80, 0x80);
            LightCyan = Color.FromArgb(0xE0, 0xFF, 0xFF);
            LightGoldenrodYellow = Color.FromArgb(0xFA, 0xFA, 0xD2);
            LightGray = Color.FromArgb(0xD3, 0xD3, 0xD3);
            LightGreen = Color.FromArgb(0x90, 0xEE, 0x90);
            LightPink = Color.FromArgb(0xFF, 0xB6, 0xC1);
            LightSalmon = Color.FromArgb(0xFF, 0xA0, 0x7A);
            LightSeaGreen = Color.FromArgb(0x20, 0xB2, 0xAA);
            LightSkyBlue = Color.FromArgb(0x87, 0xCE, 0xFA);
            LightSlateGray = Color.FromArgb(0x77, 0x88, 0x99);
            LightSteelBlue = Color.FromArgb(0xB0, 0xC4, 0xDE);
            LightYellow = Color.FromArgb(0xFF, 0xFF, 0xE0);
            Lime = Color.FromArgb(0x00, 0xFF, 0x00);
            LimeGreen = Color.FromArgb(0x32, 0xCD, 0x32);
            Linen = Color.FromArgb(0xFA, 0xF0, 0xE6);
            Magenta = Color.FromArgb(0xFF, 0x00, 0xFF);
            Maroon = Color.FromArgb(0x80, 0x00, 0x00);
            MediumAquamarine = Color.FromArgb(0x66, 0xCD, 0xAA);
            MediumBlue = Color.FromArgb(0x00, 0x00, 0xCD);
            MediumOrchid = Color.FromArgb(0xBA, 0x55, 0xD3);
            MediumPurple = Color.FromArgb(0x93, 0x70, 0xDB);
            MediumSeaGreen = Color.FromArgb(0x3C, 0xB3, 0x71);
            MediumSlateBlue = Color.FromArgb(0x7B, 0x68, 0xEE);
            MediumSpringGreen = Color.FromArgb(0x00, 0xFA, 0x9A);
            MediumTurquoise = Color.FromArgb(0x48, 0xD1, 0xCC);
            MediumVioletRed = Color.FromArgb(0xC7, 0x15, 0x85);
            MidnightBlue = Color.FromArgb(0x19, 0x19, 0x70);
            MintCream = Color.FromArgb(0xF5, 0xFF, 0xFA);
            MistyRose = Color.FromArgb(0xFF, 0xE4, 0xE1);
            Moccasin = Color.FromArgb(0xFF, 0xE4, 0xB5);
            NavajoWhite = Color.FromArgb(0xFF, 0xDE, 0xAD);
            Navy = Color.FromArgb(0x00, 0x00, 0x80);
            OldLace = Color.FromArgb(0xFD, 0xF5, 0xE6);
            Olive = Color.FromArgb(0x80, 0x80, 0x00);
            OliveDrab = Color.FromArgb(0x6B, 0x8E, 0x23);
            Orange = Color.FromArgb(0xFF, 0xA5, 0x00);
            OrangeRed = Color.FromArgb(0xFF, 0x45, 0x00);
            Orchid = Color.FromArgb(0xDA, 0x70, 0xD6);
            PaleGoldenrod = Color.FromArgb(0xEE, 0xE8, 0xAA);
            PaleGreen = Color.FromArgb(0x98, 0xFB, 0x98);
            PaleTurquoise = Color.FromArgb(0xAF, 0xEE, 0xEE);
            PaleVioletRed = Color.FromArgb(0xDB, 0x70, 0x93);
            PapayaWhip = Color.FromArgb(0xFF, 0xEF, 0xD5);
            PeachPuff = Color.FromArgb(0xFF, 0xDA, 0xB9);
            Peru = Color.FromArgb(0xCD, 0x85, 0x3F);
            Pink = Color.FromArgb(0xFF, 0xC0, 0xCB);
            Plum = Color.FromArgb(0xDD, 0xA0, 0xDD);
            PowderBlue = Color.FromArgb(0xB0, 0xE0, 0xE6);
            Purple = Color.FromArgb(0x80, 0x00, 0x80);
            Red = Color.FromArgb(0xFF, 0x00, 0x00);
            RosyBrown = Color.FromArgb(0xBC, 0x8F, 0x8F);
            RoyalBlue = Color.FromArgb(0x41, 0x69, 0xE1);
            SaddleBrown = Color.FromArgb(0x8B, 0x45, 0x13);
            Salmon = Color.FromArgb(0xFA, 0x80, 0x72);
            SandyBrown = Color.FromArgb(0xF4, 0xA4, 0x60);
            SeaGreen = Color.FromArgb(0x2E, 0x8B, 0x57);
            SeaShell = Color.FromArgb(0xFF, 0xF5, 0xEE);
            Sienna = Color.FromArgb(0xA0, 0x52, 0x2D);
            Silver = Color.FromArgb(0xC0, 0xC0, 0xC0);
            SkyBlue = Color.FromArgb(0x87, 0xCE, 0xEB);
            SlateBlue = Color.FromArgb(0x6A, 0x5A, 0xCD);
            SlateGray = Color.FromArgb(0x70, 0x80, 0x90);
            Snow = Color.FromArgb(0xFF, 0xFA, 0xFA);
            SpringGreen = Color.FromArgb(0x00, 0xFF, 0x7F);
            SteelBlue = Color.FromArgb(0x46, 0x82, 0xB4);
            Tan = Color.FromArgb(0xD2, 0xB4, 0x8C);
            Teal = Color.FromArgb(0x00, 0x80, 0x80);
            Thistle = Color.FromArgb(0xD8, 0xBF, 0xD8);
            Tomato = Color.FromArgb(0xFF, 0x63, 0x47);
            Transparent = Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF);
            Turquoise = Color.FromArgb(0x40, 0xE0, 0xD0);
            Violet = Color.FromArgb(0xEE, 0x82, 0xEE);
            Wheat = Color.FromArgb(0xF5, 0xDE, 0xB3);
            White = Color.FromArgb(0xFF, 0xFF, 0xFF);
            WhiteSmoke = Color.FromArgb(0xF5, 0xF5, 0xF5);
            Yellow = Color.FromArgb(0xFF, 0xFF, 0x00);
            YellowGreen = Color.FromArgb(0x9A, 0xCD, 0x32);
        }

        public bool IsEmpty { get { return _isEmpty; } }

        public static Color AliceBlue { get; private set; }
        public static Color AntiqueWhie { get; private set; }
        public static Color Aqua { get; private set; }
        public static Color Aquamarine { get; private set; }
        public static Color Azure { get; private set; }
        public static Color Beige { get; private set; }
        public static Color Bisque { get; private set; }
        public static Color Black { get; private set; }
        public static Color BlanchedAlmond { get; private set; }
        public static Color Blue { get; private set; }
        public static Color BlueViolet { get; private set; }
        public static Color Brown { get; private set; }
        public static Color BurlyWood { get; private set; }
        public static Color CadetBlue { get; private set; }
        public static Color Chartreuse { get; private set; }
        public static Color Chocolate { get; private set; }
        public static Color Coral { get; private set; }
        public static Color CornflowerBlue { get; private set; }
        public static Color Cornsilk { get; private set; }
        public static Color Crimson { get; private set; }
        public static Color Cyan { get; private set; }
        public static Color DarkBlue { get; private set; }
        public static Color DarkCyan { get; private set; }
        public static Color DarkGoldenrod { get; private set; }
        public static Color DarkGray { get; private set; }
        public static Color DarkGreen { get; private set; }
        public static Color DarkKhaki { get; private set; }
        public static Color DarkMagenta { get; private set; }
        public static Color DarkOliveGreen { get; private set; }
        public static Color DarkOrange { get; private set; }
        public static Color DarkOrchid { get; private set; }
        public static Color DarkRed { get; private set; }
        public static Color DarkSalmon { get; private set; }
        public static Color DarkSeaGreen { get; private set; }
        public static Color DarkSlateBlue { get; private set; }
        public static Color DarkSlateGray { get; private set; }
        public static Color DarkTurquoise { get; private set; }
        public static Color DarkViolet { get; private set; }
        public static Color DeepPink { get; private set; }
        public static Color DeepSkyBlue { get; private set; }
        public static Color DimGray { get; private set; }
        public static Color DodgerBlue { get; private set; }
        public static Color Firebrick { get; private set; }
        public static Color FloralWhite { get; private set; }
        public static Color ForestGreen { get; private set; }
        public static Color Fuchsia { get; private set; }
        public static Color Gainsboro { get; private set; }
        public static Color GhostWhite { get; private set; }
        public static Color Gold { get; private set; }
        public static Color Goldenrod { get; private set; }
        public static Color Gray { get; private set; }
        public static Color Green { get; private set; }
        public static Color GreenYellow { get; private set; }
        public static Color Honeydew { get; private set; }
        public static Color HotPink { get; private set; }
        public static Color IndianRed { get; private set; }
        public static Color Indigo { get; private set; }
        public static Color Ivory { get; private set; }
        public static Color Khaki { get; private set; }
        public static Color Lavender { get; private set; }
        public static Color LavenderBlush { get; private set; }
        public static Color LawnGreen { get; private set; }
        public static Color LemonChiffon { get; private set; }
        public static Color LightBlue { get; private set; }
        public static Color LightCoral { get; private set; }
        public static Color LightCyan { get; private set; }
        public static Color LightGoldenrodYellow { get; private set; }
        public static Color LightGray { get; private set; }
        public static Color LightGreen { get; private set; }
        public static Color LightPink { get; private set; }
        public static Color LightSalmon { get; private set; }
        public static Color LightSeaGreen { get; private set; }
        public static Color LightSkyBlue { get; private set; }
        public static Color LightSlateGray { get; private set; }
        public static Color LightSteelBlue { get; private set; }
        public static Color LightYellow { get; private set; }
        public static Color Lime { get; private set; }
        public static Color LimeGreen { get; private set; }
        public static Color Linen { get; private set; }
        public static Color Magenta { get; private set; }
        public static Color Maroon { get; private set; }
        public static Color MediumAquamarine { get; private set; }
        public static Color MediumBlue { get; private set; }
        public static Color MediumOrchid { get; private set; }
        public static Color MediumPurple { get; private set; }
        public static Color MediumSeaGreen { get; private set; }
        public static Color MediumSlateBlue { get; private set; }
        public static Color MediumSpringGreen { get; private set; }
        public static Color MediumTurquoise { get; private set; }
        public static Color MediumVioletRed { get; private set; }
        public static Color MidnightBlue { get; private set; }
        public static Color MintCream { get; private set; }
        public static Color MistyRose { get; private set; }
        public static Color Moccasin { get; private set; }
        public static Color NavajoWhite { get; private set; }
        public static Color Navy { get; private set; }
        public static Color OldLace { get; private set; }
        public static Color Olive { get; private set; }
        public static Color OliveDrab { get; private set; }
        public static Color Orange { get; private set; }
        public static Color OrangeRed { get; private set; }
        public static Color Orchid { get; private set; }
        public static Color PaleGoldenrod { get; private set; }
        public static Color PaleGreen { get; private set; }
        public static Color PaleTurquoise { get; private set; }
        public static Color PaleVioletRed { get; private set; }
        public static Color PapayaWhip { get; private set; }
        public static Color PeachPuff { get; private set; }
        public static Color Peru { get; private set; }
        public static Color Pink { get; private set; }
        public static Color Plum { get; private set; }
        public static Color PowderBlue { get; private set; }
        public static Color Purple { get; private set; }
        public static Color Red { get; private set; }
        public static Color RosyBrown { get; private set; }
        public static Color RoyalBlue { get; private set; }
        public static Color SaddleBrown { get; private set; }
        public static Color Salmon { get; private set; }
        public static Color SandyBrown { get; private set; }
        public static Color SeaGreen { get; private set; }
        public static Color SeaShell { get; private set; }
        public static Color Sienna { get; private set; }
        public static Color Silver { get; private set; }
        public static Color SkyBlue { get; private set; }
        public static Color SlateBlue { get; private set; }
        public static Color SlateGray { get; private set; }
        public static Color Snow { get; private set; }
        public static Color SpringGreen { get; private set; }
        public static Color SteelBlue { get; private set; }
        public static Color Tan { get; private set; }
        public static Color Teal { get; private set; }
        public static Color Thistle { get; private set; }
        public static Color Tomato { get; private set; }
        public static Color Transparent { get; private set; }
        public static Color Turquoise { get; private set; }
        public static Color Violet { get; private set; }
        public static Color Wheat { get; private set; }
        public static Color White { get; private set; }
        public static Color WhiteSmoke { get; private set; }
        public static Color Yellow { get; private set; }
        public static Color YellowGreen { get; private set; }

        public static Color FromArgb(int alpha, Color baseColor)
        {
            return Color.FromArgb(alpha, baseColor.R, baseColor.G, baseColor.B);
        }
        public static Color FromArgb(int r, int g, int b)
        {
            return FromArgb(255, r, g, b);
        }
        public static Color FromArgb(int a, int r, int g, int b)
        {
            if (a < 0) a = 0;
            return new Color() { _a = (byte)a, _r = (byte)r, _g = (byte)g, _b = (byte)b, _isEmpty = false };
        }
        public static Color FromHsb(byte hue, byte saturation, byte brigthness)
        {
            double dh = (double)hue / 255;
            double ds = (double)saturation / 255;
            double db = (double)brigthness / 255;
            return FromHsb(dh, ds, db);
        }
        public static Color FromHsb(double hue, double saturation, double brigthness)
        {
            double r = 0, g = 0, b = 0;
            if (brigthness != 0)
            {
                if (saturation == 0)
                    r = g = b = brigthness;
                else
                {
                    double temp2 = _GetTemp2(hue, saturation, brigthness);
                    double temp1 = 2.0f * brigthness - temp2;

                    r = _GetColorComponent(temp1, temp2, hue + 1.0f / 3.0f);
                    g = _GetColorComponent(temp1, temp2, hue);
                    b = _GetColorComponent(temp1, temp2, hue - 1.0f / 3.0f);
                }
            }
            return Color.FromArgb((int)(255 * r), (int)(255 * g), (int)(255 * b));
        }
        public static Color FromUColor(UnityEngine.Color color)
        {
            return Color.FromArgb((int)(color.a * 255), (int)(color.r * 255), (int)(color.g * 255), (int)(color.b * 255));
        }

        public override bool Equals(object obj)
        {
            if (obj is Color)
                if (this == (Color)obj)
                    return true;
            return false;
        }
        public float GetBrightness()
        {
            float num = (float)this.R / 255f;
            float num2 = (float)this.G / 255f;
            float num3 = (float)this.B / 255f;
            float num4 = num;
            float num5 = num;
            if (num2 > num4)
            {
                num4 = num2;
            }
            if (num3 > num4)
            {
                num4 = num3;
            }
            if (num2 < num5)
            {
                num5 = num2;
            }
            if (num3 < num5)
            {
                num5 = num3;
            }
            return (num4 + num5) / 2f;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public string ToHexString()
        {
            return _r.ToString("X2") + _g.ToString("X2") + _b.ToString("X2") + _a.ToString("X2");
        }
        public float GetHue()
        {
            if (this.R == this.G && this.G == this.B)
            {
                return 0f;
            }
            float fR = (float)this.R / 255f;
            float fG = (float)this.G / 255f;
            float fB = (float)this.B / 255f;
            float num4 = 0f;
            float num5 = fR;
            float num6 = fR;
            if (fG > num5)
            {
                num5 = fG;
            }
            if (fB > num5)
            {
                num5 = fB;
            }
            if (fG < num6)
            {
                num6 = fG;
            }
            if (fB < num6)
            {
                num6 = fB;
            }
            float num7 = num5 - num6;
            if (fR == num5)
            {
                num4 = (fG - fB) / num7;
            }
            else if (fG == num5)
            {
                num4 = 2f + (fB - fR) / num7;
            }
            else if (fB == num5)
            {
                num4 = 4f + (fR - fG) / num7;
            }
            num4 *= 60f;
            if (num4 < 0f)
            {
                num4 += 360f;
            }
            return num4;
        }
        public float GetSaturation()
        {
            float num = (float)this.R / 255f;
            float num2 = (float)this.G / 255f;
            float num3 = (float)this.B / 255f;
            float result = 0f;
            float num4 = num;
            float num5 = num;
            if (num2 > num4)
            {
                num4 = num2;
            }
            if (num3 > num4)
            {
                num4 = num3;
            }
            if (num2 < num5)
            {
                num5 = num2;
            }
            if (num3 < num5)
            {
                num5 = num3;
            }
            if (num4 != num5)
            {
                float num6 = (num4 + num5) / 2f;
                if ((double)num6 <= 0.5)
                {
                    result = (num4 - num5) / (num4 + num5);
                }
                else
                {
                    result = (num4 - num5) / (2f - num4 - num5);
                }
            }
            return result;
        }
        public override string ToString()
        {
            return "{ R: " + R.ToString() + "; G: " + G.ToString() + "; B: " + B.ToString() + "; A: " + A.ToString() + " }";
        }
        public UnityEngine.Color ToUColor()
        {
            return new UnityEngine.Color((float)_r / 255, (float)_g / 255, (float)_b / 255, (float)_a / 255);
        }

        private static double _GetColorComponent(double temp1, double temp2, double temp3)
        {
            temp3 = _MoveIntoRange(temp3);
            if (temp3 < 1.0f / 6.0f)
                return temp1 + (temp2 - temp1) * 6.0f * temp3;
            else if (temp3 < 0.5f)
                return temp2;
            else if (temp3 < 2.0f / 3.0f)
                return temp1 + ((temp2 - temp1) * ((2.0f / 3.0f) - temp3) * 6.0f);
            else
                return temp1;
        }
        private static double _GetTemp2(double h, double s, double l)
        {
            double temp2;
            if (l < 0.5f)
                temp2 = l * (1.0f + s);
            else
                temp2 = l + s - (l * s);
            return temp2;
        }
        private static double _MoveIntoRange(double temp3)
        {
            if (temp3 < 0.0f)
                temp3 += 1.0f;
            else if (temp3 > 1.0f)
                temp3 -= 1.0f;
            return temp3;
        }
    }
}