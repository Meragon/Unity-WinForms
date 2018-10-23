using System.Linq;

namespace Highcharts
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
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
        public static string textLegendMenu_Type = "Type";
        public static string[] textLegendMenu_TypeNames = { "None", "Area Solid", "Area Solid Outline", "Line", "Line Solid", "Point" };

        public int     categoriesMinStep = 20;
        public double? fixedMax;
        public double? fixedMin;

        private readonly Color categoriesColor  = Color.Gray;
        private readonly Font  categoriesFont   = DefaultFont10;
        private readonly Color gridTextColor    = Color.Gray;
        private readonly Pen   mousePositionPen = new Pen(Color.Gray);

        private float circleRadius    = 2;
        private int   gridTextYOffet  = -10;
        private int   gridTextXOffset = -2;
        private int   mouseX;
        private int   mouseY;
        private int   nextColorIndex;
        private int   plotRight = 8;

        private string[]     cachedCategories;
        private float        cachedCategoriesStep;
        private List<string> cachedCategoriesFiltered;
        private int          cachedGridLines = 6;
        private int          cachedGridLinesStep;
        private double       cachedGridMaxValue;
        private double       cachedGridMinValue;
        private string[]     cachedGridValues;
        private int          cachedPlotTop;
        private int          cachedPlotBottom;
        private int          cachedPlotLeft = 48;
        private int          cachedPlotRight;
        private int          cachedPlotHeight;
        private int          cachedPlotWidth;
        private double       cachedPlotMin;
        private double       cachedPlotMax;
        private int          cachedSubtitleY;

        private LegendButton[] legendButtons;

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
            series = new SeriesCollection(this);
            subtitle = new HightchartSubtitle();
            title = new HighchartTitle();
            xAxis = new XAxis();
            yAxis = new List<Axis>();

            yAxis.Add(new YAxis());

            Recalc();
        }

        public Chart chart { get; private set; }
        public Color[] colors { get; private set; }
        public Legend legend { get; private set; }
        public object linearGradientMaterial { get; set; } // Gradint material for areaSolidOutline series type.
        public SeriesCollection series { get; private set; }
        public HightchartSubtitle subtitle { get; private set; }
        public HighchartTitle title { get; private set; }
        public Axis xAxis { get; private set; }
        public List<Axis> yAxis { get; private set; }

        internal string[] Categories
        {
            get { return cachedCategories; }
        }
        
        public static SizeF MeasureStringSimple(Font font, string text)
        {
            return new SizeF() { Width = text.Length * 8, Height = font.Size }; // fast, but not accurate.
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
        public void UpdatePlot()
        {
            if (cachedPlotHeight <= 0) 
                return;
            
            cachedGridMaxValue = GetSeriesMaximum();
            cachedGridMinValue = GetSeriesMinimum();

            if (cachedPlotMax != 0 && cachedPlotMin != 0)
                if (cachedGridMaxValue == cachedPlotMax &&
                    cachedGridMinValue == cachedPlotMin)
                    return;
            
            cachedGridLinesStep = cachedPlotHeight / cachedGridLines;
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
        internal void UpdateLegend()
        {
            if (legendButtons != null)
                for (int i = 0; i < legendButtons.Length; i++)
                    legendButtons[i].Dispose();

            if (legend.enabled == false)
                return;
            
            legendButtons = new LegendButton[series.Count];
            for (int i = 0; i < legendButtons.Length; i++)
            {
                var s = series[i];
                var lb = new LegendButton(legend, s);
                if (s.name == null)
                    lb.Text = string.Concat("Series ", (i + 1).ToString());

                lb.Width = (int)MeasureStringSimple(legend.itemStyle.font, lb.Text).Width + 20;

                if (legend.layout == LegendLayouts.horizontal)
                    lb.Location = new Point(i * 120, Height - lb.Height);

                Controls.Add(lb);

                legendButtons[i] = lb;
            }

            UpdateLegendLocation();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            mouseX = MathHelper.Clamp(e.X, cachedPlotLeft, cachedPlotRight);
            mouseY = MathHelper.Clamp(e.Y, cachedPlotTop, cachedPlotBottom);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            
            g.uwfFillRectangle(chart.backgroundColor, 0, 0, Width, Height);

            DrawTitle(g);

            if (cachedPlotHeight > 0)
            {
                DrawPlotBack(g);
                DrawXAxis(g);
                DrawYAxes(g);
                
                for (int i = 0; i < series.Count; i++)
                {
                    var s = series[i];
                    if (s.visible)
                        DrawSeries(g, s);
                }
                
                DrawTooltip(g);
                DrawPlotBorder(g);
            }

            if (chart.borderWidth > 0)
            {
                var borderPen = chart.borderPen;
                borderPen.Color = chart.borderColor;
                borderPen.Width = chart.borderWidth;

                g.DrawRectangle(borderPen, 0, 0, Width, Height);
            }
        }
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            Refresh();
        }

        private void DrawSeries(Graphics g, Series s)
        {
            var sdataCount = s.data.Count;
            if (sdataCount == 0) return;

            var categoriesIndex = 0;
            var prevValue = 0D;
            var areaColor = Color.FromArgb(255 / series.Count, s.color);
            var valueRange = cachedPlotMax - cachedPlotMin;

            var pointInterval = s.pointInterval;
            if (pointInterval <= 0)
                return;
            
            var xStep = cachedCategoriesStep;
            if (sdataCount > 1)
                xStep = (float)cachedPlotWidth / (sdataCount - 1);
            xStep *= pointInterval;

            int locationX = Location.X;
            int locationY = Location.Y;
            int parentHeight = 0;
            int parentWidth = 0;
            if (Parent != null)
            {
                parentHeight = Parent.Height;
                parentWidth = Parent.Width;
            }
            
            for (float i = 0; i < sdataCount; i += pointInterval)
            {
                double currentValue = s.data[(int) i];

                bool last = i + pointInterval >= sdataCount;
                float currentValueYCoef = (float) ((currentValue - cachedPlotMin) / valueRange);
                float currentValueX = cachedPlotLeft + xStep * categoriesIndex;
                float currentValueY = cachedPlotTop + cachedPlotHeight - cachedPlotHeight * currentValueYCoef;

                if (currentValueX > cachedPlotRight)
                    currentValueX = cachedPlotRight;
                
                bool clipHorizontal = false;
                if (parentWidth != 0 && parentHeight != 0)
                {
                    if (locationX + currentValueX < -uwfOffset.X) // Left side.
                        clipHorizontal = true;
                    if (locationX + currentValueX + uwfOffset.X > parentWidth) // Right side.
                        clipHorizontal = true;
                }

                if (clipHorizontal == false)
                    switch (s.type)
                    {
                        case SeriesTypes.areaSolid:
                        {
                            if (i > 0)
                            {
                                float prevValueYCoef = (float) ((prevValue - cachedPlotMin) / valueRange);
                                float prevValueX = cachedPlotLeft + (categoriesIndex - 1) * xStep;
                                float prevValueY = cachedPlotTop + cachedPlotHeight - cachedPlotHeight * prevValueYCoef;
    
                                g.uwfFillRectangle(areaColor, prevValueX, prevValueY, currentValueX - prevValueX,
                                    cachedPlotBottom - prevValueY);
                                if (last)
                                    g.uwfFillRectangle(areaColor, currentValueX, currentValueY, xStep,
                                        cachedPlotBottom - currentValueY);
                            }
                        }
                            break;
                        case SeriesTypes.areaSolidOutline:
                        {
                            if (i > 0)
                            {
                                float prevValueYCoef = (float) ((prevValue - cachedPlotMin) / valueRange);
                                float prevValueX = cachedPlotLeft + (categoriesIndex - 1) * xStep;
                                float prevValueY = cachedPlotTop + cachedPlotHeight - cachedPlotHeight * prevValueYCoef;
                                
                                if (s.linearGradient == false)
                                {
                                    g.uwfFillRectangle(areaColor, prevValueX, prevValueY, currentValueX - prevValueX,
                                        cachedPlotBottom - prevValueY);
    
                                    if (last)
                                        g.uwfFillRectangle(areaColor, currentValueX, currentValueY, xStep,
                                            cachedPlotBottom - currentValueY);
                                }
                                else if (linearGradientMaterial != null)
                                {
                                    float clipH = cachedPlotHeight;
                                    
    #if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_WEBGL
                                    
                                    float clipY = prevValueY;
                                    
                                    // Vertical clipping.
                                    if (locationY + currentValueY < -uwfOffset.Y)
                                        clipY = -uwfOffset.Y - locationY;
                                    if (locationY + cachedPlotTop + clipH + uwfOffset.Y > parentHeight)
                                        clipH = parentHeight - uwfOffset.Y - locationY - cachedPlotTop;
                                    
                                    var unityMaterial = linearGradientMaterial as UnityEngine.Material;
                                    if (unityMaterial != null)
                                        unityMaterial.SetFloat("_Y", 1 - (clipY - cachedPlotTop) / clipH);
                                    
    #endif
                                    g.uwfFillRectangle(areaColor, prevValueX, cachedPlotTop, currentValueX - prevValueX, clipH, linearGradientMaterial);
                                    if (last)
                                        g.uwfFillRectangle(areaColor, currentValueX, cachedPlotTop, xStep, clipH, linearGradientMaterial);
                                }
    
                                g.DrawLine(s.pen, prevValueX, prevValueY, currentValueX + 1, prevValueY);
                                g.DrawLine(s.pen, currentValueX, prevValueY, currentValueX, currentValueY);
                            }
                        }
                            break;
                        case SeriesTypes.line:
                        {
                            if (i > 0)
                            {
                                float prevValueYCoef = (float) ((prevValue - cachedPlotMin) / valueRange);
                                float prevValueX = cachedPlotLeft + (categoriesIndex - 1) * xStep;
                                float prevValueY = cachedPlotTop + cachedPlotHeight - cachedPlotHeight * prevValueYCoef;
                                g.DrawLine(s.pen, prevValueX, prevValueY, currentValueX + 1, currentValueY);
                            }
                        }
                            break;
                        case SeriesTypes.lineSolid:
                        {
                            if (i > 0)
                            {
                                float prevValueYCoef = (float) ((prevValue - cachedPlotMin) / valueRange);
                                float prevValueX = cachedPlotLeft + (categoriesIndex - 1) * xStep;
                                float prevValueY = cachedPlotTop + cachedPlotHeight - cachedPlotHeight * prevValueYCoef;
                                g.DrawLine(s.pen, prevValueX, prevValueY, currentValueX + 1, prevValueY);
                                g.DrawLine(s.pen, currentValueX, prevValueY, currentValueX, currentValueY);
                            }
                        }
                            break;
                        case SeriesTypes.point:
                        {
                            g.uwfDrawImage(ApplicationResources.Images.Circle, s.color, currentValueX - circleRadius,
                                currentValueY - circleRadius, circleRadius * 2, circleRadius * 2);
                        }
                            break;
                    }

                if (hovered && mouseX > currentValueX - xStep / 2 && mouseX < currentValueX + xStep / 2)
                    g.uwfFillRectangle(s.color, currentValueX - 2, currentValueY - 2, 4, 4);

                prevValue = currentValue;
                categoriesIndex++;
            }
        }
        private void DrawTitle(Graphics g)
        {
            if (title.text != null) g.uwfDrawString(title.text, title.font, title.color, title.x, title.y, Width - title.x, title.font.Size + 6, title.align);
            if (subtitle.text != null) g.uwfDrawString(subtitle.text, subtitle.font, subtitle.color, subtitle.x, cachedSubtitleY, Width - subtitle.x, subtitle.font.Size + 6, subtitle.align);
        }
        private void DrawTooltip(Graphics g)
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

                var val = ValueAtX(s, mouseX);

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
        private void DrawPlotBack(Graphics g)
        {
            g.uwfFillRectangle(chart.plotBackgroundColor, cachedPlotLeft, cachedPlotTop, cachedPlotRight - cachedPlotLeft, cachedPlotBottom - cachedPlotTop);
        }
        private void DrawPlotBorder(Graphics g)
        {
            if (chart.plotBorderPen.Width != 0)
                g.DrawRectangle(chart.plotBorderPen, cachedPlotLeft, cachedPlotTop, cachedPlotRight - cachedPlotLeft, cachedPlotBottom - cachedPlotTop);
        }
        private void DrawXAxis(Graphics g)
        {
            if (cachedCategoriesFiltered == null) return;

            // Categories.
            var cachedCategoriesLength = cachedCategoriesFiltered.Count;
            for (int i = 0; i < cachedCategoriesLength; i++)
            {
                float categoryX = cachedPlotLeft + i * cachedCategoriesStep;
                g.DrawLine(xAxis.tickPen, categoryX, cachedPlotBottom, categoryX, cachedPlotBottom + xAxis.tickLength);
                g.uwfDrawString(
                    cachedCategoriesFiltered[i],
                    categoriesFont,
                    categoriesColor,
                    categoryX - cachedCategoriesStep / 2,
                    cachedPlotBottom,
                    cachedCategoriesStep,
                    32,
                    ContentAlignment.MiddleCenter);
            }
            if (cachedCategoriesStep == categoriesMinStep) // Draw last tick if needed.
                g.DrawLine(xAxis.tickPen, cachedPlotRight, cachedPlotBottom, cachedPlotRight, cachedPlotBottom + xAxis.tickLength);
        }
        private void DrawYAxes(Graphics g)
        {
            for (int i = 0; i < yAxis.Count; i++)
            {
                var ya = yAxis[i];

                // Grid.
                if (ya.gridLineWidth > 0)
                {
                    for (int k = 0; k < cachedGridLines; k++)
                    {
                        int gridY = cachedPlotTop + k * cachedGridLinesStep;
                        g.DrawLine(ya.gridPen, cachedPlotLeft, gridY, cachedPlotRight, gridY);
                        g.uwfDrawString(cachedGridValues[k], ya.gridFont, gridTextColor, 0, gridY + gridTextYOffet, cachedPlotLeft + gridTextXOffset, 20, HorizontalAlignment.Right);
                    }
                    g.uwfDrawString(cachedGridValues[cachedGridLines], ya.gridFont, gridTextColor, 0, cachedPlotBottom + gridTextYOffet, cachedPlotLeft + gridTextXOffset, 20, HorizontalAlignment.Right);
                }

                // Bottom line.
                xAxis.gridPen.Width = ya.gridLineWidth;
                g.DrawLine(xAxis.gridPen, cachedPlotLeft, cachedPlotBottom, cachedPlotRight, cachedPlotBottom);
            }
        }
        private double GetSeriesMaximum()
        {
            double? max = fixedMax;
            var seriesCount = series.Count;
            for (int i = 0; i < seriesCount; i++)
            {
                var s = series[i];
                if (s.visible == false) 
                    continue;
                
                var sMax = s.data.GetMax();
                if (max == null || sMax > max)
                    max = sMax;
            }

            if (max == null)
                max = 1d;
            
            return max.Value;
        }
        private double GetSeriesMinimum()
        {
            double? min = fixedMin;
            var seriesCount = series.Count;
            for (int i = 0; i < seriesCount; i++)
            {
                var s = series[i];
                if (s.visible == false)
                    continue;

                var sMin = s.data.GetMin();
                if (min == null || sMin < min)
                    min = sMin;
            }

            if (min == null)
                min = 0d;

            return min.Value;
        }
        private int GetSeriesMaximumDataAmount()
        {
            var seriesCount = series.Count;
            if (seriesCount == 0)
                return -1;

            if (seriesCount == 1)
            {
                var firstSeries = series[0];
                if (firstSeries.visible == false)
                    return -1;
                return firstSeries.data.Count;
            }

            int? a = null;
            for (int i = 0; i < seriesCount; i++)
            {
                var seriesItem = series[i];
                if (!seriesItem.visible) continue;

                if (a == null)
                    a = seriesItem.data.Count;
                else if (seriesItem.data.Count > a)
                    a = seriesItem.data.Count;
            }
            if (a == null) return -1;
            return a.Value;
        }
        private int GetSeriesMinimumDataAmount()
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
        private void Recalc()
        {
            cachedPlotBottom = Height;
            cachedPlotRight = Width - plotRight;
            cachedPlotTop = 12;
            cachedSubtitleY = subtitle.y;

            // Calc stuff that can change plot size first.

            #region Check title & subtitle.
            if (title.text != null)
            {
                cachedPlotTop = (int)title.font.Size;
                cachedSubtitleY += (int)title.font.Size;

                if (title.floating == false)
                    cachedPlotTop += title.margin;
            }
            if (subtitle.text != null && subtitle.floating == false)
                cachedPlotTop += (int)subtitle.font.Size;
            #endregion

            #region Check legend.
            if (legend.enabled && legend.floating == false)
            {
                if (legend.layout == LegendLayouts.horizontal)
                {
                    var lbHeight = UpdateLegendLocation();

                    cachedPlotBottom = Height - lbHeight + legend.margin;
                }
                else
                {
                    // vertical
                }
            }
            #endregion

            // Time to find out the plot size.

            cachedPlotBottom -= xAxis.tickLength;
            cachedPlotBottom -= 24; // Text.

            cachedPlotHeight = cachedPlotBottom - cachedPlotTop;
            cachedPlotWidth = cachedPlotRight - cachedPlotLeft;

            // Other stuff.
            RecalcCategories();
            UpdatePlot();
        }
        private int UpdateLegendLocation()
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
                    lb.Location = new Point(leftPos + cw, Height - lb.Height);

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
                    lb.Location = new Point(plotCenterX - lb.Width / 2, Height - lb.Height * (i + 1));
                    lbHeight += lb.Height;
                }

                return lbHeight;
            }
        }
        private double ValueAtX(Series s, int x)
        {
            if (s == null)
                return 0;

            var sdataCount = s.data.Count;
            if (sdataCount == 0) return 0;

            var xCoef = (float)(x - cachedPlotLeft) / cachedPlotWidth;
            var dataIndex = (int)(xCoef * sdataCount);
            if (dataIndex > sdataCount)
                return 0;

            return s.data[dataIndex];
        }
    }
}
