using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Drawing;
using UnityEngine;
using Color = System.Drawing.Color;
using Editor = System.Drawing.Editor;

namespace System.Windows.Forms
{
    public class ComboBox : ListControl
    {
        private readonly Pen borderPen = new Pen(Color.Transparent);
        private readonly Pen downButtonBorderPen = new Pen(Color.Transparent);
        private const int downButtonWidth = 17;
        private ComboBoxStyle dropDownStyle;
        private bool keyFlag;
        private string filter = string.Empty;
        private int itemHeight = 15;
        private ListBox listBox;
        private bool listBoxOpened;
        private int _maxDropDownItems = 8;
        private int selectedIndex = -1;

        public AutoCompleteMode AutoCompleteMode { get; set; }
        public AutoCompleteSource AutoCompleteSource { get; set; }
        public Color BackColorDropDownList { get; set; }
        public Color BorderColor { get; set; }
        public Color BorderColorDisabled { get; set; }
        public Color BorderColorHovered { get; set; }
        public Color DisabledColor { get; set; }
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
                    throw new ArgumentOutOfRangeException("itemHeight");
                itemHeight = value;
            }
        }
        public ComboBox.ObjectCollection Items { get; private set; }
        public Color HoverColor { get; set; }
        public Color HoverColorDropDownList { get; set; }
        public int MaxDropDownItems
        {
            get { return _maxDropDownItems; }
            set { if (value > 1 && value <= 100) _maxDropDownItems = value; }
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
        public bool WrapText { get; set; }

        public ComboBox()
        {
            Items = new ObjectCollection(this);

            AutoCompleteMode = AutoCompleteMode.None;
            AutoCompleteSource = AutoCompleteSource.None;
            BackColor = Color.White;
            BackColorDropDownList = Color.FromArgb(235, 235, 235);
            BorderColor = Color.DarkGray;
            BorderColorDisabled = Color.FromArgb(217, 217, 217);
            BorderColorHovered = Color.FromArgb(126, 180, 234);
            DisabledColor = Color.FromArgb(239, 239, 239);
            DropDownStyle = ComboBoxStyle.DropDown;
            HoverColor = Color.White;
            HoverColorDropDownList = Color.FromArgb(227, 240, 252);
            Padding = new Padding(4, 0, 4, 0);
            Size = new Size(121, 21);
        }

        public event EventHandler SelectedIndexChanged = delegate { };

        internal override bool FocusInternal()
        {
            var res = base.FocusInternal();

            filter = Text;

            return res;
        }

        protected override void OnKeyPress(KeyPressEventArgs args)
        {
            base.OnKeyPress(args);

            var e = args.uwfKeyArgs;

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
                    filter += c;
                    var itemIndex = listBox.FindItemIndex(x => x != null && x.ToString().Contains(filter));
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
            var borderColor = BorderColor;
            var arrowColor = Color.FromArgb(96, 96, 96);
            if (Enabled)
            {
                switch (DropDownStyle)
                {
                    case ComboBoxStyle.DropDown:
                        if (Focused || uwfHovered)
                        {
                            backColor = HoverColor;
                            borderColor = BorderColorHovered;
                        }
                        break;
                    case ComboBoxStyle.DropDownList:
                        backColor = Focused || uwfHovered ? HoverColorDropDownList : BackColorDropDownList;
                        break;
                }

                if (Focused || uwfHovered)
                    borderColor = BorderColorHovered;
            }
            else
            {
                backColor = DisabledColor;
                borderColor = BorderColorDisabled;
            }

            borderPen.Color = borderColor;

            #endregion

            g.uwfFillRectangle(backColor, 0, 0, Width, Height);

            switch (DropDownStyle)
            {
                case ComboBoxStyle.DropDown:

                    if (Focused && Enabled)
                    {
                        var filterBuffer = g.uwfDrawTextField(filter, Font, ForeColor, 2, 0, Width - downButtonWidth, Height,
                            HorizontalAlignment.Left);
                        if (filterBuffer != filter)
                        {
                            if (listBox != null && !listBox.IsDisposed && !listBox.Disposing)
                            {
                                listBox.Dispose();
                                listBox = null;
                                listBoxOpened = false;
                            }
                            CreateListBox(filterBuffer);
                        }
                        filter = filterBuffer;
                    }
                    else
                        g.uwfDrawString(Text, Font, ForeColor, 5, 0, Width - downButtonWidth, Height);

                    if (uwfHovered)
                    {
                        var bRect = GetButtonRect();
                        var mclient = PointToClient(MousePosition);
                        var downButtonBackColor = backColor;

                        if (bRect.Contains(mclient))
                        {
                            arrowColor = Color.Black;
                            downButtonBackColor = HoverColorDropDownList;
                            downButtonBorderPen.Color = BorderColorHovered;
                        }
                        else
                            downButtonBorderPen.Color = Color.Transparent;

                        g.uwfFillRectangle(downButtonBackColor, bRect);
                        g.DrawLine(downButtonBorderPen, bRect.X, bRect.Y, bRect.X, bRect.Y + bRect.Height);
                    }
                    break;
                case ComboBoxStyle.DropDownList:
                    g.uwfDrawString(Text, Font, ForeColor, 5, 0, Width - downButtonWidth, Height);
                    break;
            }

            g.uwfDrawImage(uwfAppOwner.Resources.CurvedArrowDown, arrowColor, Width - 16 - 1, Height / 2 - 8, 16, 16);
            g.DrawRectangle(borderPen, 0, 0, Width, Height);
        }
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            SelectedIndexChanged(this, e);
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
                listBox.Font = Font;
                listBox.uwfContext = true;
                listBox.Width = Width;
                listBox.ItemHeight = ItemHeight;
                listBox.Height = listBox.ItemHeight * (Items.Count > MaxDropDownItems ? MaxDropDownItems : Items.Count);
                listBox.WrapText = false;
                listBox.uwfShadowBox = true;
                listBox.BorderColor = listBox.BorderSelectColor;
                if (listBox.Height < listBox.ItemHeight) listBox.Height = listBox.ItemHeight;

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

                var gpoint = PointToScreen(Point.Empty);
                listBox.Location = new Point(gpoint.X, gpoint.Y + Height);
                listBox.MouseUp += ListBoxOnMouseUp;
                listBox.KeyDown += ListBoxOnKeyDown;
                listBox.Disposed += ListBoxOnDisposed;
            }
            else
                listBoxOpened = false;
        }
        private Rectangle GetButtonRect()
        {
            return new Rectangle(Width - downButtonWidth, 1, downButtonWidth, Height - 2);
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
            private ComboBox _owner;

            public ObjectCollection(ComboBox owner)
            {
                _owner = owner;
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
