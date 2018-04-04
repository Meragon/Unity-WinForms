namespace System.Drawing
{
    [Serializable]
    public struct Color : IEquatable<Color>
    {
        public static readonly Color Empty = new Color(0, 0, 0, 0) { isEmpty = true };

        // fields have faster access.
        public static readonly Color AliceBlue = Color.FromArgb(0xF0, 0xF8, 0xFF);
        public static readonly Color AntiqueWhite = Color.FromArgb(0xFA, 0xEB, 0xD7);
        public static readonly Color Aqua = Color.FromArgb(0x00, 0xFF, 0xFF);
        public static readonly Color Aquamarine = Color.FromArgb(0x7F, 0xFF, 0xD4);
        public static readonly Color Azure = Color.FromArgb(0xF0, 0xFF, 0xFF);
        public static readonly Color Beige = Color.FromArgb(0xF5, 0xF5, 0xDC);
        public static readonly Color Bisque = Color.FromArgb(0xFF, 0xE4, 0xC4);
        public static readonly Color Black = Color.FromArgb(0x00, 0x00, 0x00);
        public static readonly Color BlanchedAlmond = Color.FromArgb(0xFF, 0xEB, 0xCD);
        public static readonly Color Blue = Color.FromArgb(0x00, 0x00, 0xFF);
        public static readonly Color BlueViolet = Color.FromArgb(0x8A, 0x2B, 0xE2);
        public static readonly Color Brown = Color.FromArgb(0xA5, 0x2A, 0x2A);
        public static readonly Color BurlyWood = Color.FromArgb(0xDE, 0xB8, 0x87);
        public static readonly Color CadetBlue = Color.FromArgb(0x5F, 0x9E, 0xA0);
        public static readonly Color Chartreuse = Color.FromArgb(0x7F, 0xFF, 0x00);
        public static readonly Color Chocolate = Color.FromArgb(0xD2, 0x69, 0x1E);
        public static readonly Color Coral = Color.FromArgb(0xFF, 0x7F, 0x50);
        public static readonly Color CornflowerBlue = Color.FromArgb(0x64, 0x95, 0xED);
        public static readonly Color Cornsilk = Color.FromArgb(0xFF, 0xF8, 0xDC);
        public static readonly Color Crimson = Color.FromArgb(0xDC, 0x14, 0x3C);
        public static readonly Color Cyan = Color.FromArgb(0x00, 0xFF, 0xFF);
        public static readonly Color DarkBlue = Color.FromArgb(0x00, 0x00, 0x8B);
        public static readonly Color DarkCyan = Color.FromArgb(0x00, 0x8B, 0x8B);
        public static readonly Color DarkGoldenrod = Color.FromArgb(0xB8, 0x86, 0x0B);
        public static readonly Color DarkGray = Color.FromArgb(0xA9, 0xA9, 0xA9);
        public static readonly Color DarkGreen = Color.FromArgb(0x00, 0x64, 0x00);
        public static readonly Color DarkKhaki = Color.FromArgb(0xBD, 0xB7, 0x6B);
        public static readonly Color DarkMagenta = Color.FromArgb(0x8B, 0x00, 0x8B);
        public static readonly Color DarkOliveGreen = Color.FromArgb(0x55, 0x6B, 0x2F);
        public static readonly Color DarkOrange = Color.FromArgb(0xFF, 0x8C, 0x00);
        public static readonly Color DarkOrchid = Color.FromArgb(0x99, 0x32, 0xCC);
        public static readonly Color DarkRed = Color.FromArgb(0x8B, 0x00, 0x00);
        public static readonly Color DarkSalmon = Color.FromArgb(0xE9, 0x96, 0x7A);
        public static readonly Color DarkSeaGreen = Color.FromArgb(0x8F, 0xBC, 0x8F);
        public static readonly Color DarkSlateBlue = Color.FromArgb(0x48, 0x3D, 0x8B);
        public static readonly Color DarkSlateGray = Color.FromArgb(0x2F, 0x4F, 0x4F);
        public static readonly Color DarkTurquoise = Color.FromArgb(0x00, 0xCE, 0xD1);
        public static readonly Color DarkViolet = Color.FromArgb(0x94, 0x00, 0xD3);
        public static readonly Color DeepPink = Color.FromArgb(0xFF, 0x14, 0x93);
        public static readonly Color DeepSkyBlue = Color.FromArgb(0x00, 0xBF, 0xFF);
        public static readonly Color DimGray = Color.FromArgb(0x69, 0x69, 0x69);
        public static readonly Color DodgerBlue = Color.FromArgb(0x1E, 0x90, 0xFF);
        public static readonly Color Firebrick = Color.FromArgb(0xB2, 0x22, 0x22);
        public static readonly Color FloralWhite = Color.FromArgb(0xFF, 0xFA, 0xF0);
        public static readonly Color ForestGreen = Color.FromArgb(0x22, 0x8B, 0x22);
        public static readonly Color Fuchsia = Color.FromArgb(0xFF, 0x00, 0xFF);
        public static readonly Color Gainsboro = Color.FromArgb(0xDC, 0xDC, 0xDC);
        public static readonly Color GhostWhite = Color.FromArgb(0xF8, 0xF8, 0xFF);
        public static readonly Color Gold = Color.FromArgb(0xFF, 0xD7, 0x00);
        public static readonly Color Goldenrod = Color.FromArgb(0xDA, 0xA5, 0x20);
        public static readonly Color Gray = Color.FromArgb(0x80, 0x80, 0x80);
        public static readonly Color Green = Color.FromArgb(0x00, 0x80, 0x00);
        public static readonly Color GreenYellow = Color.FromArgb(0xAD, 0xFF, 0x2F);
        public static readonly Color Honeydew = Color.FromArgb(0xF0, 0xFF, 0xF0);
        public static readonly Color HotPink = Color.FromArgb(0xFF, 0x69, 0xB4);
        public static readonly Color IndianRed = Color.FromArgb(0xCD, 0x5C, 0x5C);
        public static readonly Color Indigo = Color.FromArgb(0x4B, 0x00, 0x82);
        public static readonly Color Ivory = Color.FromArgb(0xFF, 0xFF, 0xF0);
        public static readonly Color Khaki = Color.FromArgb(0xF0, 0xE6, 0x8C);
        public static readonly Color Lavender = Color.FromArgb(0xE6, 0xE6, 0xFA);
        public static readonly Color LavenderBlush = Color.FromArgb(0xFF, 0xF0, 0xF5);
        public static readonly Color LawnGreen = Color.FromArgb(0x7C, 0xFC, 0x00);
        public static readonly Color LemonChiffon = Color.FromArgb(0xFF, 0xFA, 0xCD);
        public static readonly Color LightBlue = Color.FromArgb(0xAD, 0xD8, 0xE6);
        public static readonly Color LightCoral = Color.FromArgb(0xF0, 0x80, 0x80);
        public static readonly Color LightCyan = Color.FromArgb(0xE0, 0xFF, 0xFF);
        public static readonly Color LightGoldenrodYellow = Color.FromArgb(0xFA, 0xFA, 0xD2);
        public static readonly Color LightGray = Color.FromArgb(0xD3, 0xD3, 0xD3);
        public static readonly Color LightGreen = Color.FromArgb(0x90, 0xEE, 0x90);
        public static readonly Color LightPink = Color.FromArgb(0xFF, 0xB6, 0xC1);
        public static readonly Color LightSalmon = Color.FromArgb(0xFF, 0xA0, 0x7A);
        public static readonly Color LightSeaGreen = Color.FromArgb(0x20, 0xB2, 0xAA);
        public static readonly Color LightSkyBlue = Color.FromArgb(0x87, 0xCE, 0xFA);
        public static readonly Color LightSlateGray = Color.FromArgb(0x77, 0x88, 0x99);
        public static readonly Color LightSteelBlue = Color.FromArgb(0xB0, 0xC4, 0xDE);
        public static readonly Color LightYellow = Color.FromArgb(0xFF, 0xFF, 0xE0);
        public static readonly Color Lime = Color.FromArgb(0x00, 0xFF, 0x00);
        public static readonly Color LimeGreen = Color.FromArgb(0x32, 0xCD, 0x32);
        public static readonly Color Linen = Color.FromArgb(0xFA, 0xF0, 0xE6);
        public static readonly Color Magenta = Color.FromArgb(0xFF, 0x00, 0xFF);
        public static readonly Color Maroon = Color.FromArgb(0x80, 0x00, 0x00);
        public static readonly Color MediumAquamarine = Color.FromArgb(0x66, 0xCD, 0xAA);
        public static readonly Color MediumBlue = Color.FromArgb(0x00, 0x00, 0xCD);
        public static readonly Color MediumOrchid = Color.FromArgb(0xBA, 0x55, 0xD3);
        public static readonly Color MediumPurple = Color.FromArgb(0x93, 0x70, 0xDB);
        public static readonly Color MediumSeaGreen = Color.FromArgb(0x3C, 0xB3, 0x71);
        public static readonly Color MediumSlateBlue = Color.FromArgb(0x7B, 0x68, 0xEE);
        public static readonly Color MediumSpringGreen = Color.FromArgb(0x00, 0xFA, 0x9A);
        public static readonly Color MediumTurquoise = Color.FromArgb(0x48, 0xD1, 0xCC);
        public static readonly Color MediumVioletRed = Color.FromArgb(0xC7, 0x15, 0x85);
        public static readonly Color MidnightBlue = Color.FromArgb(0x19, 0x19, 0x70);
        public static readonly Color MintCream = Color.FromArgb(0xF5, 0xFF, 0xFA);
        public static readonly Color MistyRose = Color.FromArgb(0xFF, 0xE4, 0xE1);
        public static readonly Color Moccasin = Color.FromArgb(0xFF, 0xE4, 0xB5);
        public static readonly Color NavajoWhite = Color.FromArgb(0xFF, 0xDE, 0xAD);
        public static readonly Color Navy = Color.FromArgb(0x00, 0x00, 0x80);
        public static readonly Color OldLace = Color.FromArgb(0xFD, 0xF5, 0xE6);
        public static readonly Color Olive = Color.FromArgb(0x80, 0x80, 0x00);
        public static readonly Color OliveDrab = Color.FromArgb(0x6B, 0x8E, 0x23);
        public static readonly Color Orange = Color.FromArgb(0xFF, 0xA5, 0x00);
        public static readonly Color OrangeRed = Color.FromArgb(0xFF, 0x45, 0x00);
        public static readonly Color Orchid = Color.FromArgb(0xDA, 0x70, 0xD6);
        public static readonly Color PaleGoldenrod = Color.FromArgb(0xEE, 0xE8, 0xAA);
        public static readonly Color PaleGreen = Color.FromArgb(0x98, 0xFB, 0x98);
        public static readonly Color PaleTurquoise = Color.FromArgb(0xAF, 0xEE, 0xEE);
        public static readonly Color PaleVioletRed = Color.FromArgb(0xDB, 0x70, 0x93);
        public static readonly Color PapayaWhip = Color.FromArgb(0xFF, 0xEF, 0xD5);
        public static readonly Color PeachPuff = Color.FromArgb(0xFF, 0xDA, 0xB9);
        public static readonly Color Peru = Color.FromArgb(0xCD, 0x85, 0x3F);
        public static readonly Color Pink = Color.FromArgb(0xFF, 0xC0, 0xCB);
        public static readonly Color Plum = Color.FromArgb(0xDD, 0xA0, 0xDD);
        public static readonly Color PowderBlue = Color.FromArgb(0xB0, 0xE0, 0xE6);
        public static readonly Color Purple = Color.FromArgb(0x80, 0x00, 0x80);
        public static readonly Color Red = Color.FromArgb(0xFF, 0x00, 0x00);
        public static readonly Color RosyBrown = Color.FromArgb(0xBC, 0x8F, 0x8F);
        public static readonly Color RoyalBlue = Color.FromArgb(0x41, 0x69, 0xE1);
        public static readonly Color SaddleBrown = Color.FromArgb(0x8B, 0x45, 0x13);
        public static readonly Color Salmon = Color.FromArgb(0xFA, 0x80, 0x72);
        public static readonly Color SandyBrown = Color.FromArgb(0xF4, 0xA4, 0x60);
        public static readonly Color SeaGreen = Color.FromArgb(0x2E, 0x8B, 0x57);
        public static readonly Color SeaShell = Color.FromArgb(0xFF, 0xF5, 0xEE);
        public static readonly Color Sienna = Color.FromArgb(0xA0, 0x52, 0x2D);
        public static readonly Color Silver = Color.FromArgb(0xC0, 0xC0, 0xC0);
        public static readonly Color SkyBlue = Color.FromArgb(0x87, 0xCE, 0xEB);
        public static readonly Color SlateBlue = Color.FromArgb(0x6A, 0x5A, 0xCD);
        public static readonly Color SlateGray = Color.FromArgb(0x70, 0x80, 0x90);
        public static readonly Color Snow = Color.FromArgb(0xFF, 0xFA, 0xFA);
        public static readonly Color SpringGreen = Color.FromArgb(0x00, 0xFF, 0x7F);
        public static readonly Color SteelBlue = Color.FromArgb(0x46, 0x82, 0xB4);
        public static readonly Color Tan = Color.FromArgb(0xD2, 0xB4, 0x8C);
        public static readonly Color Teal = Color.FromArgb(0x00, 0x80, 0x80);
        public static readonly Color Thistle = Color.FromArgb(0xD8, 0xBF, 0xD8);
        public static readonly Color Tomato = Color.FromArgb(0xFF, 0x63, 0x47);
        public static readonly Color Transparent = Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF);
        public static readonly Color Turquoise = Color.FromArgb(0x40, 0xE0, 0xD0);
        public static readonly Color Violet = Color.FromArgb(0xEE, 0x82, 0xEE);
        public static readonly Color Wheat = Color.FromArgb(0xF5, 0xDE, 0xB3);
        public static readonly Color White = Color.FromArgb(0xFF, 0xFF, 0xFF);
        public static readonly Color WhiteSmoke = Color.FromArgb(0xF5, 0xF5, 0xF5);
        public static readonly Color Yellow = Color.FromArgb(0xFF, 0xFF, 0x00);
        public static readonly Color YellowGreen = Color.FromArgb(0x9A, 0xCD, 0x32);

        private readonly byte a, r, g, b;
        private bool isEmpty;

        private Color(byte a, byte r, byte g, byte b)
        {
            this.a = a;
            this.r = r;
            this.g = g;
            this.b = b;
            isEmpty = false;
        }

        public bool IsEmpty { get { return isEmpty; } }
        public byte A { get { return a; } }
        public byte R { get { return r; } }
        public byte G { get { return g; } }
        public byte B { get { return b; } }
        
        public static bool operator !=(Color left, Color right)
        {
            return !(left == right);
        }
        public static bool operator ==(Color left, Color right)
        {
            if (left.a == right.a && 
                left.r == right.r && 
                left.g == right.g && 
                left.b == right.b && 
                left.isEmpty == right.isEmpty)
                return true;
            return false;
        }

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
            return new Color((byte)a, (byte)r, (byte)g, (byte)b);
        }

        public bool Equals(Color other)
        {
            return isEmpty == other.isEmpty && a == other.a && r == other.r && g == other.g && b == other.b;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Color && Equals((Color)obj);
        }
        public float GetBrightness()
        {
            float num = R / 255f;
            float num2 = G / 255f;
            float num3 = B / 255f;
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
            unchecked
            {
                var hashCode = isEmpty.GetHashCode();
                hashCode = (hashCode * 397) ^ a.GetHashCode();
                hashCode = (hashCode * 397) ^ r.GetHashCode();
                hashCode = (hashCode * 397) ^ g.GetHashCode();
                hashCode = (hashCode * 397) ^ b.GetHashCode();
                return hashCode;
            }
        }
        public float GetHue()
        {
            if (R == G && G == B)
            {
                return 0f;
            }
            float fR = R / 255f;
            float fG = G / 255f;
            float fB = B / 255f;
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
            float num = R / 255f;
            float num2 = G / 255f;
            float num3 = B / 255f;
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
                if (num6 <= 0.5)
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
    }
}