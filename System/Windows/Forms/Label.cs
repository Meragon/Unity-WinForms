namespace System.Windows.Forms
{
    using System.Drawing;

    public class Label : Control
    {
        private bool adjustSize;

        public Label()
        {
            base.AutoSize = true;
            BackColor = Color.Transparent;
            TabStop = false;
            TextAlign = ContentAlignment.TopLeft;

            SetStyle(ControlStyles.FixedHeight | ControlStyles.Selectable, false);
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        public override bool AutoSize
        {
            get
            {
                return base.AutoSize;
            }
            set
            {
                if (AutoSize == value)
                    return;

                base.AutoSize = value;
                AdjustSize();
            }
        }
        public virtual BorderStyle BorderStyle { get; set; }
        public ContentAlignment TextAlign { get; set; }

        protected override Padding DefaultPadding
        {
            get { return new Padding(4, 0, 4, 0); }
        }
        protected override Size DefaultSize
        {
            get { return new Size(100, 23); } // Autosize ? Preferred height.
        }

        internal void AdjustSize()
        {
            adjustSize = AutoSize;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            var p = Padding;
            var w = Width;
            var h = Height;

            if (adjustSize)
            {
                var size = g.MeasureString(Text, Font);
                Size = new Size((int)size.Width + p.Horizontal, (int)size.Height + p.Vertical);
                adjustSize = false;
            }

            g.uwfFillRectangle(BackColor, 0, 0, w, h);
            g.uwfDrawString(Text, Font, ForeColor, p.Left, p.Top, w - p.Horizontal, h - p.Vertical, TextAlign);

            ControlPaint.PrintBorder(g, ClientRectangle, BorderStyle, Border3DStyle.SunkenOuter);
        }
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            AdjustSize();
        }
    }
}
