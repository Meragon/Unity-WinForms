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
            this.scrollOrientation = ScrollOrientation.HorizontalScroll;
            this.Size = new Size(80, 15);

            this.subtractButton.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
            this.subtractButton.Image = uwfAppOwner.Resources.CurvedArrowLeft;
            this.subtractButton.Location = new Point(0, 0);
            this.subtractButton.Size = ButtonSize;

            this.addButton.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            this.addButton.Image = uwfAppOwner.Resources.CurvedArrowRight;
            this.addButton.Location = new Point(Width - ButtonSize.Width, 0);
            this.addButton.Size = ButtonSize;

            this.Refresh();
        }
    }
}
