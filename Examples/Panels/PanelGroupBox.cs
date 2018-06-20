namespace UnityWinForms.Examples.Panels
{
    using System.Windows.Forms;
    
    public class PanelGroupBox : BaseExamplePanel
    {
        public override void Initialize()
        {
            this.Create<Label>("Without text");
            this.Create<GroupBox>("", true);
            
            this.Create<Label>("With text", false, 8, 112);
            this.Create<GroupBox>("GroupBox", true);
        }
    }
}