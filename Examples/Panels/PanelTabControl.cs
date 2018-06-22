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

            var button1 = new Button();
            button1.Location = new Point(8, 8);
            button1.Text = "button1";
            
            var button2 = new Button();
            button2.Location = new Point(480, 360);
            button2.Text = "button2";
            
            var page1 = new TabPage("First");
            page1.AutoScroll = true;
            page1.Controls.Add(button1);
            page1.Controls.Add(button2);
            
            var label = new Label();
            label.Location = new Point(8, 8);
            label.Text = "label1";
            
            var page2 = new TabPage("Second");
            page2.Controls.Add(label);
            
            var tabControl = this.Create<TabControl>(8, 116);
            tabControl.BackColor = Color.Transparent;
            tabControl.Padding = new Padding(0);
            tabControl.TabPages.Add(page1);
            tabControl.TabPages.Add(page2);
            tabControl.TabPages.Add("Third");
            tabControl.TabPages.Add("Forth");
            tabControl.TabPages.Add("Fifth");
            
            // Autosize is not working for page headers.
            tabControl.SetPageButtonWidth(0, 48);
        }
    }
}