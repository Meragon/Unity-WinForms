using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Drawing
{
    public static class DrawingHelper
    {
        public static ContentAlignment ToContentAlignment(this StringFormat format)
        {
            ContentAlignment alignment = ContentAlignment.TopLeft;
            switch (format.Alignment)
            {
                case StringAlignment.Near:
                    if (format.LineAlignment == StringAlignment.Near)
                        alignment = ContentAlignment.TopLeft;
                    else if (format.LineAlignment == StringAlignment.Center)
                        alignment = ContentAlignment.MiddleLeft;
                    else alignment = ContentAlignment.BottomLeft;
                    break;
                case StringAlignment.Center:
                    if (format.LineAlignment == StringAlignment.Near)
                        alignment = ContentAlignment.TopCenter;
                    else if (format.LineAlignment == StringAlignment.Center)
                        alignment = ContentAlignment.MiddleCenter;
                    else alignment = ContentAlignment.BottomCenter;
                    break;
                case StringAlignment.Far:
                    if (format.LineAlignment == StringAlignment.Near)
                        alignment = ContentAlignment.TopRight;
                    else if (format.LineAlignment == StringAlignment.Center)
                        alignment = ContentAlignment.MiddleRight;
                    else alignment = ContentAlignment.BottomRight;
                    break;
            }

            return alignment;
        }
    }
}
