namespace System.Drawing
{
    public sealed class SolidBrush : Brush
    {
        public SolidBrush(Color color)
        {
            Color = color;
        }

        public Color Color { get; set; }

        public override object Clone()
        {
            return new SolidBrush(Color);
        }
    }
}
