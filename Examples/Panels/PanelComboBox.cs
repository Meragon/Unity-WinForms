namespace UnityWinForms.Examples.Panels
{
    using System.Windows.Forms;

    public class PanelComboBox : BaseExamplePanel
    {
        public override void Initialize()
        {
            var items = new object[]
                        {
                            "hello",
                            "world",
                            100, 
                            110,
                            120,
                            130,
                            140,
                            150,
                            160,
                            170,
                            180,
                            190
                        };


            // Normal.
            this.Create<Label>("Normal: ");
            var comboNormal = this.Create<ComboBox>();
            comboNormal.Items.AddRange(items);


            // Style: list.
            this.Create<Label>("DropDownStyle List:", false, 8, lineOffset);
            var comboList = this.Create<ComboBox>();
            comboList.Items.AddRange(items);
            comboList.DropDownStyle = ComboBoxStyle.DropDownList;

            // ItemHeight.
            this.Create<Label>("Item height:", false, 8, lineOffset);
            var numericItemHeight = this.Create<NumericUpDown>();
            var comboItemHeight = this.Create<ComboBox>("", true);

            numericItemHeight.Value = 24;
            numericItemHeight.ValueChanged += (sender, args) => comboItemHeight.ItemHeight = (int)numericItemHeight.Value;

            comboItemHeight.Items.AddRange(items);
            comboItemHeight.ItemHeight = (int)numericItemHeight.Value;
        }
    }
}
