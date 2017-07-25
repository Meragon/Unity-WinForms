namespace System.Windows.Forms
{
    using System.Drawing;

    public class DrawTreeNodeEventArgs : EventArgs
    {
        public DrawTreeNodeEventArgs(Graphics graphics, TreeNode node, Rectangle bounds, TreeNodeStates state)
        {
            Graphics = graphics;
            Node = node;
            Bounds = bounds;
            State = state;
        }

        public Rectangle Bounds { get; internal set; }
        public bool DrawDefault { get; internal set; }
        public Graphics Graphics { get; internal set; }
        public TreeNode Node { get; internal set; }
        public TreeNodeStates State { get; internal set; }
    }
}
