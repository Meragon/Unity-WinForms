namespace System.Drawing
{
    using System.Drawing.API;
    
    [Serializable]
    public class Bitmap : Image
    {
        public Bitmap(Image original)
        {
            if (original != null)
                Texture = original.Texture;
        }
        public Bitmap(int width, int height)
        {
            Texture = Graphics.ApiGraphics.CreateTexture(width, height);
        }

        private Bitmap()
        {
        }
        
        public void ClearColor(Color c, bool apply = true)
        {
            var colors = new Color[Width * Height];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = c;
            Texture.SetPixels(colors);
            if (apply) this.Apply();
        }
        public Color GetPixel(int x, int y)
        {
            return Texture.GetPixel(x, Texture.Height - y - 1);
        }
        public Color[] GetPixels(int x, int y, int w, int h)
        {
            var ucs = Texture.GetPixels(x, Texture.Height - y - 1, w, h);
            Color[] cs = new Color[ucs.Length];
            for (int i = 0; i < cs.Length; i++)
                cs[i] = ucs[i];
            return cs;
        }
        public void SetPixel(int x, int y, Color color)
        {
            Texture.SetPixel(x, Texture.Height - y - 1, color);
        }

        internal static Bitmap FromTexture(ITexture tex)
        {
            var bmp = new Bitmap();
            bmp.Texture = tex;

            return bmp;
        }
    }
}
