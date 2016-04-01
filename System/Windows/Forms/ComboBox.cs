using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public class ComboBox : ListControl
    {
#if UNITY_EDITOR
        private bool _toggleItems;
#endif

        private string _filter = String.Empty;
        private ComboBox.ObjectCollection _items;
        private bool _hovered;
        private ListBox _listBox;
        private bool _listBoxOpened;
        private int _maxDropDownItems = 8;
        private int _selectedIndex = -1;
        private bool _shouldFocus;

        public AutoCompleteMode AutoCompleteMode { get; set; }
        public AutoCompleteSource AutoCompleteSource { get; set; }
        public Color BorderColor { get; set; }
        public Color BorderColorDisabled { get; set; }
        public Color BorderColorHovered { get; set; }
        public ComboBoxStyle DropDownStyle { get; set; }
        public ComboBox.ObjectCollection Items { get { return _items; } }
        public Color HoverColor { get; set; }
        public int MaxDropDownItems
        {
            get { return _maxDropDownItems; }
            set { if (value > 1 && value <= 100) _maxDropDownItems = value; }
        }
        public override int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                //if (value == _selectedIndex) return;
                _selectedIndex = value;
                if (_selectedIndex > -1)
                {
                    //SelectedItem = _items[_selectedIndex];
                    SelectedText = SelectedItem.ToString();
                    _filter = SelectedItem.ToString();
                }
                else
                {
                    SelectedText = "";
                    _filter = String.Empty;
                }
                SelectedIndexChanged(this, null);
            }
        }
        public object SelectedItem
        {
            get
            {
                if (SelectedIndex > -1 && SelectedIndex < Items.Count)
                    return Items[SelectedIndex];
                return null;
            }
            set
            {
                SelectedText = "";
                for (int i = 0; i < Items.Count; i++)
                {
                    if (value == Items[i])
                    {
                        _selectedIndex = i;
                        _filter = Items[i].ToString();
                        SelectedText = Items[i].ToString();
                        SelectedIndexChanged(this, null);
                        break;
                    }
                }
            }
        }
        public string SelectedText { get; set; }

        public ComboBox()
        {
            _items = new ObjectCollection(this);

            AutoCompleteMode = Forms.AutoCompleteMode.None;
            AutoCompleteSource = Forms.AutoCompleteSource.None;
            BackColor = Color.FromArgb(234, 234, 234);
            DropDownStyle = ComboBoxStyle.DropDownList;

            BorderColor = Color.DarkGray;
            BorderColorDisabled = Color.FromArgb(217, 217, 217);
            BorderColorHovered = Color.FromArgb(126, 180, 234);
            HoverColor = Color.FromArgb(221, 237, 252);

            Padding = new Forms.Padding(4, 0, 4, 0);
            Size = new Size(121, 22);
        }

        public event EventHandler SelectedIndexChanged = delegate { };

        public override void Focus()
        {
            base.Focus();
            _shouldFocus = true;
        }
        protected override void OnKeyPress(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.KeyCode)
            {
                case UnityEngine.KeyCode.DownArrow:
                    {
                        if (Items.IsDisabled(_selectedIndex + 1))
                            break;
                        if (_selectedIndex + 1 < _items.Count)
                            SelectedIndex++;
                        _listBox.SelectedIndex = SelectedIndex;
                        if (_listBox.ScrollIndex - 1 < SelectedIndex)
                            _listBox.ScrollIndex = SelectedIndex - 1;

                    }
                    break;
                case UnityEngine.KeyCode.UpArrow:
                    {
                        if (Items.IsDisabled(_selectedIndex - 1))
                            break;
                        if (_selectedIndex > 0)
                            SelectedIndex--;
                        _listBox.SelectedIndex = SelectedIndex;
                        if (_listBox.ScrollIndex > SelectedIndex)
                            _listBox.ScrollIndex = SelectedIndex;
                    }
                    break;
                case UnityEngine.KeyCode.Return:
                    if (_listBox != null && !_listBox.Disposing && !_listBox.IsDisposed)
                        _listBox.SelectItem(SelectedIndex);
                    break;
            }
            if (_listBox != null)
                if (_listBox.ScrollIndex < 0)
                    _listBox.ScrollIndex = 0;
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
            if (DropDownStyle == ComboBoxStyle.DropDownList || e.X >= Width - 16)
                _CreateListBox(String.Empty);
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _hovered = true;

        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hovered = false;
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            bool down = e.Delta < 0;
            if (down)
            {
                if (_selectedIndex + 1 < _items.Count)
                    SelectedIndex++;
            }
            else
            {
                if (_selectedIndex > 0)
                    SelectedIndex--;
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;

            if (Enabled)
            {
                //g.DrawLine(new Pen(Color.FromArgb(240, 240, 240)), 1, 1, Width - 2, 1);
                if (DropDownStyle == ComboBoxStyle.DropDownList)
                    g.FillRectangle(new SolidBrush(_hovered || Focused ? Color.FromArgb(226, 239, 252) : BackColor), 0, 0, Width, Height);
                else
                {
                    g.FillRectangle(new SolidBrush(BackColor), 0, 0, Width, Height);

                    var mloc = PointToClient(MousePosition);
                    if (DropDownStyle == ComboBoxStyle.DropDownList || new Rectangle(Width - 16, 0, 16, Height).Contains(mloc))
                    {
                        g.FillRectangle(new SolidBrush(HoverColor), Width - 16, 0, 16, Height);
                        g.DrawLine(new Pen(BorderColorHovered), Width - 17, 1, Width - 17, Height - 1);
                    }
                }
            }
            else
                g.FillRectangle(new SolidBrush(Color.FromArgb(239, 239, 239)), 0, 0, Width, Height);
            if (DropDownStyle == ComboBoxStyle.DropDownList || !Enabled)
                g.DrawString(SelectedText, Font, new SolidBrush(Enabled ? ForeColor : Color.Gray), Padding.Left, Padding.Top, Width - Padding.Left - Padding.Right - 16, Height - Padding.Bottom - Padding.Top); // 16 - arrow width.
            else
            {
                if (Focused)
                {
                    if (_shouldFocus)
                        UnityEngine.GUI.SetNextControlName(Name);

                    var _filterBuffer = g.DrawTextField(_filter, Font, new SolidBrush(ForeColor), 4, 0, Width - 8 - 16, Height, HorizontalAlignment.Left);
                    if (_filterBuffer != _filter)
                    {
                        if (_listBox != null && !_listBox.IsDisposed && !_listBox.Disposing)
                        {
                            _listBox.Dispose();
                            _listBox = null;
                            _listBoxOpened = false;
                        }
                        _CreateListBox(_filter);
                    }
                    _filter = _filterBuffer;
                }
                else
                    g.DrawString(_filter, Font, new SolidBrush(ForeColor), 4, 0, Width - 8 - 16, Height, HorizontalAlignment.Left);

                if (_shouldFocus)
                {
                    UnityEngine.GUI.FocusControl(Name);
                    _shouldFocus = false;
                }
            }
            if (Enabled)
                g.DrawTexture(ApplicationBehaviour.Resources.Reserved.ComboBoxArrow, Width - 16, Height / 2 - 8, 16, 16);
            //else
            //    g.DrawTexture(Application.Resources.ComboBoxArrow, Width - 16, Height / 2 - 8, 16, 16, Color.White);
            if (Enabled)
                g.DrawRectangle(_hovered || Focused ? new Pen(BorderColorHovered) : new Pen(BorderColor), 0, 0, Width, Height);
            else
                g.DrawRectangle(new Pen(BorderColorDisabled), 0, 0, Width, Height);
        }
        protected override object OnPaintEditor(float width)
        {
            var component = base.OnPaintEditor(width);

#if UNITY_EDITOR
            Editor.NewLine(2);
            Editor.Label("ComboBox");
            Editor.EnumField("AutoCompleteMode", AutoCompleteMode);
            Editor.EnumField("AutoCompleteSource", AutoCompleteSource);
            Editor.EnumField("DropDownStyle", DropDownStyle);
            if ((_toggleItems = Editor.Foldout("Items", _toggleItems)) == true)
                for (int i = 0; i < Items.Count; i++)
                    Editor.Label(i.ToString(), Items[i].ToString());
            Editor.Label("SelectedIndex", SelectedIndex);
            Editor.Label("SelectedItem", SelectedItem);
            Editor.Label("SelectedText", SelectedText);
#endif

            return component;
        }

        private void _CreateListBox(string filter)
        {
            if (_listBox != null)
            {
                _listBox = null;
                return;
            }
            if (!_listBoxOpened)
            {
                _listBox = new ListBox();
                //listBox.scrollDirection = 1;
                //listBox.ItemWidth = Width;
                //listBox.ItemHeight = 20;
                _listBox.FixedHeight = true;
                _listBox.Font = Font;
                _listBox.BringToFront();
                _listBox.Context = true;
                _listBox.Width = Width;
                _listBox.Height = _listBox.ItemHeight * (Items.Count > MaxDropDownItems ? MaxDropDownItems : Items.Count);
                if (_listBox.Height < _listBox.ItemHeight) _listBox.Height = _listBox.ItemHeight;
                foreach (var item in Items)
                {
                    if (DropDownStyle == ComboBoxStyle.DropDownList || String.IsNullOrEmpty(filter))
                        _listBox.Items.Add(item);
                    else
                    {
                        if (item.ToString().ToLower().Contains(filter.ToLower()))
                            _listBox.Items.Add(item);
                    }
                }
                for (int i = 0; i < Items.Count; i++)
                    if (Items.IsDisabled(i))
                    {
                        _listBox.Items.Disable(i);
                        //UnityEngine.Debug.Log(i);
                    }
                if (SelectedIndex > -1)
                    _listBox.ScrollIndex = SelectedIndex;
                _listBox.SelectedIndex = SelectedIndex;

                var gpoint = PointToScreen(Point.Zero);
                _listBox.Location = gpoint + new Point(0, Height);
                _listBox.TopMost = true;
                _listBox.SelectedValueChanged += (object sender, EventArgs args) =>
                {
                    if (_listBox != null)
                    {
                        for (int i = 0; i < Items.Count; i++)
                            if (Items[i] == _listBox.SelectedItem)
                            {
                                _filter = _listBox.SelectedItem.ToString();
                                SelectedItem = _listBox.SelectedItem;
                                break;
                            }
                        //SelectedIndex = _listBox.SelectedIndex;
                        _listBox.Dispose();
                        _listBox = null;
                    }
                };
                _listBox.OnDisposing += (object sender, EventArgs args) =>
                {
                    var clientRect = new System.Drawing.Rectangle(0, 0, Width, Height);
                    var contains = clientRect.Contains(PointToClient(MousePosition));
                    if (!contains)
                        _listBoxOpened = false;
                    else
                        _listBoxOpened = !_listBoxOpened;
                    _listBox = null;
                };
            }
            else
                _listBoxOpened = false;
        }

        public class ObjectCollection : IList, ICollection, IEnumerable
        {
            private List<int> _disabledItems;
            private List<object> _items;
            private ComboBox _owner;

            public ObjectCollection(ComboBox owner)
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

            public int Add(object item)
            {
                _items.Add(item);
                return _items.Count - 1;
            }
            public void AddRange(object[] items)
            {
                _items.AddRange(items);
            }
            public void Clear()
            {
                _items.Clear();
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
            }
            public void RemoveAt(int index)
            {
                _items.RemoveAt(index);
            }
        }
    }
}
