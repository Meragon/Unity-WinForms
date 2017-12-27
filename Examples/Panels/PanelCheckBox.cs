namespace UnityWinForms.Examples.Panels
{
    using System.Windows.Forms;

    public class PanelCheckBox : BaseExamplePanel
    {
        public override void Initialize()
        {
            this.Create<Label>("Normal:");
            this.Create<CheckBox>("Text");
        }
    }
}
