using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace System.Windows.Forms
{
    public class TreeView_Old : Control
    {
        public const int DefaultNodeHeight = 16;

        public Color BorderColor { get; set; }
        public TreeNode_Old Node { get; set; }
        public StringFormat NodeAlign { get; set; }
        public int NodeHeight { get; set; }

        private List<_nodeControl> _list = new List<_nodeControl>();
        private List<_nodeControl> _renderList = new List<_nodeControl>();

        private int _selectedIndex = -1;

        private bool _scroll;
        private int _scrollStartY;
        private int _scrollIndex;
        private int _scrollY;
        private int _scrollWidth = 10;
        private int _scrollHeight = 32;
        private int _scrollItems;
        private bool _scrollHover = false;

        private bool _drag;
        private TreeNode_Old _dragNode;
        private Point _dragPosition;

        public TreeView_Old()
        {
            NodeAlign = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near };
            NodeHeight = DefaultNodeHeight;
        }

        public void AddjustScrollViewToSelectedIndex()
        {
            if (_selectedIndex < -1)
                _selectedIndex = -1;
            if (_selectedIndex > _list.Count)
                _selectedIndex = _list.Count - 1;
            if (_selectedIndex < _scrollIndex) // Up.
                _scrollIndex = _selectedIndex;
            if (_selectedIndex > _scrollIndex + _renderList.Count - 1) // Down.
                _scrollIndex = _selectedIndex - _renderList.Count + 1;
            _scrollIndex = _scrollIndex < 0 ? 0 : _scrollIndex;
            _scrollIndex = _scrollIndex > _scrollItems - _renderList.Count ? _scrollItems - _renderList.Count : _scrollIndex;
            Refresh();
            
            if (_selectedIndex > -1)
                _list[_selectedIndex].Node.OnClickCall(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0), _list[_selectedIndex].Node);
        }

        protected override void OnKeyPress(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.KeyCode)
            {
                case UnityEngine.KeyCode.DownArrow:
                    if (_selectedIndex + 1 < _list.Count)
                    { 
                        _selectedIndex++;
                        AddjustScrollViewToSelectedIndex();
                    }
                    break;
                case UnityEngine.KeyCode.LeftArrow:
                    if (_selectedIndex - _renderList.Count > -1)
                        _selectedIndex -= _renderList.Count;
                    else
                        _selectedIndex = 0;
                    AddjustScrollViewToSelectedIndex();
                    break;
                case UnityEngine.KeyCode.RightArrow:
                    if (_selectedIndex + _renderList.Count < _list.Count)
                        _selectedIndex += _renderList.Count;
                    else
                        _selectedIndex = _list.Count - 1;
                    AddjustScrollViewToSelectedIndex();
                    break;
                case UnityEngine.KeyCode.UpArrow:
                    if (_selectedIndex - 1 > -1)
                    {
                        _selectedIndex--;
                        AddjustScrollViewToSelectedIndex();
                    }
                    break;
            }
        }
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            var mclient = PointToClient(MousePosition);
            for (int i = 0; i < _renderList.Count; i++)
            {
                Rectangle nodeRect = new Rectangle(0, i * NodeHeight, Width - _scrollWidth, NodeHeight);
                if (nodeRect.Contains(mclient))
                {
                    _renderList[i].Node.CollapseExpand();
                    _renderList[i].Node.OnDoubleClickCall(e, _renderList[i].Node);
                    _RefreshList();
                    break;
                }
            }
            Refresh();
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            var mclient = PointToClient(MousePosition);

            // Scroll.
            Rectangle _scrollRect = new Rectangle(Width - _scrollWidth, _scrollY, _scrollWidth, _scrollHeight);
            if (_scrollRect.Contains(mclient))
            {
                _scroll = true;
                _scrollStartY = mclient.Y - _scrollY;
            }
            else
                _scroll = false;

            // Drag items.
            if (!_scroll)
                for (int i = 0; i < _renderList.Count; i++)
                {
                    Rectangle nodeRect = new Rectangle(0, i * NodeHeight, Width - _scrollWidth, NodeHeight);
                    if (nodeRect.Contains(mclient))
                    {
                        _selectedIndex = _renderList[i].Index;
                        if (!_renderList[i].Node.AllowDrag) break;
                        _drag = true;
                        _dragNode = _renderList[i].Node;
                        _dragPosition = mclient;
                        break;
                    }
                }
        }
        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);
            var mclient = PointToClient(MousePosition);
            for (int i = 0; i < _renderList.Count; i++)
            {
                Rectangle nodeRect = new Rectangle(0, i * NodeHeight, Width - _scrollWidth, NodeHeight);
                if (nodeRect.Contains(mclient))
                    _renderList[i].Hovered = true;
                else
                    _renderList[i].Hovered = false;
            }
            // Scroll.
            Rectangle _scrollRect = new Rectangle(Width - _scrollWidth, _scrollY, _scrollWidth, _scrollHeight);
            if (_scrollRect.Contains(mclient))
                _scrollHover = true;
            else
                _scrollHover = false;
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            for (int i = 0; i < _renderList.Count; i++)
            {
                if (_renderList[i].Hovered) _renderList[i].Hovered = false;
            }
            _scrollHover = false;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            var mclient = PointToClient(MousePosition);
            if (_scroll)
            {


                _scrollIndex = (mclient.Y - _scrollStartY) * _scrollItems / Height;
                _scrollIndex = _scrollIndex < 0 ? 0 : _scrollIndex;
                _scrollIndex = _scrollIndex > _scrollItems - _renderList.Count ? _scrollItems - _renderList.Count : _scrollIndex;

                _scrollY = _scrollIndex * Height / _scrollItems;
                Refresh();
            }
            if (_drag && _dragPosition.Distance(mclient) > 4)
            {
                Drag(_dragNode);
                _drag = false;
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _scroll = false;
            _drag = false;

            var mclient = PointToClient(MousePosition);
            for (int i = 0; i < _renderList.Count; i++)
            {
                Rectangle nodeRect = new Rectangle(0, i * NodeHeight, Width - _scrollWidth, NodeHeight);
                if (nodeRect.Contains(mclient))
                {
                    _renderList[i].Node.OnClickCall(e, _renderList[i].Node);
                    return;
                }
            }

            _selectedIndex = -1;
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            _scrollIndex -= e.Delta;
            _scrollIndex = _scrollIndex < 0 ? 0 : _scrollIndex;
            _scrollIndex = _scrollIndex > _scrollItems - _renderList.Count ? _scrollItems - _renderList.Count : _scrollIndex;
            //UnityEngine.Debug.Log(_scrollIndex);
            Refresh();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width < 16) return;

            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.FillRectangle(new SolidBrush(BackColor), 0, 0, Width, Height);
            
            for (int i = 0; i < _renderList.Count; i++)
            {
                if (_renderList[i].Index == _selectedIndex)
                    g.FillRectangle(new SolidBrush(Color.FromArgb(64, 0, 122, 204)), 0, i * NodeHeight, Width - _scrollWidth, NodeHeight);
                if (_renderList[i].Hovered)
                    g.FillRectangle(new SolidBrush(Color.FromArgb(64, 0, 122, 204)), 0, i * NodeHeight, Width - _scrollWidth, NodeHeight);
                Color _nodeColor = _renderList[i].Node.TextColor;
                if ((_nodeColor.R > 192 && _nodeColor.G > 192 && _nodeColor.B > 192) || _nodeColor.A < 192)
                    _nodeColor = Color.FromArgb(64, 64, 64);
                int nodeOffset = 8 * _renderList[i].Depth;
                g.DrawString(_renderList[i].Node.Text, Font, new SolidBrush(_nodeColor), new RectangleF(nodeOffset + 6, i * NodeHeight - 2, Width - _scrollWidth - nodeOffset, NodeHeight + 4), NodeAlign);
                if (_renderList[i].Node.Nodes.Count > 0)
                {
                    var nodeTexture = _renderList[i].Node.Collapsed ? Application.Resources.Reserved.TreeFolderClosed : Application.Resources.Reserved.TreeFolderOpened;
                    g.DrawTexture(nodeTexture, nodeOffset - 12, i * NodeHeight, 16, 16);
                }
                else
                {
                    var image = Application.Resources.Reserved.TreeFile;
                    if (_renderList[i].Node.Image != null)
                        image = _renderList[i].Node.Image;
                    g.DrawTexture(image, nodeOffset - 12, i * NodeHeight, image.width, image.height);
                }
            }
            //g.GroupEnd();

            // Scroll.
            if (_renderList.Count < _scrollItems)
            {
                Color _scrollColor = Color.FromArgb(222, 222, 230);
                if (_scrollHover || _scroll) _scrollColor = Color.FromArgb(136, 136, 136);
                g.FillRectangle(new SolidBrush(_scrollColor), Width - _scrollWidth + 2, _scrollY, _scrollWidth - 2, _scrollHeight);
            }

            g.DrawRectangle(new Pen(BorderColor), 0, 0, Width, Height);
        }

        public override void Refresh()
        {
            base.Refresh();
            _renderList = new List<_nodeControl>();
            Node.Reset();
            int totalItems = 0;
            foreach (var node in _list)
            {
                if ((node.Index - _scrollIndex) * NodeHeight < Height && (node.Index - _scrollIndex) >= 0)
                    _renderList.Add(node);
                totalItems++;
            }
            _scrollItems = totalItems;
            if (_scrollItems == 0) return;
            _scrollY = _scrollIndex * Height / _scrollItems;
            _scrollHeight = (int)(((float)_renderList.Count / _scrollItems) * Height);
        }
        public void _RefreshList()
        {
            _list = new List<_nodeControl>();
            int index = 0;
            foreach (TreeNode_Old node in Node)
            {
                if (node.Hidden) continue;
                _list.Add(new _nodeControl() { Node = node, Index = index, Depth = node.GetDepth() });
                index++;
            }
        }

        public event DragHandler Drag = delegate { };

        public delegate void DragHandler(TreeNode_Old node);

        private class _nodeControl
        {
            public TreeNode_Old Node { get; set; }
            public int Index { get; set; }
            public int Depth { get; set; }
            public bool Hovered { get; set; }
        }
    }

    public class TreeNode_Old : IEnumerable, IEnumerator
    {
        private static int _idPool;
        private int _id;

        public bool AllowDrag { get; set; }
        public object Data { get; set; }
        public UnityEngine.Texture2D Image { get; set; }
        public int Index { get; private set; }
        private string _text = "";
        public string Text
        {
            get { return _text; }
            set
            {
                //TextComponent.text = value;
                _text = value;
            }
        }

        public Color TextColor { get; set; }

        public List<TreeNode_Old> Nodes = new List<TreeNode_Old>();
        public TreeNode_Old Parent;

        private bool _collapsed = false;
        public bool Collapsed
        {
            get { return _collapsed; }
        }

        private bool _hidden = false;
        public bool Hidden
        {
            get
            {
                return _hidden;
            }
        }

        public TreeNode_Old()
        {
            _idPool++;
            _id = _idPool;
        }
        public TreeNode_Old(string text)
        {
            _idPool++;
            _id = _idPool;
            _text = text;
        }
        
        internal string GetPath()
        {
            string path = Text;
            if (Parent != null)
                path = Parent.GetPath() + " / " + path;
            return path;
        }
        internal int GetDepth()
        {
            int depth = 1;
            if (Parent != null)
                depth += Parent.GetDepth();
            return depth;
        }
        internal int GetTreeIndex() // ? dunno.
        {
            TreeNode_Old parent_node = GetRootNode();
            return parent_node.FindTreeIndex(this).Index;
        }
        internal TreeNode_Old GetRootNode()
        {
            TreeNode_Old parent_node;
            if (Parent == null)
                parent_node = this;
            else
            {
                parent_node = Parent;
                while (parent_node.Parent != null)
                    parent_node = parent_node.Parent;
            }
            return parent_node;
        }
        internal FindResult FindTreeIndex(TreeNode_Old node)
        {
            FindResult res = new FindResult();
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (node._id == Nodes[i]._id)
                    return new FindResult() { Found = true, Index = res.Index + 1 };

                var node_res = Nodes[i].FindTreeIndex(node);
                if (!_collapsed && !Nodes[i]._hidden)
                    res.Index += node_res.Index + 1;
                res.Found = node_res.Found;
                if (node_res.Found)
                    return res;
            }
            return new FindResult() { Found = false, Index = res.Index };
        }
        internal void UpdateContent()
        {
            if (_text != "") Text = _text;
        }

        public void AddNode(TreeNode_Old node)
        {
            Nodes.Add(node);
            node.Index = Nodes.Count;
            node.Parent = this;
            //if (node.Self == null && node.CreateGO)
            //    node.Self = node.CreateNode();
            node.UpdateContent();
        }
        public void CollapseExpand()
        {
            _collapsed = !_collapsed;
            if (_collapsed)
                Collapse();
            else
                Expand();
        }
        public void Collapse()
        {
            _collapsed = true;
            foreach (var n in Nodes)
                n.Hide();
        }
        public void Expand()
        {
            _collapsed = false;
            foreach (var n in Nodes)
                n.Show();
        }
        public void Hide()
        {
            /*if (Self != null)
            {
                TextComponent.color = new Color(1f, 1f, 1f, 0f);
                if (ImageComponent != null) ImageComponent.enabled = false;

                var uiRI = Self.GetComponent<UIRaycastIgnored>();
                if (uiRI == null) Self.gameObject.AddComponent<UIRaycastIgnored>();

                foreach (var n in Nodes)
                    n.Hide();

                Self.gameObject.SetActive(false);
            }*/

            foreach (var n in Nodes)
                n.Hide();

            _hidden = true;
        }
        public void RemoveNodeAt(int index)
        {
            Nodes.RemoveAt(index);
            for (int i = 0; i < Nodes.Count; i++)
                Nodes[i].Index = i + 1;
        }
        public void Show()
        {
            /*if (Self != null)
            {
                TextComponent.color = Settings.TextColor;
                if (ImageComponent != null) ImageComponent.enabled = true;

                var uiRI = Self.GetComponent<UIRaycastIgnored>();
                if (uiRI != null) UnityEngine.Object.Destroy(uiRI);

            

                //if (Self.GetComponent<Renderer>().isVisible)
                Self.gameObject.SetActive(true);
            }*/

            if (!_collapsed)
                foreach (var n in Nodes)
                    n.Show();

            _hidden = false;
        }

        private TreeNode_Old _current;

        public IEnumerator GetEnumerator()
        {
            return this;
        }
        public object Current
        {
            get
            {
                return _current;
            }
        }
        public bool MoveNext()
        {
            if (_current == null) _current = this;
            if (_current.Nodes.Count > 0)
            {
                _current = _current.Nodes[0];
                return true;
            }

            var parentNode = _current.Parent;
            while (parentNode != null && _current._id != this._id)
            {
                if (_current.Index + 1 > parentNode.Nodes.Count)
                {
                    _current = parentNode;
                    parentNode = _current.Parent;
                }
                else
                {
                    _current = parentNode.Nodes[_current.Index];
                    return true;
                }
            }

            return false;
        }
        public void Reset()
        {
            _current = this;
        }

        public delegate void OnClickDelegate(MouseEventArgs e, TreeNode_Old self);
        public event OnClickDelegate OnClick = delegate { };
        public event OnClickDelegate OnDoubleClick = delegate { };

        internal void OnClickCall(MouseEventArgs e, TreeNode_Old self)
        {
            OnClick(e, self);
        }
        internal void OnDoubleClickCall(MouseEventArgs e, TreeNode_Old self)
        {
            OnDoubleClick(e, self);
        }

        public override string ToString()
        {
            return _text;
        }

        public struct FindResult
        {
            public bool Found;
            public int Index;
        }
    }
}
