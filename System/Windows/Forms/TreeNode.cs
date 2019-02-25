namespace System.Windows.Forms
{
    using System.Drawing;

    public class TreeNode
    {
        public static readonly Color DEFAULT_FORE_COLOR = Color.FromArgb(42, 42, 42);

        internal int index;
        internal int indexY;
        internal TreeNode parent;
        internal TreeView treeView;
        internal TreeNodeCollection nodes;
        internal int textWidth;
        internal string tagString;

        private Color backColor = Color.Transparent;
        private bool enabled = true;
        private bool expanded;
        private Color foreColor = DEFAULT_FORE_COLOR;
        private Color imageColor = Color.White;
        private int imageIndex = -1;
        private int imageIndex_collapsed = -1;
        private int imageIndex_expanded = -1;
        private string text;
        private bool visible = true;

        public TreeNode(string text) : this(text, null)
        {
            this.text = text;
        }
        public TreeNode(string text, TreeNode[] children)
        {
            this.text = text;

            if (children != null)
                Nodes.AddRange(children);
        }
        internal TreeNode(TreeView treeView) : this(string.Empty)
        {
            this.treeView = treeView;
        }

        public Color BackColor
        {
            get { return backColor; }
            set { backColor = value; }
        }
        public Rectangle Bounds { get; internal set; }
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }
        public TreeNode FirstNode
        {
            get
            {
                if (Nodes.Count == 0) return null;

                return nodes[0];
            }
        }
        public Color ForeColor
        {
            get { return foreColor; }
            set { foreColor = value; }
        }
        public Color ImageColor
        {
            get { return imageColor; }
            set { imageColor = value; }
        }
        public int ImageIndex
        {
            get
            {
                if (expanded)
                {
                    if (imageIndex_expanded >= 0)
                        return imageIndex_expanded;
                }
                else if (imageIndex_collapsed >= 0)
                    return imageIndex_collapsed;

                return imageIndex;
            }
            set { imageIndex = value; }
        }
        public int ImageIndex_Collapsed { get { return imageIndex_collapsed; } set { imageIndex_collapsed = value; } }
        public int ImageIndex_Expanded { get { return imageIndex_expanded; } set { imageIndex_expanded = value; } }
        public int Index { get { return index; } }
        public bool IsExpanded { get { return expanded; } internal set { expanded = value; } }
        public bool IsSelected { get { return TreeView.SelectedNode == this; } }
        public bool IsVisible { get { return visible; } }
        public TreeNode LastNode
        {
            get
            {
                if (Nodes.Count == 0) return null;

                return nodes[nodes.Count - 1];
            }
        }
        public string Name { get; set; }
        public TreeNode NextNode
        {
            get
            {
                if (index + 1 < parent.Nodes.Count)
                    return parent.Nodes[index + 1];

                return null;
            }
        }
        public TreeNodeCollection Nodes
        {
            get
            {
                if (nodes == null)
                    nodes = new TreeNodeCollection(this);
                return nodes;
            }
        }
        public TreeNode Parent { get { return parent; } }
        public TreeNode PrevNode
        {
            get
            {
                if (index - 1 >= 0)
                    return parent.Nodes[index - 1];

                return null;
            }
        }
        public object Tag { get; set; }
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                textWidth = 0;
            }
        }
        public string ToolTipText { get; set; }
        public TreeView TreeView
        {
            get
            {
                if (treeView == null)
                    if (parent != null) treeView = parent.TreeView;
                return treeView;
            }
        }

        public void Collapse()
        {
            expanded = false;
            TreeView.Refresh();
        }
        public void EnsureVisible()
        {
            treeView.EnsureVisible(this);
        }
        public void Expand()
        {
            expanded = true;
            TreeView.Refresh();
        }
        public void ExpandAll()
        {
            ExpandAllInternal();
            TreeView.Refresh();
        }
        public void Remove()
        {
            Remove(true);
        }
        public void Toggle()
        {
            if (expanded) Collapse();
            else Expand();
        }

        public override string ToString()
        {
            return "TreeNode: " + (Text == null ? string.Empty : Text);
        }

        internal void CollapseInternal()
        {
            expanded = false;
        }
        internal void ExpandAllInternal()
        {
            expanded = true;
            var nodesCount = Nodes.Count;
            for (int i = 0; i < nodesCount; i++)
            {
                var node = Nodes[i];
                if (node.IsExpanded) continue;

                node.expanded = true;
                node.ExpandAllInternal();
            }
        }
        internal int GetXIndex()
        {
            return _GetXIndex(this, 0);
        }
        internal int GetYIndex()
        {
            return _GetYIndex(this, 0);
        }
        internal void Remove(bool notify)
        {
            if (parent != null)
            {
                parent.Nodes.RemoveAt(index);
                parent = null;
            }

            treeView = null;
        }

        private static int _GetVisibleNodesAmount(TreeNode node, int currentAmount)
        {
            if (node.IsVisible) currentAmount++;
            else return currentAmount;

            if (node.IsExpanded)
                for (int i = 0; i < node.Nodes.Count; i++)
                    currentAmount = _GetVisibleNodesAmount(node.Nodes[i], currentAmount);

            return currentAmount;
        }
        private int _GetXIndex(TreeNode currentNode, int currentX)
        {
            if (currentNode.parent == null) return currentX;

            return _GetXIndex(currentNode.parent, currentX) + 1;
        }
        private int _GetYIndex(TreeNode currentNode, int currentY)
        {
            if (currentNode.parent == null) return currentY;

            if (currentNode == TreeView.root) return --currentY; // was currentY--, should check it.

            if (currentNode.parent != TreeView.root && currentNode.parent.IsVisible) currentY++;

            for (int i = 0; i < currentNode.index; i++)
                currentY += _GetVisibleNodesAmount(currentNode.parent.Nodes[i], 0);

            return _GetYIndex(currentNode.parent, currentY);
        }
    }
}
