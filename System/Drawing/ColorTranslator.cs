namespace System.Drawing
{
    public static class ColorTranslator
    {
        public static Color FromHtml(string htmlColor)
        {
            if (string.IsNullOrEmpty(htmlColor)) return Color.Empty;

            Color result = Color.Empty;
            if (htmlColor[0] == '#')
                htmlColor = htmlColor.Substring(1);

            switch (htmlColor.Length)
            {
                case 3: // half.
                    string halfRed = char.ToString(htmlColor[0]);
                    string halfGreen = char.ToString(htmlColor[1]);
                    string halfBlue = char.ToString(htmlColor[2]);
                    int red = Convert.ToInt32(halfRed + halfRed, 16);
                    int green = Convert.ToInt32(halfGreen + halfGreen, 16);
                    int blue = Convert.ToInt32(halfBlue + halfBlue, 16);
                    result = Color.FromArgb(red, green, blue);
                    break;
                case 6: // rgb.
                    result = Color.FromArgb(
                    Convert.ToInt32(htmlColor.Substring(0, 2), 16),
                    Convert.ToInt32(htmlColor.Substring(2, 2), 16),
                    Convert.ToInt32(htmlColor.Substring(4, 2), 16));
                    break;
                case 8: // argb.
                    result = Color.FromArgb(
                    Convert.ToInt32(htmlColor.Substring(0, 2), 16),
                    Convert.ToInt32(htmlColor.Substring(2, 2), 16),
                    Convert.ToInt32(htmlColor.Substring(4, 2), 16),
                    Convert.ToInt32(htmlColor.Substring(6, 2), 16));
                    break;
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
            return string.Concat("#", c.A.ToString("X2"), c.R.ToString("X2"), c.G.ToString("X2"), c.B.ToString("X2"));
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
