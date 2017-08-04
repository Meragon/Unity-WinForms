namespace System.Windows.Forms
{
    using System.Drawing;

    public class HScrollBar : ScrollBar
    {
        private readonly Size buttonSize = new Size(17, 15);

        public HScrollBar()
        {
            scrollOrientation = ScrollOrientation.HorizontalScroll;

            subtractButton.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
            subtractButton.Image = uwfAppOwner.Resources.CurvedArrowLeft;
            subtractButton.Location = new Point(0, 0);
            subtractButton.Size = buttonSize;

            addButton.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            addButton.Image = uwfAppOwner.Resources.CurvedArrowRight;
            addButton.Location = new Point(Width - buttonSize.Width, 0);
            addButton.Size = buttonSize;

            UpdateScrollRect();
        }

        protected override Size DefaultSize
        {
            get { return new Size(80, 15); }
        }
    }
}
