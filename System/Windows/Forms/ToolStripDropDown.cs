namespace System.Windows.Forms
{
    using System.Drawing;

    public class ToolStripDropDown : ToolStrip
    {
        internal ToolStripDropDownDirection direction;
        internal readonly Pen borderPen = new Pen(Color.FromArgb(128, 128, 128));
        internal Color uwfColumnBackColor = SystemColors.Control; // ?
        
        private ToolStripItem ownerItem;

        public ToolStripDropDown()
        {
            Orientation = Orientation.Vertical;
            ShowItemToolTips = false; // I'm not really sure.
            Visible = false;
        }

        public ToolStripItem OwnerItem { get; set; }

        public new void Show()
        {
            Location = new Point();
            SetVisibleCore();
        }
        public void Show(Control control, Point position)
        {
            Location = control == null ? position : control.PointToScreen(Point.Empty).Add(position);

            SetVisibleCore();
        }
        public void Show(Control control, int x, int y)
        {
            Show(control, new Point(x, y));
        }
        public void Show(Point screenLocation)
        {
            Location = screenLocation;
            SetVisibleCore();
        }
        public void Show(int x, int y)
        {
            Location = new Point(x, y);
            SetVisibleCore();
        }

        protected override void Dispose(bool release_all)
        {
            var disposeIsBlocked = false; // By something unnatural.

            for (int i = 0; i < Items.Count; i++)
            {
                var dropDownItem = Items[i] as ToolStripDropDownItem;
                if (dropDownItem == null)
                    continue;

                if (dropDownItem.DropDown == null || 
                    dropDownItem.DropDown.IsDisposed) continue;

                disposeIsBlocked = true; // We can't dispose parent before child.
                break;
            }

            if (disposeIsBlocked)
                return;

            base.Dispose(release_all);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.KeyCode)
            {
                case Keys.Escape:
                case Keys.Left:
                    {
                        if (selectedItem != null)
                        {
                            var ownerDropDownItem = selectedItem.OwnerItem as ToolStripDropDownItem;
                            if (ownerDropDownItem != null)
                                ownerDropDownItem.CloseToolStrip();
                        }
                    }
                    break;
                case Keys.Right:
                    {
                        var dropDownItem = selectedItem as ToolStripDropDownItem;
                        if (dropDownItem != null)
                            dropDownItem.RaiseClick();
                    }
                    break;
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            var graphics = e.Graphics;

            graphics.uwfFillRectangle(uwfColumnBackColor, 2, 2, 22, Height - 4);

            base.OnPaint(e);

            graphics.DrawRectangle(borderPen, 0, 0, Width, Height);
        }
        protected override void SetBoundsCore(int argX, int argY, int argWidth, int argHeight, BoundsSpecified specified)
        {
            // Fix location
            var workingArea = Screen.PrimaryScreen.WorkingArea;
            var newBounds = WindowsFormsUtils.ConstrainToBounds(workingArea, new Rectangle(argX, argY, argWidth, argHeight));
            ////var xFixed = argX != newBounds.X;

            // TODO: need some code to prevent child going outside of screen.
            /*if (OwnerItem != null)
            {
                var parentDropDown = OwnerItem.Owner as ToolStripDropDown;
                if (parentDropDown != null && direction == ToolStripDropDownDirection.Right)
                {
                    // TODO: need to add other direction. Should be ToolStripDropDownItem, but the problem is that 
                    // it's invoked after Graphics.MeasureString, which can only be done on paint thread.
                    if (xFixed && newBounds.X + newBounds.Width - 2 >= parentDropDown.Location.X)
                        newBounds = new Rectangle(parentDropDown.Location.X - argWidth + 2, newBounds.Y, newBounds.Width, newBounds.Height);
                }
            }*/

            base.SetBoundsCore(newBounds.X, newBounds.Y, newBounds.Width, newBounds.Height, specified);
        }
        protected void SetVisibleCore()
        {
            var itemsCount = Items.Count;
            if (itemsCount == 0)
                return;

            Visible = true;

            for (int i = 0; i < itemsCount; i++)
            {
                var item = Items[i];
                item.TextAlign = ContentAlignment.MiddleLeft; // I think it's impossible to use user text align consider to WinForms.
            }

            Focus();
        }

        private bool ContainsMouse()
        {


            return false;
        }
    }
}
