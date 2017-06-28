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

        public Color Color { get; set; }
        public DashStyle DashStyle { get; set; }
        public float Width { get; set; }

        public object Clone()
        {
            var pen = new Pen(this.Color);
            pen.DashStyle = this.DashStyle;
            pen.Width = this.Width;
            return pen;
        }
        public void Dispose()
        {
        }
    }
}
