using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public class MathCustom
    {
        public static ColorF ColorLerp(ColorF from, Color to, float speed)
        {
            if (from == to) return from;
            var r = FloatLerp(from.R, to.R, speed);
            var g = FloatLerp(from.G, to.G, speed);
            var b = FloatLerp(from.B, to.B, speed);
            var a = FloatLerp(from.A, to.A, speed);
            return ColorF.FromArgb(a, r, g, b);
        }
        public static double DistanceD(float x1, float y1, float x2, float y2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }
        public static double DistanceD(PointF p1, PointF p2)
        {
            return DistanceD(p1.X, p1.Y, p2.X, p2.Y);
        }
        public static float DistanceF(float x1, float y1, float x2, float y2)
        {
            return (float)DistanceD(x1, y1, x2, y2);
        }
        public static float DistanceF(PointF p1, PointF p2)
        {
            return (float)DistanceD(p1, p2);
        }
        public static float FloatLerp(float from_value, float to_value, float speed)
        {
            return from_value + (to_value - from_value) * speed * UnityEngine.Time.deltaTime;
        }
    }
}
