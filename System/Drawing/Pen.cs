namespace System.Drawing
{
    using System.Drawing.Drawing2D;

    public sealed class Pen : IDisposable, ICloneable
    {
        public Pen(Color color)
        {
            Color = color;
            Width = 1;
        }
        
        public Pen(Color color, float width)
        {
            Color = color;
            Width = width;
        }

        ~Pen() 
        {
            Dispose(false);
        }
        
        public Color Color { get; set; }
        public DashStyle DashStyle { get; set; }
        public float Width { get; set; }

        public object Clone()
        {
            var pen = new Pen(Color);
            pen.DashStyle = DashStyle;
            pen.Width = Width;
            return pen;
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            // Nothing to dispose.
        }
    }
}
