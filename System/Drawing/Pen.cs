using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;

namespace System.Drawing
{
    public sealed class Pen
    {
        public Color Color { get; set; }
        public DashStyle DashStyle { get; set; }
        public float Width { get; set; }

        public Pen(Color color)
        {
            Color = color;
            Width = 1;
        }
    }
}
