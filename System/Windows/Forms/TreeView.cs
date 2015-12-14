using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public class TreeView : Control
    {
        private bool _drag;
        private TreeNode _dragNode;
        private Point _dragPosition;

        private bool _recalc = true;
        private TreeNodeCollection nodes;

        internal TreeNode root;

        private DrawTreeNodeEventHandler onDrawNode;

        public Color BorderColor { get; set; }
        public int ItemHeight { get; set; }
        public TreeNodeCollection Nodes { get { return nodes; } private set { nodes = value; } }
        public TreeNode SelectedNode { get; set; }

        public TreeView()
        {
            this.BackColor = Color.White;
            this.BorderColor = Color.FromArgb(130, 135, 144);
            this.ItemHeight = 22;
            this.Padding = new Padding(4);

            this.root = new TreeNode(this);
            this.root.Expand();
            nodes = new TreeNodeCollection(root);

            onDrawNode = _OnDrawNode;

            Application.UpClick += Application_UpClick;
        }

        private void Application_UpClick(object sender, MouseEventArgs e)
        {
            _drag = false;
            _dragNode = null;
            _dragPosition = Point.Empty;
        }
        private Rectangle _CalcNodeRect(TreeNode node)
        {
            int x = Padding.Left + 2;
            int y = Padding.Top;
            int width = Width;
            int height = ItemHeight;

            x += node.GetXIndex() * 8;
            y += node.GetYIndex() * ItemHeight;

            return new Rectangle(x, y, width, height);
        }
        private TreeNode _GetNodeAtPosition(TreeNode rootNode, Point position)
        {
            if (rootNode != root && rootNode.IsVisible && rootNode.Bounds.Contains(position)) return rootNode;

            if (rootNode.IsExpanded)
                for (int i = 0; i < rootNode.Nodes.Count; i++)
                {
                    var result = _GetNodeAtPosition(rootNode.Nodes[i], position);
                    if (result != null) return result;
                }

            return null;
        }
        private void _OnDrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if (e.Node.IsVisible == false) return;

            if (e.Node != root)
            {
                if (_recalc) e.Node.Bounds = _CalcNodeRect(e.Node);

                // Node drawing.
                if (e.Node.IsSelected) e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(187, 222, 251)), 0, e.Node.Bounds.Y, Width, ItemHeight);
                string stringToDraw = e.Node.Text;
                if (stringToDraw == null && e.Node.Tag != null) stringToDraw = e.Node.Tag.ToString();
                e.Graphics.DrawString(stringToDraw, Font, new SolidBrush(Color.Black), e.Node.Bounds.X + 8, e.Node.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
                // End of drawing.

                DrawNode(this, e);
            }

            if (e.Node.IsExpanded == true)
                for (int i = 0; i < e.Node.Nodes.Count; i++)
                    OnDrawNode(new DrawTreeNodeEventArgs(e.Graphics, e.Node.Nodes[i], e.Node.Nodes[i].Bounds, TreeNodeStates.Default));
        }
        private TreeNode _SelectAtPosition(MouseEventArgs e)
        {
            if (SelectedNode != null) SelectedNode.IsSelected = false;

            var selectedNode = _GetNodeAtPosition(root, e.Location);
            if (selectedNode != null)
            {
                SelectedNode = selectedNode;
                SelectedNode.IsSelected = true;
            }

            return selectedNode;
        }

        public override void Dispose()
        {
            Application.UpClick -= Application_UpClick;

            base.Dispose();
        }
        protected virtual void OnDrawNode(DrawTreeNodeEventArgs e)
        {
            if (this.onDrawNode != null) this.onDrawNode(this, e);
        }
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            var sNode = _SelectAtPosition(e);
            if (sNode != null)
            {
                sNode.Toggle();
                NodeMouseDoubleClick(this, new TreeNodeMouseClickEventArgs(sNode, e.Button, e.Clicks, e.X, e.Y));
            }
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (_SelectAtPosition(e) != null) NodeMouseClick(this, new TreeNodeMouseClickEventArgs(SelectedNode, e.Button, e.Clicks, e.X, e.Y));

            _dragNode = SelectedNode;
            _drag = true;
            _dragPosition = e.Location;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_drag)
            {
                if (_dragPosition.Distance(e.Location) > 4)
                {
                    ItemDrag(this, new ItemDragEventArgs(e.Button, _dragNode));
                    _dragPosition = Point.Empty;
                    _drag = false;
                }
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(BackColor), 0, 0, Width, Height);

            OnDrawNode(new DrawTreeNodeEventArgs(e.Graphics, root, root.Bounds, TreeNodeStates.Default));

            if (_recalc) _recalc = false;

            e.Graphics.DrawRectangle(new Pen(BorderColor), 0, 0, Width, Height);
        }
        public override void Refresh()
        {
            _recalc = true;
            UnityEngine.Debug.Log("tree updated");
        }

        public void CollapseAll()
        {
            for (int i = 0; i < Nodes.Count; i++)
                Nodes[i].Collapse();
        }
        public void ExpandAll()
        {
            for (int i = 0; i < Nodes.Count; i++)
                Nodes[i].ExpandAll();
        }

        public event DrawTreeNodeEventHandler DrawNode = delegate { };
        public event ItemDragEventHandler ItemDrag = delegate { };
        public event TreeNodeMouseClickEventHandler NodeMouseClick = delegate { };
        public event TreeNodeMouseClickEventHandler NodeMouseDoubleClick = delegate { };
    }
}
