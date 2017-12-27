namespace UnityWinForms.Examples.Panels
{
    using System.Windows.Forms;

    public class PanelLinkLabel : BaseExamplePanel
    {
        public override void Initialize()
        {
            var linkLabel = this.Create<LinkLabel>("linkLabel");
            linkLabel.LinkClicked += (sender, args) => linkLabel.LinkVisited = true;
        }
    }
}
