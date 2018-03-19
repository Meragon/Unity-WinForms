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
            var item = new ToolStripButton();
            item.Text = text;
            AddInternal(item);
            return item;
        }
        public int Add(ToolStripItem value)
        {
            AddInternal(value);
            return items.Count - 1;
        }
        public void AddRange(ToolStripItem[] toolStripItems)
        {
            AddInternal(toolStripItems);
        }
        public void AddRange(ToolStripItemCollection toolStripItems)
        {
            foreach (var item in toolStripItems)
                AddInternal(item);
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
                index++;
            }

            return -1;
        }
        public void Insert(int index, ToolStripItem value)
        {
            items.Insert(index, value);
            SetOwner(value);
        }
        public void Remove(ToolStripItem value)
        {
            items.Remove(value);
            OnAfterRemove(value);
        }
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= items.Count)
                return;

            var item = items[index];
            items.RemoveAt(index);
            OnAfterRemove(item);
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

        private void AddInternal(params ToolStripItem[] newItems)
        {
            this.items.AddRange(newItems);
            for (int i = 0; i < newItems.Length; i++)
            {
                var item = newItems[i];
                if (item == null)
                    throw new ArgumentException("item is null");

                SetOwner(item);
            }

            if (owner != null)
                owner.UpdateSize();
        }
        private void OnAfterRemove(ToolStripItem item)
        {
            if (item == null)
                return;
            item.SetOwner(null);
        }
        private void SetOwner(ToolStripItem item)
        {
            if (item == null) return;
            if (item.Owner != null)
                item.Owner.Items.Remove(item);

            item.SetOwner(owner);
        }
    }
}
