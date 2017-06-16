using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Drawing
{
    public sealed class SolidBrush : Brush
    {
        public Color Color { get; set; }
        
        public SolidBrush(Color color)
        {
            Color = color;
        }

        public override object Clone()
        {
            var brush = new SolidBrush(this.Color);
            return brush;
        }
    }
}
