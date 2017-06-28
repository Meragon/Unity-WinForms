using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public class HScrollBar : ScrollBar
    {
        private readonly Size ButtonSize = new Size(17, 15);

        public HScrollBar()
        {
            scrollOrientation = ScrollOrientation.HorizontalScroll;
            Size = new Size(80, 15);

            subtractButton.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
            subtractButton.Image = uwfAppOwner.Resources.CurvedArrowLeft;
            subtractButton.Location = new Point(0, 0);
            subtractButton.Size = ButtonSize;

            addButton.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            addButton.Image = uwfAppOwner.Resources.CurvedArrowRight;
            addButton.Location = new Point(Width - ButtonSize.Width, 0);
            addButton.Size = ButtonSize;

            Refresh();
        }
    }
}
