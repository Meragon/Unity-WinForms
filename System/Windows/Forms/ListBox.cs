using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;


namespace System.Windows.Forms
{
    public class ListBox : ListControl
    {
        private bool _drag;
        private object _dragItem;
        private Point _dragPosition;
        private ListBox.ObjectCollection _items;
        private int _hoveredItem = -1;
        private int _visibleItems = 0;
        private int _selectedIndex = -1;
        private bool _scroll;
        private bool _scrollVisible = false;
        private int _scrollStartY;
        internal int _scrollIndex;
        private bool _scrollHover;
        private int _scrollX;
        private int _scrollY;
        private int _scrollWidth = 8;
        private int _scrollHeight;

        public const int DefaultItemHeight = 20;

        public ListBox()
        {
            _items = new ObjectCollection(this);

            BackColor = Color.White;
            BorderColor = Color.DarkGray;
            ItemHeight = DefaultItemHeight;
            WrapText = true;

            Owner.UpClick += Application_UpClick;
        }

        public Color BorderColor { get; set; }
        public bool FixedHeight { get; set; }
        public virtual int ItemHeight { get; set; }
        public ListBox.ObjectCollection Items { get { return _items; } }
        internal int ScrollIndex
        {
            get { return _scrollIndex; }
            set
            {
                if (value + _visibleItems < Items.Count)
                    _scrollIndex = value;
                else
                    _scrollIndex = Items.Count - _visibleItems;
                if (_scrollIndex < 0)
                    _scrollIndex = 0;
            }
        }
        public override int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                bool changed = _selectedIndex != value && value != -1;
                _selectedIndex = value;
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
        public bool WrapText { get; set; }

        private void Application_UpClick(object sender, MouseEventArgs e)
        {
            _scroll = false;
            _drag = false;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            var mclient = PointToClient(MousePosition);
            Rectangle _scrollRect = new Rectangle(Width - _scrollWidth, _scrollY, _scrollWidth, _scrollHeight);
            if (_scrollRect.Contains(mclient))
            {
                _scroll = true;
                _scrollStartY = mclient.Y - _scrollY;
                return;
            }
            else
                _scroll = false;

            for (int i = 0; i < Items.Count; i++)
            {
                var item_rect = new Rectangle(0, i * ItemHeight, Width, ItemHeight);
                if (item_rect.Contains(mclient))
                {
                    if (Items.IsDisabled(i + ScrollIndex)) break;

                    _drag = true;
                    _dragItem = Items[i + ScrollIndex];
                    _dragPosition = mclient;
                    break;
                }
            }
        }
        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);
            var mclient = PointToClient(MousePosition);
            Rectangle _scrollRect = new Rectangle(Width - _scrollWidth, _scrollY, _scrollWidth, _scrollHeight);
            if (_scrollRect.Contains(mclient))
                _scrollHover = true;
            else
                _scrollHover = false;
            
            for (int i = 0; i < Items.Count; i++)
            {
                var item_rect = new Rectangle(0, i * ItemHeight, Width, ItemHeight);
                if (item_rect.Contains(mclient))
                {
                    _hoveredItem = i;
                    break;
                }
            }
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hoveredItem = -1;
            _scrollHover = false;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            var mclient = PointToClient(MousePosition);
            if (_scroll)
            {
                _scrollIndex = (mclient.Y - _scrollStartY) * Items.Count / Height;
                _scrollIndex = _scrollIndex < 0 ? 0 : _scrollIndex;
                _scrollIndex = _scrollIndex > Items.Count - _visibleItems ? Items.Count - _visibleItems : _scrollIndex;

                _scrollY = _scrollIndex * Height / Items.Count;
                Refresh();
            }

            if (_drag && _dragPosition.Distance(mclient) > 4)
            {
                Drag(_dragItem);
                _drag = false;
            }
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            ScrollIndex -= (int)e.Delta;

            if (ScrollIndex + _visibleItems > Items.Count) ScrollIndex = Items.Count - _visibleItems;
            if (ScrollIndex < 0) ScrollIndex = 0;
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _drag = false;

            var mclient = PointToClient(MousePosition);
            Rectangle _scrollRect = new Rectangle(Width - _scrollWidth, _scrollY, _scrollWidth, _scrollHeight);
            if (_scrollRect.Contains(mclient))
            {
                _scroll = false;
                return;
            }

            if (_scroll == false)
                for (int i = 0; i < Items.Count; i++)
                {
                    var item_rect = new Rectangle(0, i * ItemHeight, Width, ItemHeight);
                    if (item_rect.Contains(mclient))
                    {
                        if (Items.IsDisabled(i + ScrollIndex)) break;
                        SelectedIndex = i + ScrollIndex;
                        OnSelectedValueChanged(null);
                        break;
                    }
                }

            _scroll = false;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;

