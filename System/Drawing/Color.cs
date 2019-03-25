namespace System.Drawing
{
    [Serializable]
    public struct Color : IEquatable<Color>
    {
        public static readonly Color Empty = new Color();

        public static readonly Color AliceBlue            = new Color(KnownColor.AliceBlue,            0xFFF0F8FF);
        public static readonly Color AntiqueWhite         = new Color(KnownColor.AntiqueWhite,         0xFFFAEBD7);
        public static readonly Color Aqua                 = new Color(KnownColor.Aqua,                 0xFF00FFFF);
        public static readonly Color Aquamarine           = new Color(KnownColor.Aquamarine,           0xFF7FFFD4);
        public static readonly Color Azure                = new Color(KnownColor.Azure,                0xFFF0FFFF);
        public static readonly Color Beige                = new Color(KnownColor.Beige,                0xFFF5F5DC);
        public static readonly Color Bisque               = new Color(KnownColor.Bisque,               0xFFFFE4C4);
        public static readonly Color Black                = new Color(KnownColor.Black,                0xFF000000);
        public static readonly Color BlanchedAlmond       = new Color(KnownColor.BlanchedAlmond,       0xFFFFEBCD);
        public static readonly Color Blue                 = new Color(KnownColor.Blue,                 0xFF0000FF);
        public static readonly Color BlueViolet           = new Color(KnownColor.BlueViolet,           0xFF8A2BE2);
        public static readonly Color Brown                = new Color(KnownColor.Brown,                0xFFA52A2A);
        public static readonly Color BurlyWood            = new Color(KnownColor.BurlyWood,            0xFFDEB887);
        public static readonly Color CadetBlue            = new Color(KnownColor.CadetBlue,            0xFF5F9EA0);
        public static readonly Color Chartreuse           = new Color(KnownColor.Chartreuse,           0xFF7FFF00);
        public static readonly Color Chocolate            = new Color(KnownColor.Chocolate,            0xFFD2691E);
        public static readonly Color Coral                = new Color(KnownColor.Coral,                0xFFFF7F50);
        public static readonly Color CornflowerBlue       = new Color(KnownColor.CornflowerBlue,       0xFF6495ED);
        public static readonly Color Cornsilk             = new Color(KnownColor.Cornsilk,             0xFFFFF8DC);
        public static readonly Color Crimson              = new Color(KnownColor.Crimson,              0xFFDC143C);
        public static readonly Color Cyan                 = new Color(KnownColor.Cyan,                 0xFF00FFFF);
        public static readonly Color DarkBlue             = new Color(KnownColor.DarkBlue,             0xFF00008B);
        public static readonly Color DarkCyan             = new Color(KnownColor.DarkCyan,             0xFF008B8B);
        public static readonly Color DarkGoldenrod        = new Color(KnownColor.DarkGoldenrod,        0xFFB8860B);
        public static readonly Color DarkGray             = new Color(KnownColor.DarkGray,             0xFFA9A9A9);
        public static readonly Color DarkGreen            = new Color(KnownColor.DarkGreen,            0xFF006400);
        public static readonly Color DarkKhaki            = new Color(KnownColor.DarkKhaki,            0xFFBDB76B);
        public static readonly Color DarkMagenta          = new Color(KnownColor.DarkMagenta,          0xFF8B008B);
        public static readonly Color DarkOliveGreen       = new Color(KnownColor.DarkOliveGreen,       0xFF556B2F);
        public static readonly Color DarkOrange           = new Color(KnownColor.DarkOrange,           0xFFFF8C00);
        public static readonly Color DarkOrchid           = new Color(KnownColor.DarkOrchid,           0xFF9932CC);
        public static readonly Color DarkRed              = new Color(KnownColor.DarkRed,              0xFF8B0000);
        public static readonly Color DarkSalmon           = new Color(KnownColor.DarkSalmon,           0xFFE9967A);
        public static readonly Color DarkSeaGreen         = new Color(KnownColor.DarkSeaGreen,         0xFF8FBC8B);
        public static readonly Color DarkSlateBlue        = new Color(KnownColor.DarkSlateBlue,        0xFF483D8B);
        public static readonly Color DarkSlateGray        = new Color(KnownColor.DarkSlateGray,        0xFF2F4F4F);
        public static readonly Color DarkTurquoise        = new Color(KnownColor.DarkTurquoise,        0xFF00CED1);
        public static readonly Color DarkViolet           = new Color(KnownColor.DarkViolet,           0xFF9400D3);
        public static readonly Color DeepPink             = new Color(KnownColor.DeepPink,             0xFFFF1493);
        public static readonly Color DeepSkyBlue          = new Color(KnownColor.DeepSkyBlue,          0xFF00BFFF);
        public static readonly Color DimGray              = new Color(KnownColor.DimGray,              0xFF696969);
        public static readonly Color DodgerBlue           = new Color(KnownColor.DodgerBlue,           0xFF1E90FF);
        public static readonly Color Firebrick            = new Color(KnownColor.Firebrick,            0xFFB22222);
        public static readonly Color FloralWhite          = new Color(KnownColor.FloralWhite,          0xFFFFFAF0);
        public static readonly Color ForestGreen          = new Color(KnownColor.ForestGreen,          0xFF228B22);
        public static readonly Color Fuchsia              = new Color(KnownColor.Fuchsia,              0xFFFF00FF);
        public static readonly Color Gainsboro            = new Color(KnownColor.Gainsboro,            0xFFDCDCDC);
        public static readonly Color GhostWhite           = new Color(KnownColor.GhostWhite,           0xFFF8F8FF);
        public static readonly Color Gold                 = new Color(KnownColor.Gold,                 0xFFFFD700);
        public static readonly Color Goldenrod            = new Color(KnownColor.Goldenrod,            0xFFDAA520);
        public static readonly Color Gray                 = new Color(KnownColor.Gray,                 0xFF808080);
        public static readonly Color Green                = new Color(KnownColor.Green,                0xFF008000);
        public static readonly Color GreenYellow          = new Color(KnownColor.GreenYellow,          0xFFADFF2F);
        public static readonly Color Honeydew             = new Color(KnownColor.Honeydew,             0xFFF0FFF0);
        public static readonly Color HotPink              = new Color(KnownColor.HotPink,              0xFFFF69B4);
        public static readonly Color IndianRed            = new Color(KnownColor.IndianRed,            0xFFCD5C5C);
        public static readonly Color Indigo               = new Color(KnownColor.Indigo,               0xFF4B0082);
        public static readonly Color Ivory                = new Color(KnownColor.Ivory,                0xFFFFFFF0);
        public static readonly Color Khaki                = new Color(KnownColor.Khaki,                0xFFF0E68C);
        public static readonly Color Lavender             = new Color(KnownColor.Lavender,             0xFFE6E6FA);
        public static readonly Color LavenderBlush        = new Color(KnownColor.LavenderBlush,        0xFFFFF0F5);
        public static readonly Color LawnGreen            = new Color(KnownColor.LawnGreen,            0xFF7CFC00);
        public static readonly Color LemonChiffon         = new Color(KnownColor.LemonChiffon,         0xFFFFFACD);
        public static readonly Color LightBlue            = new Color(KnownColor.LightBlue,            0xFFADD8E6);
        public static readonly Color LightCoral           = new Color(KnownColor.LightCoral,           0xFFF08080);
        public static readonly Color LightCyan            = new Color(KnownColor.LightCyan,            0xFFE0FFFF);
        public static readonly Color LightGoldenrodYellow = new Color(KnownColor.LightGoldenrodYellow, 0xFFFAFAD2);
        public static readonly Color LightGray            = new Color(KnownColor.LightGray,            0xFFD3D3D3);
        public static readonly Color LightGreen           = new Color(KnownColor.LightGreen,           0xFF90EE90);
        public static readonly Color LightPink            = new Color(KnownColor.LightPink,            0xFFFFB6C1);
        public static readonly Color LightSalmon          = new Color(KnownColor.LightSalmon,          0xFFFFA07A);
        public static readonly Color LightSeaGreen        = new Color(KnownColor.LightSeaGreen,        0xFF20B2AA);
        public static readonly Color LightSkyBlue         = new Color(KnownColor.SkyBlue,              0xFF87CEFA);
        public static readonly Color LightSlateGray       = new Color(KnownColor.LightSlateGray,       0xFF778899);
        public static readonly Color LightSteelBlue       = new Color(KnownColor.LightSteelBlue,       0xFFB0C4DE);
        public static readonly Color LightYellow          = new Color(KnownColor.LightYellow,          0xFFFFFFE0);
        public static readonly Color Lime                 = new Color(KnownColor.Lime,                 0xFF00FF00);
        public static readonly Color LimeGreen            = new Color(KnownColor.LimeGreen,            0xFF32CD32);
        public static readonly Color Linen                = new Color(KnownColor.Linen,                0xFFFAF0E6);
        public static readonly Color Magenta              = new Color(KnownColor.Magenta,              0xFFFF00FF);
        public static readonly Color Maroon               = new Color(KnownColor.Maroon,               0xFF800000);
        public static readonly Color MediumAquamarine     = new Color(KnownColor.MediumAquamarine,     0xFF66CDAA);
        public static readonly Color MediumBlue           = new Color(KnownColor.MediumBlue,           0xFF0000CD);
        public static readonly Color MediumOrchid         = new Color(KnownColor.MediumOrchid,         0xFFBA55D3);
        public static readonly Color MediumPurple         = new Color(KnownColor.MediumPurple,         0xFF9370DB);
        public static readonly Color MediumSeaGreen       = new Color(KnownColor.MediumSeaGreen,       0xFF3CB371);
        public static readonly Color MediumSlateBlue      = new Color(KnownColor.MediumSlateBlue,      0xFF7B68EE);
        public static readonly Color MediumSpringGreen    = new Color(KnownColor.MediumSpringGreen,    0xFF00FA9A);
        public static readonly Color MediumTurquoise      = new Color(KnownColor.MediumTurquoise,      0xFF48D1CC);
        public static readonly Color MediumVioletRed      = new Color(KnownColor.MediumVioletRed,      0xFFC71585);
        public static readonly Color MidnightBlue         = new Color(KnownColor.MidnightBlue,         0xFF191970);
        public static readonly Color MintCream            = new Color(KnownColor.MintCream,            0xFFF5FFFA);
        public static readonly Color MistyRose            = new Color(KnownColor.MistyRose,            0xFFFFE4E1);
        public static readonly Color Moccasin             = new Color(KnownColor.Moccasin,             0xFFFFE4B5);
        public static readonly Color NavajoWhite          = new Color(KnownColor.NavajoWhite,          0xFFFFDEAD);
        public static readonly Color Navy                 = new Color(KnownColor.Navy,                 0xFF000080);
        public static readonly Color OldLace              = new Color(KnownColor.OldLace,              0xFFFDF5E6);
        public static readonly Color Olive                = new Color(KnownColor.Olive,                0xFF808000);
        public static readonly Color OliveDrab            = new Color(KnownColor.OliveDrab,            0xFF6B8E23);
        public static readonly Color Orange               = new Color(KnownColor.Orange,               0xFFFFA500);
        public static readonly Color OrangeRed            = new Color(KnownColor.OrangeRed,            0xFFFF4500);
        public static readonly Color Orchid               = new Color(KnownColor.Orchid,               0xFFDA70D6);
        public static readonly Color PaleGoldenrod        = new Color(KnownColor.PaleGoldenrod,        0xFFEEE8AA);
        public static readonly Color PaleGreen            = new Color(KnownColor.PaleGreen,            0xFF98FB98);
        public static readonly Color PaleTurquoise        = new Color(KnownColor.PaleTurquoise,        0xFFAFEEEE);
        public static readonly Color PaleVioletRed        = new Color(KnownColor.PaleVioletRed,        0xFFDB7093);
        public static readonly Color PapayaWhip           = new Color(KnownColor.PapayaWhip,           0xFFFFEFD5);
        public static readonly Color PeachPuff            = new Color(KnownColor.PeachPuff,            0xFFFFDAB9);
        public static readonly Color Peru                 = new Color(KnownColor.Peru,                 0xFFCD853F);
        public static readonly Color Pink                 = new Color(KnownColor.Pink,                 0xFFFFC0CB);
        public static readonly Color Plum                 = new Color(KnownColor.Plum,                 0xFFDDA0DD);
        public static readonly Color PowderBlue           = new Color(KnownColor.PowderBlue,           0xFFB0E0E6);
        public static readonly Color Purple               = new Color(KnownColor.Purple,               0xFF800080);
        public static readonly Color Red                  = new Color(KnownColor.Red,                  0xFFFF0000);
        public static readonly Color RosyBrown            = new Color(KnownColor.RosyBrown,            0xFFBC8F8F);
        public static readonly Color RoyalBlue            = new Color(KnownColor.RoyalBlue,            0xFF4169E1);
        public static readonly Color SaddleBrown          = new Color(KnownColor.SaddleBrown,          0xFF8B4513);
        public static readonly Color Salmon               = new Color(KnownColor.Salmon,               0xFFFA8072);
        public static readonly Color SandyBrown           = new Color(KnownColor.SandyBrown,           0xFFF4A460);
        public static readonly Color SeaGreen             = new Color(KnownColor.SeaGreen,             0xFF2E8B57);
        public static readonly Color SeaShell             = new Color(KnownColor.SeaShell,             0xFFFFF5EE);
        public static readonly Color Sienna               = new Color(KnownColor.Sienna,               0xFFA0522D);
        public static readonly Color Silver               = new Color(KnownColor.Silver,               0xFFC0C0C0);
        public static readonly Color SkyBlue              = new Color(KnownColor.SkyBlue,              0xFF87CEEB);
        public static readonly Color SlateBlue            = new Color(KnownColor.SlateBlue,            0xFF6A5ACD);
        public static readonly Color SlateGray            = new Color(KnownColor.SlateGray,            0xFF708090);
        public static readonly Color Snow                 = new Color(KnownColor.Snow,                 0xFFFFFAFA);
        public static readonly Color SpringGreen          = new Color(KnownColor.SpringGreen,          0xFF00FF7F);
        public static readonly Color SteelBlue            = new Color(KnownColor.SteelBlue,            0xFF4682B4);
        public static readonly Color Tan                  = new Color(KnownColor.Tan,                  0xFFD2B48C);
        public static readonly Color Teal                 = new Color(KnownColor.Teal,                 0xFF008080);
        public static readonly Color Thistle              = new Color(KnownColor.Thistle,              0xFFD8BFD8);
        public static readonly Color Tomato               = new Color(KnownColor.Tomato,               0xFFFF6347);
        public static readonly Color Transparent          = new Color(KnownColor.Transparent,          0x00FFFFFF);
        public static readonly Color Turquoise            = new Color(KnownColor.Turquoise,            0xFF40E0D0);
        public static readonly Color Violet               = new Color(KnownColor.Violet,               0xFFEE82EE);
        public static readonly Color Wheat                = new Color(KnownColor.Wheat,                0xFFF5DEB3);
        public static readonly Color White                = new Color(KnownColor.White,                0xFFFFFFFF);
        public static readonly Color WhiteSmoke           = new Color(KnownColor.WhiteSmoke,           0xFFF5F5F5);
        public static readonly Color Yellow               = new Color(KnownColor.Yellow,               0xFFFFFF00);
        public static readonly Color YellowGreen          = new Color(KnownColor.YellowGreen,          0xFF9ACD32);

        private const int ARGBAlphaShift = 24;
        private const int ARGBRedShift   = 16;
        private const int ARGBGreenShift = 8;
        private const int ARGBBlueShift  = 0;
        
        private const short StateKnownColorValid = 0x0001;
        private const short StateARGBValueValid  = 0x0002;
        
        private readonly byte a, r, g, b;
        private readonly short knownColor;
        private readonly short state;

        internal Color(byte a, byte r, byte g, byte b)
        {
            this.a = a;
            this.r = r;
            this.g = g;
            this.b = b;
            this.state = StateARGBValueValid;
            this.knownColor = 0;
        }
        internal Color(KnownColor knownColor, long value)
        {
            this.a = (byte)((value >> ARGBAlphaShift) & 0xFF);
            this.r = (byte)((value >> ARGBRedShift) & 0xFF);
            this.g = (byte)((value >> ARGBGreenShift) & 0xFF);
            this.b = (byte)((value >> ARGBBlueShift) & 0xFF);
            this.state = StateKnownColorValid;
            this.knownColor = (short)knownColor;
        }

        public bool IsEmpty { get { return state == 0; } }
        public bool IsKnownColor { get { return knownColor != 0; } }
        public bool IsSystemColor {
            get 
            {
                return IsKnownColor && ((KnownColor) knownColor <= KnownColor.WindowText || 
                                        (KnownColor) knownColor > KnownColor.YellowGreen);
            }
        }
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
                left.state == right.state)
                return true;
            return false;
        }

        public static Color FromArgb(int argb) 
        {
            return new Color(0, argb);
        }
        public static Color FromArgb(int alpha, Color baseColor)
        {
            return FromArgb(alpha, baseColor.R, baseColor.G, baseColor.B);
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
            return state == other.state && a == other.a && r == other.r && g == other.g && b == other.b;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Color && Equals((Color)obj);
        }
        public float GetBrightness()
        {
            float fR = R / 255f;
            float fG = G / 255f;
            float fB = B / 255f;
            float max = fR;
            float min = fR;

            if (fG > max) max = fG;
            if (fB > max) max = fB;
            if (fG < min) min = fG;
            if (fB < min) min = fB;
            
            return (max + min) / 2f;
        }
        public override int GetHashCode()
        {
            return r.GetHashCode() ^
                   g.GetHashCode() ^
                   b.GetHashCode() ^
                   a.GetHashCode() ^
                   state.GetHashCode() ^
                   knownColor.GetHashCode();
        }
        public float GetHue()
        {
            if (R == G && G == B)
                return 0f;

            float fR = R / 255f;
            float fG = G / 255f;
            float fB = B / 255f;
            float hue = 0f;
            float max = fR;
            float min = fR;

            if (fG > max) max = fG;
            if (fB > max) max = fB;
            if (fG < min) min = fG;
            if (fB < min) min = fB;
            
            float delta = max - min;
            
            if (fR == max)
            {
                hue = (fG - fB) / delta;
            }
            else if (fG == max)
            {
                hue = 2f + (fB - fR) / delta;
            }
            else
            {
                hue = 4f + (fR - fG) / delta;
            }
            
            hue *= 60f;

            if (hue < 0f)
                hue += 360f;
            
            return hue;
        }
        public float GetSaturation()
        {
            float fR = R / 255f;
            float fG = G / 255f;
            float fB = B / 255f;
            float saturation = 0f;
            float max = fR;
            float min = fR;
            if (fG > max) max = fG;
            if (fB > max) max = fB;
            if (fG < min) min = fG;
            if (fB < min) min = fB;
            
            if (max != min)
            {
                float l = (max + min) / 2f;
                if (l <= 0.5)
                {
                    saturation = (max - min) / (max + min);
                }
                else
                {
                    saturation = (max - min) / (2f - max - min);
                }
            }
            
            return saturation;
        }
        public override string ToString()
        {
            return "{ R: " + R + "; G: " + G + "; B: " + B + "; A: " + A + " }";
        }
    }
}