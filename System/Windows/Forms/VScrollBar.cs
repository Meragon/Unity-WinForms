using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public class VScrollBar : ScrollBar
    {
        private readonly Size ButtonSize = new Size(15, 17);

        public VScrollBar()
        {
            scrollOrientation = ScrollOrientation.VerticalScroll;
            Size = new Size(15, 80);

            subtractButton.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            subtractButton.Image = uwfAppOwner.Resources.CurvedArrowUp;
            subtractButton.Location = new Point(0, 0);
            subtractButton.Size = ButtonSize;

            addButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            addButton.Image = uwfAppOwner.Resources.CurvedArrowDown;
            addButton.Location = new Point(0, Height - ButtonSize.Height);
            addButton.Size = ButtonSize;

            Refresh();
        }
    }
}
