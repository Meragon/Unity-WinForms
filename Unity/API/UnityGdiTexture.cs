namespace Unity.API
{
    using System.Drawing;
    using System.Drawing.API;

    using UE = UnityEngine;

    public class UnityGdiTexture : ITexture
    {
        internal readonly UE.Texture2D texture;

        public UnityGdiTexture(int width, int height)
        {
            texture = new UE.Texture2D(width, height);
        }
        public UnityGdiTexture(UE.Texture2D tex)
        {
            texture = tex;
        }

        public int Height { get { return texture.height; } }
        public int Width { get { return texture.width; } }

        public void Apply()
        {
            texture.Apply();
        }
        public void Clear(Color color)
        {
            var colors = new Color[Width * Height];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = color;

            SetPixels(colors);
        }
        public Color GetPixel(int x, int y)
        {
            return texture.GetPixel(x, texture.height - y - 1).ToColor();
        }
        public Color[] GetPixels()
        {
            // Fix: updside down.
            return GetPixels(0, 0, Width, Height);
        }
        public Color[] GetPixels(int x, int y, int width, int height)
        {
            // Fix: updside down.
            var ucs = texture.GetPixels(x, y, width, height);
            var cs = new Color[ucs.Length];
            for (int i = 0; i < ucs.Length; i++)
                cs[i] = ucs[i].ToColor();

            return cs;
        }
        public void SetPixel(int x, int y, Color color)
        {
            texture.SetPixel(x, texture.height - y - 1, color.ToUnityColor());
        }
        public void SetPixels(Color[] colors)
        {
            // Fix: updside down.
            var ucs = new UnityEngine.Color32[colors.Length];
            for (int i = 0; i < ucs.Length; i++)
                ucs[i] = colors[i].ToUnityColor();

            texture.SetPixels32(ucs);
        }
        public void SetPixels(int x, int y, int width, int height, Color[] colors)
        {
            // Fix: updside down.
            var ucs = new UnityEngine.Color32[colors.Length];
            for (int i = 0; i < ucs.Length; i++)
                ucs[i] = colors[i].ToUnityColor();

            texture.SetPixels32(x, y, width, height, ucs);
        }
    }
}
