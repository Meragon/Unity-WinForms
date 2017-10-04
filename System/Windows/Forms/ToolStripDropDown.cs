namespace System.Windows.Forms
{
    using System.Drawing;

    public class ToolStripDropDown : ToolStrip
    {
        private static readonly Color defaultToolStripDropDownHoverColor = Color.FromArgb(160, 210, 222, 245);

        public ToolStripDropDown()
        {
            Orientation = Orientation.Vertical;
            Visible = false;
        }

        public void Show()
        {
            Location = new Point();
            ShowCore();
        }
        public void Show(Control control, Point position)
        {
            Location = control == null ? position : control.PointToScreen(Point.Empty).Add(position);

            ShowCore();
        }
        public void Show(Control control, int x, int y)
        {
            Show(control, new Point(x, y));
        }
        public void Show(Point screenLocation)
        {
            Location = screenLocation;
            ShowCore();
        }
        public void Show(int x, int y)
        {
            Location = new Point(x, y);
            ShowCore();
        }

        private void ShowCore()
        {
            int height = 0;
            int width = 160;

            var itemsCount = Items.Count;
            if (itemsCount == 0)
                return;

            var workingArea = Screen.PrimaryScreen.WorkingArea;

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

            if (Location.X + width > workingArea.Width)
                Location = new Point(workingArea.Width - width, Location.Y);

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

                item.ForeColor = defaultForeColor;
                item.HoverColor = defaultToolStripDropDownHoverColor;
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
