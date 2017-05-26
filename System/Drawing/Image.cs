using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Drawing
{
    [Serializable]
    public abstract class Image
    {
        internal ITexture uTexture { get; set; }
        
        public int Height { get { return uTexture.Height; } }
        public Size Size { get { return new Size(Width, Height); } }
        public object Tag { get; set; }
        public int Width { get { return uTexture.Width; } }
        
        public abstract void Apply();
    }

    public interface ITexture
    {
        int Height { get; }
        int Width { get; }

        void Apply();
        Color GetPixel(int x, int y);
        Color[] GetPixels();
        Color[] GetPixels(int x, int y, int width, int height);
        void SetPixel(int x, int y, Color color);
        void SetPixels(Color[] colors);
        void SetPixels(int x, int y, int width, int height, Color[] colors);
    }
}
