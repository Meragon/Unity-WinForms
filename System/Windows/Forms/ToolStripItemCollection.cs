using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Forms
{
    public class ToolStripItemCollection : IEnumerator<ToolStripItem>, IEnumerable<ToolStripItem>
    {
        private List<ToolStripItem> _items = new List<ToolStripItem>();
        private ToolStrip _owner;

        public ToolStripItemCollection(ToolStrip owner, ToolStripItem[] value)
        {
            _owner = owner;
            if (value != null)
                _items.AddRange(value);
        }

        public virtual ToolStripItem this[int index] { get { return _items[index]; } }

        private void _InitItem(ToolStripItem item)
        {
            item.Parent = _owner;
            item.Owner = _owner;
            if (_owner != null && _owner.Orientation == Orientation.Horizontal)
                item.TextAlign.Alignment = Drawing.StringAlignment.Center;
        }

        public ToolStripItem Add(string text)
        {
            ToolStripButton item = new ToolStripButton();
            _InitItem(item);
            item.Text = text;
            _items.Add(item);
            return item;
        }
        public int Add(ToolStripItem value)
        {
            _InitItem(value);
            _items.Add(value);
            return _items.Count - 1;
        }
        public void AddRange(ToolStripItem[] toolStripItems)
        {
            foreach (var item in toolStripItems)
            {
                _InitItem(item);
            }
            _items.AddRange(toolStripItems);
        }
        public void AddRange(ToolStripItemCollection toolStripItems)
        {
            foreach (var item in toolStripItems)
            {
                _InitItem(item);
            }
            _items.AddRange(toolStripItems);
        }
        public virtual void Clear()
        {
            _items.Clear();
        }
        public bool Contains(ToolStripItem value)
        {
            return _items.Contains(value);
        }
        public void CopyTo(ToolStripItem[] array, int index)
        {
            _items.CopyTo(array, index);
        }
        public int Count { get { return _items.Count; } }
        public int IndexOf(ToolStripItem value)
        {
            for (int i = 0, index = 0; i < _items.Count; i++)
            {
                if (_items[i] == value)
                    return index;
                if (_items[i].JustVisual == false)
                    index++;
            }

            return -1;
        }
        public void Insert(int index, ToolStripItem value)
        {
            _items.Insert(index, value);
        }
        public void Remove(ToolStripItem value)
        {
            _items.Remove(value);
        }
        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        public ToolStripItem Current
        {
            get { return _items.GetEnumerator().Current; }
        }
        object IEnumerator.Current
        {
            get { return _items.GetEnumerator().Current; }
        }
        public bool MoveNext()
        {
            return _items.GetEnumerator().MoveNext();
        }
        public void Reset()
        {

        }
        public void Dispose()
        {
            _items.GetEnumerator().Dispose();
        }
        public IEnumerator<ToolStripItem> GetEnumerator()
        {
            return _items.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }
}
