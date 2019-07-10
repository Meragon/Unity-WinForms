namespace System.Windows.Forms
{
    using System.Collections;
    using System.Collections.Generic;

    public class TreeNodeCollection : IList
    {
        private readonly List<TreeNode> items = new List<TreeNode>();
        private readonly TreeNode owner;

        internal TreeNodeCollection(TreeNode owner)
        {
            this.owner = owner;
            this.owner.nodes = this;
        }

        public int Count
        {
            get { return items.Count; }
        }
        public bool IsReadOnly { get { return false; } }

        bool ICollection.IsSynchronized { get { return false; } }
        object ICollection.SyncRoot { get { return this; } }
        bool IList.IsFixedSize { get { return false; } }

        object IList.this[int index]
        {
            get { return this[index]; }
            set
            {
                var node = value as TreeNode;
                if (node != null)
                {
                    this[index] = node;
                    return;
                }
                
                throw new ArgumentException(value.ToString());
            }
        }
        public virtual TreeNode this[int index]
        {
            get
            {
                if (index < 0 || index >= items.Count)
                    throw new ArgumentOutOfRangeException("index " + index);
                return items[index];
            }
            set
            {
                if (index < 0 || index >= items.Count)
                    throw new ArgumentOutOfRangeException("index");

                value.parent = owner;
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

                int index = IndexOfKey(key);
                if (IsValidIndex(index))
                    return this[index];

                return null;
            }
        }

        public virtual int Add(TreeNode node)
        {
            return AddInternal(node, 0);
        }
        public virtual TreeNode Add(string text)
        {
            TreeNode node = new TreeNode(text);
            Add(node);
            return node;
        }
        public virtual TreeNode Add(string key, string text)
        {
            TreeNode node = new TreeNode(text);
            node.Name = key;
            Add(node);
            return node;
        }
        public virtual void AddRange(TreeNode[] nodes)
        {
            if (nodes == null)
                throw new ArgumentNullException("nodes");

            if (nodes.Length == 0) return;

            for (int i = 0; i < nodes.Length; i++)
            {
                AddInternal(nodes[i], i);
            }
        }
        public virtual void Clear()
        {
            for (int i = 0; i < items.Count; i++)
                items[i].parent = null;

            items.Clear();

            if (owner.TreeView != null)
                owner.TreeView.Refresh();
        }
        public bool Contains(TreeNode node)
        {
            return IndexOf(node) != -1;
        }
        public void CopyTo(Array array, int index)
        {
            if (Count > 0)
                Array.Copy(items.ToArray(), 0, array, index, Count);
        }
        public TreeNode[] Find(string key, bool searchAllChildren)
        {
            var foundNodes = FindInternal(key, searchAllChildren, this, new ArrayList());

            var nodes = new TreeNode[foundNodes.Count];
            foundNodes.CopyTo(nodes, 0);

            return nodes;
        }
        public IEnumerator GetEnumerator()
        {
            return items.GetEnumerator();
        }
        public int IndexOf(TreeNode node)
        {
            for (int i = 0; i < Count; i++)
                if (this[i] == node)
                    return i;

            return -1;
        }
        public virtual int IndexOfKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                return -1;

            for (int i = 0; i < Count; i++)
                if (WindowsFormsUtils.SafeCompareStrings(items[i].Name, key, true))
                    return i;

            return -1;
        }
        public virtual void Insert(int index, TreeNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            node.parent = owner;
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

        int IList.Add(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            var node = value as TreeNode;
            if (node != null)
                return Add(node);
            
            return Add(value.ToString()).index;
        }
        bool IList.Contains(object value)
        {
            var node = value as TreeNode;
            return node != null && Contains(node);
        }
        int IList.IndexOf(object value)
        {
            var node = value as TreeNode;
            if (node != null)
                return IndexOf(node);

            return -1;
        }
        void IList.Insert(int index, object value)
        {
            var node = value as TreeNode;
            if (node != null)
            {
                Insert(index, node);
                return;
            }
            
            throw new ArgumentException(value.ToString());
        }
        void IList.Remove(object value)
        {
            var node = value as TreeNode;
            if (node != null)
                Remove(node);
        }

        internal void UpdateIndexes()
        {
            for (int i = 0; i < items.Count; i++)
                items[i].index = i;
        }

        private static ArrayList FindInternal(string key, bool searchAllChildren, TreeNodeCollection collection, ArrayList nodes)
        {
            if (collection == null || nodes == null)
                return null;

            var collectionCount = collection.Count;
            for (int i = 0; i < collectionCount; i++)
            {
                var node = collection[i];
                if (node == null)
                    continue;

                if (WindowsFormsUtils.SafeCompareStrings(node.Name, key, true))
                    nodes.Add(node);
            }

            if (searchAllChildren)
                for (int i = 0; i < collectionCount; i++)
                {
                    var node = collection[i];
                    if (node == null)
                        continue;

                    var nodeNodes = node.Nodes;
                    if (nodeNodes != null && nodeNodes.Count > 0)
                        nodes = FindInternal(key, true, nodeNodes, nodes);
                }
            return nodes;
        }
        private int AddInternal(TreeNode node, int delta)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            if (node.parent != null) node.Remove();

            node.parent = owner;
            node.index = items.Count;

            items.Add(node);

            if (owner.TreeView != null)
            {
                node.treeView = owner.TreeView;
            }

            return node.index;
        }
        private bool IsValidIndex(int index)
        {
            return index >= 0 && index < Count;
        }
    }
}
