namespace System.Windows.Forms
{
    public class TreeNodeMouseClickEventArgs : MouseEventArgs
    {
        public TreeNodeMouseClickEventArgs(TreeNode node, MouseButtons button, int clicks, int x, int y) : base(button, clicks, x, y, 0)
        {
            Node = node;
        }

        public TreeNode Node { get; private set; }
    }
}
