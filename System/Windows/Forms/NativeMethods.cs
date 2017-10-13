namespace System.Windows.Forms
{
    internal static class NativeMethods
    {
        public const int BDR_RAISEDINNER = 0x0004,
                         BDR_RAISEDOUTER = 0x0001,
                         BDR_SUNKENINNER = 0x0008,
                         BDR_SUNKENOUTER = 0x0002,
                         BF_LEFT = 0x0001,
                         BF_TOP = 0x0002,
                         BF_RIGHT = 0x0004,
                         BF_BOTTOM = 0x0008,
                         BF_ADJUST = 0x2000,
                         BF_FLAT = 0x4000,
                         BF_MIDDLE = 0x0800,
                         EDGE_BUMP = 0x0001 | 0x0008,
                         EDGE_ETCHED = 0x0002 | 0x0004,
                         EDGE_RAISED = 0x0001 | 0x0004,
                         EDGE_SUNKEN = 0x0002 | 0x0008;
    }
}
