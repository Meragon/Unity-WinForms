namespace System.Windows.Forms
{
    public sealed class SplitterPanel : Panel
    {
        internal bool collapsed;

        private SplitContainer owner;

        public SplitterPanel(SplitContainer owner)
        {
            this.owner = owner;
        }
    }
}