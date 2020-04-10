namespace Highcharts
{
    using System.Drawing;
    
    public static class SeriesPaint
    {
        private static readonly SolidBrush pointBrush = new SolidBrush(Color.Black);
        
        public static void PaintPoint(Series s, Graphics g, Rectangle clipRectangle, int dataIndex)
        {
            pointBrush.Color = s.color;

            var valueRange = s.GetValueRange();
            var xStep = s.GetXStep();
            
            var valueX = s.GetXFromIndex(dataIndex, xStep, clipRectangle);
            var valueY = s.GetYFromValue(s.data[dataIndex], valueRange, clipRectangle);
            var valueDiameter = s.collection.chart.circleRadius * 2;

            g.FillEllipse(pointBrush, valueX, valueY, valueDiameter, valueDiameter);
        }

        public static void PaintHoveredValueRect(Series s, Graphics g, float currentValueX, float currentValueY, float xStep)
        {
            if (s.collection.chart.hovered && 
                s.collection.chart.mouseX - s.collection.chart.cachedPlotLeft > currentValueX &&
                s.collection.chart.mouseX - s.collection.chart.cachedPlotLeft < currentValueX + xStep)
                g.uwfFillRectangle(s.color, currentValueX - 2, currentValueY - 2, 4, 4);
        }
    }
}