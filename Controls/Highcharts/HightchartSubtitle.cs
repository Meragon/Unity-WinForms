namespace Highcharts
{
    using System.Drawing;
    using System.Windows.Forms;

    public class HightchartSubtitle
    {
        public HightchartSubtitle()
        {
            align = HorizontalAlignment.Center;
            color = Color.FromArgb(0x33, 0x33, 0x33);
            floating = false;
            font = Highchart.DefaultFont12;
            text = null;
            x = 0;
            y = 0;
        }

        /// <summary>
        /// The horizontal alignment of the title. Can be one of "left", "center" and "right". Defaults to center.
        /// </summary>
        public HorizontalAlignment align { get; set; }
        public Color color { get; set; }
        /// <summary>
        /// When the title is floating, the plot area will not move to make space for it. Defaults to false.
        /// </summary>
        public bool floating { get; set; }
        public Font font { get; set; }
        /// <summary>
        /// The title of the chart. To disable the title, set the text to null. Defaults to Chart title.
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// The x position of the title relative to the alignment within chart.spacingLeft and chart.spacingRight. Defaults to 0.
        /// </summary>
        public int x { get; set; }
        /// <summary>
        /// The y position of the title relative to the alignment within chart.spacingTop and chart.spacingBottom. 
        /// By default it depends on the font size.
        /// </summary>
        public int y { get; set; }
    }
}
