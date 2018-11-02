namespace Highcharts
{
    using System.Drawing;

    public class YAxis : Axis
    {
        public YAxis() : base(false)
        {
            gridLineColor = Color.FromArgb(0xE6, 0xE6, 0xE6);
            gridLineWidth = 1;
            lineColor = Color.FromArgb(0xCC, 0xD6, 0xEB);
            lineWidth = 0;
            tickColor = Color.FromArgb(0xCC, 0xD6, 0xEB);
            tickWidth = 0;
        }
    }
}
