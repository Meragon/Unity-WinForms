namespace UnityWinForms.Examples.Panels
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public class PanelLabel : BaseExamplePanel
    {
        public override void Initialize()
        {
            // AutoSize.
            this.Create<Label>("AutoSize:");
            var textboxAutoSize = this.Create<TextBox>("write here");
            var labelAutosize = this.Create<Label>("", true);

            textboxAutoSize.Height = 20;
            textboxAutoSize.TextChanged += (sender, args) => labelAutosize.Text = textboxAutoSize.Text;
            labelAutosize.Text = textboxAutoSize.Text;
            labelAutosize.BorderStyle = BorderStyle.FixedSingle;

            // BackColor.
            this.Create<Label>("Background color:", false, 8, lineOffset);
            var colorPickerBack = this.Create<ColorPicker>();
            var labelBack = this.Create<Label>("label", true);

            colorPickerBack.Color = Color.CornflowerBlue;
            colorPickerBack.ColorChanged += (sender, args) => labelBack.BackColor = colorPickerBack.Color;
            labelBack.BackColor = colorPickerBack.Color;

            // Border style.
            var borderStyles = Enum.GetNames(typeof(BorderStyle));

            this.Create<Label>("Border style:", false, 8, lineOffset);
            var comboBorderStyle = this.Create<ComboBox>();
            var labelBorderStyle = this.Create<Label>("label", true);

            comboBorderStyle.Items.AddRange(borderStyles);
            comboBorderStyle.SelectedIndexChanged += (sender, args) =>
            {
                var selectedItem = comboBorderStyle.SelectedItem.ToString();
                labelBorderStyle.BorderStyle = (BorderStyle)Enum.Parse(typeof(BorderStyle), selectedItem);
            };

            // Text align.
            var contentAlign = Enum.GetNames(typeof(ContentAlignment));

            this.Create<Label>("Text align:", false, 8, lineOffset);
            var comboTextAlign = this.Create<ComboBox>();
            var labelTextAlign = this.Create<Label>("label", true);

            comboTextAlign.Items.AddRange(contentAlign);
            comboTextAlign.SelectedIndex = 0;
            comboTextAlign.SelectedIndexChanged += (sender, args) =>
            {
                var selectedItem = comboTextAlign.SelectedItem.ToString();
                labelTextAlign.TextAlign = (ContentAlignment)Enum.Parse(typeof(ContentAlignment), selectedItem);
            };
            labelTextAlign.AutoSize = false;
            labelTextAlign.BorderStyle = BorderStyle.Fixed3D;
            labelTextAlign.Height += 8;
            labelTextAlign.Width += 96;

            // Unity Markup format.
            this.Create<Label>("Unity Markup format:", false, 8, lineOffset + 8);
            this.Create<Label>("<color=#FF7043><b>Unity</b></color> <i>Markup <color=#26A69A>format</color></i>");
        }
    }
}
