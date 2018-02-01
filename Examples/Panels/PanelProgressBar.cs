namespace UnityWinForms.Examples.Panels
{
    using System.Windows.Forms;

    public class PanelProgressBar : BaseExamplePanel
    {
        private Timer timer;

        public override void Initialize()
        {
            var labelMarqueeText = "Marguee. Value: ";
            var labelMarquee = this.Create<Label>(labelMarqueeText);

            var progressBarMarquee = this.Create<ProgressBar>();
            progressBarMarquee.Style = ProgressBarStyle.Marquee;

            var labelBlocksText = "Blocks. Value: ";
            var labelBlocks = this.Create<Label>(labelBlocksText, false, 8, lineOffset);

            var progressBarBlocks = this.Create<ProgressBar>();
            progressBarBlocks.Style = ProgressBarStyle.Blocks;

            timer = new Timer();
            timer.Interval = 100;
            timer.Tick += (sender, args) =>
            {
                var nextValue = progressBarMarquee.Value + 1;
                if (nextValue > progressBarMarquee.Maximum)
                    nextValue = 0;

                progressBarMarquee.Value = nextValue;
                progressBarBlocks.Value = nextValue;

                labelMarquee.Text = labelMarqueeText + progressBarMarquee.Value;
                labelBlocks.Text = labelBlocksText + progressBarBlocks.Value;
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
