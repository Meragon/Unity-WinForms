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

        public override void Apply()
        {
            uTexture.Apply();
        }
        public void ClearColor(Color c, bool apply = true)
        {
            var colors = new UnityEngine.Color32[Width * Height];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = c.ToUColor();
            uTexture.SetPixels32(colors);
            if (apply) Apply();
        }
        public Color GetPixel(int x, int y)
        {
            return Color.FromUColor(uTexture.GetPixel(x, uTexture.height - y - 1));
        }
        public Color[] GetPixels(int x, int y, int w, int h)
        {
            var ucs = uTexture.GetPixels(x, uTexture.height - y - 1, w, h);
            Color[] cs = new Color[ucs.Length];
            for (int i = 0; i < cs.Length; i++)
                cs[i] = Color.FromUColor(ucs[i]);
            return cs;
        }
        public void SetPixel(int x, int y, Color color)
        {
            uTexture.SetPixel(x, uTexture.height - y - 1, color.ToUColor());
        }
    }
}
