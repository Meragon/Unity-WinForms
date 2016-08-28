using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public class TreeNode
    {
        private TreeNodeCollection nodes;

        internal int index;
        internal TreeNode parent;
        internal TreeView treeView;
        
        private bool expanded;
        private int imageIndex = -1;
        private bool visible = true;

        public static readonly Color DEFAULT_FORE_COLOR = Color.FromArgb(42, 42, 42);

        public Color BackColor { get; set; }
        public Rectangle Bounds { get; internal set; }
        public bool Enabled { get; set; }
        public TreeNode FirstNode
        {
            get
            {
                if (this.Nodes.Count == 0) return null;

                return Nodes[0];
            }
        }
        public Color ForeColor { get; set; }
        public Color ImageColor { get; set; }
        public int ImageIndex { get { return imageIndex; } set { imageIndex = value; } }
        public int Index { get { return index; } }
        public bool IsExpanded { get { return expanded; } internal set { expanded = value; } }
        public bool IsSelected { get { return TreeView.SelectedNode == this; } }
        public bool IsVisible { get { return visible; } }
        public TreeNode LastNode
        {
            get
            {
                if (this.Nodes.Count == 0) return null;

                return Nodes[Nodes.Count - 1];
            }
        }
        public string Name { get; set; }
        public TreeNode NextNode
        {
            get
            {
                if (this.index + 1 < this.parent.Nodes.Count)
                    return this.parent.Nodes[this.index + 1];

                return null;
            }
        }
        public TreeNodeCollection Nodes
        {
            get
            {
                if (nodes == null) nodes = new TreeNodeCollection(this);
                return nodes;
            }
            internal set { nodes = value; }
        }
        public TreeNode Parent { get { return parent; } }
        public TreeNode PrevNode
        {
            get
            {
                if (this.index + 1 >= 0)
                    return this.parent.Nodes[this.index - 1];

                return null;
            }
        }
        public object Tag { get; set; }
        public string Text { get; set; }
        public TreeView TreeView
        {
            get
            {
                if (treeView == null)
                    if (parent != null) treeView = parent.TreeView;
                return treeView;
            }
        }

        internal TreeNode(TreeView treeView) : this("")
        {
            this.treeView = treeView;
        }
        public TreeNode(string text) : this(text, null)
        {
            this.Text = text;
        }
        public TreeNode(string text, TreeNode[] children)
        {
            this.BackColor = Color.Transparent;
            this.Enabled = true;
            this.ForeColor = DEFAULT_FORE_COLOR;
            this.ImageColor = Color.White;
            this.Text = text;
            if (children != null)
                this.Nodes.AddRange(children);
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

            if (currentNode == TreeView.root) return currentY--;

            if (currentNode.parent != TreeView.root && currentNode.parent.IsVisible) currentY++;

            for (int i = 0; i < currentNode.index; i++)
                currentY += _GetVisibleNodesAmount(currentNode.parent.Nodes[i], 0);

            return _GetYIndex(currentNode.parent, currentY);
        }

        public void Collapse()
        {
            expanded = false;
            TreeView.Refresh();
        }
        internal void CollapseInternal()
        {
            expanded = false;
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
        internal void ExpandAllInternal()
        {
            expanded = true;
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].IsExpanded == false)
                {
                    Nodes[i].expanded = true;
                    Nodes[i].ExpandAllInternal();
                }
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
        public void Remove()
        {
            this.Remove(true);
        }
        internal void Remove(bool notify)
        {
            for (int i = 0; i < Nodes.Count; i++)
                this.Nodes[i].Remove(false);

            if (this.parent != null)
            {
                this.parent.Nodes.RemoveAt(index);
                this.parent = null;
            }

            this.treeView = null;
        }
        public void Toggle()
        {
            if (expanded == true) Collapse();
            else Expand();
        }

        public override string ToString()
        {
            return "TreeNode: " + ((this.Text == null) ? "" : this.Text);
        }
    }
}
