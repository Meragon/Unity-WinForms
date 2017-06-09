using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Drawing;
using UnityEngine;
using Color = System.Drawing.Color;


namespace System.Windows.Forms
{
    public class ListBox : ListControl
    {
        private Color borderCurrentColor;
        private Color borderColor;
        private Color borderSelectColor;
        private int borderOffset = 2;
        private readonly Pen borderPen = new Pen(Color.Black);
        private BorderStyle borderStyle = BorderStyle.Fixed3D;
        private int hoveredItem = -1;
        private bool integralHeight = true;
        private bool integralHeightAdjust = true;
        private int itemHeight = DefaultItemHeight;
        private readonly ObjectCollection items;
        private string keyFilter = "";
        private Unity.API.ApplicationBehaviour.invokeAction keyFilterIA;
        private float keyFilterResetTime = 3;
        private int visibleItemsCount = 0;
        private bool scrollAlwaysVisible;
        private int selectedIndex = -1;
        private readonly VScrollBar vScroll;

        public const int DefaultItemHeight = 13;

        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = value;
                UpdateBorderPen();
            }
        }
        public Color BorderSelectColor
        {
            get { return borderSelectColor; }
            set
            {
                borderSelectColor = value;
                UpdateBorderPen();
            }
        }
        public BorderStyle BorderStyle
        {
            get { return borderStyle; }
            set
            {
                borderStyle = value;

                UpdateBorder();
            }
        }
        public Color DisabledColor { get; set; }
        public virtual DrawMode DrawMode { get; set; }
        public Color HoverColor { get; set; }
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
        internal int ScrollIndex
        {
            get { return vScroll.Value / ItemHeight; }
            set { vScroll.Value = value * ItemHeight; }
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
                if (SelectedIndex == -1) return null;
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
        public Color SelectionBackColor { get; set; }
        public Color SelectionDisabledColor { get; set; }
        public Color SelectionForeColor { get; set; }
        public bool WrapText { get; set; }

        public ListBox()
        {
            items = new ObjectCollection(this);

            BackColor = Color.White;
            BorderColor = Color.FromArgb(130, 135, 144);
            BorderSelectColor = Color.FromArgb(126, 180, 234);
            DisabledColor = Color.Gray;
            DrawMode = DrawMode.Normal;
            DrawItem = InternalDrawItem;
            HoverColor = Color.FromArgb(221, 238, 253);
            SelectionBackColor = SystemColors.Highlight;
            SelectionDisabledColor = Color.FromArgb(101, 203, 255);
            SelectionForeColor = SystemColors.HighlightText;
            Size = new Size(120, 95);
            WrapText = true;

            vScroll = new VScrollBar();
            vScroll.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            vScroll.Location = new Point(Width - vScroll.Width, 0);
            vScroll.Height = Height;
            vScroll.Visible = false;
            Controls.Add(vScroll);

            UpdateBorder();
            UpdateBorderPen();
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
        private void ResetItemHeight()
        {
            itemHeight = DefaultItemHeight;
        }
        private void ResetKeyFilter()
        {
            keyFilter = "";
            keyFilterIA = null;
        }

        protected virtual void OnDrawItem(DrawItemEventArgs e)
        {
            if (DrawItem != null)
                DrawItem(this, e);
        }
        protected override void OnKeyPress(KeyEventArgs e)
        {
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
                        var itemIndex = FindItemIndex(x => x != null && x.ToString().ToLower().StartsWith(keyFilter));
                        SelectItem(itemIndex);

                        if (keyFilterIA == null)
                            keyFilterIA = Unity.API.ApplicationBehaviour.Invoke(ResetKeyFilter, keyFilterResetTime);
                        keyFilterIA.Seconds = keyFilterResetTime;
                    }
                    break;
            }
        }
        protected override void OnLatePaint(PaintEventArgs e)
        {
            base.OnLatePaint(e);

            e.Graphics.DrawRectangle(borderPen, 0, 0, Width, Height);
        }
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            UpdateBorderPen();
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
            base.OnMouseUp(e);

            var mclient = PointToClient(MousePosition);
            var itemIndex = IndexAt(mclient);

            SelectItem(itemIndex);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;

            const int itemTextVerticalPadding = -3;

            // Paint list.
            g.uwfFillRectangle(BackColor, 0, 0, Width, Height);
            for (int i = 0; i < visibleItemsCount && i + ScrollIndex < Items.Count; i++)
            {
                var itemIndex = i + ScrollIndex;
                if (itemIndex < 0) continue;

                var item = Items[itemIndex];
                var itemY = borderOffset + i * ItemHeight;
                var itemW = WrapText ? Width : Width * 5;
                bool itemDisabled = Items.IsDisabled(itemIndex);
                bool itemSelected = itemIndex == SelectedIndex;
                bool itemHovered = itemIndex == hoveredItem;
                var itemForeColor = ForeColor;

                if (itemDisabled)
                    itemForeColor = DisabledColor;
                else if (itemSelected)
                    itemForeColor = SelectionForeColor;

                var itemBackColor = Color.Transparent;
                if (itemSelected || itemHovered)
                {
                    itemBackColor = HoverColor;
                    if (itemDisabled)
                        itemBackColor = SelectionDisabledColor;
                    else if (itemSelected)
                        itemBackColor = SelectionBackColor;
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
                    var itemRect = new Rectangle(borderOffset + 2,
                        itemY + itemTextVerticalPadding,
                        fillWidth,
                        ItemHeight);
                    var state = DrawItemState.Default;
                    if (itemSelected)
                        state = DrawItemState.Selected;
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
            SelectedIndexChanged(this, e);
        }

        public void AdjustHeight()
        {
            integralHeightAdjust = false;
            Height = (int)(Math.Ceiling((float)Height / ItemHeight) * ItemHeight) + borderOffset * 2;
            integralHeightAdjust = true;
        }
        public void EnsureVisible()
        {
            if (SelectedIndex < ScrollIndex)
                ScrollIndex = SelectedIndex;

            if (SelectedIndex > ScrollIndex + visibleItemsCount - 1)
                ScrollIndex = SelectedIndex - visibleItemsCount + 1;

            if (ScrollIndex < 0)
                ScrollIndex = 0;
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
        internal override bool FocusInternal()
        {
            var result = base.FocusInternal();
            UpdateBorderPen();
            return result;
        }
        internal int IndexAt(Point mclient)
        {
            return ScrollIndex + (int)((mclient.Y - borderOffset) / ItemHeight);
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
                vScroll.LargeChange = Height;
                vScroll.Visible = ScrollAlwaysVisible || Height < PreferredHeight;
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
        internal void UpdateBorderPen()
        {
            borderCurrentColor = BorderColor;
            if (Focused || uwfContext)
                borderCurrentColor = BorderSelectColor;

            borderPen.Color = borderCurrentColor;
        }

        public event EventHandler SelectedIndexChanged = delegate { };
        public event DrawItemEventHandler DrawItem;

        public class ObjectCollection : IList
        {
            private readonly List<int> disabledItems = new List<int>();
            private readonly List<object> items = new List<object>();
            private readonly ListBox owner;

            public ObjectCollection(ListBox owner)
            {
                this.owner = owner;
            }
            public ObjectCollection(ListBox owner, ObjectCollection value)
            {
                this.owner = owner;
                this.AddRange(value);
            }
            public ObjectCollection(ListBox owner, object[] value)
            {
                this.owner = owner;
                this.AddRange(value);
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
                get { return false; }
            }

            public virtual object this[int index]
            {
                get { return items[index]; }
                set { items[index] = value; }
            }

            private int AddInternal(object item)
            {
                items.Add(item);
                owner.RefreshItems();

                return items.Count - 1;
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
                disabledItems.Clear();
                items.Clear();
                owner.ScrollIndex = 0;
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
            public void Disable(int itemIndex)
            {
                if (!disabledItems.Contains(itemIndex))
                    disabledItems.Add(itemIndex);
            }
            public void Enable(int itemIndex)
            {
                for (int i = 0; i < disabledItems.Count; i++)
                    if (itemIndex == disabledItems[i])
                    {
                        disabledItems.RemoveAt(i);
                        break;
                    }
            }
            public IEnumerator GetEnumerator()
            {
                return items.GetEnumerator();
            }
            public int IndexOf(object value)
            {
                return items.IndexOf(value);
            }
            public void Insert(int index, object item)
            {
                items.Insert(index, item);
            }
            public bool IsDisabled(int itemIndex)
            {
                return disabledItems.FindIndex(x => x == itemIndex) != -1;
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
        }
    }
}
