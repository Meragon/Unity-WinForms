namespace System.Windows.Forms
{
    using System.Collections;
    using System.Drawing;
    using System.Globalization;

    /// <summary>
    /// Link collection is not working. And probably won't any time soon.
    /// </summary>
    public class LinkLabel : Label, IButtonControl
    {
        private static readonly LinkComparer linkComparer = new LinkComparer();

        private readonly ArrayList links = new ArrayList(2);
        private readonly Pen underlinePen = new Pen(Color.Blue);
        private Link focusLink;
        private LinkCollection linkCollection;
        private Color linkColor = Color.Blue;
        private Color visitedLinkColor = Color.Purple;

        public LinkLabel()
        {
            Links.Add(new Link(0, -1));
            InvalidateLink(null);
        }

        public event LinkLabelLinkClickedEventHandler LinkClicked;

        public DialogResult DialogResult { get; set; }
        public LinkBehavior LinkBehavior { get; set; }
        public Color LinkColor
        {
            get { return linkColor; }
            set
            {
                if (linkColor == value)
                    return;

                linkColor = value;
                InvalidateLink(null);
            }
        }
        public LinkCollection Links
        {
            get
            {
                if (linkCollection == null)
                    linkCollection = new LinkCollection(this);

                return linkCollection;
            }
        }
        public bool LinkVisited
        {
            get
            {
                if (links.Count == 0)
                    return false;

                return ((Link)links[0]).Visited;
            }
            set
            {
                if (value == LinkVisited)
                    return;

                if (links.Count == 0)
                    Links.Add(new Link(this));

                ((Link)links[0]).Visited = value;
            }
        }
        public Color VisitedLinkColor
        {
            get { return visitedLinkColor; }
            set
            {
                if (visitedLinkColor == value) return;

                visitedLinkColor = value;
                InvalidateLink(null);
            }
        }

        private Link FocusLink
        {
            get { return focusLink; }
            set
            {
                if (focusLink == value) return;
                if (focusLink != null)
                    InvalidateLink(focusLink);

                focusLink = value;

                if (focusLink != null)
                    InvalidateLink(focusLink);
            }
        }

        public void NotifyDefault(bool value)
        { }
        public void PerformClick()
        {
            if (FocusLink == null && Links.Count > 0)
            {
                string text = Text;
                foreach (Link link in Links)
                {
                    int charStart = ConvertToCharIndex(link.Start, text);
                    int charEnd = ConvertToCharIndex(link.Start + link.Length, text);
                    if (link.Enabled && LinkInText(charStart, charEnd - charStart))
                    {
                        FocusLink = link;
                        break;
                    }
                }
            }

            if (FocusLink != null)
                OnLinkClicked(new LinkLabelLinkClickedEventArgs(FocusLink));
        }

        internal override void RaiseOnMouseClick(MouseEventArgs e)
        {
            PerformClick();
        }

        protected virtual void OnLinkClicked(LinkLabelLinkClickedEventArgs e)
        {
            var linkClicked = LinkClicked;
            if (linkClicked != null)
                linkClicked(this, e);
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            Cursor.Current = Cursors.Hand;
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            Cursor.Current = null;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            //// It's too complex...

            base.OnPaint(e);

            // Underline.
            switch (LinkBehavior)
            {
                case LinkBehavior.SystemDefault:
                case LinkBehavior.AlwaysUnderline:
                    e.Graphics.DrawLine(underlinePen, Padding.Left, Height - 4, Width - Padding.Right, Height - 4);
                    break;
                case LinkBehavior.HoverUnderline:
                    if (uwfHovered)
                        e.Graphics.DrawLine(underlinePen, Padding.Left, Height - 4, Width - Padding.Right, Height - 4);
                    break;
            }
        }

        private static int ConvertToCharIndex(int index, string text)
        {
            if (index <= 0)
                return 0;

            if (string.IsNullOrEmpty(text))
                return index;

            var stringInfo = new StringInfo(text);
            int numTextElements = stringInfo.LengthInTextElements;

            if (index > numTextElements)
                return index - numTextElements + text.Length;

            return stringInfo.SubstringByTextElements(0, index).Length;
        }

        private void InvalidateLink(Link link)
        {
            ForeColor = LinkVisited == false ? LinkColor : VisitedLinkColor;
            underlinePen.Color = ForeColor;
        }
        private bool LinkInText(int start, int length)
        {
            return start >= 0 && start < Text.Length && length > 0;
        }

        public class Link
        {
            internal int length;
            internal LinkLabel owner;
            internal LinkState state = LinkState.Normal;

            private bool enabled = true;
            private string description;
            private object linkData;
            private string name;
            private int start;
            private object userData;

            public Link()
            {
            }
            public Link(int start, int length)
            {
                this.start = start;
                this.length = length;
            }
            public Link(int start, int length, object linkData)
            {
                this.start = start;
                this.length = length;
                this.linkData = linkData;
            }

            internal Link(LinkLabel owner)
            {
                this.owner = owner;
            }

            public string Description
            {
                get { return description; }
                set { description = value; }
            }
            public bool Enabled
            {
                get { return enabled; }
                set
                {
                    if (enabled == value) return;

                    enabled = value;

                    if ((state & (LinkState.Hover | LinkState.Active)) != 0)
                        state &= ~(LinkState.Hover | LinkState.Active);

                    if (owner != null)
                        owner.InvalidateLink(this);
                }
            }
            public int Length
            {
                get
                {
                    if (length == -1)
                    {
                        if (owner != null && !string.IsNullOrEmpty(owner.Text))
                        {
                            StringInfo stringInfo = new StringInfo(owner.Text);
                            return stringInfo.LengthInTextElements - Start;
                        }

                        return 0;
                    }
                    return length;
                }
                set
                {
                    if (length == value) return;

                    length = value;
                    if (owner != null)
                        owner.Invalidate();
                }
            }
            public object LinkData
            {
                get { return linkData; }
                set { linkData = value; }
            }
            public string Name
            {
                get { return name == null ? "" : name; }
                set { this.name = value; }
            }
            public int Start
            {
                get
                {
                    return start;
                }
                set
                {
                    if (start == value) return;
                    start = value;

                    if (owner == null) return;
                    owner.links.Sort(linkComparer);
                    owner.Invalidate();
                }
            }
            public object Tag
            {
                get { return userData; }
                set { userData = value; }
            }
            public bool Visited
            {
                get
                {
                    return (state & LinkState.Visited) == LinkState.Visited;
                }
                set
                {
                    bool old = Visited;

                    if (value)
                        state |= LinkState.Visited;
                    else
                        state &= ~LinkState.Visited;

                    if (old != Visited && owner != null)
                        owner.InvalidateLink(this);
                }
            }
        }
        public class LinkCollection : IList
        {
            private readonly LinkLabel owner;
            private int lastAccessedIndex = -1;

            public LinkCollection(LinkLabel owner)
            {
                if (owner == null)
                    throw new ArgumentNullException("owner");
                this.owner = owner;
            }

            public int Count
            {
                get
                {
                    return owner.links.Count;
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return this;
                }
            }
            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }
            bool IList.IsFixedSize
            {
                get
                {
                    return false;
                }
            }
            public bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            public virtual Link this[int index]
            {
                get
                {
                    return (Link)owner.links[index];
                }
                set
                {
                    owner.links[index] = value;
                    owner.links.Sort(linkComparer);
                    owner.Invalidate();
                }
            }
            public virtual Link this[string key]
            {
                get
                {
                    if (string.IsNullOrEmpty(key))
                        return null;

                    int index = IndexOfKey(key);
                    if (IsValidIndex(index))
                        return this[index];

                    return null;
                }
            }
            object IList.this[int index]
            {
                get
                {
                    return this[index];
                }
                set
                {
                    var link = value as Link;
                    if (link == null)
                        throw new ArgumentException("value");

                    this[index] = link;
                }
            }

            public int Add(Link value)
            {
                if (value == null)
                    return -1;

                if (owner.links.Count == 1 && this[0].Start == 0 && this[0].length == -1)
                {
                    owner.links.Clear();
                    owner.FocusLink = null;
                }

                value.owner = this.owner;

                owner.links.Add(value);

                if (owner.AutoSize)
                {
                    owner.AdjustSize();
                    owner.Invalidate();
                }

                if (owner.Links.Count > 1)
                    owner.links.Sort(linkComparer);

                owner.Invalidate();

                if (owner.Links.Count > 1)
                    return IndexOf(value);

                return 0;
            }
            public virtual void Clear()
            {
                bool doLayout = owner.links.Count > 0 && owner.AutoSize;
                owner.links.Clear();

                if (doLayout)
                {
                    owner.AdjustSize();
                    owner.Invalidate();
                }

                owner.Invalidate();
            }
            public bool Contains(Link link)
            {
                return owner.links.Contains(link);
            }
            public IEnumerator GetEnumerator()
            {
                if (owner.links != null)
                    return owner.links.GetEnumerator();

                return new Link[0].GetEnumerator();
            }
            public int IndexOf(Link link)
            {
                return owner.links.IndexOf(link);
            }
            public virtual int IndexOfKey(string key)
            {
                if (string.IsNullOrEmpty(key))
                    return -1;

                if (IsValidIndex(lastAccessedIndex))
                    if (WindowsFormsUtils.SafeCompareStrings(this[lastAccessedIndex].Name, key, true))
                        return lastAccessedIndex;

                for (int i = 0; i < this.Count; i++)
                    if (WindowsFormsUtils.SafeCompareStrings(this[i].Name, key, true))
                    {
                        lastAccessedIndex = i;
                        return i;
                    }

                lastAccessedIndex = -1;
                return -1;
            }
            public void Remove(Link value)
            {
                if (value.owner != this.owner)
                    return;

                owner.links.Remove(value);

                if (owner.AutoSize)
                {
                    owner.AdjustSize();
                    owner.Invalidate();
                }

                owner.links.Sort(linkComparer);
                owner.Invalidate();

                if (owner.FocusLink == null && owner.links.Count > 0)
                    owner.FocusLink = (Link)owner.links[0];
            }
            public void RemoveAt(int index)
            {
                Remove(this[index]);
            }
            public virtual void RemoveByKey(string key)
            {
                int index = IndexOfKey(key);
                if (IsValidIndex(index))
                {
                    RemoveAt(index);
                }
            }

            int IList.Add(object value)
            {
                var link = value as Link;
                if (link != null)
                    return Add(link);

                throw new ArgumentException("value");
            }
            bool IList.Contains(object value)
            {
                var l = value as Link;
                if (l != null)
                    return Contains(l);

                return false;
            }
            void ICollection.CopyTo(Array array, int index)
            {
                owner.links.CopyTo(array, index);
            }
            int IList.IndexOf(object value)
            {
                var l = value as Link;
                if (l != null)
                    return IndexOf(l);

                return -1;
            }
            void IList.Insert(int index, object value)
            {
                var link = value as Link;
                if (link != null)
                    Add(link);

                throw new ArgumentException("value");
            }
            void IList.Remove(object value)
            {
                var link = value as Link;
                if (link != null)
                    Remove(link);
            }

            private bool IsValidIndex(int index)
            {
                return index >= 0 && index < Count;
            }
        }
        private class LinkComparer : IComparer
        {
            int IComparer.Compare(object x, object y)
            {
                int pos1 = ((Link)x).Start;
                int pos2 = ((Link)y).Start;

                return pos1 - pos2;
            }
        }
    }
}
