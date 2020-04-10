namespace Highcharts
{
    using System.Drawing;
    
    public class SeriesAreaSolidOutline : Series
    {
        public SeriesAreaSolidOutline() : this("")
        { }
        
        public SeriesAreaSolidOutline(string name) : base(name)
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
                bool last = dataIterator + pointInterval >= data.Count;

                if (!collection.chart.plotOptions.linearGradient)
                {
                    g.uwfFillRectangle(areaColor, prevValueX, prevValueY, currentValueX - prevValueX, clipRectangle.Bottom - prevValueY);

                    if (last)
                        g.uwfFillRectangle(areaColor, currentValueX, currentValueY, xStep, clipRectangle.Bottom - currentValueY);
                }
                else if (collection.chart.plotOptions.linearGradientMaterial != null)
                {
                    float clipH = clipRectangle.Height;
                    
                    int parentHeight = 0;
                    int parentWidth = 0;
            
                    if (collection.chart.Parent != null)
                    {
                        parentHeight = collection.chart.Parent.Height;
                        parentWidth = collection.chart.Parent.Width;
                    }
                    
                    // Horizontal clipping (not really working).
                    if (parentWidth != 0 && parentHeight != 0)
                    {
                        if (collection.chart.Location.X + currentValueX < -collection.chart.uwfOffset.X) // Left side.
                            continue;
                        if (collection.chart.Location.X + currentValueX + collection.chart.uwfOffset.X > parentWidth) // Right side.
                            continue;
                    }

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_WEBGL

                    float clipY = prevValueY;
                    
                    // Vertical clipping.
                    if (collection.chart.Location.Y + currentValueY < -collection.chart.uwfOffset.Y)
                        clipY = -collection.chart.uwfOffset.Y - collection.chart.Location.Y;
                    if (collection.chart.Location.Y + clipRectangle.Top + clipH + collection.chart.uwfOffset.Y > parentHeight)
                        clipH = parentHeight - collection.chart.uwfOffset.Y - collection.chart.Location.Y - clipRectangle.Top;

                    var unityMaterial = collection.chart.plotOptions.linearGradientMaterial as UnityEngine.Material;
                    if (unityMaterial != null)
                        unityMaterial.SetFloat("_Y", 1 - (clipY - clipRectangle.Top) / clipH);

#endif
                    g.uwfFillRectangle(areaColor, prevValueX, clipRectangle.Top, width, clipH,
                        collection.chart.plotOptions.linearGradientMaterial);
                    
                    if (last)
                        g.uwfFillRectangle(areaColor, currentValueX, clipRectangle.Top, xStep, clipH,
                            collection.chart.plotOptions.linearGradientMaterial);
                }

                g.DrawLine(pen, prevValueX, prevValueY, currentValueX + 1, prevValueY);
                g.DrawLine(pen, currentValueX, prevValueY, currentValueX, currentValueY);

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