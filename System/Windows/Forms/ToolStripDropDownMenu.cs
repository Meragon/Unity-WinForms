namespace System.Windows.Forms
{
    using System.Drawing;

    public class ToolStripDropDownMenu : ToolStripDropDown
    {
        ////private static Padding arrowPadding = new Padding(0, 0, 8, 0);
        ////private static Padding checkPadding = new Padding(5, 2, 2, 2);
        ////private static Padding imagePadding = new Padding(2);
        private static Padding textPadding = new Padding(8, 1, 9, 1);

        ////private Rectangle arrowRectangle = Rectangle.Empty;
        ////private Rectangle checkRectangle = Rectangle.Empty;
        private Rectangle imageMarginBounds = Rectangle.Empty;
        ////private Rectangle imageRectangle = Rectangle.Empty;
        ////private Rectangle textRectangle = Rectangle.Empty;

        public ToolStripDropDownMenu()
        {
            ShowImageMargin = true;

            uwfContext = true;
            uwfShadowBox = true;
        }

        public bool ShowCheckMargin { get; set; } // TODO: here.
        public bool ShowImageMargin { get; set; } // TOOD: here.

        internal Rectangle ImageMargin
        {
            get
            {
                imageMarginBounds.Height = this.Height;
                return imageMarginBounds;
            }
        }

        protected override Padding DefaultPadding
        {
            get
            {
                var tLeft = textPadding.Left;
                var padding = ShowCheckMargin || ShowImageMargin ? tLeft + ImageMargin.Width : tLeft;
                return new Padding(padding, 2, 1, 2);
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            CalculateInternalLayoutMetrics();
        }

        private void CalculateInternalLayoutMetrics()
        {
            imageMarginBounds = new Rectangle(0, 0, 25, Height);

            Padding = DefaultPadding;
        }
    }
}
