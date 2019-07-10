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
            treeView.Location = new Point(0, uwfHeaderHeight - 1); // All controls should be placed with Form header offset.
            treeView.Height = Height - uwfHeaderHeight + 1;
            treeView.TabStop = false;
            treeView.Width = 220;
            treeView.NodeMouseClick += TreeViewOnNodeMouseClick;

            Controls.Add(treeView);

            var labelNote = new Label();
            labelNote.Font = new Font("Arial", 14, FontStyle.Bold);
            labelNote.Location = new Point(treeView.Location.X + treeView.Width + 16, treeView.Location.Y);
            labelNote.Text = 
                "This is not completed overview of the UWF controls.\r\n" + 
                "Work in progress.\r\n\r\n" +
                "Do not forget that you can still modify controls\r\n" + 
                "with 'SWF Inspector' which is located in drop down\r\n" + 
                "menu 'Window' -> 'UnityWinForms'.";
            
            Controls.Add(labelNote);

            var nodeControls = new TreeNode("Controls");
            AddNode(nodeControls, "Button", typeof(PanelButton));
            AddNode(nodeControls, "CheckBox", typeof(PanelCheckBox));
            AddNode(nodeControls, "ComboBox", typeof(PanelComboBox));
            AddNode(nodeControls, "DateTimePicker", typeof(PanelDateTimePicker));
            AddNode(nodeControls, "FileDialog", typeof(PanelFileDialog));
            AddNode(nodeControls, "FontDialog", typeof(PanelFontDialog));
            AddNode(nodeControls, "Form", typeof(PanelForm));
            AddNode(nodeControls, "GroupBox", typeof(PanelGroupBox));
            AddNode(nodeControls, "Label", typeof(PanelLabel));
            AddNode(nodeControls, "LinkLabel", typeof(PanelLinkLabel));
            AddNode(nodeControls, "MenuStrip", typeof(PanelMenuStrip));
            AddNode(nodeControls, "MonthCalendar", typeof(PanelMonthCalendar));
            AddNode(nodeControls, "NumericUpDown", typeof(PanelNumericUpDown));
            AddNode(nodeControls, "Panel", typeof(PanelPanel));
            AddNode(nodeControls, "PictureBox", typeof(PanelPictureBox));
            AddNode(nodeControls, "ProgressBar", typeof(PanelProgressBar));
            AddNode(nodeControls, "RadioButton", typeof(PanelRadioButton));
            AddNode(nodeControls, "ScrollBar", typeof(PanelScrollBar));
            AddNode(nodeControls, "SplitContainer", typeof(PanelSplitContainer));
            AddNode(nodeControls, "TabControl", typeof(PanelTabControl));
            AddNode(nodeControls, "TrackBar", typeof(PanelTrackBar));
            AddNode(nodeControls, "TreeView", typeof(PanelTreeView));
            
            treeView.Nodes.Add(nodeControls);

            var nodeOtherControls = new TreeNode("Other");
            AddNode(nodeOtherControls, "Chart", typeof(PanelChart));
            AddNode(nodeOtherControls, "ColorPicker", typeof(PanelColorPicker));
            AddNode(nodeOtherControls, "TableView", typeof(PanelTableView));
            AddNode(nodeOtherControls, "TableViewLazy", typeof(PanelTableViewLazy));
            
            treeView.Nodes.Add(nodeOtherControls);

            // Refresh method or ExpandAll will update view list. 
            // NOTE: most controls don't need to be refreshed. Make sure to take a look 
            // at Refresh implementation in Control that you think is not working.
            treeView.ExpandAll();

            // Grip renderer is normal control. Bring it to front if you use it over other controls that can technically hide it.
            // uwfSizeGripRenderer.BringToFront();
        }

        public void SetPanel(BaseExamplePanel panel)
        {
            if (currentPanel != null && !currentPanel.IsDisposed)
                currentPanel.Dispose();

            currentPanel = panel;
            currentPanel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            currentPanel.Location = new Point(treeView.Location.X + treeView.Width, uwfHeaderHeight);
            currentPanel.Height = Height - uwfHeaderHeight - 16; // We don't want to hide SizeGripRenderer with scrollbars.
            currentPanel.Width = Width - treeView.Width;

            Controls.Add(currentPanel);

            currentPanel.Initialize();
        }
        
        private static void AddNode(TreeNode parent, string text, object tag)
        {
            var node = new TreeNode(text);
            node.Tag = tag;
            
            parent.Nodes.Add(node);
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

            SetPanel(panel);
        }
    }
}
