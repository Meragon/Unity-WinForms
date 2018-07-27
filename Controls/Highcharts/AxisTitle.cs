namespace Highcharts
{
    using System;
    using System.Windows.Forms;

    public class AxisTitle : IDisposable
    {
        private readonly Axis owner;

        public AxisTitle(Axis axis)
        {
            owner = axis;

            align = HorizontalAlignment.Center;
            offset = 0;
            margin = 0;
            text = null;
            x = 0;
            y = 0;

            if (axis.horizontal == false) margin = 10;
        }

        /// <summary>
        /// Alignment of the title relative to the axis values. Possible values are "low", "middle" or "high". Defaults to middle.
        /// </summary>
        public HorizontalAlignment align { get; set; }
        /// <summary>
        /// The pixel distance between the axis labels or line and the title. Defaults to 0 for horizontal axes, 10 for vertical
        /// </summary>
        public int margin { get; set; }
        /// <summary>
        /// The distance of the axis title from the axis line. By default, this distance is computed from the offset width of the labels, 
        /// the labels' distance from the axis and the title's margin. However when the offset option is set, it overrides all this.
        /// </summary>
        public int offset { get; set; }
        /// <summary>
        /// The actual text of the axis title. It can contain basic HTML text markup like <b/>, <i/> and spans with style.
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// Horizontal pixel offset of the title position. Defaults to 0.
        /// </summary>
        public int x { get; set; }
        /// <summary>
        /// Vertical pixel offset of the title position.
        /// </summary>
        public int y { get; set; }

        public void Dispose()
        {
            if (owner != null) owner.Dispose();
        }
    }
}
