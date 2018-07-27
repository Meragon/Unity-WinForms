namespace Highcharts
{
    using System.Drawing;

    public class XAxis : Axis
    {
        public XAxis() : base(true)
        {
            gridLineColor = Color.FromArgb(0xE6, 0xE6, 0xE6);
            gridLineWidth = 0;
            tickColor = Color.FromArgb(0xCC, 0xD6, 0xEB);
            tickWidth = 1;
        }
    }
}
