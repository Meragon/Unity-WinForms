namespace UnityWinForms.Examples.Panels
{
    using System.Windows.Forms;

    public class PanelProgressBar : BaseExamplePanel
    {
        private Timer timer;

        public override void Initialize()
        {
            var progressBar = this.Create<ProgressBar>();
            progressBar.Style = ProgressBarStyle.Marquee;

            timer = new Timer();
            timer.Interval = 100;
            timer.Tick += (sender, args) =>
            {
                var nextValue = progressBar.Value + 1;
                if (nextValue > progressBar.Maximum)
                    nextValue = 0;
                progressBar.Value = nextValue;
            };
            timer.Start();
        }

        protected override void Dispose(bool release_all)
        {
            base.Dispose(release_all);

            if (timer != null)
                timer.Dispose();
        }
    }
}
