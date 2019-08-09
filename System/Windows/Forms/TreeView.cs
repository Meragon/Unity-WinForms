namespace System.Windows.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    public class TreeView : Control
    {
        internal int       arrowSize = 16;
        internal TreeNode  hoveredNode;
        internal TreeNode  root;
        
        internal float uwfScrollSpeed = 2;
        internal Color uwfArrowImageColor = Color.FromArgb(0x26, 0x26, 0x26);
        internal Color uwfArrowImageHoveredColor = Color.FromArgb(0x27, 0xC7, 0xF7);
        internal Color uwfItemHoveredColor = Color.FromArgb(221, 238, 253);
        internal Color uwfItemSelectedColor = Color.FromArgb(187, 222, 251); // Default is SystemColors.Highlight.
        internal Color uwfItemSelectedUnfocusedColor = Color.FromArgb(187, 222, 251);

        private readonly DrawTreeNodeEventArgs nodeArgs = new DrawTreeNodeEventArgs(null, null, Rectangle.Empty, TreeNodeStates.Default);
        private readonly List<TreeNode> nodeList = new List<TreeNode>();
        private readonly DrawTreeNodeEventHandler onDrawNode;
        private readonly List<TreeNode> scrollNodeList = new List<TreeNode>();

        private bool      drag;
        private TreeNode  dragNode;
        private Point     dragPosition;
        private string    filter;
        private TreeNode  mouseDownAtNode;
        private ToolTip   nodeToolTip;
        private TreeNode  nodeToolTipLast;
        private TreeNode  selectedNode;
        private float     resetFilterTime;
        private ScrollBar vScrollBar;

        public TreeView()
        {
            BackColor = Color.White;
            BorderStyle = BorderStyle.Fixed3D;
            ItemHeight = 22;
            Padding = new Padding(4);

            root = new TreeNode(this);
            root.IsExpanded = true;

            Nodes = new TreeNodeCollection(root);

            onDrawNode = DrawNodeDefault;

            MouseHook.MouseUp += Application_UpClick;
        }

        public event TreeViewEventHandler AfterSelect;
        public event DrawTreeNodeEventHandler DrawNode;
        public event ItemDragEventHandler ItemDrag;
        public event TreeNodeMouseClickEventHandler NodeMouseClick;
        public event TreeNodeMouseClickEventHandler NodeMouseDoubleClick;
        public event TreeNodeMouseHoverEventHandler NodeMouseHover;

        public BorderStyle BorderStyle { get; set; }
        public bool FullRowSelect { get; set; }
        public ImageList ImageList { get; set; }
        public int ItemHeight { get; set; }
        public TreeNodeCollection Nodes { get; private set; }
        public TreeNode SelectedNode
        {
            get { return selectedNode;}
            set
            {
                if (selectedNode == value)
                    return;

                selectedNode = value;

                if (value != null)
                {
                    EnsureVisible(value);
                    OnAfterSelect(new TreeViewEventArgs(value));
                }
            }
        }
        public bool ShowNodeToolTips { get; set; }

        internal ScrollBar uwfVScrollBar
        {
            get
            {
                if (vScrollBar == null)
                    uwfVScrollBar = new VScrollBar();

                return vScrollBar;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                
                if (vScrollBar != null)
                    vScrollBar.Dispose();
                
                vScrollBar = value;
                vScrollBar.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
                vScrollBar.Location = new Point(Width - vScrollBar.Width, 0);
                vScrollBar.Height = Height;
                vScrollBar.ValueChanged += VScrollBarOnValueChanged;
                Controls.Add(vScrollBar);
            }
        }

        protected override Size DefaultSize
        {
            get { return new Size(121, 97); }
        }

        private int nodesOnScreen
        {
            get { return Height / ItemHeight; } 
        }
        private float scrollIndex
        {
            get
            {
                if (!uwfVScrollBar.Visible)
                    return 0;
                
                return uwfVScrollBar.Value;
            }
            set
            {
                if (uwfVScrollBar.Visible)
                    uwfVScrollBar.Value = (int)value;
            }
        }
        
        public void CollapseAll()
        {
            var nodesCount = Nodes.Count;
            for (int i = 0; i < nodesCount; i++)
                Nodes[i].CollapseInternal();
            Refresh();
        }
        public TreeNode GetNodeAt(int x, int y)
        {
            var scrollNodeListCount = scrollNodeList.Count;
            for (int i = 0; i < scrollNodeListCount; i++)
            {
                var node = scrollNodeList[i];
                var nodeY = node.Bounds.Y;
                var nodeH = node.Bounds.Height;

                if (y >= nodeY && y < nodeY + nodeH)
                    return node;
            }

            return null;
        }
        public TreeNode GetNodeAt(Point pt)
        {
            return GetNodeAt(pt.X, pt.Y);
        }
        public void ExpandAll()
        {
            var nodesCount = Nodes.Count;
            for (int i = 0; i < nodesCount; i++)
                Nodes[i].ExpandAllInternal();
            Refresh();
        }
        public override void Refresh()
        {
            nodeList.Clear();
            
            UpdateNodeList(root);
            UpdateScrollList();
        }

        internal void EnsureVisible(TreeNode node)
        {
            if (node == null)
                return;
            
            var nodeIsInScrollList = scrollNodeList.Find(x => x == node);
            if (nodeIsInScrollList != null) return;

            AdjustScrollIndexToNode(node);

            UpdateScrollList();
        }
        /// <summary>
        /// only for visible nodes.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        internal TreeNode Find(Predicate<TreeNode> match)
        {
            return nodeList.Find(match);
        }

        protected internal override void uwfOnLatePaint(PaintEventArgs e)
        {
            ControlPaint.PrintBorder(e.Graphics, ClientRectangle, BorderStyle, Border3DStyle.Flat);
        }

        protected override void Dispose(bool release_all)
        {
            MouseHook.MouseUp -= Application_UpClick;

            base.Dispose(release_all);
        }
        protected virtual void OnAfterSelect(TreeViewEventArgs e)
        {
            var handler = AfterSelect;
            if (handler != null)
                handler(this, e);
        }
        protected virtual void OnDrawNode(DrawTreeNodeEventArgs e)
        {
            if (onDrawNode != null) onDrawNode(this, e);
        }
        protected virtual void OnItemDrag(ItemDragEventArgs e)
        {
            var handler = ItemDrag;
            if (handler != null)
                handler(this, e);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

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
                        SelectNext();
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
                        SelectPrevious();
                        break;

                    case Keys.PageDown:
                        if (nodeList.Count > 0)
                        {
                            var lNodesOnScreen = nodesOnScreen;
                            var nextIndex = 0;
                            if (SelectedNode != null)
                                nextIndex = nodeList.IndexOf(SelectedNode);
                            nextIndex = MathHelper.Clamp(nextIndex + lNodesOnScreen, 0, nodeList.Count - 1);
                            SelectedNode = nodeList[nextIndex];
                        }
                        break;
                    case Keys.PageUp:
                        if (nodeList.Count > 0)
                        {
                            var lNodesOnScreen = nodesOnScreen;
                            var nextIndex = 0;
                            if (SelectedNode != null)
                                nextIndex = nodeList.IndexOf(SelectedNode);
                            nextIndex = MathHelper.Clamp(nextIndex - lNodesOnScreen, 0, nodeList.Count - 1);
                            SelectedNode = nodeList[nextIndex];
                        }
                        break;
                    case Keys.End:
                        if (nodeList.Count > 0)
                            SelectedNode = nodeList[nodeList.Count - 1];
                        break;
                    case Keys.Home:
                        if (nodeList.Count > 0)
                            SelectedNode = nodeList[0];
                        break;
                }
            }
            else if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.Down:
                        if (uwfVScrollBar.Visible)
                            uwfVScrollBar.DoScroll(ScrollEventType.SmallIncrement);
                        break;
                    case Keys.Up:
                        if (uwfVScrollBar.Visible)
                            uwfVScrollBar.DoScroll(ScrollEventType.SmallDecrement);
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
            base.OnMouseDoubleClick(e);
            
            var sNode = SelectAtPosition(e);
            if (sNode != null)
            {
                sNode.Toggle();
                OnNodeMouseDoubleClick(new TreeNodeMouseClickEventArgs(sNode, e.Button, e.Clicks, e.X, e.Y));
            }
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            mouseDownAtNode = GetNodeAt(e.Location);
            if (mouseDownAtNode == null)
                return;

            // Try to toggle node with mouse.
            if (mouseDownAtNode.Nodes.Count > 0 && 
                e.X >= mouseDownAtNode.Bounds.X && 
                e.X <= mouseDownAtNode.Bounds.X + arrowSize)
            {
                mouseDownAtNode.Toggle();
                mouseDownAtNode = null; // Prevent node from being selected.
                return;
            }
            
            drag = true;
            dragNode = mouseDownAtNode;
            dragPosition = e.Location;
        }
        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);
            
            var mclient = PointToClient(MousePosition);

            hoveredNode = GetNodeAt(mclient);
            if (hoveredNode != null)
            {
                if (ShowNodeToolTips)
                {
                    if (!string.IsNullOrEmpty(hoveredNode.ToolTipText))
                    {
                        if (nodeToolTip == null)
                        {
                            nodeToolTip = new ToolTip();
                            nodeToolTip.InitialDelay = 0;
                        }

                        if (hoveredNode != nodeToolTipLast)
                            nodeToolTip.Show(hoveredNode.ToolTipText, mclient);

                        nodeToolTipLast = hoveredNode;
                    }
                    else
                        nodeToolTipLast = null;
                }

                // Tooltip will show previous ToolTipText. Override OnMouseHover if necessary.
                OnNodeMouseHover(new TreeNodeMouseHoverEventArgs(hoveredNode));
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!drag) return;
            if (dragPosition.Distance(e.Location) < 4) return;
            
            OnItemDrag(new ItemDragEventArgs(e.Button, dragNode));
                    
            dragPosition = Point.Empty;
            drag = false;
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            
            if (uwfVScrollBar.Visible)
                scrollIndex -= e.Delta * uwfScrollSpeed;
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            var mouseUpAtNode = GetNodeAt(e.Location);
            if (mouseUpAtNode != mouseDownAtNode)
                return;
            
            if (SelectAtPosition(e) == null)
                return;

            OnNodeMouseClick(new TreeNodeMouseClickEventArgs(SelectedNode, e.Button, e.Clicks, e.X, e.Y));
        }
        protected virtual void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
        {
            var nodeMouseClick = NodeMouseClick;
            if (nodeMouseClick != null)
                nodeMouseClick(this, e);
        }
        protected virtual void OnNodeMouseDoubleClick(TreeNodeMouseClickEventArgs e)
        {
            var nodeMouseDoubleClick = NodeMouseDoubleClick;
            if (nodeMouseDoubleClick != null)
                nodeMouseDoubleClick(this, e);
        }
        protected virtual void OnNodeMouseHover(TreeNodeMouseHoverEventArgs e)
        {
            var handler = NodeMouseHover;
            if (handler != null)
                handler(this, e);
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

                // Instead of creating new args every frame for each node, we will use only cached one to prevent
                // additional memory allocations.
                // The problem is that you can't cache it somewhere else cause data in it will be always changing.
                nodeArgs.Graphics = e.Graphics;
                nodeArgs.Node = scrollNode;
                nodeArgs.Bounds = scrollNode.Bounds;
                nodeArgs.State = TreeNodeStates.Default;

                OnDrawNode(nodeArgs);
            }
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            Refresh();
        }
        
        private void AdjustScrollIndexToNode(TreeNode node)
        {
            if (node == null) return;

            UpdateNodesBounds(node);
            
            var nodeY = node.Bounds.Y + scrollIndex;

            if (scrollIndex > nodeY)
                scrollIndex = nodeY;

            if (scrollIndex + Height < nodeY + ItemHeight)
                scrollIndex = nodeY + ItemHeight - Height;
        }
        private void Application_UpClick(object sender, MouseEventArgs e)
        {
            drag = false;
            dragNode = null;
            dragPosition = Point.Empty;
        }
        private Rectangle GetNodeBounds(TreeNode node)
        {
            int x = Padding.Left + 2;
            int y = Padding.Top - (int)scrollIndex;
            int width = Width;
            int height = ItemHeight;

            x += (node.GetXIndex() - 1) * 8;
            y += node.indexY * ItemHeight;

            return new Rectangle(x, y, width, height);
        }
        private void DrawNodeDefault(object sender, DrawTreeNodeEventArgs e)
        {
            const int TEXT_SELECTION_MARGIN_LEFT = 4;
            const int TEXT_SELECTION_MARGIN_RIGHT = 4;
            const int TEXT_SELECTION_HMARGIN = TEXT_SELECTION_MARGIN_LEFT + TEXT_SELECTION_MARGIN_RIGHT;
            const int TEXT_MARGIN_TOP = 2;
            const int TEXT_MARGIN_BOTTOM = 2;
            
            var node = e.Node;
            var nodeBounds = node.Bounds;
            var nodeX = nodeBounds.X;
            var nodeY = nodeBounds.Y;
            var nodeWidth = nodeBounds.Width;
            var nodeContainsArrow = node.Nodes.Count > 0;
            var nodeContainsImage = node.ImageIndex > -1 && ImageList != null;
            
            var graphics = e.Graphics;

            // Node drawing.
            graphics.uwfFillRectangle(node.BackColor, nodeBounds.X, nodeY, nodeWidth, nodeBounds.Height);
            
            // Draw selection.
            if (node.IsSelected || node == hoveredNode)
            {
                var selectX = 0;
                var selectY = nodeY;
                var selectWidth = Width;
                var selectHeight = ItemHeight;

                if (!FullRowSelect)
                {
                    selectX = nodeX + arrowSize;
                    selectWidth = node.textWidth;

                    if (nodeContainsImage)
                        selectWidth += nodeBounds.Height - TEXT_SELECTION_MARGIN_LEFT;
                    else
                        selectX -= TEXT_SELECTION_MARGIN_LEFT;
                }

                graphics.uwfFillRectangle(node.SelectColorInternal, selectX, selectY, selectWidth, selectHeight);
            }
            
            int xOffset = nodeBounds.X;

            // Draw collapsed/expanded arrow.
            if (nodeContainsArrow)
            {
                var arrowTexture = (Bitmap) null;
                
                if (node.IsExpanded)
                {
                    if (node.ImageIndex_Expanded > -1 && ImageList != null)
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
                    if (node.ImageIndex_Collapsed > -1 && ImageList != null)
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
                    var mouseClient = PointToClient(MousePosition);
                    var arrowWidth = arrowSize;
                    var arrowHeight = arrowSize;
                    var arrowX = xOffset;
                    var arrowY = nodeY + nodeBounds.Height / 2f - arrowHeight / 2f;
                    var arrowColor = uwfArrowImageColor;

                    // Check if arrow is hovered.
                    if (nodeBounds.Contains(mouseClient) && mouseClient.X <= arrowX + arrowWidth)
                        arrowColor = uwfArrowImageHoveredColor;
                    
                    graphics.uwfDrawImage(arrowTexture, arrowColor, arrowX, arrowY, arrowWidth, arrowHeight);
                }
            }

            xOffset += arrowSize;
            
            // Draw image.
            if (nodeContainsImage)
            {
                var image = ImageList.Images[node.ImageIndex];
                if (image != null && image.Texture != null)
                {
                    var imageSize = Math.Min(nodeBounds.Height - 2, image.Height);
                    var imageX = xOffset + (nodeBounds.Height - imageSize) / 2f;
                    var imageY = nodeY + (nodeBounds.Height - imageSize) / 2f;
                    
                    graphics.uwfDrawImage(image, node.ImageColor, imageX, imageY, imageSize, imageSize);
                    
                    xOffset += nodeBounds.Height;
                }
            }

            // Draw text.
            var nodeText = node.Text;
            if (nodeText == null && node.Tag != null)
            {
                nodeText = node.Tag.ToString();
                if (nodeText != node.tagString)
                {
                    node.textWidth = 0;
                    node.tagString = nodeText;
                }
            }

            if (node.textWidth == 0)
                node.textWidth = (int) graphics.MeasureString(nodeText, Font).Width + TEXT_SELECTION_HMARGIN;

            var nodeTextX = xOffset;
            var nodeTextY = nodeY - TEXT_MARGIN_TOP;
            var nodeTextWidth = node.textWidth;
            var nodeTextHeight = e.Bounds.Height + TEXT_MARGIN_TOP + TEXT_MARGIN_BOTTOM;
            var nodeColor = node.ForeColorInternal;
            var nodeFont = node.NodeFontInternal;
            
            graphics.uwfDrawString(nodeText, nodeFont, nodeColor, nodeTextX, nodeTextY, nodeTextWidth, nodeTextHeight, ContentAlignment.MiddleLeft);
            
            // End of drawing.

            // TODO: raise when DrawNodeMode is Normal.
            var drawNode = DrawNode;
            if (drawNode != null)
                drawNode(this, e);
        }
        private TreeNode SelectAtPosition(MouseEventArgs e)
        {
            var node = GetNodeAt(e.Location);
            if (node == null || !node.Enabled) return null;

            SelectedNode = node;

            OnAfterSelect(new TreeViewEventArgs(SelectedNode));

            return SelectedNode;
        }
        private void SelectNext()
        {
            var nextNode = nodeList.FindIndex(x => x == SelectedNode); // TODO: this is slow implementation. Should remember current selectedIndex.
            
            while (!SelectNext(nextNode))
            {
                nextNode++;
            }
        }
        private bool SelectNext(int fromIndex)
        {
            if (fromIndex + 1 >= nodeList.Count) return true;
            if (!nodeList[fromIndex + 1].Enabled) return false;

            SelectedNode = nodeList[fromIndex + 1];
            AdjustScrollIndexToNode(SelectedNode);

            OnAfterSelect(new TreeViewEventArgs(SelectedNode));
            return true;

        }
        private void SelectNodeWText(string text, bool caseSensitive = false)
        {
            var lowerText = text;
            if (!caseSensitive) lowerText = text.ToLower();

            for (int i = 0; i < nodeList.Count; i++)
            {
                var node = nodeList[i];
                var nodeText = node.Text;
                
                if (string.IsNullOrEmpty(nodeText) && node.Tag != null)
                    nodeText = node.Tag.ToString();

                if (nodeText == null || nodeText.Length < text.Length) continue;

                var clippedNodeText = nodeText.Substring(0, text.Length);
                if (!caseSensitive) clippedNodeText = clippedNodeText.ToLower();

                if (clippedNodeText == lowerText)
                {
                    SelectedNode = node;
                    break;
                }
            }
        }
        private void SelectPrevious()
        {
            var prevNode = nodeList.FindIndex(x => x == SelectedNode);
            while (!SelectPrevious(prevNode))
            {
                prevNode--;
            }
        }
        private bool SelectPrevious(int fromIndex)
        {
            if (fromIndex - 1 >= 0)
            {
                if (!nodeList[fromIndex - 1].Enabled) return false;

                SelectedNode = nodeList[fromIndex - 1];
                AdjustScrollIndexToNode(SelectedNode);

                OnAfterSelect(new TreeViewEventArgs(SelectedNode));
                return true;
            }

            return true;
        }
        private void UpdateNodeList(TreeNode node)
        {
            if (!node.IsVisible)
                return;

            if (node != root)
            {
                node.indexY = nodeList.Count;
                nodeList.Add(node);
            }
            
            if (node.IsExpanded)
                for (int i = 0; i < node.Nodes.Count; i++)
                    UpdateNodeList(node.Nodes[i]);
        }
        private void UpdateNodesBounds(TreeNode node)
        {
            if (!node.IsVisible) return;
            
            if (node != root) 
                node.Bounds = GetNodeBounds(node);
        }
        private void UpdateScrollBar()
        {
            var scrollMaximum = ItemHeight * nodeList.Count - 1 + Padding.Vertical;
            var scrollVisible = scrollMaximum > Height;

            if (scrollVisible)
            {
                uwfVScrollBar.ValueChanged -= VScrollBarOnValueChanged;
                uwfVScrollBar.Maximum = scrollMaximum;
                uwfVScrollBar.SmallChange = ItemHeight;
                uwfVScrollBar.LargeChange = Height;
                uwfVScrollBar.Visible = true;
                uwfVScrollBar.ValueChanged += VScrollBarOnValueChanged;
            }
            else
            {
                if (uwfVScrollBar.Visible)
                {
                    uwfVScrollBar.Visible = false; // we can dispose scrollbar, but I think it's not effective.
                    uwfVScrollBar.ValueChanged -= VScrollBarOnValueChanged;
                    uwfVScrollBar.Value = 0;
                    uwfVScrollBar.ValueChanged += VScrollBarOnValueChanged;
                }
            }
        }
        private void UpdateScrollList()
        {
            scrollNodeList.Clear();

            // Update maximum before calculations.
            if (uwfVScrollBar.Visible)
            {
                uwfVScrollBar.ValueChanged -= VScrollBarOnValueChanged;
                uwfVScrollBar.Maximum = ItemHeight * nodeList.Count - 1 + Padding.Vertical;
                uwfVScrollBar.ValueChanged += VScrollBarOnValueChanged;
            }

            UpdateScrollBar();

            int startNode = (int)(scrollIndex / ItemHeight) - 1;
            if (startNode < 0) startNode = 0;
            int lNodesOnScreen = nodesOnScreen + 3;

            var nodeListCount = nodeList.Count;
            for (int i = startNode; i < startNode + lNodesOnScreen && i < nodeListCount; i++)
            {
                var node = nodeList[i];
                
                UpdateNodesBounds(node);
                
                var nodeBounds = node.Bounds;
                if (nodeBounds.Y + nodeBounds.Height > 0 && nodeBounds.Y - (int)scrollIndex < Height)
                    scrollNodeList.Add(node);
            }

            filter = string.Empty; // reset filter.
            resetFilterTime = 0;
        }
        private void VScrollBarOnValueChanged(object sender, EventArgs eventArgs)
        {
            UpdateScrollList();
        }
    }
}
