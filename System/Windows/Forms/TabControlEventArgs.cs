namespace System.Windows.Forms
{
    public delegate void TabControlEventHandler(object sender, TabControlEventArgs e);

    public enum TabControlAction
    {
        Selecting,
        Selected,
        Deselecting,
        Deselected
    }

    public class TabControlEventArgs : EventArgs
    {
        public TabControlEventArgs(TabPage tabPage, int tabPageIndex, TabControlAction action)
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
