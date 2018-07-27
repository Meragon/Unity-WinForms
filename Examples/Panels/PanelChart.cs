namespace UnityWinForms.Examples.Panels
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    
    using Highcharts;
    
    public class PanelChart : BaseExamplePanel
    {
        private static double x;
        private static double xStep = .02d;
        
        private Timer timer;
        
        public override void Initialize()
        {
            var seriesSin = new Series("Sin");
            seriesSin.type = SeriesTypes.areaSolid;
            
            var seriesCos = new Series("Cos");
            seriesCos.type = SeriesTypes.areaSolid;
            
            var chart = this.Create<Highchart>();
            chart.Size = new Size(480, 320);
            chart.title.text = "Test chart";
            chart.subtitle.text = "(updaing every frame)";
            chart.series.Add(seriesSin);
            chart.series.Add(seriesCos);
            chart.Refresh(); // To update plot.
            
            timer = new Timer();
            timer.Interval = 16; // Update every frame.
            timer.Tick += (sender, args) =>
            {
                seriesSin.data.Add(Math.Sin(x) + 1);
                seriesCos.data.Add(Math.Cos(x) / 2);
                
                // Limit data.
                if (seriesSin.data.Count > 100)
                {
                    seriesSin.data.RemoveAt(0);
                    seriesCos.data.RemoveAt(0);
                }
                
                chart.RecalcCategoriesAndData();

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