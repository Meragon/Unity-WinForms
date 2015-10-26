using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Forms
{
    public class TreeNodeCollection : IList, ICollection, IEnumerable
    {
        private List<TreeNode> items = new List<TreeNode>();
        private TreeNode owner;

        internal TreeNodeCollection(TreeNode owner)
        {
            this.owner = owner;
        }

        public virtual TreeNode this[int index]
        {
            get
            {
                if (index < 0 || index >= items.Count)
                    throw new ArgumentOutOfRangeException("index");
                return items[index];
            }
            set
            {
                if (index < 0 || index >= items.Count)
                    throw new ArgumentOutOfRangeException("index");

                value.parent = this.owner;
                value.index = index;
                items[index] = value;
            }
        }
        public virtual TreeNode this[string key]
        {
            get
            {
                if (string.IsNullOrEmpty(key))
                    return null;

                int index = this.IndexOfKey(key);
                if (IsValidIndex(index))
                    return this[index];

                return null;
            }
        }

        public int Count
        {
            get { return items.Count; }
        }
        public bool IsReadOnly { get { return false; } }

        bool ICollection.IsSynchronized { get { return false; } }
        object ICollection.SyncRoot { get { return this; } }
        int IList.Add(object node)
        {
            if (node == null)
                throw new ArgumentNullException("node");
            if (node is TreeNode)
                return this.Add((TreeNode)node);
            return this.Add(node.ToString()).index;
        }
        bool IList.Contains(object node)
        {
            return node is TreeNode && this.Contains((TreeNode)node);
        }
        int IList.IndexOf(object node)
        {
            if (node is TreeNode)
                return this.IndexOf((TreeNode)node);

            return -1;
        }
        void IList.Insert(int index, object node)
        {
            if (node is TreeNode)
            {
                this.Insert(index, (TreeNode)node);
                return;
            }
            throw new ArgumentException(node.ToString());
        }
        bool IList.IsFixedSize { get { return false; } }
        void IList.Remove(object node)
        {
            if (node is TreeNode)
                this.Remove((TreeNode)node);
        }
        object IList.this[int index]
        {
            get { return this[index]; }
            set
            {
                if (value is TreeNode)
                {
                    this[index] = (TreeNode)value;
                    return;
                }
                throw new ArgumentException(value.ToString());
            }
        }

        private int AddInternal(TreeNode node, int delta)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            node.parent = this.owner;
            node.index = items.Count;

            items.Add(node);

            if (owner.TreeView != null)
            {
                node.treeView = owner.TreeView;
                owner.TreeView.Refresh();
            }

            return node.index;
        }
        private bool IsValidIndex(int index)
        {
            return index >= 0 && index < Count;
        }
        internal void UpdateIndexes()
        {
            for (int i = 0; i < items.Count; i++)
                items[i].index = i;
        }

        public virtual int Add(TreeNode node)
        {
            return AddInternal(node, 0);
        }
        public virtual TreeNode Add(string text)
        {
            TreeNode node = new TreeNode(text);
            this.Add(node);
            return node;
        }
        public virtual TreeNode Add(string key, string text)
        {
            TreeNode node = new TreeNode(text);
            node.Name = key;
            this.Add(node);
            return node;
        }
        public virtual void Clear()
        {
            items.Clear();
            if (owner.TreeView != null)
                owner.TreeView.Refresh();
        }
        public bool Contains(TreeNode node)
        {
            return this.IndexOf(node) != -1;
        }
        public void CopyTo(Array dest, int index)
        {
            if (this.Count > 0)
                Array.Copy(items.ToArray(), 0, dest, index, this.Count);
        }
        public IEnumerator GetEnumerator()
        {
            return items.GetEnumerator();
        }
        public int IndexOf(TreeNode node)
        {
            for (int i = 0; i < this.Count; i++)
                if (this[i] == node)
                    return i;

            return -1;
        }
        public virtual int IndexOfKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                return -1;

            for (int i = 0; i < Count; i++)
                if (SafeCompareStrings(items[i].Name, key, true))
                    return i;

            return -1;
        }
        public virtual void Insert(int index, TreeNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            node.parent = this.owner;
            node.index = index;
            items.Insert(index, node);

            UpdateIndexes();

            if (owner.TreeView != null)
            {
                node.treeView = owner.TreeView;
                owner.TreeView.Refresh();
            }
        }
        public void Remove(TreeNode node)
        {
            int index = IndexOf(node);

            if (index != -1)
            {
                items.RemoveAt(index);
                UpdateIndexes();

                if (owner.TreeView != null)
                    owner.TreeView.Refresh();
            }
        }
        public virtual void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException("index");

            items.RemoveAt(index);
            UpdateIndexes();

            if (owner.TreeView != null)
                owner.TreeView.Refresh();
        }

        public static bool SafeCompareStrings(string string1, string string2, bool ignoreCase)
        {
            return string1 != null && string2 != null && string1.Length == string2.Length && string.Compare(string1, string2, ignoreCase) == 0;
        }
    }
}
