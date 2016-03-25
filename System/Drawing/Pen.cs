using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;

namespace System.Drawing
{
    public sealed class Pen : IDisposable, ICloneable
    {
        public Color Color { get; set; }
        public DashStyle DashStyle { get; set; }
        public float Width { get; set; }

        public Pen(Color color)
        {
            Color = color;
            Width = 1;
        }
        public Pen(Color color, float width)
        {
            Color = color;
            Width = width;
        }

        public object Clone()
        {
            Pen pen = new Pen(this.Color);
            pen.DashStyle = this.DashStyle;
            pen.Width = this.Width;
            return pen;
        }
        public void Dispose()
        {
            
        }
    }
}
