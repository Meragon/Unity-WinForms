namespace System.Windows.Forms
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;

    public class ComboBox : ListControl
    {
        internal Color uwfBackColorDropDownList = Color.FromArgb(235, 235, 235);
        internal Color uwfBorderColor = Color.DarkGray;
        internal Color uwfBorderColorDisabled = Color.FromArgb(217, 217, 217);
        internal Color uwfBorderColorHovered = Color.FromArgb(126, 180, 234);
        internal Color uwfDisabledColor = SystemColors.Control;
        internal Color uwfHoverColor = Color.White;
        internal Color uwfHoverColorDropDownList = Color.FromArgb(227, 240, 252);
        internal Color uwfListItemHoverColor = Color.FromArgb(221, 238, 253);
        internal Color uwfListItemSelectedBackgroundColor = SystemColors.Highlight;
        internal Color uwfListItemSelectedForeColor = SystemColors.HighlightText;
        internal bool  uwfWrapText;
        
        private const int DOWN_BUTTON_WIDTH = 17;

        private readonly Pen borderPen = new Pen(Color.Transparent);
        private readonly Pen downButtonBorderPen = new Pen(Color.Transparent);

        private ComboBoxStyle dropDownStyle;
        private bool keyFlag;
        private string filter = string.Empty;
        private int itemHeight = 15;
        private ListBox listBox;
        private bool listBoxOpened;
        private int maxDropDownItems = 8;
        private int selectedIndex = -1;

        public ComboBox()
        {
            Items = new ObjectCollection(this);

            AutoCompleteMode = AutoCompleteMode.None;
            AutoCompleteSource = AutoCompleteSource.None;
            BackColor = Color.White;
            DropDownStyle = ComboBoxStyle.DropDown;
            Padding = new Padding(4, 0, 4, 0);
        }

        public event EventHandler DropDown;
        public event EventHandler DropDownClosed;
        public event EventHandler SelectedIndexChanged;

        public AutoCompleteMode AutoCompleteMode { get; set; }
        public AutoCompleteSource AutoCompleteSource { get; set; }
        public ComboBoxStyle DropDownStyle
        {
            get { return dropDownStyle; }
            set
            {
                dropDownStyle = value;
                UpdateComboBoxDDStyle();
            }
        }
        public int ItemHeight
        {
            get { return itemHeight; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value");
                itemHeight = value;
            }
        }
        public ComboBox.ObjectCollection Items { get; private set; }
        public int MaxDropDownItems
        {
            get { return maxDropDownItems; }
            set { if (value > 1 && value <= 100) maxDropDownItems = value; }
        }
        public override int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                if (selectedIndex == value) return;

                selectedIndex = value;
                UpdateText();

                OnSelectedItemChanged(EventArgs.Empty);
                OnSelectedIndexChanged(EventArgs.Empty);
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
            set { SelectedIndex = Items.IndexOf(value); }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(121, 21);
            }
        }

        internal override bool FocusInternal()
        {
            var res = base.FocusInternal();

            filter = Text;

            return res;
        }

        protected virtual void OnDropDown(EventArgs e)
        {
            var handler = DropDown;
            if (handler != null)
                handler(this, e);
        }
        protected virtual void OnDropDownClosed(EventArgs e)
        {
            var handler = DropDownClosed;
            if (handler != null)
                handler(this, e);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.KeyCode)
            {
                case Keys.Down:
                    {
                        if (Items.IsDisabled(selectedIndex + 1))
                            break;
                        if (listBox != null)
                        {
                            listBox.SelectedIndex++;
                            SelectedIndex = Items.FindIndex(x => x == listBox.SelectedItem);
                            listBox.EnsureVisible();
                        }
                        else if (selectedIndex + 1 < Items.Count)
                            SelectedIndex++;
                    }
                    break;
                case Keys.Up:
                    {
                        if (Items.IsDisabled(selectedIndex - 1))
                            break;

                        if (listBox != null)
                        {
                            listBox.SelectedIndex--;
                            if (listBox.SelectedIndex < 0 && listBox.Items.Count > 0)
                                listBox.SelectedIndex = 0;
                            SelectedIndex = Items.FindIndex(x => x == listBox.SelectedItem);
                            listBox.EnsureVisible();
                        }
                        else if (selectedIndex > 0)
                            SelectedIndex--;
                    }
                    break;
                case Keys.Return:
                    if (listBox != null && !listBox.Disposing && !listBox.IsDisposed)
                    {
                        listBox.SelectItem(listBox.SelectedIndex);
                        ApplySelectedItem();
                    }
                    break;
            }
            if (listBox != null && DropDownStyle == ComboBoxStyle.DropDownList)
            {
                if (keyFlag == true)
                {
                    keyFlag = false;
                    filter = "";
                }

                char c = KeyHelper.GetLastInputChar();
                if (char.IsLetterOrDigit(c) || char.IsPunctuation(c))
                {
                    filter += char.ToLower(c);
                    var itemIndex = listBox.FindItemIndex(x => x != null && x.ToString().ToLower().Contains(filter));
                    if (itemIndex > -1)
                        listBox.SelectItem(itemIndex);
                }
            }
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.KeyCode == Keys.Space)
                RaiseOnMouseDown(new MouseEventArgs(MouseButtons.Left, Width - 1, 1, 0, 0));
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
            if (DropDownStyle == ComboBoxStyle.DropDownList || e.X >= Width - 16)
            {
                keyFlag = true;
                CreateListBox(String.Empty);
            }
        }
        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);

            if (Enabled && DropDownStyle == ComboBoxStyle.DropDown)
            {
                var mclient = PointToClient(MousePosition);
                if (mclient.X < Width - 16)
                    Cursor.Current = Cursors.IBeam;
                else
                    Cursor.Current = Cursors.Default;
            }            
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            
            Cursor.Current = Cursors.Default;
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (Focused)
            {
                bool down = e.Delta < 0;
                if (down)
                {
                    if (selectedIndex + 1 < Items.Count)
                        SelectedIndex++;
                }
                else
                {
                    if (selectedIndex > 0)
                        SelectedIndex--;
                }
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;

            #region get colors.
            var backColor = BackColor;
            var borderColor = uwfBorderColor;
            var arrowColor = Color.FromArgb(96, 96, 96);
            if (Enabled)
            {
                switch (DropDownStyle)
                {
                    case ComboBoxStyle.DropDown:
                        if (Focused || uwfHovered)
                        {
                            backColor = uwfHoverColor;
                            borderColor = uwfBorderColorHovered;
                        }
                        break;
                    case ComboBoxStyle.DropDownList:
                        backColor = Focused || uwfHovered ? uwfHoverColorDropDownList : uwfBackColorDropDownList;
                        break;
                }

                if (Focused || uwfHovered)
                    borderColor = uwfBorderColorHovered;
            }
            else
            {
                backColor = uwfDisabledColor;
                borderColor = uwfBorderColorDisabled;
            }

            borderPen.Color = borderColor;

            #endregion

            var height = Height;
            var width = Width;
            var textWidth = width - DOWN_BUTTON_WIDTH;
            if (uwfWrapText == false)
                textWidth *= 2;

            g.uwfFillRectangle(backColor, 0, 0, width, height);

            switch (DropDownStyle)
            {
                case ComboBoxStyle.DropDown:

                    if (Focused && Enabled)
                    {
                        var filterBuffer = g.uwfDrawTextField(filter, Font, ForeColor, 2, 0, textWidth, height, HorizontalAlignment.Left);
                        if (filterBuffer != filter)
                        {
                            if (listBox == null || listBox.IsDisposed || listBox.Disposing)
                                CreateListBox(filterBuffer);
                            else
                                UpdateListBoxItems(filterBuffer);
                        }
                        filter = filterBuffer;
                    }
                    else
                        g.uwfDrawString(Text, Font, ForeColor, 5, 0, textWidth, height);

                    if (uwfHovered)
                    {
                        var bRect = GetButtonRect();
                        var mclient = PointToClient(MousePosition);
                        var downButtonBackColor = backColor;

                        if (bRect.Contains(mclient))
                        {
                            arrowColor = Color.Black;
                            downButtonBackColor = uwfHoverColorDropDownList;
                            downButtonBorderPen.Color = uwfBorderColorHovered;
                        }
                        else
                            downButtonBorderPen.Color = Color.Transparent;

                        g.uwfFillRectangle(downButtonBackColor, bRect);
                        g.DrawLine(downButtonBorderPen, bRect.X, bRect.Y, bRect.X, bRect.Y + bRect.Height);
                    }
                    break;
                case ComboBoxStyle.DropDownList:
                    g.uwfDrawString(Text, Font, ForeColor, 5, -2, textWidth, height + 4, ContentAlignment.MiddleLeft);
                    break;
            }

            // Arrow.
            var arrowSize = 16;
            g.uwfFillRectangle(backColor, width - arrowSize, 0, arrowSize, height);
            g.uwfDrawImage(
                uwfAppOwner.Resources.CurvedArrowDown,
                arrowColor,
                width - arrowSize,
                height / 2 - arrowSize / 2,
                arrowSize,
                arrowSize);

            // Border.
            g.DrawRectangle(borderPen, 0, 0, Width, Height);
        }
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            var handler = SelectedIndexChanged;
            if (handler != null)
                handler(this, e);
        }
        protected virtual void OnSelectedItemChanged(EventArgs e)
        {

        }

        private void ApplySelectedItem()
        {
            if (listBox == null) return;
            var listSelectedItem = listBox.SelectedItem;
            for (int i = 0; i < Items.Count; i++)
                if (Items[i] == listSelectedItem)
                {
                    filter = listSelectedItem != null ? listSelectedItem.ToString() : "";
                    SelectedItem = listSelectedItem;
                    break;
                }
            listBox.Dispose();
            listBox = null;
        }
        private void CreateListBox(string listFilter)
        {
            if (listBox != null)
            {
                listBox = null;
                return;
            }

            if (!listBoxOpened)
            {
                listBox = new ListBox();
                listBox.BackColor = BackColor;
                listBox.Font = Font;
                listBox.ForeColor = ForeColor;
                listBox.Width = Width;
                listBox.ItemHeight = ItemHeight;
                listBox.Height = listBox.ItemHeight * (Items.Count > MaxDropDownItems ? MaxDropDownItems : Items.Count);
                
                listBox.uwfContext = true;
                listBox.uwfWrapText = false;
                listBox.uwfShadowBox = true;
                listBox.uwfBorderColor = uwfBorderColor;
                listBox.uwfBorderSelectColor = uwfBorderColorHovered;
                listBox.uwfItemHoverColor = uwfListItemHoverColor;
                listBox.uwfSelectionBackColor = uwfListItemSelectedBackgroundColor;
                listBox.uwfSelectionForeColor = uwfListItemSelectedForeColor;
                
                if (listBox.Height < listBox.ItemHeight) listBox.Height = listBox.ItemHeight;

                UpdateListBoxItems(listFilter);

                var gpoint = PointToScreen(Point.Empty);
                listBox.Location = new Point(gpoint.X, gpoint.Y + Height);
                listBox.MouseUp += ListBoxOnMouseUp;
                listBox.KeyDown += ListBoxOnKeyDown;
                listBox.Disposed += ListBoxOnDisposed;
                
                OnDropDown(EventArgs.Empty);
            }
            else
                listBoxOpened = false;
        }
        private Rectangle GetButtonRect()
        {
            return new Rectangle(Width - DOWN_BUTTON_WIDTH, 1, DOWN_BUTTON_WIDTH, Height - 2);
        }
        private void ListBoxOnMouseUp(object sender, MouseEventArgs mouseEventArgs)
        {
            if (mouseEventArgs.Button != MouseButtons.Left) return;

            ApplySelectedItem();
        }
        private void ListBoxOnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.KeyCode != Keys.Return && keyEventArgs.KeyCode != Keys.Space) return;

            ApplySelectedItem();
        }
        private void ListBoxOnDisposed(object sender, EventArgs eventArgs)
        {
            var clientRect = new System.Drawing.Rectangle(0, 0, Width, Height);
            var contains = clientRect.Contains(PointToClient(MousePosition));
            if (!contains)
                listBoxOpened = false;
            else
                listBoxOpened = !listBoxOpened;

            listBox.MouseUp -= ListBoxOnMouseUp;
            listBox.KeyDown -= ListBoxOnKeyDown;
            listBox.Disposed -= ListBoxOnDisposed;

            listBox = null;
            
            OnDropDownClosed(EventArgs.Empty);
        }
        private void UpdateComboBoxDDStyle()
        {
            switch (DropDownStyle)
            {
                case ComboBoxStyle.DropDown:
                    break;
                case ComboBoxStyle.DropDownList:
                    break;
            }
        }
        private void UpdateListBoxItems(string listFilter)
        {
            if (listBox == null)
                return;
            
            listBox.Items.Clear();
            
            bool selectedIndexChanged = false;
            for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                if (DropDownStyle == ComboBoxStyle.DropDownList || String.IsNullOrEmpty(listFilter))
                    listBox.Items.Add(item);
                else
                {
                    var itemString = item.ToString();
                    if (!itemString.ToLower().Contains(listFilter.ToLower())) continue;

                    listBox.Items.Add(item);
                    if (itemString != listFilter) continue;

                    listBox.SelectedIndex = i;
                    selectedIndexChanged = true;
                }
            }
            for (int i = 0; i < Items.Count; i++)
                if (Items.IsDisabled(i))
                    listBox.Items.Disable(i);

            if (selectedIndexChanged == false)
            {
                listBox.SelectedIndex = SelectedIndex;
                listBox.EnsureVisible();
            }
        }
        private void UpdateText()
        {
            string text = null;

            if (SelectedIndex != -1)
            {
                var item = SelectedItem;
                if (item != null)
                    text = item.ToString();
            }

            Text = text;
        }

        public class ObjectCollection : IList
        {
            private readonly List<int> disabledItems;
            private readonly List<object> items;
            private readonly ComboBox owner;

            public ObjectCollection(ComboBox owner)
            {
                this.owner = owner;
                disabledItems = new List<int>();
                items = new List<object>();
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

            public int Add(object item)
            {
                items.Add(item);
                return items.Count - 1;
            }
            public void AddRange(object[] array)
            {
                items.AddRange(array);
            }
            public void Clear()
            {
                items.Clear();
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
            public object Find(Predicate<object> match)
            {
                return items.Find(match);
            }
            public int FindIndex(Predicate<object> match)
            {
                return items.FindIndex(match);
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
            }
            public void RemoveAt(int index)
            {
                items.RemoveAt(index);
            }
        }
    }
}
