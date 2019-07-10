namespace System.Windows.Forms
{
    using System.Drawing;

    public class GroupBox : Control
    {
        private readonly Pen borderPen = new Pen(Color.LightGray);
        private int borderMarginTop;
        private int textLeftOffset = 8;
        private int textHeight;
        private int textWidth;
        private bool textUpdateSize;
        
        public GroupBox()
        {
            TabIndex = -1;

            SetStyle(ControlStyles.Selectable, false);
        }

        internal Color uwfBorderColor
        {
            get { return borderPen.Color; }
            set { borderPen.Color = value; }
        }

        protected override Size DefaultSize
        {
            get { return new Size(200, 100); }
        }

        protected internal override void uwfOnLatePaint(PaintEventArgs e)
        {
            base.uwfOnLatePaint(e);

            var g = e.Graphics;

            var text = Text;
            if (textUpdateSize)
            {
                var size = g.MeasureString(text, Font);
                textHeight = (int)size.Height;
                textWidth = (int)size.Width + 2;
                textUpdateSize = false;
                borderMarginTop = textHeight / 2 - 2;
            }
            
            if (!string.IsNullOrEmpty(text))
            {
                // We can draw 5 lines or fill rect with back color (batches vs fillrate).
                g.DrawLine(borderPen, 0, borderMarginTop, 0, Height - 1); // Left.
                g.DrawLine(borderPen, Width - 1, borderMarginTop, Width - 1, Height - 1); // Right.
                g.DrawLine(borderPen, 0, Height - 1, Width - 1, Height - 1); // Bottom.
                g.DrawLine(borderPen, 0, borderMarginTop, textLeftOffset, borderMarginTop); // Top left.
                g.DrawLine(borderPen, textLeftOffset + textWidth, borderMarginTop, Width - 1, borderMarginTop); // Top right.
                g.uwfDrawString(text, Font, ForeColor, textLeftOffset, -2, Width - textLeftOffset * 2, 22, ContentAlignment.TopLeft);
            }
            else
                g.DrawRectangle(borderPen, 0, borderMarginTop, Width, Height - borderMarginTop);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            textUpdateSize = true;
        }
    }
}
