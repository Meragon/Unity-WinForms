namespace Highcharts
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    // Based on https://www.highcharts.com/
    public class Highchart : Control
    {
        public static readonly string DefaultFontName = "Arial";
        public static readonly Font   DefaultFont10   = new Font(DefaultFontName, 10);
        public static readonly Font   DefaultFont12   = new Font(DefaultFontName, 12);
        public static readonly Font   DefaultFont12B  = new Font(DefaultFontName, 12, FontStyle.Bold);
        public static readonly Font   DefaultFont18   = new Font(DefaultFontName, 18);
        
        // Localization text.
        public static string textLegendMenu_Color = "Color...";

        public int     categoriesMinStep = 20;
        public double? fixedMax;
        public double? fixedMin;

        internal readonly Color categoriesColor  = Color.Gray;
        internal readonly Font  categoriesFont   = DefaultFont10;
        internal readonly Color gridTextColor    = Color.Gray;
        internal readonly Pen   mousePositionPen = new Pen(Color.Gray);

        internal float circleRadius    = 2;
        internal int   gridTextYOffet  = -10;
        internal int   gridTextXOffset = -2;
        internal int   mouseX;
        internal int   mouseY;
        internal int   nextColorIndex;

        internal string[]     cachedCategories;
        internal float        cachedCategoriesStep;
        internal List<string> cachedCategoriesFiltered;
        internal int          cachedGridLines = 6;
        internal double       cachedGridLinesStep;
        internal double       cachedGridMaxValue;
        internal double       cachedGridMinValue;
        internal string[]     cachedGridValues;
        internal int          cachedPlotTop;
        internal int          cachedPlotBottom;
        internal int          cachedPlotLeft;
        internal int          cachedPlotRight;
        internal int          cachedPlotHeight;
        internal int          cachedPlotWidth;
        internal double       cachedPlotMin;
        internal double       cachedPlotMax;
        internal int          cachedSubtitleY;

        private Control plot;
        private LegendButton[] legendButtons;
        private bool updateLegendButtonsFlag;

        public Highchart()
        {
            chart = new Chart();
            colors = new[]
            {
                Color.FromArgb(0x7c, 0xb5, 0xec), Color.FromArgb(0x43, 0x43, 0x48),
                Color.FromArgb(0x90, 0xed, 0x7d), Color.FromArgb(0xf7, 0xa3, 0x5c),
                Color.FromArgb(0x80, 0x85, 0xe9), Color.FromArgb(0xf1, 0x5c, 0x80),
                Color.FromArgb(0xe4, 0xd3, 0x54), Color.FromArgb(0x2b, 0x90, 0x8f),
                Color.FromArgb(0xf4, 0x5b, 0x5b), Color.FromArgb(0x91, 0xe8, 0xe1),
            };
            legend = new Legend();
            plotOptions = new PlotOptions();
            series = new SeriesCollection(this);
            subtitle = new HightchartSubtitle();
            title = new HighchartTitle();
            xAxis = new XAxis();
            yAxis = new AxisCollection(this);
            yAxis.Add(new YAxis());
            
            plot = new Control();
            plot.Padding = new Padding(1);
            plot.Visible = false;
            Controls.Add(plot);
            
            cachedPlotLeft = chart.spacingLeft + 38;
            
            Recalc();
        }

        public Chart chart { get; private set; }
        public Color[] colors { get; private set; }
        public Legend legend { get; private set; }
        public PlotOptions plotOptions { get; private set; }
        public SeriesCollection series { get; private set; }
        public HightchartSubtitle subtitle { get; private set; }
        public HighchartTitle title { get; private set; }
        public Axis xAxis { get; private set; }
        public AxisCollection yAxis { get; private set; }
        
        internal string[] Categories
        {
            get { return cachedCategories; }
        }

        public void RecalcCategories()
        {
            if (cachedPlotWidth > 0)
            {
                int maxDataAmount = GetSeriesMaximumDataAmount();
                if (maxDataAmount == -1)
                    return;

                if (maxDataAmount == 1)
                    cachedCategoriesStep = cachedPlotWidth;
                else
                    cachedCategoriesStep = (float)cachedPlotWidth / (maxDataAmount - 1);
                if (cachedCategoriesStep < categoriesMinStep)
                    cachedCategoriesStep = categoriesMinStep;

                float nextCategoriesAmount = cachedPlotWidth / cachedCategoriesStep;
                float skipFactor = maxDataAmount / nextCategoriesAmount;

                if (xAxis.categories.Count > 0)
                    cachedCategories = xAxis.categories.ToArray();
                else
                {
                    if (maxDataAmount != -1 && (cachedCategories == null || cachedCategories.Length != maxDataAmount))
                    {
                        cachedCategories = new string[maxDataAmount];
                        for (int i = 0; i < cachedCategories.Length; i++)
                            cachedCategories[i] = (i + 1).ToString();
                    }
                }

                if (cachedCategories != null && cachedCategories.Length > 0)
                {
                    if (cachedCategories.Length == 1)
                        cachedCategoriesStep = cachedPlotWidth;
                    else
                        cachedCategoriesStep = (float)cachedPlotWidth / (cachedCategories.Length - 1);

                    if (cachedCategoriesStep < categoriesMinStep)
                    {
                        cachedCategoriesStep = categoriesMinStep;

                        // Filter categories.
                        cachedCategoriesFiltered = new List<string>();
                        for (float i = 0; i < cachedCategories.Length; i += skipFactor)
                            cachedCategoriesFiltered.Add(cachedCategories[(int) i]);
                    }
                    else
                        cachedCategoriesFiltered = cachedCategories.ToList();
                }
            }
        }
        public void ResetColorIndex()
        {
            nextColorIndex = 0;
        }
        public override void Refresh()
        {
            base.Refresh();

            Recalc();
        }
        public void UpdatePlot(bool force = false)
        {
            if (cachedPlotHeight <= 0) 
                return;
            
            cachedGridMaxValue = GetSeriesMaximum();
            cachedGridMinValue = GetSeriesMinimum();
            
            if (force == false && fixedMin == null && fixedMax == null)
            {
                if (cachedPlotMax != 0 && cachedPlotMin != 0)
                    if (cachedGridMaxValue == cachedPlotMax &&
                        cachedGridMinValue == cachedPlotMin)
                        return;
            }
            
            cachedGridLinesStep = (double)cachedPlotHeight / cachedGridLines;
            cachedGridValues = new string[cachedGridLines + 1]; // + bottom line.

            const string valFormat = "0.00";
            
            if (cachedGridMaxValue <= cachedGridMinValue)
                cachedGridMaxValue = cachedGridMinValue + 1;
                
            double valRange = cachedGridMaxValue - cachedGridMinValue;
            double valStep = valRange / (cachedGridValues.Length - 1);

            int valIndex = 0;
            for (double i = cachedGridMaxValue; i > cachedGridMinValue; i -= valStep)
            {
                cachedGridValues[valIndex] = i.ToString(valFormat);
                valIndex++;
            }
            cachedGridValues[cachedGridValues.Length - 1] = cachedGridMinValue.ToString(valFormat);

            cachedPlotMin = cachedGridMinValue;
            cachedPlotMax = cachedGridMaxValue;
        }

        internal Color GetNextColor()
        {
            if (colors == null || colors.Length == 0) return Color.Gray;
            if (colors.Length == 1) return colors[0];

            var nc = colors[nextColorIndex];

            nextColorIndex++;
            if (nextColorIndex >= colors.Length)
                nextColorIndex = 0;

            return nc;
        }
        internal void AsyncUpdateLegendButtons()
        {
            updateLegendButtonsFlag = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            mouseX = MathHelper.Clamp(e.X, cachedPlotLeft, cachedPlotRight);
            mouseY = MathHelper.Clamp(e.Y, cachedPlotTop, cachedPlotBottom);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            var graphics = e.Graphics;
            
            TryUpdateLegendButtons(graphics);
            
            graphics.uwfFillRectangle(chart.backgroundColor, 0, 0, Width, Height);

            DrawTitle(graphics);

            if (cachedPlotHeight > 0)
            {
                DrawPlotBack(graphics);
                DrawYAxes(graphics);
                DrawXAxis(graphics);
                
                for (int i = 0; i < series.Count; i++)
                {
                    var s = series[i];
                    if (!s.visible) continue;
                    
                    DrawSeries(graphics, s);
                }
                
                DrawTooltip(graphics);
                DrawPlotBorder(graphics);
            }

            if (chart.borderWidth > 0)
            {
                var borderPen = chart.borderPen;
                borderPen.Color = chart.borderColor;
                borderPen.Width = chart.borderWidth;

                graphics.DrawRectangle(borderPen, 0, 0, Width, Height);
            }
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            Refresh();
        }
        protected virtual void DrawSeries(Graphics g, Series s)
        {
            var clipRect = new Rectangle(plot.Padding.Left, plot.Padding.Top, plot.Width - plot.Padding.Horizontal, plot.Height - plot.Padding.Vertical);
            
            g.GroupBegin(plot);
            s.Paint(g, clipRect); 
            g.GroupEnd();
        }
        protected virtual void DrawTitle(Graphics g)
        {
            if (title.text != null) g.uwfDrawString(title.text, title.font, title.color, title.x, title.y, Width - title.x, title.font.Size + 6, title.align);
            if (subtitle.text != null) g.uwfDrawString(subtitle.text, subtitle.font, subtitle.color, subtitle.x, cachedSubtitleY, Width - subtitle.x, subtitle.font.Size + 6, subtitle.align);
        }
        protected virtual void DrawTooltip(Graphics g)
        {
            if (hovered == false)
                return;
            if (GetSeriesMaximumDataAmount() <= 0)
                return;

            var font = DefaultFont10;

            mousePositionPen.Color = chart.plotBorderColor;
            g.DrawLine(mousePositionPen, mouseX, cachedPlotTop, mouseX, cachedPlotBottom);

            for (int i = 0, index = 0; i < series.Count; i++)
            {
                var s = series[i];
                if (s.visible == false || s.data.Count == 0)
                    continue;

                var val = GetValueAtX(s, mouseX);

                g.uwfDrawString(
                    "v: " + val,
                    font,
                    s.color,
                    cachedPlotLeft + 8,
                    cachedPlotTop + index * 14,
                    cachedPlotWidth,
                    20,
                    ContentAlignment.TopLeft);

                index++;
            }
        }
        protected virtual void DrawPlotBack(Graphics g)
        {
            g.uwfFillRectangle(chart.plotBackgroundColor, cachedPlotLeft, cachedPlotTop, cachedPlotRight - cachedPlotLeft, cachedPlotBottom - cachedPlotTop);
        }
        protected virtual void DrawPlotBorder(Graphics g)
        {
            if (chart.plotBorderPen.Width != 0)
                g.DrawRectangle(chart.plotBorderPen, cachedPlotLeft, cachedPlotTop, cachedPlotRight - cachedPlotLeft, cachedPlotBottom - cachedPlotTop);
        }
        protected virtual void DrawXAxis(Graphics g)
        {
            if (xAxis.visible == false)
                return;
            
            if (cachedCategoriesFiltered != null)
            {
                // Categories.
                var cachedCategoriesLength = cachedCategoriesFiltered.Count;
                for (int i = 0; i < cachedCategoriesLength; i++)
                {
                    float categoryX = cachedPlotLeft + i * cachedCategoriesStep;
                    g.DrawLine(xAxis.tickPen, categoryX, cachedPlotBottom + xAxis.offset, categoryX, cachedPlotBottom + xAxis.tickLength + xAxis.offset);
                    g.uwfDrawString(
                        cachedCategoriesFiltered[i],
                        categoriesFont,
                        categoriesColor,
                        categoryX - cachedCategoriesStep / 2,
                        cachedPlotBottom + xAxis.offset,
                        cachedCategoriesStep,
                        32,
                        ContentAlignment.MiddleCenter);
                }

                if (cachedCategoriesStep == categoriesMinStep) // Draw last tick if needed.
                    g.DrawLine(xAxis.tickPen, cachedPlotRight, cachedPlotBottom + xAxis.offset, cachedPlotRight, cachedPlotBottom + xAxis.tickLength + xAxis.offset);
            }
            
            // Axis line.
            if (xAxis.lineWidth > 0)
                g.DrawLine(xAxis.linePen, cachedPlotLeft, cachedPlotBottom + xAxis.offset, cachedPlotRight, cachedPlotBottom + xAxis.offset);
        }
        protected virtual void DrawYAxes(Graphics g)
        {
            for (int i = 0; i < yAxis.Count; i++)
            {
                var ya = yAxis[i];
                if (ya.visible == false)
                    continue;

                // Grid.
                if (ya.gridLineWidth > 0)
                {
                    for (int k = 0; k <= cachedGridLines; k++)
                    {
                        var gridY = (int)(cachedPlotTop + k * cachedGridLinesStep);

                        g.DrawLine(ya.gridPen, cachedPlotLeft, gridY, cachedPlotRight, gridY);
                        g.uwfDrawString(cachedGridValues[k], ya.gridFont, gridTextColor, 0, gridY + gridTextYOffet,
                            cachedPlotLeft - ya.offset + gridTextXOffset, 20, HorizontalAlignment.Right);
                    }
                }

                // Draw axis line.
                if (ya.lineWidth > 0)
                    g.DrawLine(ya.linePen, cachedPlotLeft - ya.offset, cachedPlotTop, cachedPlotLeft - ya.offset, cachedPlotBottom);
            }
        }

        internal string GetCategoryAtAx(int x)
        {
            var categories = xAxis.categories;
            if (categories == null || categories.Count == 0)
                return null;

            var categoriesCount = categories.Count;
            var xCoef = (float)(x - cachedPlotLeft) / cachedPlotWidth;
            var dataIndex = (int)(xCoef * categoriesCount);
            if (dataIndex >= categoriesCount || dataIndex < 0)
                return null;

            return categories[dataIndex];
        }
        internal double GetSeriesMaximum()
        {
            if (fixedMax != null)
                return fixedMax.Value;
            
            var max = 1d;
            var seriesCount = series.Count;
            
            for (int i = 0; i < seriesCount; i++)
            {
                var s = series[i];
                if (s.visible == false) 
                    continue;
                
                var sMax = s.data.GetMax();
                if (sMax > max)
                    max = sMax;
            }
            
            return max;
        }
        internal double GetSeriesMinimum()
        {
            if (fixedMin != null)
                return fixedMin.Value;
            
            var min = 0d;
            var seriesCount = series.Count;
            
            for (int i = 0; i < seriesCount; i++)
            {
                var s = series[i];
                if (s.visible == false)
                    continue;

                var sMin = s.data.GetMin();
                if (sMin < min)
                    min = sMin;
            }

            return min;
        }
        internal int GetSeriesMaximumDataAmount()
        {
            if (series.Count == 0)
                return -1;

            int? a = null;
            
            for (int i = 0; i < series.Count; i++)
            {
                var seriesItem = series[i];
                if (!seriesItem.visible) continue;

                if (a == null)
                    a = seriesItem.data.Count;
                else if (seriesItem.data.Count > a)
                    a = seriesItem.data.Count;
            }
            
            if (a == null) 
                return -1;
            
            return a.Value;
        }
        internal int GetSeriesMinimumDataAmount()
        {
            if (series.Count == 0)
                return -1;

            if (series.Count == 1)
            {
                if (series[0].visible == false)
                    return -1;
                return series[0].data.Count;
            }

            int? a = null;
            for (int i = 0; i < series.Count; i++)
                if (series[i].visible)
                {
                    if (a == null)
                        a = series[i].data.Count;
                    else if (series[i].data.Count < a)
                        a = series[i].data.Count;
                }
            if (a == null) return -1;
            return a.Value;
        }
        internal double GetValueAtX(Series s, int x)
        {
            if (s == null || s.data.Count == 0)
                return 0;

            var maxDataAmount = GetSeriesMaximumDataAmount();
            if (maxDataAmount == 0) return 0;

            var xMod = (float)(x - cachedPlotLeft - plot.Padding.Left) / cachedPlotWidth;
            var dataIndex = (int) (xMod * (maxDataAmount - 1));
            if (dataIndex >= s.data.Count || dataIndex < 0)
                return 0;

            return s.data[dataIndex];
        }
        internal void Recalc()
        {
            var marginBottom = chart.spacingBottom;
            var marginLeft = chart.spacingLeft + 38;
            var marginRight = chart.spacingRight;
            var marginTop = chart.spacingTop;

            if (chart.marginBottom != null) marginBottom = chart.marginBottom.Value;
            if (chart.marginLeft != null) marginLeft = chart.marginLeft.Value;
            if (chart.marginRight != null) marginRight = chart.marginRight.Value;
            if (chart.marginTop != null) marginTop = chart.marginTop.Value;
            
            cachedPlotBottom = Height - marginBottom;
            cachedPlotLeft = marginLeft;
            cachedPlotRight = Width - marginRight;
            cachedPlotTop = marginTop;
            cachedSubtitleY = subtitle.y;

            // Calc stuff that can change plot size first.

            // Check title & subtitle.
            if (title.text != null)
            {
                cachedPlotTop = (int)title.font.Size;
                cachedSubtitleY += (int)title.font.Size;

                if (title.floating == false)
                    cachedPlotTop += title.margin;
            }
            
            if (subtitle.text != null && subtitle.floating == false)
                cachedPlotTop += (int)subtitle.font.Size;
            
            // Check legend.
            if (legend.enabled && legend.floating == false)
            {
                if (legend.layout == LegendLayouts.horizontal)
                {
                    var lbHeight = UpdateLegendLocation();

                    cachedPlotBottom -= lbHeight + legend.margin + 12; // + categories text height?
                }
                else
                {
                    // vertical
                }
            }
            
            // Time to find out the plot size.
            cachedPlotBottom -= xAxis.tickLength;
            cachedPlotHeight = cachedPlotBottom - cachedPlotTop;
            cachedPlotWidth = cachedPlotRight - cachedPlotLeft;
            
            plot.Location = new Point(cachedPlotLeft, cachedPlotTop);
            plot.Size = new Size(cachedPlotWidth, cachedPlotHeight);

            // Other stuff.
            RecalcCategories();
            UpdatePlot(true);
        }
        internal int UpdateLegendLocation()
        {
            if (legendButtons == null) return 0;

            int plotCenterX = cachedPlotLeft + (cachedPlotRight - cachedPlotLeft) / 2;
            int totalLBWidth = 0;
            for (int i = 0; i < legendButtons.Length; i++)
                totalLBWidth += legendButtons[i].Width + legend.itemDistance;

            // Horizontal.
            if (cachedPlotWidth >= totalLBWidth)
            {
                int leftPos = plotCenterX - totalLBWidth / 2;
                int lbHeight = 0;
                for (int i = 0, cw = 0; i < legendButtons.Length; i++)
                {
                    var lb = legendButtons[i];
                    lb.Location = new Point(leftPos + cw, Height - lb.Height - chart.spacingBottom);

                    cw += lb.Width + legend.itemDistance;
                    lbHeight = lb.Height;
                }

                return lbHeight;
            }
            else
            {
                // Vertical.
                int lbHeight = 0;
                for (int i = 0; i < legendButtons.Length; i++)
                {
                    var lb = legendButtons[i];
                    lb.Location = new Point(plotCenterX - lb.Width / 2, Height - lb.Height * (i + 1) - chart.spacingBottom);
                    lbHeight += lb.Height;
                }

                return lbHeight;
            }
        }

        private void TryUpdateLegendButtons(Graphics g)
        {
            if (!updateLegendButtonsFlag)
                return;
            
            updateLegendButtonsFlag = false;
            
            if (legendButtons != null)
                for (int i = 0; i < legendButtons.Length; i++)
                    legendButtons[i].Dispose();

            if (!legend.enabled)
                return;
            
            legendButtons = new LegendButton[series.Count];
            
            for (int i = 0; i < legendButtons.Length; i++)
            {
                var s = series[i];
                var lb = new LegendButton(legend, s);

                lb.Width = (int) g.MeasureString(lb.Text, legend.itemStyle.font).Width + 20;

                if (legend.layout == LegendLayouts.horizontal)
                    lb.Location = new Point(i * 120, Height - lb.Height);

                Controls.Add(lb);

                legendButtons[i] = lb;
            }

            Recalc();
        }
    }
}
