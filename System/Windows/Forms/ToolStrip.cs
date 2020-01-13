namespace System.Windows.Forms
{
    using System.Drawing;
    using System.Linq;
    using System.Text;

    public class ToolStrip : ScrollableControl
    {
        internal ToolStripItem selectedItem;
        internal bool shouldFixWidth;

        private readonly ToolStripItemCollection items;

        private bool autoSize;
        private Orientation orientation = Orientation.Horizontal;
        private bool showItemToolTips = true;
        private ToolTip toolTip;
        private ToolStripItem toolTipItem;
        private Point toolTipNextLocation;
        private string toolTipNextText;

        public ToolStrip() : this(null)
        {
        }
        public ToolStrip(ToolStripItem[] items)
        {
            this.items = new ToolStripItemCollection(this, items);

            AutoSize = true;

            MouseHook.MouseUp += MouseHookOnMouseUp;
        }

        public event ToolStripItemClickedEventHandler ItemClicked;

        public override bool AutoSize
        {
            get { return autoSize; }
            set
            {
                autoSize = value;
                if (value)
                    UpdateSize();
            }
        }
        public virtual ToolStripItemCollection Items { get { return items; } }
        public bool IsDropDown
        {
            get { return this is ToolStripDropDown; }
        }
        public Orientation Orientation
        {
            get { return orientation; }
            internal set { orientation = value; }
        }
        public bool ShowItemToolTips
        {
            get { return showItemToolTips; }
            set
            {
                if (showItemToolTips == value)
                    return;

                showItemToolTips = value;
                if (!showItemToolTips)
                    UpdateToolTip(null);
            }
        }

        protected override Size DefaultSize
        {
            get { return new Size(100, 25); }
        }
        protected override Padding DefaultPadding
        {
            // It's look like original one have left offset, but padding is actually should be = 0, 0, 1, 0.
            get { return new Padding(1, 0, 1, 0); }
        }

        public override string ToString()
        {
            var sb = new StringBuilder(base.ToString());
            sb.Append(", Name: ");
            sb.Append(Name);
            sb.Append(", Items: ").Append(Items.Count);
            return sb.ToString();
        }

        internal ToolStripItem ItemAt(Point position)
        {
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var itemBounds = item.Bounds;
                if (itemBounds.Contains(position))
                    return item;

                // Also check child toolStrips.
                var dropDownItem = item as ToolStripDropDownItem;
                if (dropDownItem == null)
                    continue;

                var dropDown = dropDownItem.DropDown;
                if (dropDown != null && dropDown.IsDisposed == false)
                {
                    var screenPosition = PointToScreen(position);
                    var dropDownRect = new Rectangle(dropDown.Location, dropDown.Size);
                    if (dropDownRect.Contains(screenPosition))
                        return item;
                }
            }

            return null;
        }
        internal void NotifySelectionChange(ToolStripItem item)
        {
            throw new NotImplementedException();
        }
        internal ToolStripItem SelectNextToolStripItem(ToolStripItem start, bool forward)
        {
            var collection = Items;
            if (collection.Count == 0)
                return null;

            if (collection.Any(item => item.CanSelect) == false)
                return null;

            if (collection.Count == 1)
            {
                collection[0].Select();
                return collection[0];
            }

            var nextItem = start;
            if (nextItem == null)
                nextItem = forward ? collection[0] : collection[collection.Count - 1];
            else
            {
                var currentIndex = collection.IndexOf(nextItem);
                var nextIndex = forward ? currentIndex + 1 : currentIndex - 1;
                if (nextIndex < 0)
                    nextIndex = collection.Count - 1;
                if (nextIndex >= collection.Count)
                    nextIndex = 0;
                nextItem = collection[nextIndex];
            }

            if (nextItem.CanSelect == false)
                nextItem = SelectNextToolStripItem(nextItem, forward);

            if (start != null)
                start.Unselect();
            if (nextItem != null)
                nextItem.Select();

            return nextItem;
        }
        internal virtual void Show()
        {
            UpdateSize();

            for (int i = 0, x = Padding.Left, y = Padding.Top; i < items.Count; i++)
            {
                var item = items[i];
                item.SetBounds(x, y, item.Width, item.Height);

                if (Orientation == Orientation.Horizontal)
                    x += item.Width;
                if (Orientation == Orientation.Vertical)
                    y += item.Height;
            }
        }
        internal void UpdateItemsBounds()
        {
            var padding = Padding;
            for (int i = 0, x = padding.Left, y = padding.Top; i < items.Count; i++)
            {
                var item = items[i];
                item.UpdateBounds(x, y);

                if (orientation == Orientation.Vertical)
                    y += item.Height;
                else
                    x += item.Width;
            }
        }
        internal void UpdateSize()
        {
            UpdateItemsBounds();

            if (autoSize == false)
                return;

            if (orientation == Orientation.Vertical)
            {
                var padding = Padding;
                int height = padding.Top;
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    height += item.Height;
                }

                Height = height + padding.Bottom;
            }
        }
        internal void UpdateToolTip(ToolStripItem item)
        {
            if (ShowItemToolTips == false)
            {
                if (toolTip != null)
                {
                    toolTip.Dispose();
                    toolTip = null;
                }

                return;
            }

            if (item == toolTipItem)
                return;

            toolTipItem = item;

            if (item == null)
            {
                if (toolTip != null)
                    toolTip.Show(null, Location);
                return;
            }

            if (toolTip == null)
            {
                toolTip = new ToolTip();
                toolTip.SetToolTip(this, null);
            }

            var itemLocation = PointToClient(item.Bounds.Location);

            if (toolTip.alphaState > 0)
            {
                toolTipNextLocation = itemLocation;
                toolTipNextText = item.ToolTipText;
                toolTip.alphaState = 3;
            }
            else
                toolTip.Show(item.ToolTipText, itemLocation);
        }

        protected override ControlCollection CreateControlsInstance()
        {
            return new WindowsFormsUtils.ReadOnlyControlCollection(this, false);
        }
        protected override void Dispose(bool release_all)
        {
            MouseHook.MouseUp -= MouseHookOnMouseUp;

            base.Dispose(release_all);
        }
        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);

            if (selectedItem != null)
                selectedItem.RaiseOnMouseHover(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var prevHoveredItem = selectedItem;
            selectedItem = ItemAt(PointToClient(MousePosition));
            if (selectedItem != null)
                selectedItem.RaiseOnMouseHover(e);

            if (prevHoveredItem != selectedItem)
            {
                if (prevHoveredItem != null)
                    prevHoveredItem.RaiseOnMouseLeave(e);
                if (selectedItem != null)
                    selectedItem.RaiseOnMouseEnter(e);
            }

            UpdateToolTip(selectedItem);
        }
        protected override void OnMouseUp(MouseEventArgs e) // Click.
        {
            base.OnMouseUp(e);

            var item = selectedItem;
            if (item == null)
                return;

            var handler = ItemClicked;
            if (handler != null)
                handler(this, new ToolStripItemClickedEventArgs(item));

            item.RaiseOnMouseUp(e);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.KeyCode)
            {
                case Keys.Down:
                    selectedItem = SelectNextToolStripItem(selectedItem, true);
                    break;
                case Keys.Up:
                    selectedItem = SelectNextToolStripItem(selectedItem, false);
                    break;
                case Keys.Enter:
                case Keys.Space:
                    if (selectedItem != null)
                        selectedItem.RaiseClick();
                    break;
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var graphics = e.Graphics;
            var localItems = Items;
            var totalWidth = 0;
            var updateItemsBounds = false;
            var width = Width;
            var maxWidth = 68;
            var padding = Padding;

            for (int i = 0; i < localItems.Count; i++)
            {
                var item = localItems[i];
                if (item.boundsChanged) // We need to update all items bounds if any item bounds was changed.
                    updateItemsBounds = true;

                if (item.AutoSize && item.boundsChanged) // Check if item size need to be updated.
                {
                    var itemTextWidth = item.GetEstimatedWidth(graphics);
                    if (itemTextWidth > 0)
                    {
                        var itemWidth = itemTextWidth + item.Padding.Horizontal + 4;
                        if (orientation == Orientation.Horizontal)
                            item.Width = itemWidth;
                        
                        maxWidth = Math.Max(maxWidth, itemWidth);
                    }

                    item.boundsChanged = false;
                }

                totalWidth += item.Width;

                // Item's paint invoke.
                item.RaiseOnPaint(e);
            }

            if (updateItemsBounds)
                UpdateItemsBounds();

            if (AutoSize)
            {
                if (totalWidth != width)
                {
                    if (orientation == Orientation.Horizontal)
                        Width = totalWidth + padding.Horizontal;
                    else if (updateItemsBounds || shouldFixWidth) // Update for vertical.
                    {
                        shouldFixWidth = false;
                        Width = maxWidth + padding.Horizontal;
                        for (int i = 0; i < items.Count; i++)
                            items[i].Width = Width - 4;
                    }
                }
            }

            // ToolTip text smooth transition update.
            if (string.IsNullOrEmpty(toolTipNextText) == false && toolTip != null && toolTip.alphaState == 3 && ToolTip.instance == null)
            {
                toolTip.Show(toolTipNextText, toolTipNextLocation);
                toolTipNextText = null;
            }
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            UpdateItemsBounds();
        }

        private void MouseHookOnMouseUp(object sender, EventArgs e)
        {
            // TODO: reset selected items.

            /*bool reset = true;
            var toolStrip = sender as ToolStrip;
            if (toolStrip != null)
            {
                if (toolStrip.OwnerItem != null)
                {
                    var parent = toolStrip.OwnerItem.Parent;
                    while (true)
                    {
                        if (parent == null) break;
                        if (parent == this)
                        {
                            reset = false;
                            break;
                        }

                        if (parent.OwnerItem == null) break;

                        parent = parent.OwnerItem.Parent;
                    }
                }
            }

            var mc_pos = PointToClient(MousePosition);
            if (!ClientRectangle.Contains(mc_pos) && reset)
                for (int i = 0; i < items.Count; i++)
                    items[i].Selected = false;*/
        }
    }
}
