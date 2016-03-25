using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Drawing
{
    public sealed class ColorTranslator
    {
        public static Color FromHtml(string htmlColor)
        {
            if (string.IsNullOrEmpty(htmlColor)) return Color.Empty;

            Color result = Color.Empty;
            if (htmlColor[0] == '#' && (htmlColor.Length == 7 || htmlColor.Length == 4))
            {
                if (htmlColor.Length == 7)
                {
                    result = Color.FromArgb(
                        Convert.ToInt32(htmlColor.Substring(1, 2), 16),
                        Convert.ToInt32(htmlColor.Substring(3, 2), 16),
                        Convert.ToInt32(htmlColor.Substring(5, 2), 16)
                        );
                }
                else
                {
                    string halfRed = char.ToString(htmlColor[1]);
                    string halfGreen = char.ToString(htmlColor[2]);
                    string halfBlue = char.ToString(htmlColor[3]);
                    int red = Convert.ToInt32(halfRed + halfRed, 16);
                    int green = Convert.ToInt32(halfGreen + halfGreen, 16);
                    int blue = Convert.ToInt32(halfBlue + halfBlue, 16);
                    result = Color.FromArgb(red, green, blue);
                }
            }
            return result;
        }
        public static Color FromOle(int oleColor)
        {
            throw new NotImplementedException();
        }
        public static Color FromWin32(int win32Color)
        {
            return FromOle(win32Color);
        }
        public static string ToHtml(Color c)
        {
            throw new NotImplementedException();
        }
        public static int ToOle(Color c)
        {
            throw new NotImplementedException();
        }
        public static int ToWin32(Color c)
        {
            return c.R | c.G << 8 | c.B << 16;
        }
    }
}
