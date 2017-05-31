using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace System.Windows.Forms
{
    public class TabControl : Control
    {
        private Size itemSize = new Size();
        private Button navigationButtonLeft;
        private Button navigationButtonRight;
        private Padding padding;
        internal List<TabPageButton> pagesButtons = new List<TabPageButton>();
        private Control pagesButtonsPanel;
        private int selectedIndex = -1;

        internal int tabPageCount = 0;
        internal int tabViewIndex = 0;

        private bool AllButtonsRendered
        {
            get
            {
                if (pagesButtons.Count == 0) return true;
                var lastButton = pagesButtons.Last();

                return lastButton.Location.X + lastButton.Width < pagesButtonsPanel.Width;
            }
        }
        public Color BorderColor { get; set; }
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
        public Size ItemSize
        {
            get { return itemSize; }
            set
            {
                itemSize = value;
                UpdateSizes();
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
                    bool isVisible = pagesButtons[i].Location.X < navigationButtonLeft.Location.X;
                    if (isVisible == false)
                        return vItems;
                }
                return tabPageCount;
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
        public TabControl.TabPageCollection TabPages { get; private set; }

        public TabControl()
        {
            Controls = new ControlCollection(this);
            TabPages = new TabPageCollection(this);

            BorderColor = Color.FromArgb(172, 172, 172);
            ItemSize = new Size(42, 30);
            Padding = new Padding(3);
            Size = new Size(200, 100);

            pagesButtonsPanel = new Control();
            pagesButtonsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pagesButtonsPanel.BackColor = Color.Transparent;
            pagesButtonsPanel.Name = "buttonsPanel";
            pagesButtonsPanel.Size = new Size(Width, ItemSize.Height);

            (Controls as ControlCollection).AddInternal(pagesButtonsPanel);
        }

        public void SelectTab(int index)
        {
            HideSelectedPage();

            selectedIndex = index;

            if (selectedIndex == -1) return;
            if (selectedIndex >= TabPages.Count) throw new ArgumentOutOfRangeException("tabControl.selectedIndex");

            pagesButtons[selectedIndex].Show();
            TabPages[index].Visible = true;
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
        public void SetPageButtonWidth(int index, int width)
        {
            pagesButtons[index].Width = width;
            UpdateButtons();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.uwfFillRectangle(BackColor, 0, 0, Width, Height);

            // Borders.
            var borderPen = new Pen(BorderColor);
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
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            CheckNavButtons();
        }

        internal int AddTabPage(TabPage tabPage)
        {
            TabPageButton pageButton = new TabPageButton(this, tabPageCount);
            pageButton.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
            pageButton.BorderHoverColor = Color.Transparent;
            pageButton.BorderColor = Color.Transparent;
            pageButton.Location = new Point(tabPageCount * pageButton.Width - tabPageCount, 0);
            pageButton.Height = ItemSize.Height;
            pageButton.Text = tabPage.Text;
            pageButton.Hide();

            pagesButtonsPanel.Controls.Add(pageButton);
            pagesButtons.Add(pageButton);

            tabPageCount++;

            if (TabPages.Count > 0 && SelectedIndex == -1)
                SelectTab(0);

            return tabPageCount;
        }
        private void CheckNavButtons()
        {
            if (HeaderWidth > Width)
            {
                // Create nav. buttons.
                if (navigationButtonLeft == null || navigationButtonRight == null)
                {
                    navigationButtonRight = new Button();
                    navigationButtonRight.Anchor = AnchorStyles.Right | AnchorStyles.Top;
                    navigationButtonRight.Image = ApplicationBehaviour.GdiImages.ArrowRight;
                    navigationButtonRight.ImageColor = Color.Black;
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
                    navigationButtonLeft.Image = ApplicationBehaviour.GdiImages.ArrowLeft;
                    navigationButtonLeft.ImageColor = Color.Black;
                    navigationButtonLeft.Size = navigationButtonRight.Size;
                    navigationButtonLeft.Location = new Point(navigationButtonRight.Location.X - navigationButtonRight.Width, navigationButtonRight.Location.Y);
                    navigationButtonLeft.Click += (s, a) =>
                    {
                        tabViewIndex--;
                        if (tabViewIndex < 0)
                            tabViewIndex = 0;
                        UpdateButtons();
                    };

                    (Controls as ControlCollection).AddInternal(navigationButtonLeft);
                    (Controls as ControlCollection).AddInternal(navigationButtonRight);

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
        internal int FindTabPage(TabPage tabPage)
        {
            if (TabPages != null)
                for (int i = 0; i < tabPageCount; i++)
                    if (TabPages[i].Equals(tabPage))
                        return i;

            return -1;
        }
        internal void HideSelectedPage()
        {
            if (selectedIndex == -1) return;
            if (selectedIndex >= TabPages.Count) return;

            pagesButtons[selectedIndex].Hide();
            TabPages[selectedIndex].Visible = false;
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
                for (int i = tabViewIndex, locX = 0; i < pagesButtons.Count; i++)
                {
                    pagesButtons[i].Location = new Point(locX, pagesButtons[i].Location.Y);
                    if (pagesButtons[i].Location.X < pagesButtonsPanel.Width)
                        pagesButtons[i].Visible = true;
                    else
                        pagesButtons[i].Visible = false;

                    locX += pagesButtons[i].Width - 1;
                }
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

        public new class ControlCollection : Control.ControlCollection
        {
            private TabControl owner;

            public ControlCollection(TabControl owner) : base(owner)
            {
                this.owner = owner;
            }

            public override void Add(Control value)
            {
                if (value is TabPage == false) throw new ArgumentException("value is not TabPage");

                var tabPage = value as TabPage;
                tabPage.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
                tabPage.Bounds = owner.DisplayRectangle;
                tabPage.Visible = false;

                base.Add(value);

                owner.AddTabPage(tabPage);
            }
            public override void Remove(Control value)
            {
                base.Remove(value);

                if (value is TabPage == false) return;

                int index = owner.FindTabPage((TabPage)value);
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
        public class TabPageCollection : IList, ICollection, IEnumerable
        {
            private TabControl owner;

            public TabPageCollection(TabControl owner)
            {
                this.owner = owner;
            }

            public int Count { get { return owner.tabPageCount; } }
            public bool IsReadOnly { get { return false; } }

            public virtual TabPage this[int index]
            {
                get
                {
                    for (int i = 0, innerIndex = 0; i < owner.Controls.Count; i++)
                    {
                        if (owner.Controls[i] is TabPage == false) continue;

                        if (index == innerIndex)
                            return owner.Controls[i] as TabPage;
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
                if (value == null) throw new NullReferenceException("tabPageCollection.Add");
                owner.Controls.Add(value);
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
                if (pages == null) throw new NullReferenceException("tabPageCollection.AddRange");
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
                if (tabPage == null) throw new NullReferenceException("tabPageCollection.Insert");

                owner.Controls.Insert(index, tabPage);
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

            bool IList.IsFixedSize
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
            bool ICollection.IsSynchronized
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
            object ICollection.SyncRoot
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
            object IList.this[int index]
            {
                get
                {
                    throw new NotImplementedException();
                }

                set
                {
                    throw new NotImplementedException();
                }
            }

            int IList.Add(object value)
            {
                if (value is TabPage == false) throw new ArgumentException("tabPageCollection.Add(value is not TabPage)");

                Add(value as TabPage);
                return owner.tabPageCount - 1;
            }
            bool IList.Contains(object value)
            {
                if (value is TabPage == false) throw new ArgumentException("tabPageCollection.Contains(value is not TabPage)");

                return Contains(value as TabPage);
            }
            int IList.IndexOf(object value)
            {
                if (value is TabPage == false) throw new ArgumentException("tabPageCollection.IndexOf(value is not TabPage)");

                return IndexOf(value as TabPage);
            }
            void IList.Insert(int index, object value)
            {
                if (value is TabPage == false) throw new ArgumentException("tabPageCollection.Insert(value is not TabPage)");

                Insert(index, value as TabPage);
            }
            void IList.Remove(object value)
            {
                if (value is TabPage == false) throw new ArgumentException("tabPageCollection.Remove(value is not TabPage)");

                Remove(value as TabPage);
            }

            void ICollection.CopyTo(Array array, int index)
            {
                if (Count > 0)
                    System.Array.Copy(owner.Controls.ToArray(), 0, array, index, Count);
            }
        }

        internal class TabPageButton : Button
        {
            private TabControl owner;
            private int index;
            private bool hidden = false;

            public Color EnabledBackColor { get; set; }
            public Color EnabledBackHoverColor { get; set; }
            public Color DisabledBackColor { get; set; }
            public Color DisabledBackHoverColor { get; set; }

            public TabPageButton(TabControl owner, int index)
            {
                this.owner = owner;
                this.index = index;

                BorderSelectColor = Color.Transparent;
                EnabledBackColor = Color.White;
                EnabledBackHoverColor = Color.White;
                DisabledBackColor = Color.FromArgb(235, 235, 235);
                DisabledBackHoverColor = Color.FromArgb(223, 238, 252);

                Click += TabPageButton_Click;
            }

            public void Hide()
            {
                hidden = true;
                Update();
            }
            public void Show()
            {
                hidden = false;
                Update();
            }
            public void Update()
            {
                if (hidden)
                {
                    BackColor = DisabledBackColor;
                    Location = new Point(Location.X, 2);
                    Height = owner.ItemSize.Height - 2;
                    HoverColor = DisabledBackHoverColor;
                }
                else
                {
                    BackColor = EnabledBackColor;
                    Location = new Point(Location.X, 0);
                    Height = owner.ItemSize.Height;
                    HoverColor = EnabledBackHoverColor;
                }
            }

            private void TabPageButton_Click(object sender, EventArgs e)
            {
                owner.SelectTab(index);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                // Draw borders.
                var borderPen = new Pen(owner.BorderColor);
                e.Graphics.DrawLine(borderPen, 0, 0, Width, 0); // Top.
                e.Graphics.DrawLine(borderPen, 0, 0, 0, Height - 1); // Left.
                e.Graphics.DrawLine(borderPen, Width - 1, 0, Width - 1, Height - 1); // Right.
                if (hidden == false)
                {

                }
                else
                {

                }
            }
        }
    }
}
