namespace System.Drawing
{
    using System.Drawing.API;    
    
    public abstract class Image
    {
        public int Height { get { return uTexture.Height; } }
        public Size Size { get { return new Size(Width, Height); } }
        public object Tag { get; set; }
        public int Width { get { return uTexture.Width; } }

        internal ITexture uTexture { get; set; }
    }
}
