namespace Highcharts
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    internal sealed class LegendButton : Button
    {
        private const int DefaultTextMarginLeft = 16;
        
        public LegendButton(Legend l, Series s)
        {
            Legend = l;
            Series = s;
            TextLeftMargin = DefaultTextMarginLeft;
            
            TextFromSeries();
        }

        public Legend Legend { get; private set; }
        public Series Series { get; private set; }
        public int TextLeftMargin { get; set; }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            switch (e.Button)
            {
                case MouseButtons.Left:
                    Series.visible = !Series.visible;

                    var highcharts = Parent as Highchart;
                    if (highcharts != null) 
                        highcharts.UpdatePlot(); // Update plot min and max values range.
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

                    var context = new ContextMenuStrip();
                    context.Items.Add(itemColor);
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
            
            if (!Series.visible)
            {
                seriesColor = Legend.itemHiddenStyle.color;
                textColor = seriesColor;
            }

            // Text also supports hover style.
            if (hovered)
                textColor = Legend.itemHoverStyle.color;
            
            Series.PaintIcon(graphics, new Rectangle(0, 0, TextLeftMargin, 16));

            graphics.uwfDrawString(Text, Font, textColor, TextLeftMargin, 0, Width - TextLeftMargin, Height, ContentAlignment.MiddleCenter);
        }
        
        protected override void OnPaintBackground(PaintEventArgs e)
        { }
        
        private void TextFromSeries()
        {
            Text = Series.name != null 
                ? Series.name 
                : "Series " + (Series.index + 1);
        }
    }
}
