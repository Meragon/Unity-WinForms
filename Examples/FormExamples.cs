namespace UnityWinForms.Examples
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Examples.Panels;

    public sealed class FormExamples : Form
    {
        private readonly TreeView treeView;

        private BaseExamplePanel currentPanel;

        public FormExamples()
        {
            // NOTE: most properties and methods that starts with 'uwf' are
            // internal and not supported by original Windows Forms.

            BackColor = Color.FromArgb(239, 235, 233);
            MinimumSize = new Size(320, 240);
            Text = "Unity-WinForms";
            Size = new Size(640, 480);
            SizeGripStyle = SizeGripStyle.Show;

            treeView = new TreeView();
            treeView.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
            treeView.BorderColor = uwfBorderColor;
            treeView.Location = new Point(0, uwfHeaderHeight - 1); // All controls should be placed with Form header offset.
            treeView.Height = Height - uwfHeaderHeight + 1;
            treeView.Width = 220;
            treeView.NodeMouseClick += TreeViewOnNodeMouseClick;

            Controls.Add(treeView);

            var labelNote = new Label();
            labelNote.Font = new Font("Arial", 14, FontStyle.Bold);
            labelNote.Location = new Point(treeView.Location.X + treeView.Width + 16, treeView.Location.Y);
            labelNote.Text = "This is not completed overview of UWF controls. \r\nWork in progress.";
            
            Controls.Add(labelNote);

            var nodeButton = new TreeNode("Button");
            nodeButton.Tag = typeof(PanelButton);

            var nodeCheckBox = new TreeNode("CheckBox");
            nodeCheckBox.Tag = typeof(PanelCheckBox);

            var nodeComboBox = new TreeNode("ComboBox");
            nodeComboBox.Tag = typeof(PanelComboBox);

            var nodeDateTimePicker = new TreeNode("DateTimePicker");
            nodeDateTimePicker.Tag = typeof(PanelDateTimePicker);

            var nodeFileDialog = new TreeNode("FileDialog");
            nodeFileDialog.Tag = typeof(PanelFileDialog);

            var nodeLabel = new TreeNode("Label");
            nodeLabel.Tag = typeof(PanelLabel);

            var nodeLinkLabel = new TreeNode("LinkLabel");
            nodeLinkLabel.Tag = typeof(PanelLinkLabel);

            var nodeMenuStrip = new TreeNode("MenuStrip");
            nodeMenuStrip.Tag = typeof(PanelMenuStrip);

            var nodeMonthCalendar = new TreeNode("MonthCalendar");
            nodeMonthCalendar.Tag = typeof(PanelMonthCalendar);

            var nodeNumericUpDown = new TreeNode("NumericUpDown");
            nodeNumericUpDown.Tag = typeof(PanelNumericUpDown);

            var nodePictureBox = new TreeNode("PictureBox");
            nodePictureBox.Tag = typeof(PanelPictureBox);

            var nodeProgressBar = new TreeNode("ProgressBar");
            nodeProgressBar.Tag = typeof(PanelProgressBar);

            var nodeScrollBar = new TreeNode("ScrollBar");
            nodeScrollBar.Tag = typeof(PanelScrollBar);

            var nodeControls = new TreeNode("Controls");
            nodeControls.Nodes.Add(nodeButton);
            nodeControls.Nodes.Add(nodeCheckBox);
            nodeControls.Nodes.Add(nodeComboBox);
            nodeControls.Nodes.Add(nodeDateTimePicker);
            nodeControls.Nodes.Add(nodeFileDialog);
            nodeControls.Nodes.Add(nodeLabel);
            nodeControls.Nodes.Add(nodeLinkLabel);
            nodeControls.Nodes.Add(nodeMenuStrip);
            nodeControls.Nodes.Add(nodeMonthCalendar);
            nodeControls.Nodes.Add(nodeNumericUpDown);
            nodeControls.Nodes.Add(nodePictureBox);
            nodeControls.Nodes.Add(nodeProgressBar);
            nodeControls.Nodes.Add(nodeScrollBar);

            treeView.Nodes.Add(nodeControls);

            nodeControls.Expand();

            // Refresh method or ExpandAll will update view list. 
            // NOTE: most controls don't need to be refreshed. Make sure to take a look 
            // at Refresh implementation in Control that you think is not working.
            treeView.Refresh();

            // Grip renderer is normal control. Bring it to front if you use it over other controls that can technicaly hide it.
            // Unfortunately it will not dock with scrollbars.
            uwfSizeGripRenderer.BringToFront();
        }

        private void TreeViewOnNodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button != MouseButtons.Left || e.Node == null || e.Node.Tag == null)
                return;

            var panelType = e.Node.Tag as Type;
            if (panelType == null)
                return;

            var panel = Activator.CreateInstance(panelType) as BaseExamplePanel;
            if (panel == null)
                return;

            if (currentPanel != null && currentPanel.IsDisposed == false)
                currentPanel.Dispose();

            currentPanel = panel;
            currentPanel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            currentPanel.Location = new Point(treeView.Location.X + treeView.Width, uwfHeaderHeight);
            currentPanel.Height = Height - uwfHeaderHeight;
            currentPanel.Width = Width - treeView.Width;

            Controls.Add(currentPanel);

            currentPanel.Initialize();
            currentPanel.UpdateScroll();

            uwfSizeGripRenderer.BringToFront();
        }
    }
}
