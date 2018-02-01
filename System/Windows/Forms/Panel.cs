namespace System.Windows.Forms
{
    using System.Drawing;

    public class Panel : ScrollableControl
    {
        public Panel()
        {
            SetStyle(ControlStyles.Selectable | ControlStyles.AllPaintingInWmPaint, false);
        }

        public virtual BorderStyle BorderStyle { get; set; }

        protected override Size DefaultSize
        {
            get { return new Size(200, 100); }
        }

        protected internal override void uwfOnLatePaint(PaintEventArgs e)
        {
            base.uwfOnLatePaint(e);

            ControlPaint.PrintBorder(e.Graphics, ClientRectangle, BorderStyle, Border3DStyle.Sunken);
        }
    }
}
