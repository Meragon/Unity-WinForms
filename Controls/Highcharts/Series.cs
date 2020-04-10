namespace Highcharts
{
    using System;
    using System.Drawing;

    public abstract class Series : IDisposable
    {
        internal SeriesCollection collection;
        internal readonly Pen pen = new Pen(Color.Transparent);

        private string _name;

        protected Series() : this("")
        { }
        
        protected Series(string name)
        {
            color = Color.Gray;
            data = new DataCollection(this);
            pointInterval = 1;
            visible = true;
            
            this.name = name;
        }

        public event EventHandler NameChanged;

        public Color color
        {
            get { return pen.Color; }
            set { pen.Color = value; }
        }
        public DataCollection data { get; private set; }
        public int index { get; internal set; }
        public string name
        {
            get { return _name; }
            set
            {
                if (_name == value)
                    return;

                _name = value;
                OnNameChanged(EventArgs.Empty);
            }
        }
        /// <summary>
        /// If no x values are given for the points in a series, pointInterval defines the interval of the x values. 
        /// For example, if a series contains one value every decade starting from year 0, set pointInterval to 10.
        /// Defaults to 1.
        /// </summary>
        public float pointInterval { get; set; }
        public object tag { get; set; }
        public bool visible { get; set; }
        public int yAxis { get; set; }

        public void Dispose()
        {
            if (pen != null) pen.Dispose();
        }

        public override string ToString()
        {
            return "{name=" + name + "}";
        }
        
        internal void Paint(Graphics g, Rectangle clipRectangle)
        {
            if (!visible || data.Count == 0 || pointInterval <= 0) return;
            
            OnPaint(g, clipRectangle);
        }

        internal void PaintIcon(Graphics g, Rectangle clipRectangle)
        {
            OnPaintIcon(g, clipRectangle);
        }

        public double GetValueRange()
        {
            return collection.chart.cachedPlotMax - collection.chart.cachedPlotMin;
        }

        public float GetXFromIndex(float dataIndex, float xStep, Rectangle clipRectangle)
        {
            return (float) Math.Ceiling(clipRectangle.Left + xStep * dataIndex);
        }
        
        public float GetXStep()
        {
            var chartDataAmount = collection.chart.GetSeriesMaximumDataAmount();
            if (chartDataAmount < 1)
                return collection.chart.cachedPlotWidth / 2f; // Middle point of the plot.
            
            return (float) collection.chart.cachedPlotWidth / (chartDataAmount - 1);
        }

        public float GetYFromValue(double value, double range, Rectangle clipRectangle)
        {
            return (float) Math.Ceiling(clipRectangle.Bottom - clipRectangle.Height * GetYModFromValue(value, range));
        }

        public float GetYModFromValue(double value, double range)
        {
            return (float) ((value - collection.chart.cachedPlotMin) / range);
        }
        
        protected virtual void OnNameChanged(EventArgs e)
        {
            var handler = NameChanged;
            if (handler != null)
                handler(this, e);
        }
        protected abstract void OnPaint(Graphics g, Rectangle clipRectangle);
        protected abstract void OnPaintIcon(Graphics g, Rectangle clipRectangle);
    }
}