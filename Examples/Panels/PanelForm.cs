namespace UnityWinForms.Examples.Panels
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    
    public class PanelForm : BaseExamplePanel
    {
        public override void Initialize()
        {
            var buttonForm = this.Create<Button>("Form");
            buttonForm.Click += (sender, args) =>
            {
                var form = new Form();
                form.Text = "Form";
                form.Show();
            };

            var buttonDialog = this.Create<Button>("Dialog");
            buttonDialog.Click += OnButtonDialogOnClick;

            var buttonMdi = this.Create<Button>("Mdi");
            buttonMdi.Click += OnButtonMdiOnClick;
        }
        
        private void OnButtonDialogOnClick(object sender, EventArgs args)
        {
            var form = new Form();
            form.Text = "Dialog";
            form.ShowDialog();
            var buttonDialog = form.Create<Button>("Dialog");
            buttonDialog.Click += OnButtonDialogOnClick;
        }
        private void OnButtonMdiOnClick(object sender, EventArgs args)
        {
            var form = new Form();
            form.IsMdiContainer = true;
            form.Text = "Mdi Form";
            form.Size = new Size(640, 480);
            form.SizeGripStyle = SizeGripStyle.Show;
            form.Show();

            var formTree = new Form();
            formTree.BackColor = Color.White;
            formTree.Text = "Inner Form";
            formTree.MdiParent = form;

            var tree = new TreeView();
            tree.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top;
            tree.BackColor = Color.White;
            tree.BorderStyle = BorderStyle.None;
            tree.Location = new Point(0, formTree.uwfHeaderHeight);
            tree.Size = new Size(formTree.Width, formTree.Height - formTree.uwfHeaderHeight - 16);

            for (int i = 0; i < 50; i++)
                tree.Nodes.Add("test node " + i);

            tree.Refresh();

            formTree.Controls.Add(tree);
            formTree.SizeGripStyle = SizeGripStyle.Show;
            formTree.Show();
            
            var formColor = new ColorPickerForm();
            formColor.MdiParent = form;
            formColor.Color = tree.BackColor;
            formColor.Show();
            formColor.ColorChanged += (o, eventArgs) => tree.BackColor = formColor.Color;
        }
    }
}