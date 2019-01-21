namespace System.Windows.Forms
{
    using System.Drawing;

    public class ToolStripDropDownItem : ToolStripItem
    {
        private readonly ToolStripItemCollection dropDownItems;
        private ToolStripDropDown dropDownToolStrip;
        private float mouseHoverTime;
        private bool pressed;

        protected ToolStripDropDownItem()
        {
            dropDownItems = new ToolStripItemCollection(Parent, null);

            ArrowColor = Color.Black;
            ArrowImage = ApplicationResources.Images.DropDownRightArrow;
        }

        internal delegate void uwfDropDownHandler(ToolStripDropDown dropDown);
        internal event uwfDropDownHandler uwfDropDownCreated; // Workaround to not implement ToolStripRenderer.
        
        public ToolStripDropDown DropDown
        {
            get { return dropDownToolStrip; }
        }
        public ToolStripItemCollection DropDownItems { get { return dropDownItems; } }
        public override bool Pressed
        {
            get
            {
                return pressed;
            }
        }

        internal Color ArrowColor { get; set; }
        internal Bitmap ArrowImage { get; set; }

        internal void CloseEverything()
        {
            // Self
            var parent = Owner as ToolStripDropDown;
            if (parent != null)
            {
                parent.Dispose();

                var parentItem = parent.OwnerItem as ToolStripDropDownItem;
                if (parentItem != null)
                    parentItem.CloseEverything();
            }
        }
        internal void CloseToolStrip()
        {
            if (dropDownToolStrip == null)
                return;

            if (dropDownToolStrip.IsDisposed == false)
                dropDownToolStrip.Dispose();
        }
        internal void CreateToolStrip()
        {
            if (Enabled == false)
                return;

            if (dropDownItems.Count == 0)
                return;

            var dropDownBounds = new Rectangle(Point.Empty, Owner.Size);

            for (int i = 0; i < dropDownItems.Count; i++) // Reset items.
            {
                var item = dropDownItems[i];
                item.Unselect();

                var dropDownItem = item as ToolStripDropDownItem;
                if (dropDownItem == null) continue;

                dropDownItem.ArrowImage = ApplicationResources.Images.DropDownRightArrow;
                dropDownItem.ArrowColor = Color.Black;
            }

            var direction = ToolStripDropDownDirection.Right;
            if (Owner.IsDropDown == false)
                direction = ToolStripDropDownDirection.BelowRight;

            dropDownToolStrip = new ToolStripDropDownMenu();
            dropDownToolStrip.OwnerItem = this;
            dropDownToolStrip.Items.AddRange(dropDownItems);
            dropDownToolStrip.selectedItem = dropDownToolStrip.SelectNextToolStripItem(null, true);
            dropDownToolStrip.shouldFixWidth = true;
            dropDownToolStrip.direction = direction;
            dropDownToolStrip.Location = DropDownDirectionToDropDownBounds(direction, dropDownBounds).Location;
            dropDownToolStrip.Disposed += DropDownToolStrip_Disposed;
            ((ToolStripDropDownMenu)dropDownToolStrip).Show(dropDownToolStrip.Location);

            if (uwfDropDownCreated != null)
                uwfDropDownCreated(dropDownToolStrip);

            pressed = true;
        }
        internal void ResetToolStrip()
        {
            if (dropDownToolStrip == null)
                return;

            var clientRect = new Rectangle(dropDownToolStrip.Location, dropDownToolStrip.Size);
            var contains = clientRect.Contains(Owner.PointToClient(Control.MousePosition));
            if (!contains)
                pressed = false;
            else
                pressed = !pressed;

            dropDownToolStrip.Disposed -= DropDownToolStrip_Disposed;
            dropDownToolStrip = null;

            //Owner.Focus();
        }
        internal override void Unselect()
        {
            base.Unselect();

            if (dropDownToolStrip == null)
                return;

            CloseToolStrip();
        }

        protected internal override void DrawImage(Graphics graphics)
        {
            var image = Image;
            if (image != null)
            {
                var rect = Bounds;
                graphics.uwfDrawImage(
                    image,
                    uwfImageColor,
                    rect.X + 2,
                    rect.Y + 2,
                    rect.Height - 4,
                    rect.Height - 4);
            }
        }

        protected override void OnClick(EventArgs e)
        {
            if (Enabled == false)
                return;

            base.OnClick(e);

            if (DropDownItems.Count == 0)
            {
                CloseEverything();
                return;
            }

            if (dropDownToolStrip != null)
                return;

            if (!pressed)
                CreateToolStrip();
            else
                pressed = false;
        }
        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);

            if (Owner == null || Owner.IsDropDown == false)
                return;

            // Create toolstrip if hovering item with mouse.
            if (dropDownToolStrip != null)
            {
                mouseHoverTime = 0f;
                return;
            }

            mouseHoverTime += swfHelper.GetDeltaTime();
            if (mouseHoverTime > .5f)
                CreateToolStrip();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (dropDownItems.Count > 0 && Owner != null && Owner.Orientation == Orientation.Vertical)
            {
                var bounds = Bounds;
                e.Graphics.uwfDrawImage(
                    ArrowImage,
                    ArrowColor,
                    bounds.X + bounds.Width - 25,
                    bounds.Y + (bounds.Height - ArrowImage.Height) / 2,
                    ArrowImage.Width,
                    ArrowImage.Height);
            }
        }

        private Rectangle DropDownDirectionToDropDownBounds(ToolStripDropDownDirection dropDownDirection, Rectangle dropDownBounds)
        {
            var offset = Point.Empty;

            switch (dropDownDirection)
            {
                case ToolStripDropDownDirection.AboveLeft:
                    offset.X = -dropDownBounds.Width + this.Width;
                    offset.Y = -dropDownBounds.Height + 1;
                    break;
                case ToolStripDropDownDirection.AboveRight:
                    offset.Y = -dropDownBounds.Height + 1;
                    break;
                case ToolStripDropDownDirection.BelowRight:
                    offset.Y = this.Height - 2;
                    break;
                case ToolStripDropDownDirection.BelowLeft:
                    offset.X = -dropDownBounds.Width + this.Width;
                    offset.Y = this.Height - 1;
                    break;
                case ToolStripDropDownDirection.Right:
                    offset.X = this.Width;
                    if (!IsOnDropDown)
                        offset.X -= 1;
                    else
                    {
                        offset.X -= 6;
                        offset.Y -= 2;
                    }
                    break;

                case ToolStripDropDownDirection.Left:
                    offset.X = -dropDownBounds.Width;
                    break;
            }

            var itemScreenLocation = Owner.PointToScreen(Point.Empty);
            itemScreenLocation.Offset(Bounds.Location);
            dropDownBounds.Location = new Point(itemScreenLocation.X + offset.X, itemScreenLocation.Y + offset.Y);
            return dropDownBounds;
        }
        private void DropDownToolStrip_Disposed(object sender, EventArgs e)
        {
            ResetToolStrip();
        }
    }
}