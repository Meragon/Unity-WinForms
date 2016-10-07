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
            this.scrollOrientation = ScrollOrientation.VerticalScroll;
            this.Size = new Size(15, 80);

            this.subtractButton.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.subtractButton.Image = ApplicationBehaviour.Resources.Images.CurvedArrowUp;
            this.subtractButton.Location = new Point(0, 0);
            this.subtractButton.Size = ButtonSize;

            this.addButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.addButton.Image = ApplicationBehaviour.Resources.Images.CurvedArrowDown;
            this.addButton.Location = new Point(0, Height - ButtonSize.Height);
            this.addButton.Size = ButtonSize;

            this.Refresh();
        }
    }
}
