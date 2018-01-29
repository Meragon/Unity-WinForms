namespace System.Windows.Forms
{
    public class VScrollProperties : ScrollProperties
    {
        public VScrollProperties(ScrollableControl container) : base(container)
        {
        }
        internal override int PageSize
        {
            get { return ParentControl.ClientRectangle.Height; }
        }
        internal override int Orientation
        {
            get { return NativeMethods.SB_VERT; }
        }
        internal override int HorizontalDisplayPosition
        {
            get { return ParentControl.DisplayRectangle.X; }
        }
        internal override int VerticalDisplayPosition
        {
            get { return -value; }
        }
    }
}
