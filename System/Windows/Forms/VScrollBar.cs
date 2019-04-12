namespace System.Windows.Forms
{
    using System.Drawing;

    public class VScrollBar : ScrollBar
    {
        public VScrollBar()
        {
            scrollOrientation = ScrollOrientation.VerticalScroll;

            subtractButton.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            subtractButton.Image = uwfAppOwner.Resources.CurvedArrowUp;
            subtractButton.Size = new Size(SystemInformation.VerticalScrollBarWidth, SystemInformation.VerticalScrollBarArrowHeight);
            subtractButton.Location = new Point(0, 0);

            addButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            addButton.Image = uwfAppOwner.Resources.CurvedArrowDown;
            addButton.Size = new Size(SystemInformation.VerticalScrollBarWidth, SystemInformation.VerticalScrollBarArrowHeight);
            addButton.Location = new Point(0, Height - addButton.Height);

            UpdateScrollRect();
        }

        protected override Size DefaultSize
        {
            get { return new Size(SystemInformation.VerticalScrollBarWidth, 80); }
        }
    }
}
