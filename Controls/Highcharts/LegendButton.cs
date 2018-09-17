namespace Highcharts
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    internal sealed class LegendButton : Button
    {
        internal bool seriesEnabled;

        private readonly Pen seriesPen = new Pen(Color.Transparent, 2);
        
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

                    Series.visible = seriesEnabled;
                    if (Parent != null) ((Highchart)Parent).RecalcCategories();
                    break;
                case MouseButtons.Right:
                    
                    var itemColor = new ToolStripMenuItem(Highchart.textLegendMenu_Color);
                    itemColor.Click += (sender, args) =>
                    {
                        var form = new ColorPickerForm();
                        form.Color = Series.color;
                        form.ColorChanged += (sender2, args2) => Series.color = form.Color; 
                        form.ShowDialog();
                    };
                    
                    var itemType = new ToolStripMenuItem(Highchart.textLegendMenu_Type);

                    var seriesTypes = Highchart.textLegendMenu_TypeNames;
                    for (int i = 0; i < seriesTypes.Length; i++)
                    {
                        int index = i;
                        var item = new ToolStripMenuItem(seriesTypes[i]);
                        item.Click += (s, a) => { Series.type = (SeriesTypes)index; };
                        itemType.DropDownItems.Add(item);
                    }

                    var context = new ContextMenuStrip();
                    context.Items.Add(itemColor);
                    context.Items.Add(itemType);
                    context.Show(null, MousePosition);
                    break;
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            var graphics = e.Graphics;

            // User original series color or hidden color for icon and use styles for text.
            var seriesColor = Series.color;
            var textColor = Legend.itemStyle.color;
            if (seriesEnabled == false)
            {
                seriesColor = Legend.itemHiddenStyle.color;
                textColor = seriesColor;
            }

            // Text also supports hover style.
            if (hovered)
                textColor = Legend.itemHoverStyle.color;
            
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

            graphics.uwfDrawString(Text, Font, textColor, 16, 0, Width - 16, Height, ContentAlignment.MiddleCenter);
        }
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }
    }
}
