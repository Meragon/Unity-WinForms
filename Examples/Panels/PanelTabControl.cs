namespace UnityWinForms.Examples.Panels
{
    using System.Drawing;
    using System.Windows.Forms;

    public class PanelTabControl : BaseExamplePanel
    {
        public override void Initialize()
        {
            this.Create<Label>("Empty");
            this.Create<TabControl>("", true);

            var tabControl = this.Create<TabControl>(8, 116);
            tabControl.BackColor = Color.Transparent;
            tabControl.Padding = new Padding(0);
            
            tabControl.TabPages.Add("First");
            tabControl.TabPages.Add("Second");
            tabControl.TabPages.Add("Third");
            tabControl.TabPages.Add("Forth");
            tabControl.TabPages.Add("Fifth");
            
            // Autosize is not working for page headers.
            tabControl.SetPageButtonWidth(0, 64);
            tabControl.SetPageButtonWidth(1, 64);
            tabControl.SetPageButtonWidth(2, 64);
            tabControl.SetPageButtonWidth(3, 64);
            tabControl.SetPageButtonWidth(4, 64);
            
            // TODO: remove this.
            tabControl.CheckNavButtons();
        }
    }
}