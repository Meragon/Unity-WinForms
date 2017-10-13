namespace System.Windows.Forms
{
    public enum Border3DStyle
    {
        Adjust = NativeMethods.BF_ADJUST,
        Bump = NativeMethods.EDGE_BUMP,
        Etched = NativeMethods.EDGE_ETCHED,
        Flat = NativeMethods.BF_FLAT | NativeMethods.EDGE_SUNKEN,
        Raised = NativeMethods.EDGE_RAISED,
        RaisedInner = NativeMethods.BDR_RAISEDINNER,
        RaisedOuter = NativeMethods.BDR_RAISEDOUTER,
        Sunken = NativeMethods.EDGE_SUNKEN,
        SunkenInner = NativeMethods.BDR_SUNKENINNER,
        SunkenOuter = NativeMethods.BDR_SUNKENOUTER,
    }
}
