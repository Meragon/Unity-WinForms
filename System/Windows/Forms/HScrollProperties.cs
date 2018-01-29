namespace System.Windows.Forms
{
    public class HScrollProperties : ScrollProperties
    {
        public HScrollProperties(ScrollableControl container) : base(container)
        {
        }

        internal override int PageSize
        {
            get { return ParentControl.ClientRectangle.Width; }
        }
        internal override int Orientation
        {
            get { return NativeMethods.SB_HORZ; }
        }
        internal override int HorizontalDisplayPosition
        {
            get { return -value; }
        }
        internal override int VerticalDisplayPosition
        {
            get { return ParentControl.DisplayRectangle.Y; }
        }
    }
}
