namespace UnityWinForms.Examples.Panels
{
    using System.Windows.Forms;

    public class PanelFileDialog : BaseExamplePanel
    {
        public override void Initialize()
        {
            var labelFilter = this.Create<Label>("Filter:");
            var textFilter = this.Create<TextBox>();
            textFilter.Text = "All files|*.*";
            textFilter.Width = 320;
            
            var labelPath = this.Create<Label>("Selected path");
            var buttonOpen = this.Create<Button>("Open");
            var buttonSave = this.Create<Button>("Save");

            buttonOpen.Click += (sender, args) =>
            {
                var ofd = new OpenFileDialog();
                ofd.Filter = textFilter.Text;
                ofd.InitialDirectory = "C:\\";
                ofd.ShowDialog(
                    (form, result) =>
                    {
                        if (result == DialogResult.OK)
                            labelPath.Text = ofd.FileName;
                    });
            };
            buttonSave.Click += (sender, args) =>
            {
                var sfd = new SaveFileDialog();
                sfd.Filter = textFilter.Text;
                sfd.ShowDialog(
                    (form, result) =>
                    {
                        if (result == DialogResult.OK)
                            labelPath.Text = sfd.FileName;
                    });
            };
        }
    }
}