            g.FillRectangle(new SolidBrush(BackColor), 0, 0, Width, Height);
            for (int i = 0; i < _visibleItems && i + ScrollIndex < Items.Count; i++)
            {
                bool disabled = Items.IsDisabled(i + ScrollIndex);
                if (i + _scrollIndex == SelectedIndex || i == _hoveredItem)
                    g.FillRectangle(new SolidBrush(disabled ? Color.FromArgb(101, 203, 255) : Color.FromArgb(64, 0, 122, 204)), 0, i * ItemHeight, Width, ItemHeight);

                g.DrawString(Items[i + ScrollIndex].ToString(), Font, new SolidBrush(disabled ? Color.Gray : ForeColor), 4, i * ItemHeight, WrapText ? Width : Width * 5, ItemHeight);
            }

            _scrollVisible = false;
            if (Items.Count > _visibleItems)
                _scrollVisible = true;

            if (_scrollVisible && Items.Count > 0)
            {
                _scrollX = Width - _scrollWidth;
                _scrollY = ScrollIndex * Height / Items.Count;
                _scrollHeight = (int)((float)(_visibleItems * Height) / Items.Count);
                if (_scrollHeight < 4)
                    _scrollHeight = 4;
                Color _scrollColor = Color.FromArgb(222, 222, 230);
                if (_scrollHover || _scroll) _scrollColor = Color.FromArgb(136, 136, 136);
                g.FillRectangle(new SolidBrush(_scrollColor), _scrollX, _scrollY, _scrollWidth, _scrollHeight);
            }
        }
        protected override void OnResize(Point delta)
        {
            base.OnResize(delta);

            _visibleItems = Height / ItemHeight;
        }
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);

            SelectedIndexChanged(this, e);
        }
        protected override void OnLatePaint(PaintEventArgs e)
        {
            base.OnLatePaint(e);
            e.Graphics.DrawRectangle(Context ? new Pen(Color.FromArgb(126, 180, 234)) : new Pen(BorderColor), 0, 0, Width, Height);
        }

        internal void SelectItem(int index)
        {
            SelectedIndex = index;
            OnSelectedValueChanged(null);
        }

        public event ListBox.DragHandler Drag = delegate { };
        public event EventHandler SelectedIndexChanged = delegate { };

        public delegate void DragHandler(object item);

        public class ObjectCollection : IList, ICollection, IEnumerable
        {
            private List<int> _disabledItems;
            private List<object> _items;
            private ListBox _owner;

            public ObjectCollection(ListBox owner)
            {
                _owner = owner;
                _disabledItems = new List<int>();
                _items = new List<object>();
            }

            public int Count { get { return _items.Count; } }
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
                get { return _items[index]; }
                set { _items[index] = value; }
            }

            private void _UpdateOwnerHeight()
            {
                _owner._visibleItems = _owner.Height / _owner.ItemHeight;
                if (_owner.FixedHeight) return;
                int cnt = _owner._visibleItems;
                if (_items.Count < cnt)
                    cnt = _items.Count;
                if (_owner.Context)
                    _owner.Height = (_owner.ItemHeight * cnt);
            }

            public int Add(object item)
            {
                _items.Add(item);
                _UpdateOwnerHeight();
                return _items.Count - 1;
            }
            public void AddRange(object[] items)
            {
                _items.AddRange(items);
                _UpdateOwnerHeight();
            }
            public void Clear()
            {
                _disabledItems.Clear();
                _items.Clear();
                _UpdateOwnerHeight();
                _owner.ScrollIndex = 0;
                _owner._selectedIndex = -1;
            }
            public bool Contains(object value)
            {
                return _items.Contains(value);
            }
            public void CopyTo(Array array, int index)
            {
                _items.CopyTo((object[])array, index);
            }
            public void CopyTo(object[] destination, int arrayIndex)
            {
                _items.CopyTo(destination, arrayIndex);
            }
            public void Disable(int itemIndex)
            {
                if (!_disabledItems.Contains(itemIndex))
                    _disabledItems.Add(itemIndex);
            }
            public void Enable(int itemIndex)
            {
                for (int i = 0; i < _disabledItems.Count; i++)
                    if (itemIndex == _disabledItems[i])
                    {
                        _disabledItems.RemoveAt(i);
                        break;
                    }
            }
            public IEnumerator GetEnumerator()
            {
                return _items.GetEnumerator();
            }
            public int IndexOf(object value)
            {
                return _items.IndexOf(value);
            }
            public void Insert(int index, object item)
            {
                _items.Insert(index, item);
            }
            public bool IsDisabled(int itemIndex)
            {
                return _disabledItems.FindIndex(x => x == itemIndex) != -1;
            }
            public void Remove(object value)
            {
                _items.Remove(value);
                _UpdateOwnerHeight();
            }
            public void RemoveAt(int index)
            {
                _items.RemoveAt(index);
                _UpdateOwnerHeight();
            }
        }
    }
}
