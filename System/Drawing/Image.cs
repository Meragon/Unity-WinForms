namespace System.Drawing
{
    using System.Drawing.API;    
    
    public abstract class Image
    {
        internal Image()
        {
            Color = Color.White;
        }
        
        public int Height { get { return Texture.Height; } }
        public Size Size { get { return new Size(Width, Height); } }
        public object Tag { get; set; }
        public int Width { get { return Texture.Width; } }

        internal Color Color { get; set; }
        internal ITexture Texture { get; set; }
    }
}
