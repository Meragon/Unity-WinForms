using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Forms
{
    public sealed class Cursors
    {
        private static AppGdiImages.CursorImages Images = ApplicationBehaviour.GdiImages.Cursors;

        private static Cursor appStarting = null;
        private static Cursor arrow = null;
        private static Cursor cross = null;
        private static Cursor defaultCursor = null;
        private static Cursor iBeam = null;
        private static Cursor no = null;
        private static Cursor sizeAll = null;
        private static Cursor sizeNESW = null;
        private static Cursor sizeNS = null;
        private static Cursor sizeNWSE = null;
        private static Cursor sizeWE = null;
        private static Cursor upArrow = null;
        private static Cursor wait = null;
        private static Cursor help = null;
        private static Cursor hSplit = null;
        private static Cursor vSplit = null;
        private static Cursor noMove2D = null;
        private static Cursor noMoveHoriz = null;
        private static Cursor noMoveVert = null;
        private static Cursor panEast = null;
        private static Cursor panNE = null;
        private static Cursor panNorth = null;
        private static Cursor panNW = null;
        private static Cursor panSE = null;
        private static Cursor panSouth = null;
        private static Cursor panSW = null;
        private static Cursor panWest = null;
        private static Cursor hand = null;

        public static Cursor Default
        {
            get
            {
                if (defaultCursor == null)
                    defaultCursor = new Cursor(Images.Default);
                return defaultCursor;
            }
        }
        public static Cursor Hand
        {
            get
            {
                if (hand == null)
                {
                    hand = new Cursor(Images.Hand);
                    hand.SetHotspot(Cursor.PointPositionTypes.Middle, Cursor.PointPositionTypes.Middle);
                }
                return hand;
            }
        }
        public static Cursor Help
        {
            get
            {
                if (help == null)
                    help = new Cursor(Images.Help);
                return help;
            }
        }
        public static Cursor HSplit
        {
            get
            {
                if (hSplit == null)
                {
                    hSplit = new Cursor(Images.HSplit);
                    HSplit.SetHotspot(Cursor.PointPositionTypes.Middle, Cursor.PointPositionTypes.Middle);
                }
                return hSplit;
            }
        }
        public static Cursor SizeAll
        {
            get
            {
                if (sizeAll == null)
                {
                    sizeAll = new Cursor(Images.SizeAll);
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
                    sizeNESW = new Cursor(Images.SizeNESW);
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
                    sizeNS = new Cursor(Images.SizeNS);
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
                    sizeNWSE = new Cursor(Images.SizeNWSE);
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
                    sizeWE = new Cursor(Images.SizeWE);
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
                    vSplit = new Cursor(Images.VSplit);
                    vSplit.SetHotspot(Cursor.PointPositionTypes.Middle, Cursor.PointPositionTypes.Middle);
                }
                return vSplit;
            }
        }
    }
}
