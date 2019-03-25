namespace System.Drawing
{
    [Flags]
    public enum FontStyle
    {
        Regular = 0,
        Bold = 1,
        Italic = 2,
        Underline = 4,
        Strikeout = 8,
    }
    
    public sealed class Font
    {
        internal object fontObject;

        private readonly string name;
        private readonly FontStyle style;

        public Font(string familyName, float emSize)
        {
            name = familyName;
            Size = emSize;
        }
        public Font(string familyName, float emSize, FontStyle style)
        {
            name = familyName;
            this.style = style;
            Size = emSize;
        }

        public string Name { get { return name; } }
        public float Size { get; set; }
        public FontStyle Style { get { return style; } }

        public override string ToString()
        {
            return string.Format("[{0}, {1}, {2}]", Name, Size, Style);
        }
    }
}
