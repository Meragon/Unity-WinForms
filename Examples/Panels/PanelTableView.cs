using System.Drawing;

namespace UnityWinForms.Examples.Panels
{
    using System.Windows.Forms;
    
    public class PanelTableView : BaseExamplePanel
    {
        public override void Initialize()
        {
            // Empty table.
            this.Create<TableView>();
            
            var table = this.Create<TableView>(8, 158);
            table.Size = new Size(360, 240);
            table.Columns.Add("column1", "column1");
            table.Columns.Add("column2", "column2");
            table.Columns.Add("column3", "column3");
            table.Columns.Add("column4", "column4");
            table.Columns.Add("column5", "column5");

            for (int i = 0; i < 20; i++)
                table.Rows.Add();

            var combo = new ComboBox();
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.Items.AddRange(new object[] { "1", "2", "3" });
            
            var label = new Label();
            label.Text = "label1";
            
            var buttonHideRowsButtons = new Button();
            buttonHideRowsButtons.Text = "Hide rows buttons";
            buttonHideRowsButtons.Click += (sender, args) =>
            {
                // TODO: don't know why, but columns headers becomes hidden aswell.
                table.HideRowHeaders();
                table.Refresh();
            };
            
            // Set a new control for table cell.
            table.Rows[1].ItemsControls[0] = combo;
            table.Rows[1].ItemsControls[1] = label;
            table.Rows[2].ItemsControls[0] = buttonHideRowsButtons;

            table.Columns[0].Width = 128;
            table.Refresh();
        }
    }
}