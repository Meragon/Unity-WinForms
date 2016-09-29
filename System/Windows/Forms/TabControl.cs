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
        private List<TabPageButton> buttons = new List<TabPageButton>();
        private int selectedIndex = -1;
        internal int tabCount = 0;

        public Color BorderColor { get; set; }
        public new Rectangle DisplayRectangle { get { return new Rectangle(4, 24, Width - 8, Height - 28); } }
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { SelectTab(value); }
        }
        public int TabCount { get { return tabCount; } }
        public TabControl.TabPageCollection TabPages { get; private set; }

        public TabControl()
        {
            BorderColor = Color.FromArgb(172, 172, 172);
            Controls = new ControlCollection(this);
            TabPages = new TabPageCollection(this);
        }

        public void SelectTab(int index)
        {
            HideSelectedPage();

            selectedIndex = index;

            if (selectedIndex == -1) return;
            if (selectedIndex >= TabPages.Count) throw new ArgumentOutOfRangeException("tabControl.selectedIndex");

            buttons[selectedIndex].Show();
            TabPages[index].Visible = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(BackColor, 0, 0, Width, Height);

            // Borders.
            var borderPen = new Pen(BorderColor);
            e.Graphics.DrawLine(borderPen, 0, 20, 0, Height); // Left.
            e.Graphics.DrawLine(borderPen, Width - 1, 20, Width - 1, Height); // Right.
            e.Graphics.DrawLine(borderPen, 0, Height - 1, Width, Height - 1); // Botttom.

            if (selectedIndex != -1 && selectedIndex < tabCount)
            {
                var activeButton = buttons[selectedIndex];
                e.Graphics.DrawLine(borderPen, 0, 20, activeButton.Location.X + 1, 20);
                e.Graphics.DrawLine(borderPen, activeButton.Location.X + activeButton.Width - 1, 20, Width, 20);
            }
        }

        internal int AddTabPage(TabPage tabPage)
        {
            if (TabPages.Count > 0 && SelectedIndex == -1)
                SelectedIndex = 0;

            TabPageButton pageButton = new TabPageButton(this, tabCount);
            pageButton.HoverBorderColor = Color.Transparent;
            pageButton.NormalBorderColor = Color.Transparent;
            pageButton.Location = new Point(tabCount * pageButton.Width - tabCount, 0);
            pageButton.Height = 20;
            pageButton.Text = tabPage.Text;
            pageButton.Hide();

            (Controls as ControlCollection).AddInternal(pageButton);
            buttons.Add(pageButton);

            tabCount++;
            return tabCount;
        }
        internal void HideSelectedPage()
        {
            if (selectedIndex == -1) return;
            if (selectedIndex >= TabPages.Count) return;

            buttons[selectedIndex].Hide();
            TabPages[selectedIndex].Visible = false;
        }
        protected void RemoveAll()
        {
            tabCount = 0;
            selectedIndex = -1;

            buttons.Clear();

            for (; Controls.Count > 0;)
                Controls[0].Dispose();
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
                tabPage.Anchor = AnchorStyles.All;
                tabPage.Bounds = owner.DisplayRectangle;
                tabPage.Visible = false;

                base.Add(value);

                owner.AddTabPage(tabPage);
            }
            public override void Remove(Control value)
            {
                // TODO:
                base.Remove(value);
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

            public int Count { get { return owner.tabCount; } }
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
                return owner.Controls.IndexOf(page);
            }
            public virtual int IndexOfKey(string key)
            {
                return owner.Controls.FindIndex(x => x.Name == key);
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
                return owner.tabCount - 1;
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

        private class TabPageButton : Button
        {
            private TabControl owner;
            private int index;
            private bool hidden = false;

            public TabPageButton(TabControl owner, int index)
            {
                this.owner = owner;
                this.index = index;

                Click += TabPageButton_Click;
            }

            public void Hide()
            {
                hidden = true;
                BackColor = Color.FromArgb(235, 235, 235);
                Location = new Point(Location.X, 2);
                Height = 18;
                HoverColor = Color.FromArgb(223, 238, 252);
            }
            public void Show()
            {
                hidden = false;
                BackColor = Color.White;
                Location = new Point(Location.X, 0);
                Height = 20;
                HoverColor = Color.White;
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
