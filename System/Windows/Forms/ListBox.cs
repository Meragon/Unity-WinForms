namespace System.Windows.Forms
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;

    public class ListBox : ListControl
    {
        public const int DefaultItemHeight = 13;

        private readonly Pen borderPen = new Pen(Color.Black);
        private readonly ObjectCollection items;
        private readonly VScrollBar vScroll;

        private int borderOffset = 2;
        private BorderStyle borderStyle = BorderStyle.Fixed3D;
        private int hoveredItem = -1;
        private bool integralHeight = true;
        private bool integralHeightAdjust = true;
        private int itemHeight = DefaultItemHeight;
        private string keyFilter = "";
        private Timer keyTimer;
        private int keyFilterResetTime = 3; // seconds.
        private int visibleItemsCount = 0;
        private bool scrollAlwaysVisible;
        private int selectedIndex = -1;

        public ListBox()
        {
            items = new ObjectCollection(this);

            BackColor = Color.White;
            DrawMode = DrawMode.Normal;
            DrawItem = InternalDrawItem;
            uwfWrapText = true;

            uwfItemDisabledColor = Color.Gray;
            uwfItemHoverColor = Color.FromArgb(221, 238, 253);
            uwfSelectionBackColor = SystemColors.Highlight;
            uwfSelectionDisabledColor = Color.FromArgb(101, 203, 255);
            uwfSelectionForeColor = SystemColors.HighlightText;
            
            vScroll = new VScrollBar();
            vScroll.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            vScroll.Location = new Point(Width - vScroll.Width, 0);
            vScroll.Height = Height;
            vScroll.Visible = false;
            Controls.Add(vScroll);

            UpdateBorder();
        }

        public event EventHandler SelectedIndexChanged;
        public event DrawItemEventHandler DrawItem;

        public BorderStyle BorderStyle
        {
            get { return borderStyle; }
            set
            {
                borderStyle = value;

                UpdateBorder();
            }
        }
        public virtual DrawMode DrawMode { get; set; }
        [Obsolete("Use with caution, 'Anchor' property can resize this control unexpectedly")]
        public bool IntegralHeight
        {
            get { return integralHeight; }
            set
            {
                integralHeight = value;
                if (integralHeight)
                {
                    integralHeightAdjust = true;
                    AdjustHeight();
                }
            }
        }
        public virtual int ItemHeight
        {
            get { return itemHeight; }
            set
            {
                itemHeight = value;
                RefreshItems();
            }
        }
        public ObjectCollection Items { get { return items; } }
        public int PreferredHeight
        {
            get
            {
                int height = 0;
                var itemsCount = Items.Count;
                if (itemsCount == 0) itemsCount = 1;
                height = ItemHeight * itemsCount + borderOffset * 2;
                return height;
            }
        }
        public bool ScrollAlwaysVisible
        {
            get { return scrollAlwaysVisible; }
            set
            {
                scrollAlwaysVisible = value;
                RefreshItems();
            }
        }
        public override int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                if (value < -1 || value >= items.Count) return;

                bool changed = selectedIndex != value && value != -1;
                selectedIndex = value;
                if (changed) OnSelectedIndexChanged(EventArgs.Empty);
            }
        }
        public object SelectedItem
        {
            get
            {
                if (SelectedIndex == -1 || SelectedIndex >= Items.Count) 
                    return null;
                
                return Items[SelectedIndex];
            }
            set
            {
                for (int i = 0; i < Items.Count; i++)
                    if (value == Items[i])
                    {
                        SelectItem(i);
                        break;
                    }
            }
        }
        
        internal Color uwfItemDisabledColor { get; set; }
        internal Color uwfItemHoverColor { get; set; }
        internal Color uwfSelectionBackColor { get; set; }
        internal Color uwfSelectionDisabledColor { get; set; }
        internal Color uwfSelectionForeColor { get; set; }
        internal int uwfScrollIndex
        {
            get { return vScroll.Value / ItemHeight; }
            set { vScroll.Value = value * ItemHeight; }
        }
        internal bool uwfWrapText { get; set; }
        
        protected override Size DefaultSize
        {
            get { return new Size(120, 96); }
        }

        internal void EnsureVisible()
        {
            if (SelectedIndex < uwfScrollIndex)
                uwfScrollIndex = SelectedIndex;

            if (SelectedIndex > uwfScrollIndex + visibleItemsCount - 1)
                uwfScrollIndex = SelectedIndex - visibleItemsCount + 1;

            if (uwfScrollIndex < 0)
                uwfScrollIndex = 0;
        }
        internal int FindItemIndex(Predicate<object> match)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                if (match(item)) return i;
            }
            return -1;
        }
        internal int IndexAt(Point mclient)
        {
            return uwfScrollIndex + (int)((mclient.Y - borderOffset) / ItemHeight);
        }
        internal void SelectItem(int index)
        {
            if (index < 0 && Items.Count == 0) return;

            if (index < 0) index = 0;
            if (index >= Items.Count) index = Items.Count - 1;

            SelectedIndex = index;
            EnsureVisible();
            OnSelectedValueChanged(EventArgs.Empty);
        }
        internal void RefreshItems()
        {
            visibleItemsCount = (int)Math.Ceiling((float)(Height - borderOffset * 2) / ItemHeight);

            if (vScroll != null)
            {
                vScroll.Maximum = Items.Count * ItemHeight;
                vScroll.SmallChange = ItemHeight;
                vScroll.LargeChange = vScroll.Height;
                vScroll.Visible = ScrollAlwaysVisible || visibleItemsCount < Items.Count;
            }
        }
        internal void UpdateBorder()
        {
            switch (borderStyle)
            {
                case BorderStyle.None:
                    borderOffset = 0;
                    borderPen.Width = 0;
                    vScroll.Location = new Point(Width - vScroll.Width, 0);
                    vScroll.Height = Height;
                    break;
                case BorderStyle.FixedSingle:
                case BorderStyle.Fixed3D:
                    borderOffset = 2;
                    borderPen.Width = 1;
                    vScroll.Location = new Point(Width - vScroll.Width - borderOffset, borderOffset);
                    vScroll.Height = Height - borderOffset * 2;
                    break;
            }
        }
        
        protected internal override void uwfOnLatePaint(PaintEventArgs e)
        {
            base.uwfOnLatePaint(e);

            ControlPaint.PrintBorder(e.Graphics, ClientRectangle, BorderStyle, Border3DStyle.Flat);
        }

        protected virtual void OnDrawItem(DrawItemEventArgs e)
        {
            if (DrawItem != null)
                DrawItem(this, e);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            
            switch (e.KeyCode)
            {
                case Keys.Down:
                case Keys.Right:
                    SelectItem(SelectedIndex + 1);
                    break;
                case Keys.Left:
                case Keys.Up:
                    SelectItem(SelectedIndex - 1);
                    break;

                case Keys.PageDown:
                    SelectItem(SelectedIndex + visibleItemsCount - 1);
                    break;
                case Keys.PageUp:
                    SelectItem(SelectedIndex - visibleItemsCount + 1);
                    break;

                case Keys.Home:
                    SelectItem(0);
                    break;
                case Keys.End:
                    SelectItem(Items.Count - 1);
                    break;

                default:

                    // Key filter.
                    char c = KeyHelper.GetLastInputChar();
                    if (char.IsLetterOrDigit(c) || char.IsPunctuation(c))
                    {
                        keyFilter += char.ToLower(c);
                        var itemIndex = FindItemIndex(x =>
                        {
                            if (x == null)
                                return false;

                            var xStr = x.ToString();
                            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                            return xStr != null && xStr.ToLower().StartsWith(keyFilter);
                        });
                        SelectItem(itemIndex);

                        if (keyTimer == null)
                        {
                            keyTimer = new Timer();
                            keyTimer.Interval = keyFilterResetTime * 1000;
                            keyTimer.Tick += (sender, args) => ResetKeyFilter();
                        }
                        
                        keyTimer.Stop();
                        keyTimer.Start();
                    }
                    break;
            }
        }
        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);
            var mclient = PointToClient(MousePosition);
            var itemIndex = IndexAt(mclient);

            hoveredItem = itemIndex;
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            hoveredItem = -1;
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (e.Delta < 0)
                vScroll.DoScroll(ScrollEventType.SmallIncrement);
            else
                vScroll.DoScroll(ScrollEventType.SmallDecrement);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            var mclient = PointToClient(MousePosition);
            var itemIndex = IndexAt(mclient);

            SelectItem(itemIndex);

            base.OnMouseUp(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;

            const int itemTextVerticalPadding = -3;

            // Paint list.
            g.uwfFillRectangle(BackColor, 0, 0, Width, Height);
            
            for (int i = 0; i < visibleItemsCount && i + uwfScrollIndex < Items.Count; i++)
            {
                var itemIndex = i + uwfScrollIndex;
                if (itemIndex < 0) continue;

                var item = Items[itemIndex];
                var itemY = borderOffset + i * ItemHeight;
                var itemW = uwfWrapText ? Width : Width * 5;
                var itemSelected = itemIndex == SelectedIndex;
                var itemHovered = itemIndex == hoveredItem;
                var itemForeColor = ForeColor;

                if (itemSelected)
                    itemForeColor = uwfSelectionForeColor;

                var itemBackColor = Color.Transparent;
                if (itemSelected || itemHovered)
                {
                    itemBackColor = uwfItemHoverColor;
                    if (itemSelected)
                        itemBackColor = uwfSelectionBackColor;
                }

                var fillWidth = Width;
                if (vScroll.Visible)
                    fillWidth = vScroll.Location.X - borderOffset;

                var itemText = "";
                if (item != null)
                    itemText = item.ToString();

                if (DrawMode == DrawMode.Normal)
                {
                    g.uwfFillRectangle(itemBackColor, borderOffset, itemY, fillWidth, ItemHeight);
                    g.uwfDrawString(
                        itemText,
                        Font,
                        itemForeColor,
                        borderOffset + 2,
                        itemY + itemTextVerticalPadding,
                        itemW,
                        ItemHeight - itemTextVerticalPadding * 2,
                        ContentAlignment.MiddleLeft);
                }
                else
                {
                    var itemRect = new Rectangle(
                        borderOffset + 2,
                        itemY,
                        fillWidth,
                        ItemHeight);
                    
                    var state = itemSelected ? DrawItemState.Selected : DrawItemState.Default;
                    var args = new DrawItemEventArgs(g, Font, itemRect, itemIndex, state, itemForeColor, itemBackColor);
                    
                    OnDrawItem(args);
                }
            }
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (IntegralHeight && integralHeightAdjust)
                AdjustHeight();

            RefreshItems();
        }
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            var selectedIndexChanged = SelectedIndexChanged;
            if (selectedIndexChanged != null)
                selectedIndexChanged(this, e);
        }

        private void AdjustHeight()
        {
            // TODO: precise losing in long term.
            integralHeightAdjust = false;
            Height = (int)(Math.Ceiling((float)Height / ItemHeight) * ItemHeight) + borderOffset * 2;
            integralHeightAdjust = true;
        }
        private void InternalDrawItem(object sender, DrawItemEventArgs e)
        {
            var item = Items[e.Index];
            var itemText = "";
            if (item != null)
                itemText = item.ToString();

            e.DrawBackground();
            e.Graphics.uwfDrawString(itemText, e.Font, e.ForeColor, e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height, ContentAlignment.MiddleLeft);
            e.DrawFocusRectangle();
        }
        private void ResetKeyFilter()
        {
            keyFilter = "";
            keyTimer.Dispose();
            keyTimer = null;
        }

        public class ObjectCollection : IList
        {
            private readonly List<object> items = new List<object>();
            private readonly ListBox owner;

            public ObjectCollection(ListBox owner)
            {
                this.owner = owner;
            }
            public ObjectCollection(ListBox owner, ObjectCollection value)
            {
                this.owner = owner;
                AddRange(value);
            }
            public ObjectCollection(ListBox owner, object[] value)
            {
                this.owner = owner;
                AddRange(value);
            }

            public int Count { get { return items.Count; } }
            public bool IsFixedSize
            {
                get { return false; }
            }
            public bool IsReadOnly { get { return false; } }
            public bool IsSynchronized
            {
                get { return false; }
            }
            public object SyncRoot
            {
                get { return this; }
            }

            public virtual object this[int index]
            {
                get { return items[index]; }
                set { items[index] = value; }
            }

            public int Add(object item)
            {
                var index = AddInternal(item);
                return index;
            }
            public void AddRange(ObjectCollection value)
            {
                for (int i = 0; i < value.Count; i++)
                    AddInternal(value[i]);
            }
            public void AddRange(object[] array)
            {
                for (int i = 0; i < array.Length; i++)
                    AddInternal(array[i]);
            }
            public void Clear()
            {
                items.Clear();
                owner.uwfScrollIndex = 0;
                owner.selectedIndex = -1;
            }
            public bool Contains(object value)
            {
                return items.Contains(value);
            }
            public void CopyTo(Array array, int index)
            {
                items.CopyTo((object[])array, index);
            }
            public void CopyTo(object[] destination, int arrayIndex)
            {
                items.CopyTo(destination, arrayIndex);
            }
            public IEnumerator GetEnumerator()
            {
                return items.GetEnumerator();
            }
            public int IndexOf(object value)
            {
                return items.IndexOf(value);
            }
            public void Insert(int index, object value)
            {
                items.Insert(index, value);
            }
            public void Remove(object value)
            {
                items.Remove(value);
                owner.RefreshItems();
            }
            public void RemoveAt(int index)
            {
                items.RemoveAt(index);
                owner.RefreshItems();
            }

            private int AddInternal(object item)
            {
                items.Add(item);
                owner.RefreshItems();

                return items.Count - 1;
            }
        }
    }
}
