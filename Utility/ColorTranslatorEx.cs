using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Drawing
{
    public static class ColorTranslatorEx
    {
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
        public static string ToHexString(this Color c)
        {
            return c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2") + c.A.ToString("X2");
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
