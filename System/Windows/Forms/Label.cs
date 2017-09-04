namespace System.Windows.Forms
{
    using System.Drawing;

    public class Label : Control
    {
        internal Color foreColor;

        private static readonly Pen borderSinglePen = new Pen(SystemColors.WindowFrame);
        private static readonly Pen borderFixed3dBottomPen = new Pen(SystemColors.Window);
        private static readonly Pen borderFixed3dTopPen = new Pen(SystemColors.ButtonShadow);
        private static readonly Padding defaultPadding = new Padding(4, 0, 4, 0);

        private bool adjustSize;

        public Label()
        {
            foreColor = base.ForeColor;

            base.AutoSize = true;
            BackColor = Color.Transparent;
            Padding = defaultPadding;
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
        public override Color ForeColor
        {
            get { return foreColor; }
            set { foreColor = value; }
        }
        public ContentAlignment TextAlign { get; set; }

        protected override Size DefaultSize
        {
            get { return new Size(100, 23); } // Autosize ? Preferred height.
        }

        internal void AdjustSize()
        {
            if (AutoSize == false)
                return;

            adjustSize = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;

            if (adjustSize)
            {
                var size = g.MeasureString(Text, Font);
                Size = new Size((int)size.Width + Padding.Horizontal, (int)size.Height + Padding.Vertical);
                adjustSize = false;
            }

            var p = Padding;
            var w = Width;
            var h = Height;

            g.uwfFillRectangle(BackColor, 0, 0, w, h);
            g.uwfDrawString(Text, Font, foreColor, p.Left, p.Top, w - p.Horizontal, h - p.Vertical, TextAlign);

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
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            AdjustSize();
        }
    }
}
