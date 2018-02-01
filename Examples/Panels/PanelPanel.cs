namespace UnityWinForms.Examples.Panels
{
    using System;
    using System.Linq;
    using System.Windows.Forms;

    public class PanelPanel : BaseExamplePanel
    {
        public override void Initialize()
        {
            var panel = this.Create<Panel>();
            panel.BorderStyle = BorderStyle.Fixed3D;

            for (int i = 0; i < 20; i++)
            {
                panel.Create<Label>("label" + i);
                panel.Create<Button>("button" + i, true, 32);
            }

            var checkBoxAutoscroll = this.Create<CheckBox>("AutoScroll", false, 8, panel.Height + 8);
            checkBoxAutoscroll.CheckedChanged += (sender, args) => { panel.AutoScroll = checkBoxAutoscroll.Checked; };

            var borderStyles = Enum.GetNames(typeof(BorderStyle)).Cast<object>().ToArray();

            this.Create<Label>("BorderStyle:", false, 8, lineOffset);
            var comboBorder = this.Create<ComboBox>();
            comboBorder.Items.AddRange(borderStyles);
            comboBorder.SelectedIndex = 2;
            comboBorder.SelectedIndexChanged += (sender, args) =>
                {
                    var selectedItem = comboBorder.SelectedItem.ToString();
                    panel.BorderStyle = (BorderStyle)Enum.Parse(typeof(BorderStyle), selectedItem);
                };
        }
    }
}
