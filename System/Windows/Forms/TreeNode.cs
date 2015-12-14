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
        private bool selected;
        private bool visible = true;

        public Rectangle Bounds { get; internal set; }
        public int Index { get { return index; } }
        public bool IsExpanded { get { return expanded; } internal set { expanded = value; } }
        public bool IsSelected { get { return selected; } internal set { selected = value; } }
        public bool IsVisible { get { return visible; } }
        public string Name { get; set; }
        public TreeNodeCollection Nodes
        {
            get
            {
                if (nodes == null) nodes = new TreeNodeCollection(this);
                return nodes;
            }
            internal set { nodes = value; }
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

        internal TreeNode(TreeView treeView)
        {
            this.treeView = treeView;
        }
        public TreeNode(string text)
        {
            this.Text = text;
        }
        public TreeNode(string text, TreeNode[] children)
        {
            this.Text = text;
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

            if (currentNode == TreeView.root) return currentX--;

            if (currentNode.parent != TreeView.root && currentNode.parent.IsVisible) currentX++;

            currentX = _GetXIndex(currentNode.parent, currentX);

            return currentX;
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
        public void Expand()
        {
            expanded = true;
            TreeView.Refresh();
        }
        public void ExpandAll()
        {
            expanded = true;
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].IsExpanded == false)
                {
                    Nodes[i].expanded = true;
                    Nodes[i].ExpandAll();
                }
            }
            TreeView.Refresh();
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

            if (notify && this.parent != null)
            {
                this.parent.Nodes.Remove(this);

                for (int i = this.index; i < this.parent.Nodes.Count - 1; i++)
                {
                    (this.parent.Nodes[i] = this.parent.Nodes[i + 1]).index = i;
                }

                this.parent = null;
            }

            this.treeView = null;
        }
        public void Toggle()
        {
            if (expanded == true) Collapse();
            else Expand();
        }
    }
}
