namespace Highcharts
{
    using System.Drawing;
    
    public class SeriesPoint : Series
    {
        private readonly SolidBrush pointBrush = new SolidBrush(Color.Black);
        
        public SeriesPoint() : this("")
        { }

        public SeriesPoint(string name) : base(name)
        { }

        protected override void OnPaint(Graphics g, Rectangle clipRectangle)
        {
            pointBrush.Color = color;

            var pointRadius = collection.chart.circleRadius;
            var valueRange = GetValueRange();
            var xStep = GetXStep();
            
            for (float dataIterator = 0; dataIterator < data.Count; dataIterator += pointInterval)
            {
                int dataIndex = (int) dataIterator;
                 
                double currentValue = data[dataIndex];

                float currentValueX = GetXFromIndex(dataIndex, xStep, clipRectangle) - pointRadius;
                float currentValueY = GetYFromValue(currentValue, valueRange, clipRectangle) - pointRadius;
                
                g.FillEllipse(pointBrush, currentValueX, currentValueY, pointRadius * 2, pointRadius * 2);
                
                SeriesPaint.PaintHoveredValueRect(this, g, currentValueX, currentValueY, xStep);
            }
        }
        
        protected override void OnPaintIcon(Graphics g, Rectangle clipRectangle)
        {
            pointBrush.Color = color;
            
            g.FillEllipse(pointBrush,  clipRectangle.Left + 4, clipRectangle.Top + 8, 8, 8);
        }
    }
}