namespace System.Windows.Forms
{
    using System.Drawing;

    public class TreeNode
    {
        public static readonly Color DEFAULT_FORE_COLOR = Color.Empty;

        internal int index;
        internal int indexY;
        internal TreeNode parent;
        internal TreeView treeView;
        internal TreeNodeCollection nodes;
        internal int textWidth;
        internal string tagString;

        private bool expanded;
        private Color foreColor = DEFAULT_FORE_COLOR;
        private int imageIndex = -1;
        private string text;
        private bool visible = true;

        public TreeNode(string text) : this(text, null)
        {
            this.text = text;
        }
        public TreeNode(string text, TreeNode[] children)
        {
            ImageColor = Color.White;
            Enabled = true;
            BackColor = Color.Transparent;
            ImageIndex_Expanded = -1;
            ImageIndex_Collapsed = -1;
            this.text = text;

            if (children != null)
                Nodes.AddRange(children);
        }
        
        internal TreeNode(TreeView treeView) : this(string.Empty)
        {
            this.treeView = treeView;
        }

        public Color BackColor { get; set; }
        public Rectangle Bounds { get; internal set; }
        public bool Enabled { get; set; }
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
        public Color ImageColor { get; set; }
        public int ImageIndex
        {
            get
            {
                var value = imageIndex;
                if (expanded)
                {
                    if (ImageIndex_Expanded >= 0)
                        value = ImageIndex_Expanded;
                }
                else if (ImageIndex_Collapsed >= 0)
                    value = ImageIndex_Collapsed;

                return value;
            }
            set { imageIndex = value; }
        }
        public int ImageIndex_Collapsed { get; set; }
        public int ImageIndex_Expanded { get; set; }
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
        public Font NodeFont { get; set; }
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
                if (treeView == null && parent != null)
                    treeView = parent.TreeView;
                return treeView;
            }
        }

        internal Color ForeColorInternal
        {
            get { return ForeColor.IsEmpty ? TreeView.ForeColor : ForeColor; }
        }
        internal Font NodeFontInternal
        {
            get
            {
                if (NodeFont != null)
                    return NodeFont;

                return TreeView.Font;
            }
        }
        internal Color SelectColorInternal
        {
            get
            {
                if (!IsSelected) return TreeView.uwfItemHoveredColor;
                
                if (!TreeView.Focused)
                    return TreeView.uwfItemSelectedUnfocusedColor;

                return TreeView.uwfItemSelectedColor;

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
            return GetXIndex(this, 0);
        }
        internal int GetYIndex()
        {
            return GetYIndex(this, 0);
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

        private static int GetVisibleNodesAmount(TreeNode node, int currentAmount)
        {
            if (node.IsVisible) currentAmount++;
            else return currentAmount;

            if (node.IsExpanded)
                for (int i = 0; i < node.Nodes.Count; i++)
                    currentAmount = GetVisibleNodesAmount(node.Nodes[i], currentAmount);

            return currentAmount;
        }
        private int GetXIndex(TreeNode currentNode, int currentX)
        {
            if (currentNode.parent == null) return currentX;

            return GetXIndex(currentNode.parent, currentX) + 1;
        }
        private int GetYIndex(TreeNode currentNode, int currentY)
        {
            if (currentNode.parent == null) return currentY;

            if (currentNode == TreeView.root) return --currentY; // was currentY--, should check it.

            if (currentNode.parent != TreeView.root && currentNode.parent.IsVisible) currentY++;

            for (int i = 0; i < currentNode.index; i++)
                currentY += GetVisibleNodesAmount(currentNode.parent.Nodes[i], 0);

            return GetYIndex(currentNode.parent, currentY);
        }
    }
}
