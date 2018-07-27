namespace Highcharts
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    public class Axis : IDisposable
    {
        internal Pen  gridPen = new Pen(Color.Transparent);
        internal bool horizontal;
        internal Pen  tickPen = new Pen(Color.Transparent);

        internal Axis(bool type)
        {
            horizontal = type;

            categories = new List<string>();
            gridFont = Highchart.DefaultFont10;
            max = null;
            min = null;
            tickLength = 10;
            title = new AxisTitle(this);
        }

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
            get { return (int)gridPen.Width; }
            set { gridPen.Width = value; }
        }
        public double? max { get; set; }
        public double? min { get; set; }
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
        public AxisTitle title { get; set; }

        public void Dispose()
        {
            if (gridPen != null) gridPen.Dispose();
            if (tickPen != null) tickPen.Dispose();
        }
    }
}
