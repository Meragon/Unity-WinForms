using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Drawing
{
    public sealed class SolidBrush : Brush
    {
        public Color Color { get; set; }

        public static bool operator!=(SolidBrush left, SolidBrush right)
        {
            return !(left == right);
        }
        public static bool operator==(SolidBrush left, SolidBrush right)
        {
            if (object.ReferenceEquals(left, null) || object.ReferenceEquals(right, null)) return false;

            return left.Color == right.Color;
        }

        public SolidBrush(Color color)
        {
            Color = color;
        }

        public override object Clone()
        {
            SolidBrush brush = new SolidBrush(this.Color);
            return brush;
        }
    }
}
