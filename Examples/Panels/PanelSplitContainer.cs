namespace UnityWinForms.Examples.Panels
{
    using System.Drawing;
    using System.Windows.Forms;
    
    public class PanelSplitContainer : BaseExamplePanel
    {
        public override void Initialize()
        {
            var checkOrientation = this.Create<CheckBox>("Vertical");
            checkOrientation.Checked = true;
            
            var container = this.Create<SplitContainer>();
            container.Size = new Size(260, 200);
            container.BorderStyle = BorderStyle.Fixed3D;
            container.Panel1.BackColor = Color.CornflowerBlue;
            container.Panel2.BackColor = Color.LightCoral;
            container.Panel1.Create<Label>("Panel1");
            container.Panel2.Create<Label>("Panel2");
            
            checkOrientation.CheckedChanged += (sender, args) =>
            {
                if (checkOrientation.Checked)
                    container.Orientation = Orientation.Vertical;
                else
                    container.Orientation = Orientation.Horizontal;
            };
        }
    }
}