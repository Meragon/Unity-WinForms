namespace System.Windows.Forms
{
    using System.Drawing;

    public class GroupBox : Control
    {
        private readonly Pen borderPen = new Pen(Color.LightGray);

        public GroupBox()
        {
            BackColor = Color.FromArgb(240, 240, 240);
            ForeColor = Color.Gray;
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

        protected override void uwfOnLatePaint(PaintEventArgs e)
        {
            base.uwfOnLatePaint(e);
            e.Graphics.DrawRectangle(borderPen, 0, 0, Width, Height);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;

            g.uwfFillRectangle(BackColor, 0, 0, Width, Height);
            g.uwfDrawString(Text, Font, ForeColor, 8, 0, Width - 16, Height - 0);
        }
    }
}
