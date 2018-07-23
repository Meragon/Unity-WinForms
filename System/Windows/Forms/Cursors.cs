namespace System.Windows.Forms
{
    using System.Drawing;

    public static class Cursors
    {
        internal static AppGdiImages.CursorImages images;
        
        private static Cursor defaultCursor;
        private static Cursor sizeAll;
        private static Cursor sizeNESW;
        private static Cursor sizeNS;
        private static Cursor sizeNWSE;
        private static Cursor sizeWE;
        private static Cursor help;
        private static Cursor hSplit;
        private static Cursor vSplit;
        private static Cursor hand;
        private static Cursor iBeam;

        public static Cursor Default
        {
            get
            {
                if (defaultCursor == null)
                    defaultCursor = new Cursor(images.Default);
                return defaultCursor;
            }
        }
        public static Cursor Hand
        {
            get
            {
                if (hand == null)
                {
                    hand = new Cursor(images.Hand);
                    hand.HotSpot = new Point(-12, -6);
                }

                return hand;
            }
        }
        public static Cursor Help
        {
            get
            {
                if (help == null)
                    help = new Cursor(images.Help);
                return help;
            }
        }
        public static Cursor HSplit
        {
            get
            {
                if (hSplit == null)
                {
                    hSplit = new Cursor(images.HSplit);
                    HSplit.SetHotspot(Cursor.PointPositionTypes.Middle, Cursor.PointPositionTypes.Middle);
                }

                return hSplit;
            }
        }
        public static Cursor IBeam
        {
            get
            {
                if (iBeam == null)
                {
                    iBeam = new Cursor(images.IBeam);
                    iBeam.SetHotspot(Cursor.PointPositionTypes.Middle, Cursor.PointPositionTypes.Middle);
                }

                return iBeam;
            }
        }
        public static Cursor SizeAll
        {
            get
            {
                if (sizeAll == null)
                {
                    sizeAll = new Cursor(images.SizeAll);
                    sizeAll.SetHotspot(Cursor.PointPositionTypes.Middle, Cursor.PointPositionTypes.Middle);
                }

                return sizeAll;
            }
        }
        public static Cursor SizeNESW
        {
            get
            {
                if (sizeNESW == null)
                {
                    sizeNESW = new Cursor(images.SizeNESW);
                    sizeNESW.SetHotspot(Cursor.PointPositionTypes.Middle, Cursor.PointPositionTypes.Middle);
                }

                return sizeNESW;
            }
        }
        public static Cursor SizeNS
        {
            get
            {
                if (sizeNS == null)
                {
                    sizeNS = new Cursor(images.SizeNS);
                    sizeNS.SetHotspot(Cursor.PointPositionTypes.Middle, Cursor.PointPositionTypes.Middle);
                }

                return sizeNS;
            }
        }
        public static Cursor SizeNWSE
        {
            get
            {
                if (sizeNWSE == null)
                {
                    sizeNWSE = new Cursor(images.SizeNWSE);
                    sizeNWSE.SetHotspot(Cursor.PointPositionTypes.Middle, Cursor.PointPositionTypes.Middle);
                }

                return sizeNWSE;
            }
        }
        public static Cursor SizeWE
        {
            get
            {
                if (sizeWE == null)
                {
                    sizeWE = new Cursor(images.SizeWE);
                    sizeWE.SetHotspot(Cursor.PointPositionTypes.Middle, Cursor.PointPositionTypes.Middle);
                }

                return sizeWE;
            }
        }
        public static Cursor VSplit
        {
            get
            {
                if (vSplit == null)
                {
                    vSplit = new Cursor(images.VSplit);
                    vSplit.SetHotspot(Cursor.PointPositionTypes.Middle, Cursor.PointPositionTypes.Middle);
                }

                return vSplit;
            }
        }
    }
}
