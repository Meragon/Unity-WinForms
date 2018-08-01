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

            var r1 = groupBox.Create<RadioButton>("First");
            var r2 = groupBox.Create<RadioButton>("Second");
            var r3 = groupBox.Create<RadioButton>("Third");

            // hack: to make sure tabulation will work fine.
            r1.TabIndex = 1;
            r2.TabIndex = 2;
            r3.TabIndex = 3;
        }
    }
}