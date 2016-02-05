using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Drawing
{
    [Serializable]
    public abstract class Image
    {
        internal UnityEngine.Texture2D uTexture { get; set; }

        public Color Color { get; set; }
        public int Height { get { return uTexture.height; } }
        public Size Size { get { return new Size(Width, Height); } }
        public int Width { get { return uTexture.width; } }

        public static Image FromTexture(UnityEngine.Texture2D original)
        {
            return new Bitmap(original);
        }
    }
}
