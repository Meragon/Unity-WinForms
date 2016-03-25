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
            return left.Color != right.Color;
        }
        public static bool operator==(SolidBrush left, SolidBrush right)
        {
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
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
