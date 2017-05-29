using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace System.Drawing
{
    public static class UnityGdiHelper
    {
        public static Point FromVector2(UnityEngine.Vector2 vector)
        {
            return new Point((int)vector.x, (int)vector.y);
        }
        public static Bitmap ToBitmap(this Texture2D texture)
        {
            if (texture == null)
                return null;

            return Bitmap.FromTexture(Graphics.GAPI.CreateTexture(texture));
        }
        public static Color ToColor(this UnityEngine.Color color)
        {
            return Color.FromArgb((int)(color.a * 255), (int)(color.r * 255), (int)(color.g * 255), (int)(color.b * 255));
        }
        public static Vector2 ToVector2(this Point point)
        {
            return new UnityEngine.Vector2(point.X, point.Y);
        }
        public static UnityEngine.Color ToUnityColor(this Color color)
        {
            return new UnityEngine.Color((float)color.R / 255, (float)color.G / 255, (float)color.B / 255, (float)color.A / 255);
        }
    }
}
