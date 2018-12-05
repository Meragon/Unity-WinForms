namespace UnityWinForms.Examples.Panels
{
    using System.Windows.Forms;

    public class PanelScrollBar : BaseExamplePanel
    {
        public override void Initialize()
        {
            var scrollValue = this.Create<Label>();
            var hscroll = this.Create<HScrollBar>();
            var vscroll = this.Create<VScrollBar>();

            hscroll.ValueChanged += (sender, args) => scrollValue.Text = hscroll.Value.ToString();
            
            vscroll.Minimum = -200;
            vscroll.Height = 320;
            vscroll.ValueChanged += (sender, args) => scrollValue.Text = vscroll.Value.ToString();
        }
    }
}
