namespace UnityWinForms.Examples.Panels
{
    using System;
    using System.Windows.Forms;

    public class PanelPictureBox : BaseExamplePanel
    {
        public override void Initialize()
        {
            var borderStyles = Enum.GetNames(typeof(BorderStyle));

            var pictureBox = this.Create<PictureBox>();
            this.Create<Label>("Border style:", false, 8, 64);
            var comboBorder = this.Create<ComboBox>();

            comboBorder.Items.AddRange(borderStyles);
            comboBorder.SelectedIndex = 0;
            comboBorder.SelectedIndexChanged += (sender, args) =>
            {
                var selectedItem = comboBorder.SelectedItem.ToString();
                pictureBox.BorderStyle = (BorderStyle)Enum.Parse(typeof(BorderStyle), selectedItem);
            };

            pictureBox.BackgroundImage = uwfAppOwner.Resources.ArrowUp;
            pictureBox.BackgroundImageLayout = ImageLayout.Tile;
            pictureBox.Image = uwfAppOwner.Resources.DateTimePicker;
        }
    }
}
