namespace Highcharts
{
    using System.Drawing;
    
    public class SeriesLineSolid : Series
    {
        private readonly Pen iconPen = new Pen(Color.Black, 2);
        
        public SeriesLineSolid() : this("") 
        { }

        public SeriesLineSolid(string name) : base(name)
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
            
            var prevValue = data[0];
            var prevValueX = GetXFromIndex(0, xStep, clipRectangle);
            var prevValueY = GetYFromValue(prevValue, valueRange, clipRectangle);
            
            for (float dataIterator = pointInterval; dataIterator < data.Count; dataIterator += pointInterval)
            {
                int dataIndex = (int) dataIterator;
                 
                double currentValue = data[dataIndex];

                float currentValueX = GetXFromIndex(dataIndex, xStep, clipRectangle);
                float currentValueY = GetYFromValue(currentValue, valueRange, clipRectangle);

                g.DrawLine(pen, prevValueX, prevValueY, currentValueX + 1, prevValueY);
                g.DrawLine(pen, currentValueX, prevValueY, currentValueX, currentValueY);

                prevValueX = currentValueX;
                prevValueY = currentValueY;
                
                SeriesPaint.PaintHoveredValueRect(this, g, currentValueX, currentValueY, xStep);
            }
        }
        
        protected override void OnPaintIcon(Graphics g, Rectangle clipRectangle)
        {
            iconPen.Color = color;
            
            g.DrawLine(iconPen, clipRectangle.Left, clipRectangle.Top + 11, clipRectangle.Left + 16,  clipRectangle.Top + 11);
        }
    }
}