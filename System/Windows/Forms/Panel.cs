namespace System.Windows.Forms
{
    using System.Drawing;

    public class Panel : ScrollableControl
    {
        private static readonly Pen borderSinglePen = new Pen(SystemColors.WindowFrame);
        private static readonly Pen borderFixed3dBottomPen = new Pen(SystemColors.Window, 2);
        private static readonly Pen borderFixed3dTopPen = new Pen(SystemColors.ButtonShadow, 2);

        public Panel()
        {
            SetStyle(ControlStyles.Selectable | ControlStyles.AllPaintingInWmPaint, false);
        }

        public virtual BorderStyle BorderStyle { get; set; }

        protected override Size DefaultSize
        {
            get { return new Size(200, 100); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            var w = Width;
            var h = Height;

            g.uwfFillRectangle(BackColor, 0, 0, w, h);

            switch (BorderStyle)
            {
                case BorderStyle.FixedSingle:
                    g.DrawRectangle(borderSinglePen, 0, 0, w, h);
                    break;
                case BorderStyle.Fixed3D:
                    g.DrawLine(borderFixed3dTopPen, 0, 0, w - 1, 0);
                    g.DrawLine(borderFixed3dTopPen, 0, 0, 0, h - 1);
                    g.DrawLine(borderFixed3dBottomPen, 0, h - 1, w - 1, h - 1);
                    g.DrawLine(borderFixed3dBottomPen, w - 1, 0, w - 1, h - 1);
                    break;
            }
        }
    }
}
