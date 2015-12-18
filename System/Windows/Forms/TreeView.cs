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

        private ImageList _imageList;
        private TreeNodeCollection _nodes;
        private List<TreeNode> _nodeList;

        private bool _scrollVisible = false; // smooth scrolling.
        private bool _scroll;
        private bool _scrollHovered;
        private float _scrollIndex;
        private float _scrollY;
        private float _scrollHeight;
        private float _scrollWidth = 10;
        private float _scrollStartY;
        private float _scroll_ItemsEstimatedHeigh;
        private List<TreeNode> _scrollNodeList;

        internal TreeNode root;

        private DrawTreeNodeEventHandler onDrawNode;

        public Color BorderColor { get; set; }
        public ImageList ImageList { get { return _imageList; } set { _imageList = value; } }
        public int ItemHeight { get; set; }
        public TreeNodeCollection Nodes { get { return _nodes; } private set { _nodes = value; } }
        public float ScrollIndex { get { return _scrollIndex; } }
        public TreeNode SelectedNode { get; set; }
        public bool SmoothScrolling { get; set; }
        public bool UseNodeBoundsForSelection { get; set; }

        public TreeView()
        {
            this.BackColor = Color.White;
            this.BorderColor = Color.FromArgb(130, 135, 144);
            this.ImageList = new ImageList();
            this.ItemHeight = 22;
            this.Padding = new Padding(4);
            this.root = new TreeNode(this);
            this.root.Expand();
            this.Resize += TreeView_Resize;
            this.SmoothScrolling = true;

            _nodes = new TreeNodeCollection(root);
            _nodeList = new List<TreeNode>();
            _scrollNodeList = new List<TreeNode>();

            onDrawNode = _OnDrawNode;

            Application.UpClick += _Application_UpClick;
        }

        private void _AddjustScrollIndexToSelectedNode()
        {
            if (SelectedNode != null)
            {
                if (_scrollIndex > SelectedNode.Bounds.Y)
                {
                    
                    _scrollIndex = SelectedNode.Bounds.Y;
                    _UpdateScrollList();
                }
                if (_scrollIndex + Height < SelectedNode.Bounds.Y + ItemHeight)
                {
                    _scrollIndex = SelectedNode.Bounds.Y + ItemHeight - Height;
                    _UpdateScrollList();
                }
            }
        }
        private void _Application_UpClick(object sender, MouseEventArgs e)
        {
            _drag = false;
            _dragNode = null;
            _dragPosition = Point.Empty;

            _scroll = false;
        }
        private void _FixScrollIndex()
        {
            if (_nodeList == null || _nodeList.Count == 0)
            {
                _scrollIndex = 0;
                return;
            }

            if (SmoothScrolling == false)
                _scrollIndex = (float)Math.Ceiling(_scrollIndex / ItemHeight) * ItemHeight;

            if (_scrollIndex > _nodeList.Last().Bounds.Y + ItemHeight - Height) _scrollIndex = _nodeList.Last().Bounds.Y + ItemHeight - Height;
            if (_scrollIndex < 0) _scrollIndex = 0;
        }
        private TreeNode _GetNodeAtPosition(TreeNode rootNode, Point position)
        {
            if (rootNode != root && rootNode.IsVisible)
            {
                var rootNodeRect = rootNode.Bounds.AddjustY((int)-_scrollIndex);
                if (UseNodeBoundsForSelection == false) rootNodeRect.X = 0;
                if (rootNodeRect.Contains(position))
                    return rootNode;
            }

            if (rootNode.IsExpanded)
                for (int i = 0; i < rootNode.Nodes.Count; i++)
                {
                    var result = _GetNodeAtPosition(rootNode.Nodes[i], position);
                    if (result != null) return result;
                }

            return null;
        }
        private Rectangle _GetNodeBounds(TreeNode node)
        {
            _nodeList.Add(node);

            int x = Padding.Left + 2;
            int y = Padding.Top;
            int width = Width;
            int height = ItemHeight;

            x += (node.GetXIndex() - 1) * 8;
            y += (_nodeList.Count - 1) * ItemHeight;

            return new Rectangle(x, y, width, height);
        }
        private void _OnDrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            // Node drawing.
            if (e.Node.IsSelected) e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(187, 222, 251)), UseNodeBoundsForSelection ? e.Node.Bounds.X : 0, e.Node.Bounds.Y - (int)_scrollIndex, Width, ItemHeight);

            bool hasImage = false;
            int imageWidth = 0;
            if (e.Node.ImageIndex > -1)
            {
                var image = ImageList.Images[e.Node.ImageIndex];
                if (image != null)
                {
                    e.Graphics.DrawImage(image, e.Node.Bounds.X, e.Node.Bounds.Y + e.Node.Bounds.Height / 2 - image.Height / 2 - (int)_scrollIndex, image.Width, image.Height);
                    hasImage = true;
                    imageWidth = image.Width;
                }
            }

            string stringToDraw = e.Node.Text;
            if (stringToDraw == null && e.Node.Tag != null) stringToDraw = e.Node.Tag.ToString();
            e.Graphics.DrawString(stringToDraw, Font, new SolidBrush(e.Node.TextColor), e.Node.Bounds.X + (hasImage ? imageWidth + 2 : 0), e.Node.Bounds.Y - (int)_scrollIndex - 2, Width, e.Bounds.Height + 4, ContentAlignment.MiddleLeft);
            // End of drawing.

            DrawNode(this, e);
        }
        private TreeNode _SelectAtPosition(MouseEventArgs e)
        {
            SelectedNode = _GetNodeAtPosition(root, e.Location);
            SelectedNodeChanged(this, new TreeViewEventArgs(SelectedNode));

            return SelectedNode;
        }
        private void TreeView_Resize(object sender, EventArgs e)
        {
            _FixScrollIndex();
            _UpdateScrollList();
        }
        private void _UpdateScrollList()
        {
            _scrollNodeList = new List<TreeNode>();

            int startNode = (int)(_scrollIndex / ItemHeight) - 1;
            if (startNode < 0) startNode = 0;
            int nodesOnScreen = Height / ItemHeight + 3; // Magic number.

            for (int i = startNode; i < startNode + nodesOnScreen && i < _nodeList.Count; i++)
            {
                if (_nodeList[i].Bounds.Y + _nodeList[i].Bounds.Height > 0 && _nodeList[i].Bounds.Y - (int)_scrollIndex < Height)
                {
                    _scrollNodeList.Add(_nodeList[i]);
                }
            }
        }

        public override void Dispose()
        {
            Application.UpClick -= _Application_UpClick;

            base.Dispose();
        }
        protected virtual void OnDrawNode(DrawTreeNodeEventArgs e)
        {
            if (this.onDrawNode != null) this.onDrawNode(this, e);
        }
        protected override void OnKeyPress(KeyEventArgs e)
        {
            base.OnKeyPress(e);
            if (e.Modifiers == UnityEngine.EventModifiers.None)
            {
                switch (e.KeyCode)
                {
                    case UnityEngine.KeyCode.DownArrow:
                        var nextNode = _nodeList.FindIndex(x => x == SelectedNode); // TODO: this is slow implementation. Should remember current selectedIndex.
                        if (nextNode + 1 < _nodeList.Count)
                        {
                            SelectedNode = _nodeList[nextNode + 1];
                            _AddjustScrollIndexToSelectedNode();
                            SelectedNodeChanged(this, new TreeViewEventArgs(SelectedNode));
                        }
                        break;
                    case UnityEngine.KeyCode.LeftArrow:
                        if (SelectedNode != null)
                        {
                            SelectedNode.Collapse();
                        }
                        break;
                    case UnityEngine.KeyCode.RightArrow:
                        if (SelectedNode != null)
                        {
                            SelectedNode.Expand();
                        }
                        break;
                    case UnityEngine.KeyCode.UpArrow:
                        var prevNode = _nodeList.FindIndex(x => x == SelectedNode);
                        if (prevNode - 1 >= 0)
                        {
                            SelectedNode = _nodeList[prevNode - 1];
                            _AddjustScrollIndexToSelectedNode();
                            SelectedNodeChanged(this, new TreeViewEventArgs(SelectedNode));
                        }
                        break;
                }
            }
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
            base.OnMouseDown(e);
            if (_scrollHovered)
            {
                var mclient = PointToClient(MousePosition);

                _scroll = true;
                _scrollStartY = mclient.Y - _scrollY;
                return;
            }

            if (_SelectAtPosition(e) != null) NodeMouseClick(this, new TreeNodeMouseClickEventArgs(SelectedNode, e.Button, e.Clicks, e.X, e.Y));

            _dragNode = SelectedNode;
            _drag = true;
            _dragPosition = e.Location;
        }
        protected override void OnMouseHover(EventArgs e)
        {
            var mclient = PointToClient(MousePosition);
            RectangleF _scrollRect = new RectangleF(Width - _scrollWidth, _scrollY, _scrollWidth, _scrollHeight);
            if (_scrollRect.Contains(mclient))
                _scrollHovered = true;
            else
                _scrollHovered = false;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_scroll)
            {
                var mclient = PointToClient(MousePosition);

                _scrollIndex = (mclient.Y - _scrollStartY) * (_scroll_ItemsEstimatedHeigh / Height);

                _FixScrollIndex();
                _UpdateScrollList();
            }
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
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            _scrollIndex -= e.Delta * 4;

            _FixScrollIndex();
            _UpdateScrollList();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(BackColor), 0, 0, Width, Height);

            for (int i = 0; i < _scrollNodeList.Count; i++)
            {
                OnDrawNode(new DrawTreeNodeEventArgs(e.Graphics, _scrollNodeList[i], _scrollNodeList[i].Bounds, TreeNodeStates.Default));
            }

            #region Scroll.
            _scroll_ItemsEstimatedHeigh = Padding.Top + Padding.Bottom + _nodeList.Count * ItemHeight;
            if (_scroll_ItemsEstimatedHeigh > Height) _scrollVisible = true;
            else _scrollVisible = false;

            if (_scrollVisible)
            {
                Color _scrollColor = Color.FromArgb(222, 222, 230);
                if (_scrollHovered || _scroll) _scrollColor = Color.FromArgb(136, 136, 136);

                float _scrollYCoeff = Height / _scroll_ItemsEstimatedHeigh;
                _scrollHeight = Height * _scrollYCoeff;
                if (_scrollHeight < 8) _scrollHeight = 8;
                _scrollY = _scrollIndex * _scrollYCoeff;

                e.Graphics.FillRectangle(new SolidBrush(_scrollColor), Width - _scrollWidth + 2, _scrollY, _scrollWidth - 2, _scrollHeight);
            }
            #endregion

            e.Graphics.DrawRectangle(new Pen(BorderColor), 0, 0, Width, Height);
        }
        protected virtual void ProccesNode(TreeNode node)
        {
            if (node.IsVisible == false) return;

            if (node != root) node.Bounds = _GetNodeBounds(node);

            if (node.IsExpanded == true)
                for (int i = 0; i < node.Nodes.Count; i++)
                    ProccesNode(node.Nodes[i]);
        }
        public override void Refresh()
        {
            _nodeList = new List<TreeNode>();
            _scrollNodeList = new List<TreeNode>();

            ProccesNode(root);
            _UpdateScrollList();
        }

        public void CollapseAll()
        {
            for (int i = 0; i < Nodes.Count; i++)
                Nodes[i].CollapseInternal();
            Refresh();
        }
        public void ExpandAll()
        {
            for (int i = 0; i < Nodes.Count; i++)
                Nodes[i].ExpandAllInternal();
            Refresh();
        }
        public TreeNode Find(Predicate<TreeNode> match)
        {
            return _nodeList.Find(match);
        }

        public event DrawTreeNodeEventHandler DrawNode = delegate { };
        public event ItemDragEventHandler ItemDrag = delegate { };
        public event TreeNodeMouseClickEventHandler NodeMouseClick = delegate { };
        public event TreeNodeMouseClickEventHandler NodeMouseDoubleClick = delegate { };
        public event TreeViewEventHandler SelectedNodeChanged = delegate { };
    }
}
