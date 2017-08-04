namespace System.Windows.Forms
{
    using System.Drawing;

    public class Panel : ScrollableControl
    {
        protected readonly Pen borderPen = new Pen(Color.White);

        public Panel()
        {
            SetStyle(ControlStyles.Selectable | ControlStyles.AllPaintingInWmPaint, false);
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

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.uwfFillRectangle(BackColor, 0, 0, Width, Height);
            e.Graphics.DrawRectangle(borderPen, 0, 0, Width, Height);
        }
    }
}
