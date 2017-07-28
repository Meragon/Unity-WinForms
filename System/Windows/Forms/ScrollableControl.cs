namespace System.Windows.Forms
{
    [Serializable]
    public class ScrollableControl : Control
    {
        public ScrollableControl()
        {
            SetStyle(ControlStyles.ContainerControl, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, false);
        }
    }
}
