namespace UnityWinForms.Examples.Panels
{
    using System.Windows.Forms;

    public class PanelScrollBar : BaseExamplePanel
    {
        public override void Initialize()
        {
            this.Create<HScrollBar>();
            this.Create<VScrollBar>();
        }
    }
}
