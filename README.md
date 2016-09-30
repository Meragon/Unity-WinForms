# Unity Windows Forms

This is WinForms wrapper (including System.Drawing). Here you can create base controls and make your own custom controls. The reason why I made it? Because now I can quickly create GUI through the code. But, yeah, right now there is a poor implementation of interaction with the editor.

### Base Controls atm
- Label;
- Button;
- TextBox;
- CheckBox;
- ListBox;
- ComboBox;
- NumericUpDown
- TreeView;
- PictureBox;
- FileDialog;
- Form;
- ToolStrip & MenuStrip;
- \* TabControl;
- GroupBox (still no Panel);
- ColorPicker;
- \* Accordion;
- ToolTip;
- MonthCalendar;

\* - not really good.

### Usage

1. Attach ApplicationBehaviour script to GameObject;
2. Add Arial font to resources;
3. Add other fonts and icons; 
4. Use keyword 'new' to create new Controls (don't need to use Application.Run); 

### Screenshots
![scr1](http://i.imgur.com/LCQsFgv.png)
![scr1](http://i.imgur.com/njQZbCP.png)
![scr1](http://i.imgur.com/I9H0AWt.png)
![scr1](http://i.imgur.com/nZUFZCe.png)
![scr1](http://i.imgur.com/GpiWviP.png)

Form code from above
```sh
public FormTest()
        {
            Text = "Unity WinForms";

            Label label1 = new Label();
            label1.Location = new Point(16, 32);
            label1.Font = new System.Drawing.Font("Segoe UI", 12);
            label1.Text = "Label";
            Controls.Add(label1);

            Button button1 = new Button();
            button1.Location = new Point(label1.Location.X, label1.Location.Y + label1.Height + 4);
            button1.Size = new System.Drawing.Size(128, 24);
            button1.Text = "Button";
            Controls.Add(button1);

            CheckBox checkBox1 = new CheckBox();
            checkBox1.Checked = true;
            checkBox1.Location = new Point(button1.Location.X, button1.Location.Y + button1.Height + 4);
            checkBox1.Text = "CheckBox";
            Controls.Add(checkBox1);

            ListBox listBox1 = new ListBox();
            listBox1.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            listBox1.Location = new Point(checkBox1.Location.X, checkBox1.Location.Y + checkBox1.Height + 4);
            listBox1.ItemHeight = 22;
            listBox1.Items.AddRange(new object[] { 1, 2, 3, 10, 20, "hello world", this.ToString() });
            listBox1.Size = new System.Drawing.Size(Width - 32 - 96, listBox1.ItemHeight * 5 - 1);
            Controls.Add(listBox1);

            ComboBox comboBox1 = new ComboBox();
            comboBox1.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.Location = new Point(listBox1.Location.X + listBox1.Width + 4, listBox1.Location.Y);
            comboBox1.Items.AddRange(new object[] { "c1", 2, "c3" });
            comboBox1.Size = new System.Drawing.Size(92, comboBox1.Height);
            Controls.Add(comboBox1);

            NumericUpDown numeric1 = new NumericUpDown();
            numeric1.Location = new Point(listBox1.Location.X, listBox1.Location.Y + listBox1.Height + 4);
            numeric1.Maximum = 1000;
            numeric1.Minimum = 10;
            numeric1.Value = 20;
            Controls.Add(numeric1);

            TreeNode homeNode = new TreeNode("home");
            homeNode.ImageIndex = 0;
            for (int i = 0; i < 100; i++)
            {
                TreeNode indexedNode = new TreeNode(i.ToString() + "_node");
                indexedNode.ImageIndex = 1;
                homeNode.Nodes.Add(indexedNode);
            }

            TreeView treeView1 = new TreeView();
            treeView1.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
            treeView1.Location = new Point(numeric1.Location.X + numeric1.Width + 4, numeric1.Location.Y);
            treeView1.ImageList.Images.Add(new Bitmap(Application.Resources.Reserved.ArrowDown));
            treeView1.ImageList.Images.Add(new Bitmap(Application.Resources.Reserved.ArrowRight));
            treeView1.Nodes.Add(homeNode);
            treeView1.Size = new System.Drawing.Size(Width - 32 - 4 - numeric1.Width, 96);
            Controls.Add(treeView1);

            treeView1.ExpandAll();

            PictureBox pBox = new PictureBox();
            pBox.Location = new Point(numeric1.Location.X, numeric1.Location.Y + numeric1.Height + 4);
            pBox.Image = Application.Resources.Reserved.ComboBoxArrow;
            pBox.ImageBackColor = Color.White;
            pBox.ImageBorderColor = Color.Gray;
            pBox.Size = new Size(32, 32);
            Controls.Add(pBox);

             var cPicker = new ColorPicker();
             cPicker.Location = new Point(pBox.Location.X, pBox.Location.Y + pBox.Height + 4);
             cPicker.Size = numeric1.Size;
             Controls.Add(cPicker);
        }
```