using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Forms
{
    public class TreeNode
    {
        private TreeNodeCollection nodes;

        internal int index;
        internal TreeNode parent;
        internal TreeView treeView;

        private bool expaned;
        private bool selected;
        private bool visible;

        public bool IsExpanded { get { return expaned; } }
        public bool IsSelected { get { return selected; } }
        public bool IsVisible { get { return visible; } }
        public string Name { get; set; }
        public TreeNodeCollection Nodes { get { return nodes; } }
        public string Text { get; set; }
        public TreeView TreeView { get { return treeView; } }

        private void Init()
        {
            this.nodes = new TreeNodeCollection(this);
            this.visible = true;
        }

        internal TreeNode(TreeView treeView)
        {
            this.treeView = treeView;

            Init();
        }
        public TreeNode(string text)
        {
            this.nodes = new TreeNodeCollection(this);
            this.Text = text;

            Init();
        }
    }
}
