namespace Highcharts
{
    using System.Drawing;
    
    public class SeriesAreaSolid : Series
    {
        public SeriesAreaSolid() : this("")
        { }

        public SeriesAreaSolid(string name) : base(name)
        { }

        protected override void OnPaint(Graphics g, Rectangle clipRectangle)
        {
            if (data.Count == 1)
            {
                SeriesPaint.PaintPoint(this, g, clipRectangle, 0);
                return;
            }
            
            var valueRange = GetValueRange();
            var xStep = GetXStep();
            var areaColorAlpha = collection.GetColorAlpha();
            var areaColor = Color.FromArgb(areaColorAlpha, color);
            
            var prevValue = data[0];
            var prevValueX = GetXFromIndex(0, xStep, clipRectangle);
            var prevValueY = GetYFromValue(prevValue, valueRange, clipRectangle);
           
            for (float dataIterator = pointInterval; dataIterator < data.Count; dataIterator += pointInterval)
            {
                double currentValue = data[(int) dataIterator];

                float currentValueX = GetXFromIndex(dataIterator, xStep, clipRectangle);
                float currentValueY = GetYFromValue(currentValue, valueRange, clipRectangle);

                float width = currentValueX - prevValueX;

                g.uwfFillRectangle(areaColor, prevValueX, prevValueY, width, clipRectangle.Bottom - prevValueY);
                
                if (dataIterator + pointInterval >= data.Count)
                    g.uwfFillRectangle(areaColor, currentValueX, currentValueY, xStep, clipRectangle.Bottom - currentValueY);

                prevValueX = currentValueX;
                prevValueY = currentValueY;
                
                SeriesPaint.PaintHoveredValueRect(this, g, currentValueX, currentValueY, xStep);
            }
        }

        protected override void OnPaintIcon(Graphics g, Rectangle clipRectangle)
        {
            g.uwfFillRectangle(color, clipRectangle.Left + 4, clipRectangle.Top + 8, 8, 8);
        }
    }
}