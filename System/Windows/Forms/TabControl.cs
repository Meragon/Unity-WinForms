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
        public const int DefaultHeaderHeight = 24;

        public int HeaderHeight { get; set; }
        public int SelectedIndex { get { return TabPages.CurrentIndex; } }
        public TabPageCollection TabPages { get; set; }

        public TabControl()
        {
            HeaderHeight = DefaultHeaderHeight;
            TabPages = new TabPageCollection(this);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            g.FillRectangle(new SolidBrush(BackColor), 0, HeaderHeight, Width, Height - HeaderHeight);

        }
        public void UpdateTabs()
        {
            for (int i = 0; i < TabPages.Count; i++)
            {
                // Create header if not exists.
                if (TabPages[i].HeaderButton == null)
                {
                    TabPages[i].HeaderButton = CreateHeaderFromPage(TabPages[i]);
                    Controls.Add(TabPages[i].HeaderButton);
                }
                // Hide tabs.
                if (i != TabPages.CurrentIndex)
                {
                    HideTabPage(TabPages[i]);
                    SetHeaderActivity(TabPages[i].HeaderButton, false);
                }
            }
            if (TabPages.CurrentIndex != -1)
            {
                ShowTabPage(TabPages[TabPages.CurrentIndex]);
                SetHeaderActivity(TabPages[TabPages.CurrentIndex].HeaderButton, true);
            }
        }
        public void UpdateHeadersLocation()
        {
            int? offset = null;

            // Move left.
            if (TabPages.CurrentIndex - 1 >= 0 && TabPages[TabPages.CurrentIndex - 1].HeaderButton.Location.X < 0)
                offset = -TabPages[TabPages.CurrentIndex - 1].Offset;

            // Move right.
            if (TabPages.CurrentIndex + 1 < TabPages.Count && TabPages[TabPages.CurrentIndex + 1].Offset + TabPages[TabPages.CurrentIndex + 1].Width > Width)
                offset = Width - (TabPages[TabPages.CurrentIndex + 1].Offset + TabPages[TabPages.CurrentIndex + 1].Width);

            if (offset != null)
                for (int i = 0; i < TabPages.Count; i++)
                {
                    if (TabPages[i].HeaderButton != null)
                    {
                        TabPages[i].HeaderButton.Location = new Point(TabPages[i].Offset + offset.Value, TabPages[i].HeaderButton.Location.Y);
                    }
                }
        }

        private void HideTabPage(TabPage page)
        {
            if (page.Objects != null)
                foreach (var go in page.Objects)
                    go.Visible = false;
        }
        private void ShowTabPage(TabPage page)
        {
            if (page.Objects != null)
                foreach (var go in page.Objects)
                    go.Visible = true;
        }
        private Button CreateHeaderFromPage(TabPage page)
        {
            Button btn = new Button();
            btn.Name = "pageButton" + page.Index.ToString();
            btn.Text = page.Text;
            btn.Size = new Size(page.Width, HeaderHeight);
            btn.Location = new Point(page.Offset, 0);
            btn.NormalColor = Color.FromArgb(204, 206, 219);
            btn.NormalBorderColor = Color.Transparent;
            btn.HoverBorderColor = Color.Transparent;

            btn.Click += (object sender, EventArgs e) =>
            {
                TabPages[page.Index].Focus();
                UpdateTabs();
                UpdateHeadersLocation();
                page.RaiseClick();
            };

            return btn;
        }
        private void SetHeaderActivity(Button header, bool active)
        {
            if (!active)
            {
                header.NormalColor = Color.FromArgb(0, 204, 206, 219);
                header.HoverColor = Color.FromArgb(238, 238, 242);
                header.ForeColor = Color.DarkGray;
            }
            else
            {
                header.NormalColor = Color.FromArgb(204, 206, 219);
                header.HoverColor = header.NormalColor;
                header.ForeColor = Color.FromArgb(64, 64, 64);
            }
        }

        public class TabPageCollection : IEnumerator<TabPage>, IEnumerable
        {
            private List<TabPage> _tabs;
            private TabControl _owner;

            private int _currentIndex = -1;
            public int CurrentIndex { get { return _currentIndex; } }
            public int HeaderLeftOffset { get; set; }

            public TabPageCollection(TabControl owner)
            {
                _tabs = new List<TabPage>();
                _owner = owner;
            }

            public TabPage this[int index]
            {
                get { return _tabs[index]; }
                set { _tabs[index] = value; }
            }
            public TabPage Add(string text)
            {
                return Add(text, null);
            }
            public TabPage Add(string text, Control[] objects)
            {
                if (_currentIndex == -1) _currentIndex = 0;
                TabPage page = new TabPage(this, text, objects);
                _tabs.Add(page);
                return page;
            }
            public int Count { get { return _tabs.Count; } }
            public TabPage Find(Predicate<TabPage> match)
            {
                return _tabs.Find(match);
            }
            public int FindIndex(Predicate<TabPage> match)
            {
                return _tabs.FindIndex(match);
            }
            public void Focus(int index)
            {
                if (index > _tabs.Count)
                    return;
                _currentIndex = index;
            }
            public TabPage Current
            {
                get { return _tabs.GetEnumerator().Current; }
            }
            object IEnumerator.Current
            {
                get { return _tabs.GetEnumerator().Current; }
            }
            public bool MoveNext()
            {
                return _tabs.GetEnumerator().MoveNext();
            }
            public void Reset()
            {

            }
            public void Dispose()
            {
                _tabs.GetEnumerator().Dispose();
            }
            public IEnumerator GetEnumerator()
            {
                return _tabs.GetEnumerator();
            }
        }

        public class TabPage
        {
            private static int _IdPool = 0;
            private int _id;
            public int Id { get { return _id; } }
            private TabControl.TabPageCollection _owner;

            public Button HeaderButton { get; set; }
            public int Index { get { return _owner.FindIndex(x => x.Id == Id); } }
            public Control[] Objects { get; set; }
            public int Offset
            {
                get
                {
                    int offset = _owner.HeaderLeftOffset;
                    foreach (TabPage page in _owner)
                    {
                        if (page.Id != Id)
                            offset += page.Width;
                        else
                            break;
                    }
                    return offset;
                }
            }
            public string Text { get; set; }
            public int Width { get; set; }

            public TabPage(TabControl.TabPageCollection owner, string text)
            {
                _IdPool++;
                _id = _IdPool;
                _owner = owner;
                Text = text;
                Width = _CalcWidth();
            }
            public TabPage(TabControl.TabPageCollection owner, string text, Control[] objects)
            {
                _IdPool++;
                _id = _IdPool;
                _owner = owner;
                Text = text;
                Objects = objects;
                Width = _CalcWidth();
            }

            private int _CalcWidth()
            {
                int offset = 8;
                return Text.Length * 8 + offset * 2;
            }

            public void Focus()
            {
                _owner.Focus(_owner.FindIndex(x => x.Id == Id));
            }
            internal void RaiseClick()
            {
                OnClick();
            }

            public delegate void OnClickDelegate();
            public event OnClickDelegate OnClick = delegate { };
        }
    }
}
