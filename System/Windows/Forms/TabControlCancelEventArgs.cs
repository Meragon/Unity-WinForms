namespace System.Windows.Forms
{
    using System.ComponentModel;

    public delegate void TabControlCancelEventHandler(object sender, TabControlCancelEventArgs e);

    public class TabControlCancelEventArgs : CancelEventArgs
    {
        public TabControlCancelEventArgs(TabPage tabPage, int tabPageIndex, bool cancel, TabControlAction action) : base(cancel)
        {
            Action = action;
            TabPage = tabPage;
            TabPageIndex = tabPageIndex;
        }

        public TabControlAction Action { get; private set; }
        public TabPage TabPage { get; private set; }
        public int TabPageIndex { get; private set; }
    }
}
