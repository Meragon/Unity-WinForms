namespace System.Windows.Forms
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public class TabControl : Control
    {
        internal bool buttonAdjustSize;
        internal Padding buttonTextPadding = new Padding(6, 0, 6, 0);
        internal List<TabPageButton> pagesButtons = new List<TabPageButton>();
        internal int tabPageCount;
        internal int tabViewIndex;

        private readonly Pen borderPen = new Pen(Color.FromArgb(172, 172, 172));
        private readonly Control pagesButtonsPanel;
        private Size itemSize = new Size(62, 21);
        private Button navigationButtonLeft;
        private Button navigationButtonRight;
        private Padding padding;
        private int selectedIndex = -1;

        public TabControl()
        {
            TabPages = new TabPageCollection(this);
            Padding = new Padding(3);

            pagesButtonsPanel = new Control();
            pagesButtonsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pagesButtonsPanel.BackColor = Color.Transparent;
            pagesButtonsPanel.Name = "buttonsPanel";
            pagesButtonsPanel.Size = new Size(Width, ItemSize.Height);

            ((ControlCollection)Controls).AddInternal(pagesButtonsPanel);
        }

        public event TabControlEventHandler Deselected;
        public event TabControlCancelEventHandler Deselecting;
        public event TabControlEventHandler Selected;
        public event TabControlCancelEventHandler Selecting;

        public new Rectangle DisplayRectangle
        {
            get
            {
                return new Rectangle(
                    Padding.Left,
                    ItemSize.Height + Padding.Top,
                    Width - Padding.Horizontal,
                    Height - ItemSize.Height - Padding.Vertical);
            }
        }
        public Size ItemSize
        {
            get { return itemSize; }
            set
            {
                itemSize = value;
                UpdateSizes();
            }
        }
        public new Padding Padding
        {
            get { return padding; }
            set
            {
                padding = value;
                UpdateSizes();
            }
        }
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { SelectTab(value); }
        }
        public TabPage SelectedTab
        {
            get
            {
                if (selectedIndex == -1) return null;
                return TabPages[selectedIndex];
            }
            set
            {
                var tabIndex = TabPages.IndexOf(value);
                SelectTab(tabIndex);
            }
        }
        public int TabCount { get { return tabPageCount; } }
        public TabPageCollection TabPages { get; private set; }

        internal Color uwfBorderColor
        {
            get { return borderPen.Color; }
            set { borderPen.Color = value; }
        }

        protected override Size DefaultSize
        {
            get { return new Size(200, 100); }
        }

        private bool AllButtonsRendered
        {
            get
            {
                if (pagesButtons.Count == 0) return true;
                var lastButton = pagesButtons.Last();

                return lastButton.Location.X + lastButton.Width < pagesButtonsPanel.Width;
            }
        }
        private int HeaderWidth
        {
            get
            {
                int w = 0;

                for (int i = 0; i < pagesButtons.Count; i++)
                    w += pagesButtons[i].Width;

                return w;
            }
        }
        private int MaxVisibleTabIndex
        {
            get
            {
                if (navigationButtonLeft == null)
                    return tabPageCount;

                for (int i = tabViewIndex, vItems = 0; i < pagesButtons.Count; i++, vItems++)
                {
                    var isVisible = pagesButtons[i].Location.X < navigationButtonLeft.Location.X;
                    if (!isVisible)
                        return vItems;
                }
                return tabPageCount;
            }
        }

        public void SelectTab(int index)
        {
            HideSelectedPage();

            if (index == -1)
            {
                selectedIndex = index;
                return;
            }

            if (index >= TabPages.Count)
                throw new ArgumentOutOfRangeException("index");

            var selectedPage = TabPages[index];
            var cancelArgs = new TabControlCancelEventArgs(selectedPage, index, false, TabControlAction.Selecting);

            OnSelecting(cancelArgs);

            if (cancelArgs.Cancel)
                return;

            selectedIndex = index;

            pagesButtons[selectedIndex].ShowInternal();
            selectedPage.Visible = true;

            OnSelected(new TabControlEventArgs(selectedPage, index, TabControlAction.Selected));
        }
        public void SelectTab(string tabPageName)
        {
            var index = TabPages.IndexOfKey(tabPageName);
            if (index == -1)
                return;
            SelectTab(index);
        }
        public void SelectTab(TabPage tabPage)
        {
            var index = TabPages.IndexOf(tabPage);
            if (index == -1)
                return;
            SelectTab(index);
        }
        
        internal int AddTabPage(TabPage tabPage)
        {
            var pageButton = new TabPageButton(this, tabPageCount);
            pageButton.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
            pageButton.adjustWidthToText = buttonAdjustSize;
            pageButton.uwfBorderHoverColor = Color.Transparent;
            pageButton.uwfBorderColor = Color.Transparent;
            pageButton.Location = new Point(tabPageCount * pageButton.Width - tabPageCount, 0);
            pageButton.Size = itemSize;
            pageButton.Text = tabPage.Text;
            pageButton.textPadding = buttonTextPadding;
            pageButton.HideInternal();

            pagesButtonsPanel.Controls.Add(pageButton);
            pagesButtons.Add(pageButton);

            tabPageCount++;

            if (TabPages.Count > 0 && SelectedIndex == -1)
                SelectTab(0);

            UpdateButtons();

            return tabPageCount;
        }
        internal int FindTabPage(TabPage tabPage)
        {
            if (TabPages != null)
                for (int i = 0; i < tabPageCount; i++)
                    if (TabPages[i] != null && TabPages[i].Equals(tabPage))
                        return i;

            return -1;
        }
        internal bool HideSelectedPage()
        {
            if (selectedIndex == -1) return true;
            if (selectedIndex >= TabPages.Count) return true;

            var selectedPage = TabPages[selectedIndex];
            var cancelArgs = new TabControlCancelEventArgs(selectedPage, selectedIndex, false, TabControlAction.Deselecting);

            OnDeselecting(cancelArgs);

            if (cancelArgs.Cancel)
                return false;

            pagesButtons[selectedIndex].HideInternal();
            TabPages[selectedIndex].Visible = false;

            OnDeselected(new TabControlEventArgs(selectedPage, selectedIndex, TabControlAction.Deselected));

            return true;
        }
        internal void RemoveTabPage(int index)
        {
            if (index < 0 || index >= tabPageCount)
                throw new ArgumentException("tabControl.RemoveTabPage(index)");

            tabPageCount--;

            TabPages.RemoveAt(index);
            Controls.Remove(pagesButtons[index]);
            pagesButtons.RemoveAt(index);

            if (selectedIndex == tabPageCount)
                SelectedIndex = tabPageCount - 1;

            SelectTab(SelectedIndex);
            UpdateButtons();
        }
        internal void SetPageButtonWidth(int index, int width)
        {
            pagesButtons[index].Width = width;
            UpdateButtons();
        }
        internal void UpdateButtons()
        {
            if (navigationButtonLeft == null || navigationButtonRight == null)
            {
                for (int i = 0; i < pagesButtons.Count; i++)
                    pagesButtons[i].Visible = true;
            }
            else
            {
                for (int i = 0; i < tabViewIndex; i++)
                    pagesButtons[i].Visible = false;
            }

            for (int i = tabViewIndex, locX = 0; i < pagesButtons.Count; i++)
            {
                var button = pagesButtons[i];
                button.Location = new Point(locX, button.Location.Y);
                button.Visible = button.Location.X < pagesButtonsPanel.Width;

                locX += button.Width - 1;
            }
        }
        internal void UpdateSizes()
        {
            for (int i = 0; i < TabPages.Count; i++)
            {
                var page = TabPages[i];
                page.Location = DisplayRectangle.Location;
                page.Size = new Size(DisplayRectangle.Width, DisplayRectangle.Height);
            }
            if (pagesButtonsPanel != null)
                pagesButtonsPanel.Height = ItemSize.Height;
        }

        protected internal override void uwfOnLatePaint(PaintEventArgs e)
        {
            // Borders.
            if (tabPageCount > 0)
            {
                e.Graphics.DrawLine(borderPen, 0, Height - 1, Width, Height - 1); // Botttom.
                e.Graphics.DrawLine(borderPen, 0, ItemSize.Height, 0, Height); // Left.
                e.Graphics.DrawLine(borderPen, Width - 1, ItemSize.Height, Width - 1, Height); // Right.

                if (selectedIndex != -1 && selectedIndex < tabPageCount)
                {
                    var activeButton = pagesButtons[selectedIndex];
                    if (activeButton.Visible)
                    {
                        e.Graphics.DrawLine(borderPen, 0, ItemSize.Height, activeButton.Location.X + 1, ItemSize.Height);
                        e.Graphics.DrawLine(borderPen, activeButton.Location.X + activeButton.Width - 1, ItemSize.Height, Width, ItemSize.Height);
                    }
                    else
                        e.Graphics.DrawLine(borderPen, 0, ItemSize.Height, Width, ItemSize.Height);
                }
            }
            else // Draw empty.
            {
                e.Graphics.DrawRectangle(borderPen, 0, 0, Width, Height);
            }
        }

        protected override Control.ControlCollection CreateControlsInstance()
        {
            return new ControlCollection(this);
        }
        protected virtual void OnDeselected(TabControlEventArgs e)
        {
            var handler = Deselected;
            if (handler != null)
                handler(this, e);
        }
        protected virtual void OnDeselecting(TabControlCancelEventArgs e)
        {
            var handler = Deselecting;
            if (handler != null)
                handler(this, e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.uwfFillRectangle(BackColor, 0, 0, Width, Height);
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            CheckNavButtons();
        }
        protected void RemoveAll()
        {
            tabPageCount = 0;
            selectedIndex = -1;

            for (int i = 0; i < pagesButtons.Count; i++)
                pagesButtons[i].Dispose();
            pagesButtons.Clear();

            for (int i = 0; i < Controls.Count; i++)
                if (Controls[i] is TabPage)
                {
                    Controls[i].Dispose();
                    i--;
                }

            navigationButtonLeft.Dispose();
            navigationButtonRight.Dispose();
            navigationButtonLeft = null;
            navigationButtonRight = null;
        }
        protected virtual void OnSelected(TabControlEventArgs e)
        {
            var handler = Selected;
            if (handler != null)
                handler(this, e);
        }
        protected virtual void OnSelecting(TabControlCancelEventArgs e)
        {
            var handler = Selecting;
            if (handler != null)
                handler(this, e);
        }

        internal void CheckNavButtons()
        {
            if (HeaderWidth > Width)
            {
                // Create nav. buttons.
                if (navigationButtonLeft == null || navigationButtonRight == null)
                {
                    navigationButtonRight = new Button();
                    navigationButtonRight.Anchor = AnchorStyles.Right | AnchorStyles.Top;
                    navigationButtonRight.Image = uwfAppOwner.Resources.ArrowRight;
                    navigationButtonRight.uwfImageColor = Color.Black;
                    navigationButtonRight.Size = new Size(16, 16);
                    navigationButtonRight.Location = new Point(Width - navigationButtonRight.Width, ItemSize.Height - navigationButtonRight.Height - 2);
                    navigationButtonRight.Click += (s, a) =>
                    {
                        if (AllButtonsRendered) return;

                        tabViewIndex++;
                        if (tabViewIndex > MaxVisibleTabIndex)
                            tabViewIndex = MaxVisibleTabIndex;
                        UpdateButtons();
                    };

                    navigationButtonLeft = new Button();
                    navigationButtonLeft.Anchor = AnchorStyles.Right | AnchorStyles.Top;
                    navigationButtonLeft.Image = uwfAppOwner.Resources.ArrowLeft;
                    navigationButtonLeft.uwfImageColor = Color.Black;
                    navigationButtonLeft.Size = navigationButtonRight.Size;
                    navigationButtonLeft.Location = new Point(navigationButtonRight.Location.X - navigationButtonRight.Width, navigationButtonRight.Location.Y);
                    navigationButtonLeft.Click += (s, a) =>
                    {
                        tabViewIndex--;
                        if (tabViewIndex < 0)
                            tabViewIndex = 0;
                        UpdateButtons();
                    };

                    ((ControlCollection)Controls).AddInternal(navigationButtonLeft);
                    ((ControlCollection)Controls).AddInternal(navigationButtonRight);

                    pagesButtonsPanel.Width = navigationButtonLeft.Location.X - 2;
                }
            }
            else
            {
                if (navigationButtonLeft != null)
                {
                    navigationButtonLeft.Dispose();
                    navigationButtonLeft = null;
                }
                if (navigationButtonRight != null)
                {
                    navigationButtonRight.Dispose();
                    navigationButtonRight = null;
                }
            }

            UpdateButtons();
        }

        public new class ControlCollection : Control.ControlCollection
        {
            private readonly TabControl owner;

            public ControlCollection(TabControl owner) : base(owner)
            {
                this.owner = owner;
            }

            public override void Add(Control value)
            {
                var page = value as TabPage;
                if (page == null) 
                    throw new ArgumentException("value");

                page.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
                page.Bounds = owner.DisplayRectangle;
                page.Visible = false;

                base.Add(value);

                owner.AddTabPage(page);
            }
            public override void Remove(Control item)
            {
                base.Remove(item);

                var page = item as TabPage;
                if (page == null) return;

                int index = owner.FindTabPage(page);
                int curSelectedIndex = owner.SelectedIndex;

                if (index != -1)
                {
                    owner.RemoveTabPage(index);
                    if (index == curSelectedIndex)
                        owner.SelectedIndex = 0;
                }
            }

            internal void AddInternal(Control value)
            {
                base.Add(value);
            }
        }
        public class TabPageCollection : IList
        {
            private readonly TabControl owner;

            public TabPageCollection(TabControl owner)
            {
                this.owner = owner;
            }

            public int Count { get { return owner.tabPageCount; } }
            public bool IsReadOnly { get { return false; } }

            bool IList.IsFixedSize
            {
                get { return false; }
            }
            bool ICollection.IsSynchronized
            {
                get { return false; }
            }
            object ICollection.SyncRoot
            {
                get { return this; }
            }
            object IList.this[int index]
            {
                get { return this[index]; }
                set { throw new NotImplementedException(); }
            }

            public virtual TabPage this[int index]
            {
                get
                {
                    for (int i = 0, innerIndex = 0; i < owner.Controls.Count; i++)
                    {
                        var page = owner.Controls[i] as TabPage;
                        if (page == null) continue;

                        if (index == innerIndex)
                            return page;
                        
                        innerIndex++;
                    }
                    return null;
                }
            }
            public virtual TabPage this[string key]
            {
                get { return owner.Controls.Find(x => x.Name == key) as TabPage; }
            }

            public void Add(string text)
            {
                TabPage page = new TabPage();
                page.Text = text;
                Add(page);
            }
            public void Add(TabPage value)
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                owner.Controls.Add(value);
                owner.CheckNavButtons();
            }
            public void Add(string key, string text)
            {
                TabPage page = new TabPage();
                page.Name = key;
                page.Text = text;
                Add(page);
            }
            public void Add(string key, string text, int imageIndex)
            {
                TabPage page = new TabPage();
                page.ImageIndex = imageIndex;
                page.Name = key;
                page.Text = text;
                Add(page);
            }
            public void Add(string key, string text, string imageKey)
            {
                TabPage page = new TabPage();
                page.ImageKey = imageKey;
                page.Name = key;
                page.Text = text;
            }
            public void AddRange(TabPage[] pages)
            {
                if (pages == null) throw new ArgumentNullException("pages");
                for (int i = 0; i < pages.Length; i++)
                    Add(pages[i]);
            }
            public virtual void Clear()
            {
                owner.RemoveAll();
            }
            public bool Contains(TabPage page)
            {
                return owner.Controls.Contains(page);
            }
            public virtual bool ContainsKey(string key)
            {
                return owner.Controls.Find(x => x.Name == key) != null;
            }
            public IEnumerator GetEnumerator()
            {
                return owner.Controls.GetEnumerator();
            }
            public int IndexOf(TabPage page)
            {
                for (int i = 0, pageIndex = 0; i < owner.Controls.Count; i++)
                {
                    if (owner.Controls[i] is TabPage)
                    {
                        if (owner.Controls[i] == page)
                            return pageIndex;
                        pageIndex++;
                    }
                }
                return -1;
            }
            public virtual int IndexOfKey(string key)
            {
                for (int i = 0, pageIndex = 0; i < owner.Controls.Count; i++)
                {
                    if (owner.Controls[i] is TabPage)
                    {
                        if (owner.Controls[i].Name == key)
                            return pageIndex;
                        pageIndex++;
                    }
                }
                return -1;
            }
            public void Insert(int index, string text)
            {
                TabPage page = new TabPage();
                page.Text = text;
                Insert(index, page);
            }
            public void Insert(int index, TabPage tabPage)
            {
                if (tabPage == null) throw new ArgumentNullException("tabPage");

                owner.Controls.Insert(index, tabPage);
                owner.CheckNavButtons();
            }
            public void Insert(int index, string key, string text)
            {
                TabPage page = new TabPage();
                page.Name = key;
                page.Text = text;
                Insert(index, page);
            }
            public void Insert(int index, string key, string text, int imageIndex)
            {
                TabPage page = new TabPage();
                page.ImageIndex = imageIndex;
                page.Name = key;
                page.Text = text;
                Insert(index, page);
            }
            public void Insert(int index, string key, string text, string imageKey)
            {
                TabPage page = new TabPage();
                page.ImageKey = imageKey;
                page.Name = key;
                page.Text = text;
                Insert(index, page);
            }
            public void Remove(TabPage value)
            {
                owner.Controls.Remove(value);
                owner.CheckNavButtons();
            }
            public void RemoveAt(int index)
            {
                var page = this[index];
                Remove(page);
            }
            public virtual void RemoveByKey(string key)
            {
                int index = IndexOfKey(key);
                if (index != -1)
                    RemoveAt(index);
            }

            int IList.Add(object value)
            {
                var page = value as TabPage;
                if (page == null) throw new ArgumentException("value");

                Add(page);
                return owner.tabPageCount - 1;
            }
            bool IList.Contains(object value)
            {
                var page = value as TabPage;
                if (page == null) throw new ArgumentException("value");

                return Contains(page);
            }
            int IList.IndexOf(object value)
            {
                var page = value as TabPage;
                if (page == null) throw new ArgumentException("value");

                return IndexOf(page);
            }
            void IList.Insert(int index, object value)
            {
                var page = value as TabPage;
                if (page == null) throw new ArgumentException("value");

                Insert(index, page);
            }
            void IList.Remove(object value)
            {
                var page = value as TabPage;
                if (page == null) throw new ArgumentException("value");

                Remove(page);
            }

            void ICollection.CopyTo(Array array, int index)
            {
                if (Count > 0)
                    owner.Controls.CopyTo(array, index);
            }
        }

        internal class TabPageButton : Button
        {
            internal bool adjustWidthToText;
            internal Color enabledBackColor = Color.White;
            internal Color enabledBackHoverColor = Color.White;
            internal Color disabledBackColor = SystemColors.Control;
            internal Color disabledBackHoverColor = Color.FromArgb(223, 238, 252);
            internal Padding textPadding = new Padding(6, 0, 6, 0);

            private readonly Pen borderPen = new Pen(Color.Gray);
            private readonly TabControl owner;
            private readonly int index;
            private bool hidden;

            public TabPageButton(TabControl owner, int index)
            {
                this.owner = owner;
                this.index = index;

                uwfBorderSelectColor = Color.Transparent;
            }

            public void HideInternal()
            {
                hidden = true;
                Update();
            }
            public void ShowInternal()
            {
                hidden = false;
                Update();
            }
            public void Update()
            {
                if (hidden)
                {
                    BackColor = disabledBackColor;
                    Location = new Point(Location.X, 2);
                    Height = owner.ItemSize.Height - 2;
                    uwfHoverColor = disabledBackHoverColor;
                }
                else
                {
                    BackColor = enabledBackColor;
                    Location = new Point(Location.X, 0);
                    Height = owner.ItemSize.Height;
                    uwfHoverColor = enabledBackHoverColor;
                }
            }

            protected override void OnClick(EventArgs e)
            {
                base.OnClick(e);

                owner.SelectTab(index);
            }
            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                var g = e.Graphics;

                if (adjustWidthToText)
                {
                    Width = (int)g.MeasureString(Text, Font).Width + textPadding.Horizontal;
                    adjustWidthToText = false;
                    owner.UpdateButtons();
                }

                var w = Width;
                var h = Height;

                // Draw borders.
                borderPen.Color = owner.uwfBorderColor;
                g.DrawLine(borderPen, 0, 0, w, 0); // Top.
                g.DrawLine(borderPen, 0, 0, 0, h - 1); // Left.
                g.DrawLine(borderPen, w - 1, 0, w - 1, h - 1); // Right.
            }
        }
    }
}
