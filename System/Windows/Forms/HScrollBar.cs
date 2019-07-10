namespace System.Windows.Forms
{
    using System.Drawing;

    public class HScrollBar : ScrollBar
    {
        public HScrollBar()
        {
            scrollOrientation = ScrollOrientation.HorizontalScroll;

            subtractButton.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
            subtractButton.Image = uwfAppOwner.Resources.CurvedArrowLeft;
            subtractButton.Size = new Size(SystemInformation.HorizontalScrollBarArrowWidth, SystemInformation.HorizontalScrollBarHeight);
            subtractButton.Location = new Point(0, 0);

            addButton.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            addButton.Image = uwfAppOwner.Resources.CurvedArrowRight;
            addButton.Size = new Size(SystemInformation.HorizontalScrollBarArrowWidth, SystemInformation.HorizontalScrollBarHeight);
            addButton.Location = new Point(Width - addButton.Width, 0);

            UpdateScrollRect();
        }

        protected override Size DefaultSize
        {
            get { return new Size(80, SystemInformation.HorizontalScrollBarHeight); }
        }
    }
}
