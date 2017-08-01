namespace System.Windows.Forms
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;

    public class ToolStripItemCollection : IEnumerator<ToolStripItem>, IEnumerable<ToolStripItem>
    {
        private readonly List<ToolStripItem> items = new List<ToolStripItem>();
        private readonly ToolStrip owner;

        public ToolStripItemCollection(ToolStrip owner, ToolStripItem[] value)
        {
            this.owner = owner;
            if (value != null)
                items.AddRange(value);
        }

        public int Count { get { return items.Count; } }
        public ToolStripItem Current
        {
            get { return items.GetEnumerator().Current; }
        }
        object IEnumerator.Current
        {
            get { return items.GetEnumerator().Current; }
        }

        public virtual ToolStripItem this[int index] { get { return items[index]; } }

        public ToolStripItem Add(string text)
        {
            ToolStripButton item = new ToolStripButton();
            _InitItem(item);
            item.Text = text;
            items.Add(item);
            return item;
        }
        public int Add(ToolStripItem value)
        {
            _InitItem(value);
            items.Add(value);
            return items.Count - 1;
        }
        public void AddRange(ToolStripItem[] toolStripItems)
        {
            foreach (var item in toolStripItems)
            {
                _InitItem(item);
            }
            items.AddRange(toolStripItems);
        }
        public void AddRange(ToolStripItemCollection toolStripItems)
        {
            foreach (var item in toolStripItems)
            {
                _InitItem(item);
            }
            items.AddRange(toolStripItems);
        }
        public virtual void Clear()
        {
            items.Clear();
        }
        public bool Contains(ToolStripItem value)
        {
            return items.Contains(value);
        }
        public void CopyTo(ToolStripItem[] array, int index)
        {
            items.CopyTo(array, index);
        }
        public int IndexOf(ToolStripItem value)
        {
            for (int i = 0, index = 0; i < items.Count; i++)
            {
                if (items[i] == value)
                    return index;
                if (items[i].JustVisual == false)
                    index++;
            }

            return -1;
        }
        public void Insert(int index, ToolStripItem value)
        {
            items.Insert(index, value);
        }
        public void Remove(ToolStripItem value)
        {
            items.Remove(value);
        }
        public void RemoveAt(int index)
        {
            items.RemoveAt(index);
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
        public IEnumerator<ToolStripItem> GetEnumerator()
        {
            return items.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        private void _InitItem(ToolStripItem item)
        {
            item.Parent = owner;
            item.Owner = owner;
            if (owner != null && owner.Orientation == Orientation.Horizontal)
                item.TextAlign = ContentAlignment.MiddleCenter;
        }
    }
}
