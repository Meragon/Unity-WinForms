using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public class DrawTreeNodeEventArgs : EventArgs
    {
        private Graphics _graphics;
        private TreeNode _node;
        private Rectangle _bounds;
        private TreeNodeStates _state;

        public DrawTreeNodeEventArgs(Graphics graphics, TreeNode node, Rectangle bounds, TreeNodeStates state)
        {
            _graphics = graphics;
            _node = node;
            _bounds = bounds;
            _state = state;
        }

        public Rectangle Bounds { get { return _bounds; } }
        public bool DrawDefault { get; set; }
        public Graphics Graphics { get { return _graphics; } }
        public TreeNode Node { get { return _node; } }
        public TreeNodeStates State { get { return _state; } }
    }
}
