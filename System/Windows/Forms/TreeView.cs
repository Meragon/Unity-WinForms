namespace System.Windows.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public class TreeView : Control
    {
        internal int arrowSize = 16;
        internal TreeNode root;

        protected List<TreeNode> scrollNodeList;
        protected ScrollBar vScrollBar;

        private readonly Pen borderPen = new Pen(Color.White);
        private readonly DrawTreeNodeEventArgs nodeArgs = new DrawTreeNodeEventArgs(null, null, Rectangle.Empty, TreeNodeStates.Default);
        private readonly DrawTreeNodeEventHandler onDrawNode;

        private bool drag;
        private TreeNode dragNode;
        private Point dragPosition;
        private string filter;
        private TreeNode hoveredNode;
        private List<TreeNode> nodeList = new List<TreeNode>();
        private float resetFilterTime;

        public TreeView()
        {
            vScrollBar = new VScrollBar();
            vScrollBar.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            vScrollBar.Location = new Point(Width - vScrollBar.Width, 0);
            vScrollBar.Height = Height;
            vScrollBar.ValueChanged += VScrollBarOnValueChanged;
            Controls.Add(vScrollBar);

            BackColor = Color.White;
            BorderColor = Color.FromArgb(130, 135, 144);
            ImageList = new ImageList();
            ItemHeight = 22;
            Padding = new Padding(4);
            root = new TreeNode(this);
            root.Expand();
            Resize += TreeView_Resize;
            ScrollBarColor = Color.FromArgb(222, 222, 230);
            ScrollBarHoverColor = Color.FromArgb(136, 136, 136);
            ScrollSpeed = 2;
            SelectionColor = Color.FromArgb(187, 222, 251);
            SelectionHoverColor = Color.FromArgb(221, 238, 253);
            Size = new Size(121, 97);
            SmoothScrolling = true;

            Nodes = new TreeNodeCollection(root);
            scrollNodeList = new List<TreeNode>();

            onDrawNode = _OnDrawNode;

            uwfAppOwner.UpClick += _Application_UpClick;
        }

        public event DrawTreeNodeEventHandler DrawNode = delegate { };
        public event ItemDragEventHandler ItemDrag = delegate { };
        public event TreeNodeMouseClickEventHandler NodeMouseClick = delegate { };
        public event TreeNodeMouseClickEventHandler NodeMouseDoubleClick = delegate { };
        public event TreeViewEventHandler SelectedNodeChanged = delegate { };

        public Color BorderColor
        {
            get { return borderPen.Color; }
            set { borderPen.Color = value; }
        }
        public ImageList ImageList { get; set; }
        public int ItemHeight { get; set; }
        public TreeNodeCollection Nodes { get; private set; }
        public Color ScrollBarColor { get; set; }
        public Color ScrollBarHoverColor { get; set; }
        public float ScrollSpeed { get; set; }
        public TreeNode SelectedNode { get; set; }
        public Color SelectionColor { get; set; }
        public Color SelectionHoverColor { get; set; }
        public bool SmoothScrolling { get; set; }
        public bool UseNodeBoundsForSelection { get; set; }
        public bool WrapText { get; set; }

        internal float ScrollIndex { get { return vScrollBar.Value; } set { vScrollBar.Value = (int)value; } }

        public void CollapseAll()
        {
            var nodesCount = Nodes.Count;
            for (int i = 0; i < nodesCount; i++)
                Nodes[i].CollapseInternal();
            Refresh();
        }
        public void ExpandAll()
        {
            var nodesCount = Nodes.Count;
            for (int i = 0; i < nodesCount; i++)
                Nodes[i].ExpandAllInternal();
            Refresh();
        }
        public TreeNode Find(Predicate<TreeNode> match)
        {
            return nodeList.Find(match);
        }
        public override void Refresh()
        {
            nodeList = new List<TreeNode>();
            scrollNodeList = new List<TreeNode>();

            ProccesNode(root);
            _UpdateScrollList();
            _FixScrollIndex();
        }

        internal void EnsureVisible(TreeNode node)
        {
            var nodeIsInScrollList = scrollNodeList.Find(x => x == node);
            if (nodeIsInScrollList != null) return;

            var nodeIndex = nodeList.FindIndex(x => x == node);
            if (nodeIndex == -1) return; // Is not exist in tree.

            ScrollIndex = node.Bounds.Y;

            _FixScrollIndex();
            _UpdateScrollList();
        }

        protected override void Dispose(bool release_all)
        {
            uwfAppOwner.UpClick -= _Application_UpClick;

            base.Dispose(release_all);
        }
        protected virtual void OnDrawNode(DrawTreeNodeEventArgs e)
        {
            if (onDrawNode != null) onDrawNode(this, e);
        }
        protected override void OnKeyPress(KeyPressEventArgs args)
        {
            base.OnKeyPress(args);

            var e = args.uwfKeyArgs;

            if (e.Modifiers == Keys.None)
            {
                switch (e.KeyCode)
                {
                    case Keys.Space:
                    case Keys.Return:
                        if (SelectedNode != null)
                            SelectedNode.Toggle();
                        break;

                    case Keys.Down:
                        _SelectNext();
                        break;
                    case Keys.Left:
                        if (SelectedNode != null)
                            SelectedNode.Collapse();
                        break;
                    case Keys.Right:
                        if (SelectedNode != null)
                            SelectedNode.Expand();
                        break;
                    case Keys.Up:
                        _SelectPrevious();
                        break;
                }
            }

            char c = KeyHelper.GetLastInputChar();
            if (char.IsLetterOrDigit(c) || char.IsPunctuation(c))
            {
                filter += c;
                resetFilterTime = 3; // sec.
                SelectNodeWText(filter);
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

            dragNode = SelectedNode;
            drag = true;
            dragPosition = e.Location;
        }
        protected override void OnMouseHover(EventArgs e)
        {
            var mclient = PointToClient(MousePosition);

            hoveredNode = _GetNodeAtPosition(root, mclient);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (drag)
            {
                if (dragPosition.Distance(e.Location) > 4)
                {
                    ItemDrag(this, new ItemDragEventArgs(e.Button, dragNode));
                    dragPosition = Point.Empty;
                    drag = false;
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
            if (resetFilterTime > 0)
            {
                resetFilterTime -= swfHelper.GetDeltaTime();
                if (resetFilterTime <= 0)
                    filter = string.Empty;
            }

            e.Graphics.uwfFillRectangle(BackColor, 0, 0, Width, Height);

            for (int i = 0; i < scrollNodeList.Count; i++)
            {
                var scrollNode = scrollNodeList[i];
                
                // Instead of creating new args every frame for each node, we will use only cached one.
                // The problem is that you can't cache it somewhere else cause data in it will be always changing.
                nodeArgs.Graphics = e.Graphics;
                nodeArgs.Node = scrollNode;
                nodeArgs.Bounds = scrollNode.Bounds;
                nodeArgs.State = TreeNodeStates.Default;
                
                OnDrawNode(nodeArgs); 
            }
        }
        protected override void uwfOnLatePaint(PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(borderPen, 0, 0, Width, Height);
        }
        protected virtual void ProccesNode(TreeNode node)
        {
            if (node.IsVisible == false) return;

            if (node != root) node.Bounds = _GetNodeBounds(node);

            if (node.IsExpanded == true)
                for (int i = 0; i < node.Nodes.Count; i++)
                    ProccesNode(node.Nodes[i]);
        }
        protected void SelectNodeWText(string text, bool caseSencitive = false)
        {
            string lowerText = text;
            if (caseSencitive == false) lowerText = text.ToLower();

            for (int i = 0; i < nodeList.Count; i++)
            {
                var node = nodeList[i];
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
            drag = false;
            dragNode = null;
            dragPosition = Point.Empty;
        }
        private void _FixScrollIndex()
        {
            if (nodeList == null || nodeList.Count == 0)
            {
                ScrollIndex = 0;
                return;
            }

            if (SmoothScrolling == false)
                ScrollIndex = (float)Math.Ceiling(ScrollIndex / ItemHeight) * ItemHeight;

            if (ScrollIndex > nodeList.Last().Bounds.Y + ItemHeight - Height) ScrollIndex = nodeList.Last().Bounds.Y + ItemHeight - Height;
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
            nodeList.Add(node);

            int x = Padding.Left + 2;
            int y = Padding.Top;
            int width = Width;
            int height = ItemHeight;

            x += (node.GetXIndex() - 1) * 8;
            y += (nodeList.Count - 1) * ItemHeight;

            return new Rectangle(x, y, width, height);
        }
        private void _OnDrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            var node = e.Node;
            var nodeBounds = node.Bounds;
            var nodeY = nodeBounds.Y - ScrollIndex; // TODO: to node.Bounds.Y

            var graphics = e.Graphics;

            // Node drawing.
            graphics.uwfFillRectangle(node.BackColor, nodeBounds.X, nodeY, nodeBounds.Width, nodeBounds.Height);
            if (node.IsSelected || node == hoveredNode)
                graphics.uwfFillRectangle((node.IsSelected ? SelectionColor : SelectionHoverColor), UseNodeBoundsForSelection ? nodeBounds.X : 0, nodeY, Width, ItemHeight);

            int xOffset = nodeBounds.X;

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
                        arrowTexture = uwfAppOwner.Resources.TreeNodeExpanded;
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
                        arrowTexture = uwfAppOwner.Resources.TreeNodeCollapsed;
                }

                if (arrowTexture != null)
                {
                    var arrowWidth = arrowSize;
                    var arrowHeight = arrowSize;
                    graphics.DrawImage(arrowTexture, xOffset, nodeY + nodeBounds.Height / 2f - arrowHeight / 2f, arrowWidth, arrowHeight);
                }
            }

            xOffset += arrowSize;

            // Draw image.
            if (node.ImageIndex > -1)
            {
                var image = ImageList.Images[node.ImageIndex];
                if (image != null && image.uTexture != null)
                {
                    graphics.uwfDrawImage(image, node.ImageColor, xOffset, nodeY + nodeBounds.Height / 2f - image.Height / 2f, image.Width, image.Height);
                    xOffset += image.Width + 2;
                }
            }

            // Draw text.
            string stringToDraw = node.Text;
            if (stringToDraw == null && node.Tag != null) stringToDraw = node.Tag.ToString();
            graphics.uwfDrawString(stringToDraw, Font, node.ForeColor, xOffset, nodeY - 2, (WrapText ? Width : Width * 16), e.Bounds.Height + 4, ContentAlignment.MiddleLeft);
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
            var nextNode = nodeList.FindIndex(x => x == SelectedNode); // TODO: this is slow implementation. Should remember current selectedIndex.
            while (_SelectNext(nextNode) == false)
            {
                nextNode++;
            }
        }
        private bool _SelectNext(int fromIndex)
        {
            if (fromIndex + 1 < nodeList.Count)
            {
                if (nodeList[fromIndex + 1].Enabled == false) return false;

                SelectedNode = nodeList[fromIndex + 1];
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
            var prevNode = nodeList.FindIndex(x => x == SelectedNode);
            while (_SelectPrevious(prevNode) == false)
            {
                prevNode--;
            }
        }
        private bool _SelectPrevious(int fromIndex)
        {
            if (fromIndex - 1 >= 0)
            {
                if (nodeList[fromIndex - 1].Enabled == false) return false;

                SelectedNode = nodeList[fromIndex - 1];
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
                vScrollBar.ValueChanged -= VScrollBarOnValueChanged;
                vScrollBar.Maximum = ItemHeight * nodeList.Count - 1;
                vScrollBar.SmallChange = ItemHeight;
                vScrollBar.LargeChange = Height;
                vScrollBar.Visible = vScrollBar.Maximum > Height;
                vScrollBar.ValueChanged += VScrollBarOnValueChanged;
            }
        }
        private void _UpdateScrollList()
        {
            scrollNodeList = new List<TreeNode>();

            int startNode = (int)(ScrollIndex / ItemHeight) - 1;
            if (startNode < 0) startNode = 0;
            int nodesOnScreen = Height / ItemHeight + 3; // Magic number.

            for (int i = startNode; i < startNode + nodesOnScreen && i < nodeList.Count; i++)
            {
                var node = nodeList[i];
                if (node.Bounds.Y + node.Bounds.Height > 0 && node.Bounds.Y - (int)ScrollIndex < Height)
                    scrollNodeList.Add(node);
            }

            UpdateScrollBar();

            filter = string.Empty; // reset filter.
            resetFilterTime = 0;
        }
        private void VScrollBarOnValueChanged(object sender, EventArgs eventArgs)
        {
            _UpdateScrollList();
        }
    }
}
