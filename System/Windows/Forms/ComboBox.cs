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
        private const int ARROW_IMAGE_SIZE = 16;

        private readonly Pen borderPen = new Pen(Color.Transparent);
        private readonly Pen downButtonBorderPen = new Pen(Color.Transparent);

        private string keyFilter;
        private bool resetKeyFilter;
        private int itemHeight = 15;
        private ComboListBoxPanel listBoxPanel; // Need this to move a listBox down and overlap it's painting. 
        private bool listBoxOpened;
        private bool listBoxRecentlyClosed;
        private int maxDropDownItems = 8;
        private int selectedIndex = -1;
        private bool selectTextOnFocus;

        public ComboBox()
        {
            Items = new ObjectCollection(this);

            AutoCompleteMode = AutoCompleteMode.None;
            AutoCompleteSource = AutoCompleteSource.None;
            BackColor = Color.White;
            DropDownStyle = ComboBoxStyle.DropDown;
            Padding = new Padding(4, 0, 4, 0);
            Text = ""; // At the start DrawTextField will return string.Empty, it should be equal to filter value.
        }

        public event EventHandler DropDown;
        public event EventHandler DropDownClosed;
        public event EventHandler SelectedIndexChanged;

        public AutoCompleteMode AutoCompleteMode { get; set; }
        public AutoCompleteSource AutoCompleteSource { get; set; }
        public ComboBoxStyle DropDownStyle { get; set; }
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
            get { return new Size(121, 21); }
        }

        private int SelectedIndexInternal
        {
            get { return listBoxPanel == null ? SelectedIndex : listBoxPanel.listBox.SelectedIndex; }
        }
        
        internal override bool FocusInternal()
        {
            if (Enabled && DropDownStyle == ComboBoxStyle.DropDown && !Focused)
                selectTextOnFocus = true;
            
            return base.FocusInternal();
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
                    SelectItem(SelectedIndexInternal + 1);
                    break;
             
                case Keys.Left:
                    if (DropDownStyle == ComboBoxStyle.DropDownList)
                        SelectItem(SelectedIndexInternal - 1);
                    break;
                
                case Keys.Right:
                    if (DropDownStyle == ComboBoxStyle.DropDownList)
                        SelectItem(SelectedIndexInternal + 1);
                    break;
                
                case Keys.Up:
                    SelectItem(SelectedIndexInternal - 1);
                    break;
                
                case Keys.PageDown:
                    SelectItem(SelectedIndexInternal + MaxDropDownItems);
                    break;
                
                case Keys.PageUp:
                    SelectItem(SelectedIndexInternal - MaxDropDownItems);
                    break;
                
                case Keys.Home:
                    SelectItem(0);
                    break;
                
                case Keys.End:
                    SelectItem(Items.Count);
                    break;
                
                case Keys.Return:
                    if (listBoxPanel != null && !listBoxPanel.Disposing && !listBoxPanel.IsDisposed)
                    {
                        listBoxPanel.listBox.SelectItem(listBoxPanel.listBox.SelectedIndex);
                        ApplySelectedItem();
                    }
                    break;
            }
            
            if (listBoxPanel != null && DropDownStyle == ComboBoxStyle.DropDownList)
            {
                // Filter with key input.
                if (resetKeyFilter)
                {
                    resetKeyFilter = false;
                    keyFilter = "";
                }

                char c = KeyHelper.GetLastInputChar();
                if (char.IsLetterOrDigit(c) || char.IsPunctuation(c))
                {
                    keyFilter += char.ToLower(c);
                    var itemIndex = listBoxPanel.listBox.FindItemIndex(x => x != null && x.ToString().ToLower().Contains(keyFilter));
                    if (itemIndex > -1)
                        listBoxPanel.listBox.SelectItem(itemIndex);
                }
            }
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            
            Focus();
            
            if (DropDownStyle == ComboBoxStyle.DropDownList || e.X >= Width - DOWN_BUTTON_WIDTH)
            {
                resetKeyFilter = true;
                ToggleListBox();
            }
        }
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            
            OnMouseDown(e);
        }
        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);

            if (Enabled && DropDownStyle == ComboBoxStyle.DropDown)
            {
                var localMouse = PointToClient(MousePosition);
                if (localMouse.X < Width - DOWN_BUTTON_WIDTH)
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
            base.OnMouseWheel(e);
            
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

            if (listBoxRecentlyClosed) 
                // Wait for a frame to prevent creating new listBox when previous was recently closed.
                // (issue with disposing context controls and processing mouse input at the same time)
                listBoxRecentlyClosed = false;
            
            var g = e.Graphics;
            
            // Colors.
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

            var height = Height;
            var width = Width;
            var textWidth = width - DOWN_BUTTON_WIDTH;
            if (uwfWrapText == false)
                textWidth *= 2;

            // Back.
            g.uwfFillRectangle(backColor, 0, 0, width, height);

            // Style.
            switch (DropDownStyle)
            {
                case ComboBoxStyle.DropDown:

                    if (Focused && Enabled)
                    {
                        // Select text.
                        if (selectTextOnFocus || shouldFocus)
                            g.uwfFocusNext();

                        var filterBuffer = g.uwfDrawTextField(Text, Font, ForeColor, 2, 0, textWidth, height, HorizontalAlignment.Left);
                        if (filterBuffer != Text)
                        {
                            if (listBoxPanel == null || listBoxPanel.IsDisposed || listBoxPanel.Disposing) 
                                CreateListBox();

                            listBoxPanel.UpdateListBoxItems(filterBuffer);
                        }
                        
                        Text = filterBuffer;

                        if (shouldFocus)
                        {
                            e.Graphics.uwfFocus();
                            shouldFocus = false;
                        }
                        
                        if (selectTextOnFocus)
                        {
                            var te = UnityEngine.GUIUtility.GetStateObject(typeof(UnityEngine.TextEditor),
                                UnityEngine.GUIUtility.keyboardControl) as UnityEngine.TextEditor;
                            if (te != null)
                                te.SelectAll();

                            selectTextOnFocus = false;
                        }
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
            var arrowImage = uwfAppOwner.Resources.CurvedArrowDown; 
            var arrowX = width - ARROW_IMAGE_SIZE;
            var arrowY = (height - ARROW_IMAGE_SIZE) / 2f;

            g.uwfFillRectangle(backColor, arrowX, 0, ARROW_IMAGE_SIZE, height);
            g.uwfDrawImage(arrowImage, arrowColor, arrowX, arrowY, ARROW_IMAGE_SIZE, ARROW_IMAGE_SIZE);

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
            if (listBoxPanel == null) return;
            var listSelectedItem = listBoxPanel.listBox.SelectedItem;
            
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i] != listSelectedItem) continue;
                
                Text = listSelectedItem != null ? listSelectedItem.ToString() : "";
                SelectedItem = listSelectedItem;
                break;
            }
            
            listBoxPanel.Dispose();
            listBoxPanel = null;
        }
        private void CloseListBox()
        {
            listBoxOpened = false;
            
            if (listBoxPanel == null)
                return;

            listBoxPanel.listBox.SelectedIndexChanged -= ListBoxOnSelectedIndexChanged;
            listBoxPanel.listBox.MouseUp -= ListBoxOnMouseUp;
            listBoxPanel.listBox.KeyDown -= ListBoxOnKeyDown;
            listBoxPanel.listBox.Disposed -= ListBoxOnDisposed;
            listBoxPanel.Dispose();
            listBoxPanel = null;
            
            OnDropDownClosed(EventArgs.Empty);
            
            listBoxRecentlyClosed = true;
        }
        private void CreateListBox()
        {
            if (listBoxPanel != null)
            {
                listBoxPanel = null;
                return;
            }
            
            if (listBoxRecentlyClosed)
                return;

            var screenLocation = PointToScreen(Point.Empty);
            var height = ItemHeight * (Items.Count > MaxDropDownItems ? MaxDropDownItems : Items.Count);
            if (height < ItemHeight) 
                height = ItemHeight;
            
            listBoxPanel = new ComboListBoxPanel(this);
            listBoxPanel.Location = new Point(screenLocation.X, screenLocation.Y + Height);
            listBoxPanel.Size = new Size(Width, height + 4); // + Vertical padding for borders.
            listBoxPanel.InitializeComponent();
            
            listBoxPanel.listBox.SelectedIndexChanged += ListBoxOnSelectedIndexChanged;
            listBoxPanel.listBox.MouseUp += ListBoxOnMouseUp;
            listBoxPanel.listBox.KeyDown += ListBoxOnKeyDown;
            listBoxPanel.listBox.Disposed += ListBoxOnDisposed;

            listBoxPanel.uwfContext = true;
            
            listBoxPanel.UpdateListBoxItems(string.Empty);
            
            OnDropDown(EventArgs.Empty);
            
            listBoxOpened = true;
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
            CloseListBox();
        }
        private void ListBoxOnSelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedIndex = Items.FindIndex(x => x == listBoxPanel.listBox.SelectedItem);
        }
        private void SelectItem(int index)
        {
            if (listBoxPanel != null)
            {
                listBoxPanel.listBox.SelectItem(index);
                return;
            }
            
            if (index < 0 && Items.Count == 0) return;

            if (index < 0) index = 0;
            if (index >= Items.Count) index = Items.Count - 1;

            SelectedIndex = index;
        }
        private void ToggleListBox()
        {
            if (listBoxOpened)
                CloseListBox();
            else
                CreateListBox();
        }
        
        private void UpdateText()
        {
            string text = string.Empty;

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
            private readonly List<object> items;
            private readonly ComboBox owner;

            public ObjectCollection(ComboBox owner)
            {
                this.owner = owner;
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
            public void Remove(object value)
            {
                items.Remove(value);
            }
            public void RemoveAt(int index)
            {
                items.RemoveAt(index);
            }
        }

        private class ComboListBoxPanel : Control
        {
            public ComboListBox listBox;

            private readonly ComboBox owner;
            private float currentY;
            private float listMoveSpeed;
            
            public ComboListBoxPanel(ComboBox owner)
            {
                this.owner = owner;

                BackColor = Color.Transparent;

                uwfShadowBox = true;
                uwfShadowHandler += UwfShadowHandler;
            }
            
            public void InitializeComponent()
            {
                listBox = new ComboListBox();
                listBox.BackColor = owner.BackColor;
                listBox.BorderColor = owner.uwfBorderColorHovered;
                listBox.Font = owner.Font;
                listBox.ForeColor = owner.ForeColor;
                listBox.Width = owner.Width;
                listBox.IntegralHeight = false;
                listBox.ItemHeight = owner.ItemHeight;
                listBox.Size = new Size(Width, Height);
                listBox.Location = new Point(0, -listBox.Height);

                currentY = listBox.Location.Y;
                
                listBox.uwfWrapText = false;
                //listBox.uwfShadowBox = true;
                listBox.uwfItemHoverColor = owner.uwfListItemHoverColor;
                listBox.uwfSelectionBackColor = owner.uwfListItemSelectedBackgroundColor;
                listBox.uwfSelectionForeColor = owner.uwfListItemSelectedForeColor;

                Controls.Add(listBox);

                listMoveSpeed = Math.Max(240, Height * 2);
            }
            public void UpdateListBoxItems(string listFilter)
            {
                if (listBox == null)
                    return;

                var filterLower = listFilter != null ? listFilter.ToLower() : "";
            
                listBox.Items.Clear();
            
                bool selectedIndexChanged = false;
                for (int i = 0; i < owner.Items.Count; i++)
                {
                    var item = owner.Items[i];
                    if (owner.DropDownStyle == ComboBoxStyle.DropDownList || string.IsNullOrEmpty(filterLower))
                        listBox.Items.Add(item);
                    else
                    {
                        var itemString = item.ToString().ToLower();
                        if (!itemString.StartsWith(filterLower)) continue;

                        listBox.Items.Add(item);
                        
                        if (itemString != filterLower) continue;

                        listBox.SelectedIndex = i;
                        selectedIndexChanged = true;
                    }
                }
                
                if (!selectedIndexChanged)
                {
                    listBox.SelectedIndex = owner.SelectedIndex;
                    listBox.EnsureVisible();
                }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                
                if (currentY < 0)
                {
                    currentY = MathHelper.Step(currentY, 0, listMoveSpeed);
                    listBox.Location = new Point(0, (int) currentY);
                }
            }
            
            private void UwfShadowHandler(PaintEventArgs e)
            {
                if (listBox == null)
                    return;
            
                var listScreenLocation = listBox.uwfShadowPointToScreen(Point.Empty);
                var thisScreenLocation = this.uwfShadowPointToScreen(Point.Empty);
                
                int shX = listScreenLocation.X + 6;
                int shY = thisScreenLocation.Y + 6;
                var shadowColor = defaultShadowColor;
                var localWidth = listBox.Width;
                var localHeight = listBox.Height - (thisScreenLocation.Y - listScreenLocation.Y);
                var graphics = e.Graphics;
                
                graphics.uwfFillRectangle(shadowColor, shX + 6, shY + 6, localWidth - 12, localHeight - 12);
                graphics.uwfFillRectangle(shadowColor, shX + 5, shY + 5, localWidth - 10, localHeight - 10);
                graphics.uwfFillRectangle(shadowColor, shX + 4, shY + 4, localWidth - 8, localHeight - 8);
                graphics.uwfFillRectangle(shadowColor, shX + 3, shY + 3, localWidth - 6, localHeight - 6);
                graphics.uwfFillRectangle(shadowColor, shX + 2, shY + 2, localWidth - 4, localHeight - 4);
            }
        }
        
        private class ComboListBox : ListBox
        {
            private readonly Pen borderPen = new Pen(Color.Gray);
            
            public ComboListBox()
            {
                BorderStyle = BorderStyle.FixedSingle;
            }

            public Color BorderColor
            {
                get { return borderPen.Color; }
                set { borderPen.Color = value; }
            }

            

            protected override Size DefaultSize
            {
                get { return new Size(base.DefaultSize.Width, 0); }
            }

            protected internal override void uwfOnLatePaint(PaintEventArgs e)
            {
                e.Graphics.DrawRectangle(borderPen, 0, 0, Width, Height);
            }
        }
    }
}
