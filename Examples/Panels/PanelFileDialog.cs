namespace UnityWinForms.Examples.Panels
{
    using System.Windows.Forms;

    public class PanelFileDialog : BaseExamplePanel
    {
        public override void Initialize()
        {
            var labelPath = this.Create<Label>("path");
            var buttonOpen = this.Create<Button>("Open");
            var buttonSave = this.Create<Button>("Save");

            buttonOpen.Click += (sender, args) =>
            {
                var ofd = new OpenFileDialog();
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
