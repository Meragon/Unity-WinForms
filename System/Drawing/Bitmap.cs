using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Drawing
{
    [Serializable]
    public class Bitmap : Image
    {
        public static implicit operator UnityEngine.Texture2D(Bitmap image)
        {
            if (image == null) return null;
            return image.uTexture;
        }
        public static implicit operator Bitmap(UnityEngine.Texture2D text)
        {
            return new Bitmap(text);
        }

        public Bitmap(UnityEngine.Texture2D original)
        {
            Color = Color.White;
            uTexture = original;
        }
        public Bitmap(UnityEngine.Texture2D original, Color color)
        {
            Color = color;
            uTexture = original;
        }
        public Bitmap(int width, int height)
        {
            Color = Color.White;
            uTexture = new UnityEngine.Texture2D(width, height);
        }

        public void Apply()
        {
            uTexture.Apply();
        }
        public Color GetPixel(int x, int y)
        {
            return Color.FromUColor(uTexture.GetPixel(x, uTexture.height - y));
        }
        public void SetPixel(int x, int y, Color color)
        {
            uTexture.SetPixel(x, uTexture.height - y, color.ToUColor());
        }
    }
}
