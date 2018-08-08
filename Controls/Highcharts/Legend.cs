namespace Highcharts
{
    using System.Drawing;

    public enum LegendLayouts
    {
        horizontal,
        vertical
    }

    public class Legend
    {
        public Legend()
        {
            enabled = true;
            floating = false;
            itemDistance = 20;
            itemHiddenStyle = new LegendItemStyle(Color.FromArgb(0xCC, 0xCC, 0xCC));
            itemHoverStyle = new LegendItemStyle(Color.FromArgb(0x00, 0x00, 0x00));
            itemStyle = new LegendItemStyle(Color.FromArgb(0x33, 0x33, 0x33));
            layout = LegendLayouts.horizontal;
            margin = 12;
        }

        /// <summary>
        /// Enable or disable the legend.
        /// </summary>
        public bool enabled { get; set; }
        /// <summary>
        /// When the legend is floating, the plot area ignores it and is allowed to be placed below it. Defaults to false.
        /// </summary>
        public bool floating { get; set; }
        /// <summary>
        /// In a legend with horizontal layout, the itemDistance defines the pixel distance between each item. Defaults to 20.
        /// </summary>
        public int itemDistance { get; set; }
        public LegendItemStyle itemHiddenStyle { get; set; }
        public LegendItemStyle itemHoverStyle { get; set; }
        public LegendItemStyle itemStyle { get; set; }
        public LegendLayouts layout { get; set; }
        /// <summary>
        /// If the plot area sized is calculated automatically and the legend is not floating, the legend margin 
        /// is the space between the legend and the axis labels or plot area. Defaults to 12.
        /// </summary>
        public int margin { get; set; }
    }
}
