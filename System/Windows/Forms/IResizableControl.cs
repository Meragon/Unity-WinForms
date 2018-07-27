namespace System.Windows.Forms
{
    using System.Drawing;

    [Flags]
    public enum ControlResizeTypes
    {
        None = 0,

        Right = 1,
        Down = 2,
        Left = 4,
        Up = 8,

        DownUp = Down | Up,
        RightDown = Right | Down,
        LeftDown = Left | Down,
        LeftRight = Left | Right,
        LeftUp = Left | Up,
        RightUp = Right | Up,
    }

    public interface IResizableControl
    {
        /// <summary>
        /// Updates cursor type depending on resize type.
        /// </summary>
        /// <param name="mclient">Relative mouse position to client</param>
        /// <returns></returns>
        ControlResizeTypes GetResizeAt(Point mclient);
    }
}
