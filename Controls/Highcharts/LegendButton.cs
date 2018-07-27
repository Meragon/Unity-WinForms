namespace Highcharts
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    internal sealed class LegendButton : Button
    {
        internal bool seriesEnabled;

        private readonly Pen seriesPen = new Pen(Color.Transparent, 2);
        private Color seriesColor;

        public LegendButton(Legend l, Series s)
        {
            Legend = l;
            Series = s;
            Text = s.name;

            OnMouseClick(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        }

        public Legend Legend { get; private set; }
        public Series Series { get; private set; }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            switch (e.Button)
            {
                case MouseButtons.Left:
                    seriesEnabled = !seriesEnabled;

                    if (seriesEnabled)
                    {
                        ForeColor = Legend.itemStyle.color;
                        seriesColor = Series.color;
                    }
                    else
                    {
                        ForeColor = Legend.itemHiddenStyle.color;
                        seriesColor = ForeColor;
                    }

                    Series.visible = seriesEnabled;
                    if (Parent != null) ((Highchart)Parent).RecalcCategoriesAndData();
                    break;
                case MouseButtons.Right:
                    var context = new ContextMenuStrip();

                    var seriesTypes = Enum.GetNames(typeof(SeriesTypes));
                    for (int i = 0; i < seriesTypes.Length; i++)
                    {
                        int index = i;
                        var item = new ToolStripMenuItem(seriesTypes[i]);
                        item.Click += (s, a) => { Series.type = (SeriesTypes)index; };
                        context.Items.Add(item);
                    }

                    context.Show(null, MousePosition);
                    break;
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            var graphics = e.Graphics;

            switch (Series.type)
            {
                case SeriesTypes.areaSolid:
                case SeriesTypes.areaSolidOutline:
                    graphics.uwfFillRectangle(seriesColor, 4, 8, 8, 8);
                    break;
                case SeriesTypes.line:
                    seriesPen.Color = seriesColor;
                    graphics.DrawLine(seriesPen, 0, 11, 16, 11);
                    break;
                case SeriesTypes.lineSolid:
                    seriesPen.Color = seriesColor;
                    graphics.DrawLine(seriesPen, 0, 11, 16, 11);
                    graphics.uwfDrawImage(ApplicationResources.Images.Circle, seriesColor, 4, 8, 8, 8);
                    break;
                case SeriesTypes.point:
                    graphics.uwfDrawImage(ApplicationResources.Images.Circle, seriesColor, 4, 8, 8, 8);
                    break;
            }

            graphics.uwfDrawString(Text, Font, ForeColor, 16, 0, Width - 16, Height, ContentAlignment.MiddleCenter);
        }
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }
    }
}
