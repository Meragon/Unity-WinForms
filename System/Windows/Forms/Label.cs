namespace System.Windows.Forms
{
    using System.Drawing;

    [Serializable]
    public class Label : Control
    {
        public Label()
        {
            BackColor = Color.Transparent;
            Padding = new Padding(4, 0, 8, 0);
            TabStop = false;
            TextAlign = ContentAlignment.TopLeft;

            SetStyle(ControlStyles.FixedHeight | ControlStyles.Selectable, false);
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        public ContentAlignment TextAlign { get; set; }

        protected override Size DefaultSize
        {
            get { return new Size(100, 23); } // Autosize ? Preferred height.
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;

            g.uwfFillRectangle(BackColor, 0, 0, Width, Height);
            g.uwfDrawString(Text, Font, ForeColor, Padding.Left, Padding.Top, Width - Padding.Right - Padding.Left, Height - Padding.Bottom - Padding.Top, TextAlign);
        }
    }
}
