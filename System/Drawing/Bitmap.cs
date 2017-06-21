using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Drawing
{
    [Serializable]
    public class Bitmap : Image
    {
        private Bitmap()
        {
            
        }
        public Bitmap(Image original)
        {
            if (original != null)
                uTexture = original.uTexture;
        }
        public Bitmap(int width, int height)
        {
            uTexture = Graphics.ApiGraphics.CreateTexture(width, height);
        }
        
        public void ClearColor(Color c, bool apply = true)
        {
            var colors = new Color[Width * Height];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = c;
            uTexture.SetPixels(colors);
            if (apply) this.Apply();
        }
        public Color GetPixel(int x, int y)
        {
            return uTexture.GetPixel(x, uTexture.Height - y - 1);
        }
        public Color[] GetPixels(int x, int y, int w, int h)
        {
            var ucs = uTexture.GetPixels(x, uTexture.Height - y - 1, w, h);
            Color[] cs = new Color[ucs.Length];
            for (int i = 0; i < cs.Length; i++)
                cs[i] = ucs[i];
            return cs;
        }
        public void SetPixel(int x, int y, Color color)
        {
            uTexture.SetPixel(x, uTexture.Height - y - 1, color);
        }

        public static Bitmap FromTexture(ITexture tex)
        {
            var bmp = new Bitmap();
            bmp.uTexture = tex;

            return bmp;
        }
    }
}
