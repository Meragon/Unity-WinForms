namespace System.Windows.Forms
{
    [Flags]
    public enum Border3DSide
    {
        Left = NativeMethods.BF_LEFT,
        Top = NativeMethods.BF_TOP,
        Right = NativeMethods.BF_RIGHT,
        Bottom = NativeMethods.BF_BOTTOM,
        Middle = NativeMethods.BF_MIDDLE,
        All = Left | Top | Right | Bottom | Middle,
    }
}
