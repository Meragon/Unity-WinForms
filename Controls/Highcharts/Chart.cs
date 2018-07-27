namespace Highcharts
{
    using System;
    using System.Drawing;

    public class Chart : IDisposable
    {
        internal Pen borderPen     = new Pen(Color.White);
        internal Pen plotBorderPen = new Pen(Color.Transparent);

        public Chart()
        {
            backgroundColor = Color.FromArgb(0xFF, 0xFF, 0xFF);
            borderColor = Color.FromArgb(0x33, 0x5C, 0xAD);
            borderWidth = 0;
            plotBackgroundColor = Color.Transparent;
            plotBorderColor = Color.FromArgb(0xCC, 0xCC, 0xCC);
            plotBorderWidth = 0;
        }

        public Color backgroundColor { get; set; }
        public Color borderColor { get; set; }
        public int borderWidth { get; set; }
        public Color plotBackgroundColor { get; set; }
        public Color plotBorderColor
        {
            get { return plotBorderPen.Color; }
            set { plotBorderPen.Color = value; }
        }
        public int plotBorderWidth
        {
            get { return (int)plotBorderPen.Width; }
            set { plotBorderPen.Width = value; }
        }

        public void Dispose()
        {
            if (borderPen != null) borderPen.Dispose();
            if (plotBorderPen != null) plotBorderPen.Dispose();
        }
    }
}
