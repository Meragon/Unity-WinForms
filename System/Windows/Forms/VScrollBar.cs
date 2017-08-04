namespace System.Windows.Forms
{
    using System.Drawing;

    public class VScrollBar : ScrollBar
    {
        private readonly Size buttonSize = new Size(15, 17);

        public VScrollBar()
        {
            scrollOrientation = ScrollOrientation.VerticalScroll;

            subtractButton.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            subtractButton.Image = uwfAppOwner.Resources.CurvedArrowUp;
            subtractButton.Location = new Point(0, 0);
            subtractButton.Size = buttonSize;

            addButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            addButton.Image = uwfAppOwner.Resources.CurvedArrowDown;
            addButton.Location = new Point(0, Height - buttonSize.Height);
            addButton.Size = buttonSize;

            UpdateScrollRect();
        }

        protected override Size DefaultSize
        {
            get { return new Size(15, 80); }
        }
    }
}
