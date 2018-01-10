namespace System.Windows.Forms
{
    using System.Drawing;

    public enum ControlResizeTypes
    {
        None,

        Right,
        Down,
        Left,
        Up,

        RightDown,
        LeftDown,
        LeftUp,
        RightUp
    }

    public interface IResizableControl
    {
        ControlResizeTypes GetResizeAt(Point mclient);
    }
}
