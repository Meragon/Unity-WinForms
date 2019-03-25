namespace UnityWinForms.Examples.Panels
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    
    using Highcharts;
    
    public class PanelChart : BaseExamplePanel
    {
        private double x;
        private double xStep = .02d;
        
        private Timer timer;
        
        public override void Initialize()
        {
            this.Create<Label>("Press LMB on the legend button to enable/disable series, press RMB to show the context menu.");
            
            var seriesSin = new Series("Sin");
            seriesSin.linearGradient = true;
            seriesSin.type = SeriesTypes.areaSolidOutline;
            
            var seriesCos = new Series("Cos");
            seriesCos.linearGradient = true;
            seriesCos.type = SeriesTypes.areaSolidOutline;
            
            var seriesX = new Series("Rounded X");
            seriesX.linearGradient = true;
            seriesX.type = SeriesTypes.areaSolid;
            
            var chart = this.Create<Highchart>();
            chart.fixedMin = -2; // Set plot min value.
            chart.linearGradientMaterial = UnitWinFormsExamples.s_chartGradient; // Can't make clipping working for this shader.
            chart.Size = new Size(480, 320);
            chart.title.text = "Test chart";
            chart.subtitle.text = "(updating every frame)";
            chart.legend.itemHoverStyle = new LegendItemStyle(Color.FromArgb(0, 122, 204));
            chart.series.Add(seriesSin);
            chart.series.Add(seriesCos);
            chart.series.Add(seriesX);
            chart.Refresh(); // To update plot.

            // Set custom series colors. Colors are updating when new series is added to the chart.
            seriesSin.color = Color.FromArgb(205, 196, 67);
            seriesCos.color = Color.FromArgb(235, 101, 79);
            seriesX.color = Color.Gray;
            
            timer = new Timer();
            timer.Interval = 16; // Update every frame (1000 msec / 60 fps = 16.(6) frames needs to be updated per 1 second).
            timer.Tick += (sender, args) =>
            {
                var sin = Math.Truncate((Math.Sin(x) + 1) * 100) / 100;
                var cos = Math.Truncate((Math.Cos(x) / 2) * 100) / 100;
                var sx = Math.Truncate((Math.Floor(x * 2) / 6) * 100) / 100;
                
                seriesSin.data.Add(sin);
                seriesCos.data.Add(cos);
                seriesX.data.Add(sx);
                
                // Limit data.
                if (seriesSin.data.Count > 100)
                {
                    seriesSin.data.RemoveAt(0);
                    seriesCos.data.RemoveAt(0);
                    seriesX.data.RemoveAt(0);
                }
                
                chart.RecalcCategories();
                chart.UpdatePlot();

                // Update X.
                x += xStep;
                if (x > Math.PI)
                    x = -Math.PI;
            };
            timer.Start();
        }

        protected override void Dispose(bool release_all)
        {
            base.Dispose(release_all);
            
            timer.Dispose();
            x = 0;
        }
    }
}