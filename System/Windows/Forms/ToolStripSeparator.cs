namespace System.Windows.Forms
{
    using System.Drawing;

    public class ToolStripSeparator : ToolStripItem
    {
        protected Pen borderColor = new Pen(Color.FromArgb(215, 215, 215));
        protected Pen borderColor2 = new Pen(Color.White);

        public ToolStripSeparator()
        {
            JustVisual = true;
            Height = 0;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var ex = e.ClipRectangle.X + 32;
            var ey = e.ClipRectangle.Y;
            var ex2 = ex + e.ClipRectangle.Width - 8;
            var ey2 = e.ClipRectangle.Y + 1;

            e.Graphics.DrawLine(borderColor, ex, ey, ex2, ey);
            e.Graphics.DrawLine(borderColor2, ex, ey2, ex2, ey2);
        }
    }
}