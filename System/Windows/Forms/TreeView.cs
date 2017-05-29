using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public class TreeView : Control
    {
        private readonly Pen borderPen = new Pen(Color.White);
        private bool _drag;
        private TreeNode _dragNode;
        private Point _dragPosition;
        private string _filter;
        private TreeNode _hoveredNode;
        private DrawTreeNodeEventArgs nodeArgs = new DrawTreeNodeEventArgs(null, null, Rectangle.Empty, TreeNodeStates.Default);
        private List<TreeNode> _nodeList = new List<TreeNode>();
        private float _resetFilterTime;
        protected ScrollBar vScrollBar;

        protected List<TreeNode> scrollNodeList;

        internal TreeNode root;

        private readonly DrawTreeNodeEventHandler onDrawNode;

        public Color BorderColor
        {
            get { return borderPen.Color; }
            set { borderPen.Color = value; }
        }
        public ImageList ImageList { get; set; }
        public int ItemHeight { get; set; }
        public TreeNodeCollection Nodes { get; private set; }
        public Color ScrollBarColor { get; set; }
        internal float ScrollIndex { get { return vScrollBar.Value; } set { vScrollBar.Value = (int)value; } }
        public Color ScrollBarHoverColor { get; set; }
        public float ScrollSpeed { get; set; }
        public TreeNode SelectedNode { get; set; }
        public Color SelectionColor { get; set; }
        public Color SelectionHoverColor { get; set; }
        public bool SmoothScrolling { get; set; }
        public bool UseNodeBoundsForSelection { get; set; }
        public bool WrapText { get; set; }

        public TreeView()
        {
            vScrollBar = new VScrollBar();
            vScrollBar.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            vScrollBar.Location = new Point(Width - vScrollBar.Width, 0);
            vScrollBar.Height = Height;
            vScrollBar.ValueChanged += VScrollBarOnValueChanged;
            Controls.Add(vScrollBar);

            this.BackColor = Color.White;
            this.BorderColor = Color.FromArgb(130, 135, 144);
            this.CanSelect = true;
            this.ImageList = new ImageList();
            this.ItemHeight = 22;
            this.Padding = new Padding(4);
            this.root = new TreeNode(this);
            this.root.Expand();
            this.Resize += TreeView_Resize;
            this.ScrollBarColor = Color.FromArgb(222, 222, 230);
            this.ScrollBarHoverColor = Color.FromArgb(136, 136, 136);
            this.ScrollSpeed = 2;
            this.SelectionColor = Color.FromArgb(187, 222, 251);
            this.SelectionHoverColor = Color.FromArgb(221, 238, 253);
            this.Size = new Size(121, 97);
            this.SmoothScrolling = true;

            Nodes = new TreeNodeCollection(root);
            scrollNodeList = new List<TreeNode>();

            onDrawNode = _OnDrawNode;

            uwfAppOwner.UpClick += _Application_UpClick;
        }

        private void _AddjustScrollIndexToSelectedNode()
        {
            if (SelectedNode != null)
            {
                if (ScrollIndex > SelectedNode.Bounds.Y)
                    ScrollIndex = SelectedNode.Bounds.Y;
                
                if (ScrollIndex + Height < SelectedNode.Bounds.Y + ItemHeight)
                    ScrollIndex = SelectedNode.Bounds.Y + ItemHeight - Height;
                
            }
        }
        private void _Application_UpClick(object sender, MouseEventArgs e)
        {
            _drag = false;
            _dragNode = null;
            _dragPosition = Point.Empty;
        }
        private void _FixScrollIndex()
        {
            if (_nodeList == null || _nodeList.Count == 0)
            {
                ScrollIndex = 0;
                return;
            }

            if (SmoothScrolling == false)
                ScrollIndex = (float)Math.Ceiling(ScrollIndex / ItemHeight) * ItemHeight;

            if (ScrollIndex > _nodeList.Last().Bounds.Y + ItemHeight - Height) ScrollIndex = _nodeList.Last().Bounds.Y + ItemHeight - Height;
            if (ScrollIndex < 0) ScrollIndex = 0;
        }
        private TreeNode _GetNodeAtPosition(TreeNode rootNode, Point position)
        {
            if (rootNode != root && rootNode.IsVisible)
            {
                int nodeWidth = Width;
                var rootNodeRect = new Rectangle(rootNode.Bounds.X, rootNode.Bounds.Y - (int)ScrollIndex, nodeWidth, rootNode.Bounds.Height);
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
            var node = e.Node;
            var nodeY = node.Bounds.Y - ScrollIndex; // TODO: to node.Bounds.Y

            // Node drawing.
            e.Graphics.uwfFillRectangle(node.BackColor, node.Bounds.X, nodeY, node.Bounds.Width, node.Bounds.Height);
            if (node.IsSelected || node == _hoveredNode)
                e.Graphics.uwfFillRectangle((node.IsSelected ? SelectionColor : SelectionHoverColor), UseNodeBoundsForSelection ? node.Bounds.X : 0, nodeY, Width, ItemHeight);

            int xOffset = node.Bounds.X;

            // Draw collapsed/expanded arrow.
            if (node.Nodes.Count > 0)
            {
                Bitmap arrowTexture = null;
                if (node.IsExpanded)
                {
                    if (node.ImageIndex_Expanded > -1)
                    {
                        var img = ImageList.Images[node.ImageIndex_Expanded];
                        if (img != null)
                            arrowTexture = img as Bitmap;
                    }
                    else
                        arrowTexture = ApplicationBehaviour.GdiImages.TreeNodeExpanded;
                }
                else
                {
                    if (node.ImageIndex_Collapsed > -1)
                    {
                        var img = ImageList.Images[node.ImageIndex_Collapsed];
                        if (img != null)
                            arrowTexture = img as Bitmap;
                    }
                    else
                        arrowTexture = ApplicationBehaviour.GdiImages.TreeNodeCollapsed;
                }

                if (arrowTexture != null)
                {
                    e.Graphics.DrawImage(arrowTexture, xOffset, nodeY + node.Bounds.Height / 2 - arrowTexture.Height / 2, arrowTexture.Width, arrowTexture.Height);
                    xOffset += arrowTexture.Width;
                }
            }

            // Draw image.
            if (node.ImageIndex > -1)
            {
                var image = ImageList.Images[node.ImageIndex];
                if (image != null && image.uTexture != null)
                {
                    e.Graphics.uwfDrawImage(image, node.ImageColor, xOffset, nodeY + node.Bounds.Height / 2 - image.Height / 2, image.Width, image.Height);
                    xOffset += image.Width + 2;
                }
            }

            // Draw text.
            string stringToDraw = node.Text;
            if (stringToDraw == null && node.Tag != null) stringToDraw = node.Tag.ToString();
            e.Graphics.uwfDrawString(stringToDraw, Font, node.ForeColor, xOffset, nodeY - 2, (WrapText ? Width : Width * 16), e.Bounds.Height + 4, ContentAlignment.MiddleLeft);
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
        private void _SelectNode(TreeNode node)
        {
            SelectedNode = node;
            SelectedNodeChanged(this, new TreeViewEventArgs(node));
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
            UpdateScrollBar();
        }
        private void UpdateScrollBar()
        {
            if (vScrollBar != null)
            {
                vScrollBar.Maximum = ItemHeight * _nodeList.Count;
                vScrollBar.SmallChange = ItemHeight;
                vScrollBar.LargeChange = Height;
                vScrollBar.Visible = vScrollBar.Maximum > Height;
            }
        }
        private void _UpdateScrollList()
        {
            scrollNodeList = new List<TreeNode>();

            int startNode = (int)(ScrollIndex / ItemHeight) - 1;
            if (startNode < 0) startNode = 0;
            int nodesOnScreen = Height / ItemHeight + 3; // Magic number.

            for (int i = startNode; i < startNode + nodesOnScreen && i < _nodeList.Count; i++)
            {
                var node = _nodeList[i];
                if (node.Bounds.Y + node.Bounds.Height > 0 && node.Bounds.Y - (int)ScrollIndex < Height)
                    scrollNodeList.Add(node);
            }

            UpdateScrollBar();
        }
        private void VScrollBarOnValueChanged(object sender, EventArgs eventArgs)
        {
            _UpdateScrollList();
        }

        public override void Dispose()
        {
            uwfAppOwner.UpClick -= _Application_UpClick;

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
                    case UnityEngine.KeyCode.Space:
                    case UnityEngine.KeyCode.Return:
                        if (SelectedNode != null)
                            SelectedNode.Toggle();
                        break;

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

            char c = KeyHelper.GetLastInputChar();
            if (char.IsLetterOrDigit(c) || char.IsPunctuation(c))
            {
                _filter += c;
                _resetFilterTime = 3; // sec.
                SelectNodeWText(_filter);
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

            if (_SelectAtPosition(e) != null) NodeMouseClick(this, new TreeNodeMouseClickEventArgs(SelectedNode, e.Button, e.Clicks, e.X, e.Y));

            _dragNode = SelectedNode;
            _drag = true;
            _dragPosition = e.Location;
        }
        protected override void OnMouseHover(EventArgs e)
        {
            var mclient = PointToClient(MousePosition);

            _hoveredNode = _GetNodeAtPosition(root, mclient);
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
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            ScrollIndex -= e.Delta * ScrollSpeed;

            _FixScrollIndex();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            // Reset filter.
            if (_resetFilterTime > 0)
            {
                _resetFilterTime -= Application.DeltaTime;
                if (_resetFilterTime <= 0)
                    _filter = "";
            }

            e.Graphics.uwfFillRectangle(BackColor, 0, 0, Width, Height);

            for (int i = 0; i < scrollNodeList.Count; i++)
            {
                var scrollNode = scrollNodeList[i];
                
                nodeArgs.Graphics = e.Graphics;
                nodeArgs.Node = scrollNode;
                nodeArgs.Bounds = scrollNode.Bounds;
                nodeArgs.State = TreeNodeStates.Default;
                
                OnDrawNode(nodeArgs); 
            }
        }
        protected override void OnLatePaint(PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(borderPen, 0, 0, Width, Height);
        }
        protected override object uwfOnPaintEditor(float width)
        {
            var control = base.uwfOnPaintEditor(width);

            Editor.NewLine(1);
            Editor.Label("   TreeView");

            Editor.ColorField("BorderColor", BorderColor, (c) => { BorderColor = c; });

            var itemHeightBuffer = Editor.IntField("ItemHeight", ItemHeight);
            if (itemHeightBuffer.Changed)
                ItemHeight = itemHeightBuffer.Value[0];

            Editor.ColorField("ScrollBarColor", ScrollBarColor, (c) => { ScrollBarColor = c; });
            Editor.ColorField("ScrollBarHoverColor", ScrollBarHoverColor, (c) => { ScrollBarHoverColor = c; });

            var scrollSpeedBuffer = Editor.Slider("ScrollSpeed", ScrollSpeed, 0, 255);
            if (scrollSpeedBuffer.Changed)
                ScrollSpeed = scrollSpeedBuffer.Value;

            var scrollIndexBuffer = Editor.Slider("ScrollIndex", ScrollIndex, -1, _nodeList.Count * ItemHeight);
            if (scrollIndexBuffer.Changed)
                ScrollIndex = scrollIndexBuffer.Value;

            Editor.ColorField("SelectionColor", SelectionColor, (c) => { SelectionColor = c; });
            Editor.ColorField("SelectionHoverColor", SelectionHoverColor, (c) => { SelectionHoverColor = c; });

            var smoothScrollingBuffer = Editor.BooleanField("SmoothScrolling", SmoothScrolling);
            if (smoothScrollingBuffer.Changed)
                SmoothScrolling = smoothScrollingBuffer.Value;

            var useNodeBoundsForSelectionBuffer = Editor.BooleanField("UseNodeBoundsForSelection", UseNodeBoundsForSelection);
            if (useNodeBoundsForSelectionBuffer.Changed)
                UseNodeBoundsForSelection = useNodeBoundsForSelectionBuffer.Value;

            var wrapTextBuffer = Editor.BooleanField("WrapText", WrapText);
            if (wrapTextBuffer.Changed)
                WrapText = wrapTextBuffer.Value;

            Editor.Label("HoveredNode", _hoveredNode);
            Editor.Label("SelectedNode", SelectedNode);

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
            scrollNodeList = new List<TreeNode>();

            ProccesNode(root);
            _UpdateScrollList();
            _FixScrollIndex();
        }

        internal void EnsureVisible(TreeNode node)
        {
            var nodeIsInScrollList = scrollNodeList.Find(x => x == node);
            if (nodeIsInScrollList != null) return;

            var nodeIndex = _nodeList.FindIndex(x => x == node);
            if (nodeIndex == -1) return; // Is not exist in tree.

            ScrollIndex = node.Bounds.Y;

            _FixScrollIndex();
            _UpdateScrollList();
        }
        protected void SelectNodeWText(string text, bool caseSencitive = false)
        {
            string lowerText = text;
            if (caseSencitive == false) lowerText = text.ToLower();

            for (int i = 0; i < _nodeList.Count; i++)
            {
                var node = _nodeList[i];
                string nodeText = node.Text;
                if (string.IsNullOrEmpty(nodeText) && node.Tag != null)
                    nodeText = node.Tag.ToString();

                if (nodeText == null || nodeText.Length < text.Length) continue;

                string clippedNodeText = nodeText.Substring(0, text.Length);
                if (caseSencitive == false) clippedNodeText = clippedNodeText.ToLower();

                if (clippedNodeText == lowerText)
                {
                    EnsureVisible(node);
                    _SelectNode(node);
                    break;
                }
            }
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
