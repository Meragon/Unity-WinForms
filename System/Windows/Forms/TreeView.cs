using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public class TreeView : Control
    {
        private TreeNodeCollection nodes;
        private TreeNode root;

        public Color BorderColor { get; set; }
        public TreeNodeCollection Nodes { get { return nodes; } }

        public TreeView()
        {
            this.BackColor = Color.White;
            this.BorderColor = Color.FromArgb(130, 135, 144);
            this.root = new TreeNode(this);
            nodes = new TreeNodeCollection(root);

            DrawNode += TreeView_DrawNode;
        }

        private void TreeView_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            // draw nodes here.
            // also should somehow remeber global rect position for current node.

            for (int i = 0; i < e.Node.Nodes.Count; i++)
                DrawNode(this, new DrawTreeNodeEventArgs(e.Graphics, e.Node.Nodes[i], new Rectangle(), e.State));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(BackColor), 0, 0, Width, Height);

            for (int i = 0; i < root.Nodes.Count; i++)
                DrawNode(this, new DrawTreeNodeEventArgs(e.Graphics, root.Nodes[i], new Rectangle(0, i * 22, Width, 22), TreeNodeStates.Default));

            e.Graphics.DrawRectangle(new Pen(BorderColor), 0, 0, Width, Height);
        }

        public event DrawTreeNodeEventHandler DrawNode = delegate { };
    }
}
