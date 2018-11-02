namespace Highcharts
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    public class Axis : IDisposable
    {
        internal Pen  gridPen = new Pen(Color.Transparent);
        internal bool horizontal;
        internal Pen  linePen = new Pen(Color.Transparent);
        internal Pen  tickPen = new Pen(Color.Transparent);

        private int _offset;

        internal Axis(bool type)
        {
            horizontal = type;

            categories = new List<string>();
            gridFont = Highchart.DefaultFont10;
            max = null;
            min = null;
            tickLength = 10;
            title = new AxisTitle(this);
            visible = true;
        }

        public event EventHandler OffsetChanged;

        public List<string> categories { get; private set; }

        /// <summary>
        /// Color of the grid lines extending the ticks across the plot area.
        /// </summary>
        public Color gridLineColor
        {
            get { return gridPen.Color; }
            set { gridPen.Color = value; }
        }
        [Obsolete("dunno")]
        public Font gridFont { get; set; }

        /// <summary>
        /// The width of the grid lines extending the ticks across the plot area.
        /// </summary>
        public int gridLineWidth
        {
            get { return (int) gridPen.Width; }
            set { gridPen.Width = value; }
        }

        /// <summary>
        /// The color of the line marking the axis itself.
        /// </summary>
        public Color lineColor
        {
            get { return linePen.Color; }
            set { linePen.Color = value; }
        }

        /// <summary>
        /// The width of the line marking the axis itself.
        /// </summary>
        public int lineWidth
        {
            get { return (int) linePen.Width; }
            set { linePen.Width = value; }
        }

        public double? max { get; set; }
        public double? min { get; set; }

        /// <summary>
        /// The distance in pixels from the plot area to the axis line. A positive offset moves the axis
        /// with it's line, labels and ticks away from the plot area. This is typically used when two or
        /// more axes are displayed on the same side of the plot. With multiple axes the offset is dynamically
        /// adjusted to avoid collision, this can be overridden by setting offset explicitly.
        ///
        /// Defaults to 0.
        /// </summary>
        public int offset
        {
            get { return _offset; }
            set
            {
                if (_offset == value)
                    return;
                
                _offset = value;
                OnOffsetChanged();
            }
        }

        /// <summary>
        /// Color for the main tick marks.
        /// </summary>
        public Color tickColor
        {
            get { return tickPen.Color; }
            set { tickPen.Color = value; }
        }

        /// <summary>
        /// The pixel length of the main tick marks.
        /// </summary>
        public int tickLength { get; set; }

        /// <summary>
        /// The pixel width of the major tick marks.
        /// </summary>
        public int tickWidth { get; set; }

        /// <summary>
        /// The axis title, showing next to the axis line.
        /// </summary>
        public AxisTitle title { get; set; }

        /// <summary>
        /// Whether axis, including axis title, line, ticks and labels, should be visible.
        ///
        /// Defaults to true.
        /// </summary>
        public bool visible { get; set; }

        public void Dispose()
        {
            if (gridPen != null) gridPen.Dispose();
            if (tickPen != null) tickPen.Dispose();
        }

        protected virtual void OnOffsetChanged()
        {
            var handler = OffsetChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}