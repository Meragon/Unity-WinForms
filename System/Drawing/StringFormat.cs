namespace System.Drawing
{
    public enum StringAlignment
    {
        Near = 0,
        Center = 1,
        Far = 2,
    }

    public sealed class StringFormat
    {
        public StringAlignment Alignment { get; set; }
        public StringAlignment LineAlignment { get; set; }
    }
}
