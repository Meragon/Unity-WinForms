namespace System.Windows.Forms
{
    [Flags]
    public enum ControlStyles
    {
        ContainerControl = 0x1,
        UserPaint = 0x2,
        Opaque = 0x4,
        ResizeRedraw = 0x10,
        FixedWidth = 0x20,
        FixedHeight = 0x40,
        StandardClick = 0x100,
        Selectable = 0x200,
        UserMouse = 0x400,
        SupportsTransparentBackColor = 0x800,
        StandardDoubleClick = 0x1000,
        AllPaintingInWmPaint = 0x2000,
        CacheText = 0x4000,
        EnableNotifyMessage = 0x8000,
        DoubleBuffer = 0x10000,
        OptimizedDoubleBuffer = 0x20000,
        UseTextForAccessibility = 0x40000,
    }
}
