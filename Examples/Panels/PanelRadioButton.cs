namespace UnityWinForms.Examples.Panels
{
    using System.Windows.Forms;
    
    public class PanelRadioButton : BaseExamplePanel
    {
        public override void Initialize()
        {
            var radio = this.Create<RadioButton>("AutoCheck = false");
            radio.AutoCheck = false;
            radio.Width = 200;
            
            var groupBox = this.Create<GroupBox>();

            groupBox.Create<RadioButton>("First");
            groupBox.Create<RadioButton>("Second");
            groupBox.Create<RadioButton>("Third");
        }
    }
}