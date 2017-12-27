namespace UnityWinForms.Examples.Panels
{
    using System.Drawing;
    using System.Windows.Forms;

    public class PanelMenuStrip : BaseExamplePanel
    {
        public override void Initialize()
        {
            var itemFile_New = new ToolStripMenuItem("New");
            var itemFile_Open = new ToolStripMenuItem("Open");
            var itemFile_Save = new ToolStripMenuItem("Save");
            var itemFile_Exit = new ToolStripMenuItem("Exit");

            itemFile_New.ShortcutKeys = Keys.Control | Keys.N;
            itemFile_Save.ShortcutKeys = Keys.Control | Keys.S;
            itemFile_Exit.ShortcutKeys = Keys.Control | Keys.W;

            itemFile_Open.DropDownItems.Add(new ToolStripMenuItem("file1.txt"));
            itemFile_Open.DropDownItems.Add(new ToolStripMenuItem("file2.txt"));
            itemFile_Open.DropDownItems.Add(new ToolStripMenuItem("file3.txt"));
            itemFile_Open.DropDownItems.Add(new ToolStripSeparator());
            itemFile_Open.DropDownItems.Add(new ToolStripMenuItem("last"));

            itemFile_Exit.Image = uwfAppOwner.Resources.Close;
            itemFile_Exit.uwfImageColor = Color.FromArgb(64, 64, 64);

            var itemEdit_Undo = new ToolStripMenuItem("Undo");
            var itemEdit_Redo = new ToolStripMenuItem("Redo");

            itemEdit_Undo.ShortcutKeys = Keys.Control | Keys.Z;
            itemEdit_Redo.ShortcutKeys = Keys.Control | Keys.Y;

            var itemFile = new ToolStripMenuItem("File");
            var itemEdit = new ToolStripMenuItem("Edit");
            var itemView = new ToolStripMenuItem("View");

            itemFile.DropDownItems.Add(itemFile_New);
            itemFile.DropDownItems.Add(itemFile_Open);
            itemFile.DropDownItems.Add(itemFile_Save);
            itemFile.DropDownItems.Add(new ToolStripSeparator());
            itemFile.DropDownItems.Add(itemFile_Exit);

            itemEdit.DropDownItems.Add(itemEdit_Undo);
            itemEdit.DropDownItems.Add(itemEdit_Redo);

            itemView.DropDownItems.Add(new ToolStripMenuItem("nothing"));
            
            var menu = new MenuStrip();
            menu.Items.Add(itemFile);
            menu.Items.Add(itemEdit);
            menu.Items.Add(itemView);

            Controls.Add(menu);
        }
    }
}
