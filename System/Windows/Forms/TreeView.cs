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

        private TreeNode _hoveredNode;
        private bool _scrollVisible = false;
        private bool _scroll;
        private bool _scrollHovered;
        protected float scrollIndex;
        private float _scrollbarY;
        private float _scrollbarHeight;
        private float _scrollbarWidth = 10;
        private float _scrollStartY;
        private float _scroll_ItemsEstimatedHeigh;
        private List<TreeNode> _scrollNodeList;

        internal TreeNode root;

        private DrawTreeNodeEventHandler onDrawNode;

        public Color BorderColor { get; set; }
        public ImageList ImageList { get { return _imageList; } set { _imageList = value; } }
        public int ItemHeight { get; set; }
        public Color HoverColor { get; set; }
        public TreeNodeCollection Nodes { get { return _nodes; } private set { _nodes = value; } }
        public Color ScrollColor { get; set; }
        public float ScrollIndex { get { return scrollIndex; } internal set { scrollIndex = value; } }
        public Color ScrollHoverColor { get; set; }
        public float ScrollSpeed { get; set; }
        public TreeNode SelectedNode { get; set; }
        public Color SelectionColor { get; set; }
        public bool SmoothScrolling { get; set; }
        public bool UseNodeBoundsForSelection { get; set; }
        public bool WrapText { get; set; }

        public TreeView()
        {
            this.BackColor = Color.White;
            this.BorderColor = Color.FromArgb(130, 135, 144);
            this.ImageList = new ImageList();
            this.ItemHeight = 22;
            this.HoverColor = Color.FromArgb(221, 238, 253);
            this.Padding = new Padding(4);
            this.root = new TreeNode(this);
            this.root.Expand();
            this.Resize += TreeView_Resize;
            this.ScrollColor = Color.FromArgb(222, 222, 230);
            this.ScrollHoverColor = Color.FromArgb(136, 136, 136);
            this.ScrollSpeed = 2;
            this.SelectionColor = Color.FromArgb(187, 222, 251);
            this.SmoothScrolling = true;

            _nodes = new TreeNodeCollection(root);
            _nodeList = new List<TreeNode>();
            _scrollNodeList = new List<TreeNode>();

            onDrawNode = _OnDrawNode;

            Owner.UpClick += _Application_UpClick;
        }

        private void _AddjustScrollIndexToSelectedNode()
        {
            if (SelectedNode != null)
            {
                if (scrollIndex > SelectedNode.Bounds.Y)
                {
                    
                    scrollIndex = SelectedNode.Bounds.Y;
                    _UpdateScrollList();
                }
                if (scrollIndex + Height < SelectedNode.Bounds.Y + ItemHeight)
                {
                    scrollIndex = SelectedNode.Bounds.Y + ItemHeight - Height;
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
                scrollIndex = 0;
                return;
            }

            if (SmoothScrolling == false)
                scrollIndex = (float)Math.Ceiling(scrollIndex / ItemHeight) * ItemHeight;

            if (scrollIndex > _nodeList.Last().Bounds.Y + ItemHeight - Height) scrollIndex = _nodeList.Last().Bounds.Y + ItemHeight - Height;
            if (scrollIndex < 0) scrollIndex = 0;
        }
        private TreeNode _GetNodeAtPosition(TreeNode rootNode, Point position)
        {
            if (rootNode != root && rootNode.IsVisible)
            {
                int nodeWidth = Width;
                var rootNodeRect = new Rectangle(rootNode.Bounds.X, rootNode.Bounds.Y - (int)scrollIndex, nodeWidth, rootNode.Bounds.Height);
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
            e.Graphics.FillRectangle(e.Node.BackColor, e.Node.Bounds.X, e.Node.Bounds.Y, e.Node.Bounds.Width, e.Node.Bounds.Height);
            if (e.Node.IsSelected || e.Node == _hoveredNode)
                e.Graphics.FillRectangle((e.Node.IsSelected ? SelectionColor : HoverColor), UseNodeBoundsForSelection ? e.Node.Bounds.X : 0, e.Node.Bounds.Y - (int)scrollIndex, Width, ItemHeight);

            bool hasImage = false;
            int imageWidth = 0;
            if (e.Node.ImageIndex > -1)
            {
                var image = ImageList.Images[e.Node.ImageIndex];
                if (image != null && image.uTexture != null)
                {
                    e.Graphics.DrawTexture(image.uTexture, e.Node.Bounds.X, e.Node.Bounds.Y + e.Node.Bounds.Height / 2 - image.Height / 2 - (int)scrollIndex, image.Width, image.Height, e.Node.ImageColor);
                    hasImage = true;
                    imageWidth = image.Width;
                }
            }

            string stringToDraw = e.Node.Text;
            if (stringToDraw == null && e.Node.Tag != null) stringToDraw = e.Node.Tag.ToString();
            e.Graphics.DrawString(stringToDraw, Font, e.Node.ForeColor, e.Node.Bounds.X + (hasImage ? imageWidth + 2 : 0), e.Node.Bounds.Y - (int)scrollIndex - 2, (WrapText ? Width : Width * 16), e.Bounds.Height + 4, ContentAlignment.MiddleLeft);
            // End of drawing.

            DrawNode(this, e);
        }
        private TreeNode _SelectAtPosition(MouseEventArgs e)
        {
            var node = _GetNodeAtPosition(root, e.Location);
            if (node == null || node.Enabled == false) return null;

            SelectedNode = node;
            SelectedNodeChanged(this, new TreeViewEventArgs(SelectedNode));

            return SelectedNode;
        }
        private void _SelectNext()
        {
            var nextNode = _nodeList.FindIndex(x => x == SelectedNode); // TODO: this is slow implementation. Should remember current selectedIndex.
            while (_SelectNext(nextNode) == false)
            {
                nextNode++;
            }
        }
        private bool _SelectNext(int fromIndex)
        {
            if (fromIndex + 1 < _nodeList.Count)
            {
                if (_nodeList[fromIndex + 1].Enabled == false) return false;

                SelectedNode = _nodeList[fromIndex + 1];
                _AddjustScrollIndexToSelectedNode();
                SelectedNodeChanged(this, new TreeViewEventArgs(SelectedNode));
                return true;
            }

            return true;
        }
        private void _SelectPrevious()
        {
            var prevNode = _nodeList.FindIndex(x => x == SelectedNode);
            while (_SelectPrevious(prevNode) == false)
            {
                prevNode--;
            }
        }
        private bool _SelectPrevious(int fromIndex)
        {
            if (fromIndex - 1 >= 0)
            {
                if (_nodeList[fromIndex - 1].Enabled == false) return false;

                SelectedNode = _nodeList[fromIndex - 1];
                _AddjustScrollIndexToSelectedNode();
                SelectedNodeChanged(this, new TreeViewEventArgs(SelectedNode));
                return true;
            }

            return true;
        }
        private void TreeView_Resize(object sender, EventArgs e)
        {
            _FixScrollIndex();
            _UpdateScrollList();
        }
        private void _UpdateScrollList()
        {
            _scrollNodeList = new List<TreeNode>();

            int startNode = (int)(scrollIndex / ItemHeight) - 1;
            if (startNode < 0) startNode = 0;
            int nodesOnScreen = Height / ItemHeight + 3; // Magic number.

            for (int i = startNode; i < startNode + nodesOnScreen && i < _nodeList.Count; i++)
            {
                if (_nodeList[i].Bounds.Y + _nodeList[i].Bounds.Height > 0 && _nodeList[i].Bounds.Y - (int)scrollIndex < Height)
                {
                    _scrollNodeList.Add(_nodeList[i]);
                }
            }
        }

        public override void Dispose()
        {
            Owner.UpClick -= _Application_UpClick;

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
                        _SelectNext();
                        break;
                    case UnityEngine.KeyCode.LeftArrow:
                        if (SelectedNode != null)
                            SelectedNode.Collapse();
                        break;
                    case UnityEngine.KeyCode.RightArrow:
                        if (SelectedNode != null)
                            SelectedNode.Expand();
                        break;
                    case UnityEngine.KeyCode.UpArrow:
                        _SelectPrevious();
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
                _scrollStartY = mclient.Y - _scrollbarY;
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
            RectangleF _scrollRect = new RectangleF(Width - _scrollbarWidth, _scrollbarY, _scrollbarWidth, _scrollbarHeight);
            if (_scrollRect.Contains(mclient))
                _scrollHovered = true;
            else
                _scrollHovered = false;

            _hoveredNode = _GetNodeAtPosition(root, mclient);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_scroll)
            {
                var mclient = PointToClient(MousePosition);

                scrollIndex = (mclient.Y - _scrollStartY) * (_scroll_ItemsEstimatedHeigh / Height);

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
            scrollIndex -= e.Delta * ScrollSpeed;

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
                Color _scrollColor = ScrollColor;
                if (_scrollHovered || _scroll) _scrollColor = ScrollHoverColor;

                float _scrollYCoeff = Height / _scroll_ItemsEstimatedHeigh;
                _scrollbarHeight = Height * _scrollYCoeff;
                if (_scrollbarHeight < 8) _scrollbarHeight = 8;
                _scrollbarY = scrollIndex * _scrollYCoeff;

                e.Graphics.FillRectangle(_scrollColor, Width - _scrollbarWidth + 2, _scrollbarY, _scrollbarWidth - 2, _scrollbarHeight);
            }
            #endregion

            e.Graphics.DrawRectangle(new Pen(BorderColor), 0, 0, Width, Height);
        }
        protected override object OnPaintEditor(float width)
        {
            var control = base.OnPaintEditor(width);

            Editor.NewLine(1);
            Editor.Label("   TreeView");

            Editor.ColorField("BorderColor", BorderColor, (c) => { BorderColor = c; });

            var itemHeightBuffer = Editor.IntField("ItemHeight", ItemHeight);
            if (itemHeightBuffer.Changed)
                ItemHeight = itemHeightBuffer.Value[0];

            var scrollSpeedBuffer = Editor.Slider("ScrollSpeed", ScrollSpeed, 0, 255);
            if (scrollSpeedBuffer.Changed)
                ScrollSpeed = scrollSpeedBuffer.Value;

            Editor.ColorField("SelectionColor", SelectionColor, (c) => { SelectionColor = c; });

            var smoothScrollingBuffer = Editor.BooleanField("SmoothScrolling", SmoothScrolling);
            if (smoothScrollingBuffer.Changed)
                SmoothScrolling = smoothScrollingBuffer.Value;

            var useNodeBoundsForSelectionBuffer = Editor.BooleanField("UseNodeBoundsForSelection", UseNodeBoundsForSelection);
            if (useNodeBoundsForSelectionBuffer.Changed)
                UseNodeBoundsForSelection = useNodeBoundsForSelectionBuffer.Value;

            var wrapTextBuffer = Editor.BooleanField("WrapText", WrapText);
            if (wrapTextBuffer.Changed)
                WrapText = wrapTextBuffer.Value;

            if (Editor.Button("Refresh"))
                Refresh();

            return control;
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
            _FixScrollIndex();
        }

        internal void EnsureVisible(TreeNode node)
        {
            var nodeIsInScrollList =_scrollNodeList.Find(x => x == node);
            if (nodeIsInScrollList != null) return;

            var nodeIndex = _nodeList.FindIndex(x => x == node);
            if (nodeIndex == -1) return; // Is not exist in tree.

            scrollIndex = node.Bounds.Y;

            _FixScrollIndex();
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
