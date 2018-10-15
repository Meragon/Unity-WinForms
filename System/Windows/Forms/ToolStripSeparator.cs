namespace System.Windows.Forms
{
    using System.Drawing;

    public class ToolStripSeparator : ToolStripItem
    {
        protected readonly Pen borderColor = new Pen(defaultSeparatorBorderColor);
        protected readonly Pen borderColor2 = new Pen(Color.White);

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

            if (Owner == null)
                return;

            var g = e.Graphics;
            var bounds = Bounds;
            
            if (Owner.Orientation == Orientation.Vertical)
            {
                var ex = bounds.X;
                var ey = bounds.Y + Height / 2 - 1;
                var ex2 = bounds.X + bounds.Width - 8;
                var ey2 = ey + 1;
                
                ex += Owner.Padding.Left;

                g.DrawLine(borderColor, ex, ey, ex2, ey);
                g.DrawLine(borderColor2, ex, ey2, ex2, ey2);
            }
            else
            {
                var ex = bounds.X + Width / 2 -1;
                var ey = Owner.Padding.Top;
                var ex2 = ex + 1;
                var ey2 = Owner.Height - Owner.Padding.Vertical + 1;
                
                g.DrawLine(borderColor, ex, ey, ex, ey2);
                g.DrawLine(borderColor2, ex2, ey, ex2, ey2);
            }
        }

        protected internal override void SetBounds(Rectangle nbounds)
        {
            //UnityEngine.Debug.Log(nbounds.Width);
            
            base.SetBounds(nbounds);
        }
    }
}