namespace System.Windows.Forms
{
    using System.Drawing;

    public class ToolStripDropDown : ToolStrip
    {
        public ToolStripDropDown()
        {
            Orientation = Orientation.Vertical;
            Visible = false;
        }

        /// <summary>
        /// use null, MousePosition
        /// </summary>
        /// <param name="control"></param>
        /// <param name="position"></param>
        public void Show(Control control, Point position)
        {
            if (control == null)
                Location = position;
            else
                Location = control.PointToScreen(Point.Empty).Add(position);

            int height = 0;
            int width = 160;

            var itemsCount = Items.Count;
            for (int i = 0; i < itemsCount; i++)
            {
                var item = Items[i];
                if (item.JustVisual) continue;

                var menuItem = item as ToolStripMenuItem;
                if (menuItem != null && !string.IsNullOrEmpty(menuItem.ShortcutKeys))
                    width = 220;
                height += 24;
            }
            Size = new Size(width, height);

            if (Location.X + width > Screen.PrimaryScreen.WorkingArea.Width)
                Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - width, Location.Y);

            Visible = true;

            for (int i = 0; i < itemsCount; i++)
            {
                var item = Items[i];
                var dropDownItem = item as ToolStripDropDownItem;
                if (dropDownItem != null)
                {
                    dropDownItem.ArrowImage = uwfAppOwner.Resources.DropDownRightArrow;
                    dropDownItem.ArrowColor = Color.Black;
                }

                item.ForeColor = Color.FromArgb(64, 64, 64);
                item.HoverColor = Color.FromArgb(160, 210, 222, 245);
                item.TextAlign = ContentAlignment.MiddleLeft;
                switch (Orientation)
                {
                    case Orientation.Horizontal:

                        break;
                    case Orientation.Vertical:
                        item.Size = new Size(Size.Width, 24);
                        break;
                }
            }
        }
    }
}
