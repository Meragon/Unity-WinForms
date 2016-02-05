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
            return image.uTexture;
        }
        public static implicit operator Bitmap(UnityEngine.Texture2D text)
        {
            return new Bitmap(text);
        }

        internal Bitmap()
        {

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

        public void Apply()
        {
            uTexture.Apply();
        }
        public Color GetPixel(int x, int y)
        {
            return Color.FromUColor(uTexture.GetPixel(x, y));
        }
        public void SetPixel(int x, int y, Color color)
        {
            uTexture.SetPixel(x, y, color.ToUColor());
        }
    }
}
