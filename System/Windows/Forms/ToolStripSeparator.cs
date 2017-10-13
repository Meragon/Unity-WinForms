namespace System.Windows.Forms
{
    using System.Drawing;

    public class ToolStripSeparator : ToolStripItem
    {
        protected Pen borderColor = new Pen(defaultSeparatorBorderColor);
        protected Pen borderColor2 = new Pen(Color.White);

        private const int WINBAR_SEPARATORTHICKNESS = 6;
        private static readonly Color defaultSeparatorBorderColor = Color.FromArgb(189, 189, 189);

        public override bool CanSelect
        {
            get { return false; }
        }

        protected override Size DefaultSize
        {
            get { return new Size(WINBAR_SEPARATORTHICKNESS, WINBAR_SEPARATORTHICKNESS); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            var bounds = Bounds;
            var ex = bounds.X;
            var ey = bounds.Y + Height / 2 - 1;
            var ex2 = bounds.X + bounds.Width - 8;
            var ey2 = ey + 1;

            if (Owner != null && Owner.Orientation == Orientation.Vertical)
                ex += Owner.Padding.Left;

            g.DrawLine(borderColor, ex, ey, ex2, ey);
            g.DrawLine(borderColor2, ex, ey2, ex2, ey2);
        }
    }
}