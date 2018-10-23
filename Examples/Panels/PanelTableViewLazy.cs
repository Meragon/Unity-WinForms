namespace UnityWinForms.Examples.Panels
{
    using System.Diagnostics;
    using System.Windows.Forms;
    
    public class PanelTableViewLazy : BaseExamplePanel
    {
        public override void Initialize()
        {
            var watch = Stopwatch.StartNew();
            var label = this.Create<Label>();
            var table = this.Create<TableView>();
            table.Width = 480;
            table.Height = 320;
            
            // This will prevent allocating default cell controls(NumericUpDown).
            table.SkipControlsInitializations = true;
                
            // Max reasonable can be 1024x1024, but amount of columns and rows controls will slow it down.  
            const int ROWS_COUNT = 512;
            const int COLUMN_COUNT = 512;

            // Add columns.
            for (int i = 0; i < COLUMN_COUNT; i++)
            {
                var columnName = i.ToString();
                
                table.Columns.Add(columnName, columnName);
            }
            
            // Add rows.
            for (int i = 0; i < ROWS_COUNT; i++)
            {
                table.Rows.Add();
                table.Rows[i].ItemsControls.CreateControlOnCellVisible = (columnIndex, rowIndex) => new TextBox()
                {
                    Text = "C: " + columnIndex + ", R: " + rowIndex,
                    TextAlign = HorizontalAlignment.Center
                };
            }
            
            table.Refresh();
            table.TryInitializeDefferedControls();
            
            watch.Stop();
            label.Text = "Table[" + COLUMN_COUNT + ", " + ROWS_COUNT + "] was created in " + watch.ElapsedMilliseconds + " ms.";
        }
    }
}