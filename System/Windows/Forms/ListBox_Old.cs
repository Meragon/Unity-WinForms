using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public class ListBox_Old : Control
    {
        private int maxItems = 0;
        private int currentScrollIndex = 0;
        public int scrollDirection = 3;

        public const int DefaultItemWidth = 128;
        public const int DefaultItemHeight = 13;

        public int ItemWidth { get; set; }
        public int ItemHeight { get; set; }
        public ObjectCollection Items { get; set; }
        public bool OwnerDraw { get; set; }

        public ListBox_Old()
        {
            ItemWidth = DefaultItemWidth;
            ItemHeight = DefaultItemHeight;
            Items = new ObjectCollection();
            Padding = new Padding(4);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            //e.Graphics.FillRectangle(Brushes.DarkRed, 0, 0, Width, Height);

            //e.Graphics.DrawRectangle(Pens.LightGray, new Rectangle(0, 0, Width, Height));
            if (!Visible) return;

            UpdateScroll();
            UpdateItems(e.Graphics);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            for (int i = 0; i < Items.Count; i++)
            {
                Rectangle itemRect = new Rectangle(Items[i].Position.X, Items[i].Position.Y, ItemWidth, ItemHeight);
                if (itemRect.Contains(e.Location))
                {
                    Items[i].OnClickCall();
                    break;
                }
            }
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            currentScrollIndex += e.Delta;
            
            if (currentScrollIndex > Items.Count - maxItems + 1)
                currentScrollIndex = Items.Count - maxItems + 1;
            if (currentScrollIndex < 0)
                currentScrollIndex = 0;
        }

        public void UpdateScroll()
        {
            if (Items.Count == 0)
            {
                return;
            }
            switch (scrollDirection)
            {
                case 1:
                case 2:
                    maxItems = (int)(Height / ItemHeight);
                    //Scroll.size = (float)maxItems / Items.Count;
                    break;
                case 3:
                case 4:
                    maxItems = (int)(Width / ItemWidth) + 1;
                    //Scroll.size = (float)maxItems / listBox.Items.Count;
                    break;
            }
        }
        public void UpdateItems(Graphics g)
        {
            bool up = currentScrollIndex < 0;

            int i = up ? maxItems : - 1;

            i = i < -1 ? -1 : i;
            i = i > Items.Count ? Items.Count : i;


            for (; ; )
            {
                if (up)
                {
                    i--;
                    if (i < 0) break;
                }
                else
                {
                    i++;
                    if (i + 1 > Items.Count) break;
                }

                if (i < currentScrollIndex || i + 1 > currentScrollIndex + maxItems)
                {
                    if (up)
                    {
                        // ScrollUp.
                        if (i < currentScrollIndex)
                            break;
                    }
                    else
                    {
                        // ScrollDown
                        if (i + 1 > currentScrollIndex + maxItems)
                            break;
                    }
                    continue;
                }

                _UpdateItem(i);
                if (!OwnerDraw)
                    g.DrawString(Items[i].Text, Font, new SolidBrush(Color.FromArgb(64, 64, 64)), Items[i].Position);

                Items[i].OnRenderCall(g);
            }
        }
        private void _UpdateItem(int index)
        {
            switch (scrollDirection)
            {
                case 1:
                case 2:
                    Items[index].Position = new Point(Padding.Left, (index * ItemHeight) + ItemHeight * currentScrollIndex);

                    break;
                case 3:
                case 4:
                    Items[index].Position = new Point(Padding.Left + (index * ItemWidth) - ItemWidth * currentScrollIndex, Padding.Top);
                    break;
            }
        }

        public class ObjectCollection : IEnumerator<ObjectCollection.ItemEntity>, IEnumerable
        {
            private List<ItemEntity> items;

            public ObjectCollection()
            {
                items = new List<ItemEntity>();
            }

            public ItemEntity this[int index]
            {
                get { return items[index]; }
                set
                {
                    items[index] = value;
                }
            }
            public ItemEntity Add(object value)
            {
                if (this.items == null)
                    throw new NullReferenceException("items");

                ItemEntity item = new ItemEntity();
                item.Value = value;
                item.Text = value.ToString();
                this.items.Add(item);
                return item;
            }
            public void Clear()
            {
                items = new List<ItemEntity>();
            }
            public int Count { get { return items.Count; } }
            public void RemoveAt(int index)
            {
                items.RemoveAt(index);
            }

            [Serializable]
            public class ItemEntity
            {
                public object Value { get; set; }
                public string Text { get; set; }
                public Point Position { get; set; }

                public void OnClickCall()
                {
                    OnClick(this);
                }
                public void OnRenderCall(Graphics g)
                {
                    OnRender(g, this);
                }

                public delegate void OnClickDelegate(ItemEntity self);
                public delegate void OnRenderDelegate(Graphics g, ItemEntity self);
                public event OnClickDelegate OnClick = delegate { };
                public event OnRenderDelegate OnRender = delegate { };
            }

            public ObjectCollection.ItemEntity Current
            {
                get { return items.GetEnumerator().Current; }
            }
            object IEnumerator.Current
            {
                get { return items.GetEnumerator().Current; }
            }
            public bool MoveNext()
            {
                return items.GetEnumerator().MoveNext();
            }
            public void Reset()
            {

            }
            public void Dispose()
            {
                items.GetEnumerator().Dispose();
            }
            public IEnumerator GetEnumerator()
            {
                return items.GetEnumerator();
            }
        }
    }

    [Serializable]
    public struct Padding
    {
        private int _left;
        private int _bottom;
        private int _right;
        private int _top;

        public int Left { get { return _left; } set { _left = value; } }
        public int Bottom { get { return _bottom; } set { _bottom = value; } }
        public int Right { get { return _right; } set { _right = value; } }
        public int Top { get { return _top; } set { _top = value; } }

        public static readonly Padding Empty = new Padding(0);

        public Padding(int all)
        {
            _left = _bottom = _right = _top = 0;

            this.Left = all;
            this.Bottom = all;
            this.Right = all;
            this.Top = all;
        }
        public Padding(int left, int bottom, int right, int top)
        {
            _left = _bottom = _right = _top = 0;

            this.Left = left;
            this.Bottom = bottom;
            this.Right = right;
            this.Top = top;
        }

        public override string ToString()
        {
            return "{ L: " + _left.ToString()  + "; B: " + _bottom.ToString() + "; R: " + _right.ToString() + "; T: " + _top + " }";
        }
    }
}
