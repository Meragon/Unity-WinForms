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
            spacingBottom = 15;
            spacingLeft = 10;
            spacingRight = 10;
            spacingTop = 10;
        }

        public Color backgroundColor { get; set; }
        public Color borderColor { get; set; }
        public int borderWidth { get; set; }
        public int?[] margin
        {
            get { return new[] {marginTop, marginRight, marginBottom, marginLeft}; }
            set
            {
                if (value == null)
                {
                    marginBottom = null;
                    marginLeft = null;
                    marginRight = null;
                    marginTop = null;
                }
                else if (value.Length != 4)
                {
                    throw new NotSupportedException("margin");
                }
                else
                {
                    marginBottom = value[2];
                    marginLeft = value[3];
                    marginRight = value[1];
                    marginTop = value[0];
                }
            }
        }
        public int? marginBottom { get; set; }
        public int? marginLeft { get; set; }
        public int? marginRight { get; set; }
        public int? marginTop { get; set; }
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
        public int spacingBottom { get; set; }
        public int spacingLeft { get; set; }
        public int spacingRight { get; set; }
        public int spacingTop { get; set; }
        //public ZoomTypes zoomType { get; set; }

        public void Dispose()
        {
            if (borderPen != null) borderPen.Dispose();
            if (plotBorderPen != null) plotBorderPen.Dispose();
        }
    }
}
