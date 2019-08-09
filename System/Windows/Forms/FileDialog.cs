#if UNITY_STANDALONE || UNITY_ANDROID
#define IO_SUPPORTED
#endif

namespace System.Windows.Forms
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;

    public abstract class FileDialog : Form
    {
        internal static Size savedFormSize = new Size(720, 400);

        internal FileRenderer fileRenderer;
        internal Button       buttonOk;
        internal Button       buttonCancel;
        internal Button       buttonBack;
        internal Button       buttonUp;
        internal Button       buttonRefresh;
        internal ComboBox     comboFilter;
        internal Label        labelFilename;
        internal TextBox      textBoxPath;
        internal TextBox      textBoxFilename;           
        
        private readonly bool handleFormSize;
        private readonly Pen splitterPen = new Pen(Color.FromArgb(232, 232, 232));
        private string initialDir;
        private string filename;
        
        internal FileDialog()
        {
#if !IO_SUPPORTED
            throw new NotSupportedException();
#endif

            BackColor = Color.White;
            Filter = "All files|*.*";
            MinimumSize = new Drawing.Size(240, 240);
            KeyPreview = true;
            Padding = new Padding(12, 12, 12, 12);
            SizeGripStyle = SizeGripStyle.Show;
            Text = "File Dialog";

            handleFormSize = false;
            Size = savedFormSize;
            handleFormSize = true;

            // Button Back.
            buttonBack = new Button();
            buttonBack.BackgroundImageLayout = ImageLayout.Center;
            buttonBack.Enabled = false;
            buttonBack.Font = new Drawing.Font("Arial", 16, FontStyle.Bold);
            buttonBack.Image = ApplicationResources.Images.FileDialogBack;
            buttonBack.Location = new Point(Padding.Left, uwfHeaderHeight + Padding.Top);
            buttonBack.BackColor = Color.Transparent;
            buttonBack.uwfBorderColor = Color.Transparent;
            buttonBack.uwfBorderDisableColor = Color.Transparent;
            buttonBack.uwfImageColor = Color.Gray;
            buttonBack.uwfImageHoverColor = buttonBack.uwfImageColor;
            buttonBack.uwfImageDisabledColor = Color.FromArgb(207, 207, 207);
            buttonBack.Size = new Size(22, 22);
            if (buttonBack.Image == null) buttonBack.Text = "◀";
            buttonBack.Click += (sender, args) => ButtonBack();
            Controls.Add(buttonBack);

            var buttonBackTooltip = new ToolTip();
            buttonBackTooltip.SetToolTip(buttonBack, "Back (ALT + Left Arrow)");
            
            // Button Up.
            buttonUp = new Button();
            buttonUp.BackgroundImageLayout = ImageLayout.Center;
            buttonUp.Font = new Drawing.Font("Arial", 16, FontStyle.Bold);
            buttonUp.Image = ApplicationResources.Images.FileDialogUp;
            buttonUp.Location = new Point(buttonBack.Location.X + buttonBack.Width + 8, buttonBack.Location.Y);
            buttonUp.BackColor = Color.Transparent;
            buttonUp.uwfBorderColor = Color.Transparent;
            buttonUp.Size = new Drawing.Size(22, 22);
            if (buttonUp.Image == null) buttonUp.Text = "▲";
            buttonUp.Click += (sender, args) => ButtonUp();
            Controls.Add(buttonUp);

            var buttonUpTooltip = new ToolTip();
            buttonUpTooltip.SetToolTip(buttonUp, "Up (ALT + Up Arrow)");
            
            // Button Refresh.
            buttonRefresh = new Button();
            buttonRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonRefresh.BackColor = Color.Transparent;
            buttonRefresh.Image = ApplicationResources.Images.FileDialogRefresh;
            buttonRefresh.Size = new Size(22, 22);
            buttonRefresh.Location = new Point(Width - Padding.Right - buttonRefresh.Width, buttonUp.Location.Y);
            //buttonRefresh.uwfImageColor = Color.FromArgb(64, 64, 64);
            //buttonRefresh.uwfImageHoverColor = buttonRefresh.uwfImageColor;
            //buttonRefresh.uwfBorderColor = Color.Transparent;
            
            buttonRefresh.Click += (sender, args) => ButtonRefresh();
            if (buttonRefresh.Image == null) buttonRefresh.Text = "R";
            Controls.Add(buttonRefresh);

            var buttonRefreshTooltip = new ToolTip();
            buttonRefreshTooltip.SetToolTip(buttonRefresh, "Refresh (F5)");
            
            // Textbox Path.
            textBoxPath = new PathTextBox(this);
            textBoxPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxPath.Font = new Drawing.Font("Arial", 12);
            textBoxPath.Location = new Point(buttonUp.Location.X + buttonUp.Width + 8, buttonUp.Location.Y);
            textBoxPath.Size = new Drawing.Size(Width - textBoxPath.Location.X - Padding.Right - buttonRefresh.Width + 1, buttonBack.Height);
            Controls.Add(textBoxPath);
            
            // Button Cancel.
            buttonCancel = new Button();
            buttonCancel.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            buttonCancel.Location = new Point(Width - Padding.Right - buttonCancel.Width, Height - Padding.Bottom - buttonCancel.Height);
            buttonCancel.Text = "Cancel";
            buttonCancel.Click += (sender, args) =>
            {
                DialogResult = Forms.DialogResult.Cancel;
                Close();
            };
            Controls.Add(buttonCancel);
            
            // Button Ok.
            buttonOk = new Button();
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonOk.Location = new Point(buttonCancel.Location.X - buttonOk.Width - 8, buttonCancel.Location.Y);
            buttonOk.Text = "Ok";
            buttonOk.Click += (sender, args) => { OpenFile(); };
            Controls.Add(buttonOk);
            
            // Label Filename.
            labelFilename = new Label();
            labelFilename.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            labelFilename.Location = new Point(8, buttonOk.Location.Y - 30);
            labelFilename.Size = new Drawing.Size(64, 22);
            labelFilename.Text = "File: ";
            labelFilename.TextAlign = ContentAlignment.MiddleRight;
            Controls.Add(labelFilename);
            
            // Textbox Filename.
            textBoxFilename = new TextBox();
            textBoxFilename.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBoxFilename.Location = new Point(labelFilename.Location.X + labelFilename.Width, labelFilename.Location.Y);
            textBoxFilename.Size = new Drawing.Size(Width - 32 - (buttonOk.Width + 8 + buttonCancel.Width) - labelFilename.Width, 22);
            Controls.Add(textBoxFilename);
            
            // Combobox Filter.
            comboFilter = new ComboBox();
            comboFilter.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            comboFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            comboFilter.Size = new Drawing.Size(buttonOk.Width + 8 + buttonCancel.Width, 22);
            comboFilter.Location = new Point(Width - Padding.Right - comboFilter.Width, textBoxFilename.Location.Y);
            Controls.Add(comboFilter);
            
            // File Render.
            fileRenderer = new FileRenderer(this);
            fileRenderer.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            fileRenderer.Location = new Point(Padding.Left, buttonBack.Location.Y + buttonBack.Height + 8);
            fileRenderer.Name = "fileRenderer";
            fileRenderer.Size = new Drawing.Size(Width - Padding.Left - Padding.Right, textBoxFilename.Location.Y - buttonBack.Location.Y - buttonBack.Height - 16);
            fileRenderer.DirectoryChanged += () =>
            {
                if (fileRenderer.prevPathes.Count > 0)
                    buttonBack.Enabled = true;
                textBoxPath.Text = fileRenderer.currentPath;
                textBoxPath.Refresh();
            };
            fileRenderer.SelectedFileChanged += (file) => textBoxFilename.Text = file.ToString();
            fileRenderer.FileOpened += (file) => OpenFile();
            Controls.Add(fileRenderer);
            
            textBoxPath.Text = fileRenderer.currentPath;
            textBoxPath.Refresh();
            textBoxPath.KeyDown += (sender, args) =>
            {
                if (args.KeyCode == Keys.Enter)
                    fileRenderer.SetDirectory(textBoxPath.Text);
            };

            fileRenderer.filesTree.NodeMouseClick += filesTree_NodeMouseClick;

            //AcceptButton = buttonOk;
            InitialDirectory = UnityEngine.Application.dataPath;
            Shown += FileDialog_Shown;
        }

        public string FileName
        {
            get { return filename; }
            set
            {
                filename = value;
                textBoxFilename.Text = value;
            }
        }
        public string Filter { get; set; }
        public string InitialDirectory
        {
            get { return initialDir; }
            set { initialDir = value; }
        }

        internal string currentFilter
        {
            get
            {
                if (comboFilter.SelectedIndex < 0 || string.IsNullOrEmpty(Filter)) return "*.*";
                
                var fs = Filter.Split('|');
                return fs[comboFilter.SelectedIndex * 2 + 1];
            }
        }
        protected internal virtual void ButtonBack()
        {
            buttonBack.Enabled = fileRenderer.Back() && fileRenderer.prevPathes.Count > 0;
            textBoxPath.Text = fileRenderer.currentPath;
        }
        protected internal virtual void ButtonUp()
        {
            fileRenderer.Up();
        }
        protected internal virtual void ButtonRefresh()
        {
            fileRenderer.SetDirectory(fileRenderer.currentPath);
        }
        protected internal virtual void filesTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var contextMenu = new ContextMenuStrip();

                ToolStripMenuItem itemOpen = new ToolStripMenuItem("Select");
                itemOpen.Click += (sender2, args) => { OpenFile(); };
                contextMenu.Items.Add(itemOpen);

                ToolStripSeparator itemSep = new ToolStripSeparator();
                contextMenu.Items.Add(itemSep);

                ToolStripMenuItem itemProperties = new ToolStripMenuItem("Properties");
                itemProperties.Click += (sender2, args) => { new FormFileInfo(fileRenderer.currentPath + "/" + textBoxFilename.Text).ShowDialog(); };
                contextMenu.Items.Add(itemProperties);

                contextMenu.Show(null, MousePosition);
            }
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            
            if (e.KeyCode == Keys.Escape)
            {
                if (textBoxPath.Focused)
                {
                    Focus();
                    return;
                }
                
                DialogResult = DialogResult.Cancel;
                Close();
            }

            // Next folder.
            if (e.KeyCode == Keys.Return && textBoxPath.Focused == false)
                fileRenderer.Next();

            // Refresh directory.
            if (e.KeyCode == Keys.F5)
                ButtonRefresh();

            if (e.Alt)
            {
                switch (e.KeyCode)
                {
                    case Keys.Left: ButtonBack(); break;
                    case Keys.Up: ButtonUp(); break;
                }
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (fileRenderer != null)
            {
                var grayFillY = fileRenderer.Location.Y;
                e.Graphics.DrawLine(splitterPen, 0, grayFillY - 1, Width, grayFillY - 1);
                e.Graphics.uwfFillRectangle(SystemColors.Control, 0, grayFillY, Width, Height - grayFillY);
            }
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (handleFormSize)
                savedFormSize = Size;
        }
        protected internal void OpenFile()
        {
            filename = fileRenderer.currentPath + "/" + textBoxFilename.Text;
            
#if IO_SUPPORTED

            // Check if file is not a directory.
            if (!System.IO.Directory.Exists(filename))
            {
                // Add extension to the end of file if needed.
                var hasExtension = System.IO.Path.HasExtension(filename);
                if (!hasExtension)
                {
                    var extension = currentFilter;
                    if (extension.Contains('.'))
                        filename += extension.Substring(extension.IndexOf('.'));
                }
            }

#endif
            
            DialogResult = DialogResult.OK;
            Close();
        }

        private void FileDialog_Shown(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Filter))
            {
                comboFilter.Visible = false;
                textBoxFilename.Width = Width - textBoxFilename.Location.X - (Width - comboFilter.Location.X - comboFilter.Width);
            }
            else
            {
                var fs = Filter.Split('|');
                if (fs.Length % 2 != 0)
                {
                    Close();
                    throw new ArgumentException("filter");
                }

                for (int i = 0; i < fs.Length; i += 2)
                    comboFilter.Items.Add(fs[i] + " (" + fs[i + 1] + ")");

                if (comboFilter.Items.Count > 0)
                {
                    comboFilter.SelectedIndex = 0;
                    comboFilter.SelectedIndexChanged += (s, a) => { fileRenderer.SetDirectory(fileRenderer.currentPath); };
                }
            }

            fileRenderer.SetDirectory(InitialDirectory);
            fileRenderer.filesTree.Focus();
        }

        #region classes

        internal class FileRenderer : Control
        {
            public string currentPath;
            public List<string> prevPathes;

            internal TreeView filesTree;

            private readonly FileDialog owner;
            private string fromFolder;
            private FileInfo[] currentFiles;

            public FileRenderer(FileDialog owner)
            {
                this.owner = owner;

                prevPathes = new List<string>();

                filesTree = new FileTreeView();
                filesTree.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
                filesTree.BorderStyle = BorderStyle.None;
                filesTree.Size = new Drawing.Size(Width, Height);
                filesTree.AfterSelect += filesTree_SelectedNodeChanged;
                filesTree.NodeMouseDoubleClick += filesTree_NodeMouseDoubleClick;
                Controls.Add(filesTree);

                filesTree.ImageList = new ImageList();
                filesTree.ImageList.Images.Add(ApplicationResources.Images.FileDialogFolder);
                filesTree.ImageList.Images.Add(ApplicationResources.Images.FileDialogFile);
            }

            public delegate void DirectoryChangedHandler();
            public delegate void FileHandler(FileInfo file);

            public event DirectoryChangedHandler DirectoryChanged;
            public event FileHandler FileOpened;
            public event FileHandler SelectedFileChanged;

            public bool Back()
            {
                if (prevPathes.Count == 0) return false;

                currentPath = prevPathes.Last();
                prevPathes.RemoveAt(prevPathes.Count - 1);
                SetDirectory(currentPath);
                return true;
            }
            public void Next()
            {
                if (filesTree.SelectedNode == null) return;

                var fInfo = (FileInfo)filesTree.SelectedNode.Tag;
                if (fInfo.IsDirectory)
                {
                    prevPathes.Add(currentPath);
                    currentPath += fInfo.Name;
                    SetDirectory(currentPath);
                    
                    if (DirectoryChanged != null)
                        DirectoryChanged();
                }
                else
                {
                    if (FileOpened != null)
                        FileOpened(fInfo);
                }
            }
            public void SetDirectory(string path, bool addPrevPath = false)
            {
#if IO_SUPPORTED
                if (path.Length <= 2) return;
                if (System.IO.Directory.Exists(path) == false) return;

                if (addPrevPath)
                    prevPathes.Add(currentPath);
                
                currentPath = path.Replace("\\", "/");
                var filter = owner.currentFilter;
                
                string[] files;
                try
                {
                    if (filter == "*" || filter == "*.*")
                        files = System.IO.Directory.GetFiles(currentPath, "*.*")
                            .Select(f => f.Substring(currentPath.Length))
                            .ToArray();
                    else
                        files = System.IO.Directory.GetFiles(currentPath, "*.*")
                            .Where(f => filter.Contains(System.IO.Path.GetExtension(f).ToLower()))
                            .Select(f => f.Substring(currentPath.Length))
                            .ToArray();
                }
                catch (UnauthorizedAccessException ex)
                {
                    // Restore previous path.
                    if (prevPathes.Count > 0)
                    {
                        var prevPath = prevPathes[prevPathes.Count - 1];
                        prevPathes.RemoveAt(prevPathes.Count - 1);
                        
                        if (prevPath != currentPath) // Or we can get stack overflow.
                            SetDirectory(prevPath);
                    }
                    
                    MessageBox.Show(ex.Message, "Error");
                    return;
                }

                var dirs = System.IO.Directory.GetDirectories(currentPath)
                    .Select(f => f.Substring(currentPath.Length))
                    .ToArray();

                currentFiles = new FileInfo[dirs.Length + files.Length];
                
                for (int i = 0; i < dirs.Length; i++)
                {
                    currentFiles[i] = new FileInfo();
                    currentFiles[i].IsDirectory = true;
                    currentFiles[i].Name = dirs[i];
                }
                
                for (int i = dirs.Length; i < dirs.Length + files.Length; i++)
                {
                    currentFiles[i] = new FileInfo();
                    currentFiles[i].Name = files[i - dirs.Length];
                }

                filesTree.Nodes.Clear();
                
                for (int i = 0; i < currentFiles.Length; i++)
                {
                    var file = currentFiles[i];
                    file.CurrentPath = currentPath;
                    
                    var fileNode = new TreeNode(file.ToString());
                    fileNode.Tag = file;
                    fileNode.ImageIndex = file.IsDirectory ? 0 : 1;

                    filesTree.Nodes.Add(fileNode);
                    if (fileNode.Text == fromFolder)
                        filesTree.SelectedNode = fileNode;
                }
                filesTree.ExpandAll();

                if (DirectoryChanged != null)
                    DirectoryChanged();

                fromFolder = null;

                owner.textBoxFilename.Text = "";
#endif
            }
            public void Up()
            {
#if IO_SUPPORTED
                System.IO.DirectoryInfo parent;
                try
                {
                    // Can throw NullReferenceExepcetion for path like 'C:\'.
                    parent = System.IO.Directory.GetParent(currentPath);
                }
                catch (Exception e) { return; }
                
                if (parent == null || parent.Exists == false) return;
                
                prevPathes.Add(currentPath);
                fromFolder = System.IO.Path.GetFileName(currentPath);
                SetDirectory(parent.FullName);
#endif
            }

            private void filesTree_SelectedNodeChanged(object sender, TreeViewEventArgs e)
            {
                if (e.Node != null && SelectedFileChanged != null)
                    SelectedFileChanged((FileInfo)e.Node.Tag);
            }
            private void filesTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
            {
                Next();
            }

            private class FileTreeView : TreeView
            {
                public Pen selectionPen = new Pen(Color.White);

                public FileTreeView()
                {
                    uwfItemSelectedColor = Color.Transparent;
                    uwfItemHoveredColor = Color.Transparent;
                    uwfItemSelectedUnfocusedColor = Color.Transparent;
                }
                
                protected override void OnDrawNode(DrawTreeNodeEventArgs e)
                {
                    var selectionBounds = new Rectangle(e.Node.Bounds.X, e.Node.Bounds.Y, 660, e.Node.Bounds.Height);
                    
                    if (e.Node.IsSelected || e.Node == hoveredNode)
                    {
                        var backColor = Color.FromArgb(209, 232, 255);
                        
                        if (e.Node.IsSelected)
                        {
                            if (Focused)
                                selectionPen.Color = Color.FromArgb(60, 162, 223);
                            else
                            {
                                backColor = Color.FromArgb(247, 247, 247);
                                selectionPen.Color = Color.FromArgb(222, 222, 222);
                            }
                        }
                        else
                        {
                            // Hovered.
                            backColor = Color.FromArgb(229, 243, 251);
                            selectionPen.Color = Color.FromArgb(112, 192, 231);
                        }
                        
                        e.Graphics.uwfFillRectangle(backColor, selectionBounds);
                        e.Graphics.DrawRectangle(selectionPen, selectionBounds);
                    }
                    
                    base.OnDrawNode(e);

#if IO_SUPPORTED
                    var info = e.Node.Tag as FileInfo;
                    if (info == null)
                        return;

                    var fInfo = info.Info;
                    if (fInfo == null)
                        return;

                    var foreColor = e.Node.ForeColor;
                    if (foreColor.IsEmpty)
                        foreColor = ForeColor;
                    
                    e.Graphics.uwfDrawString(
                        fInfo.LastWriteTime.ToString("dd.MM.yyyy HH:mm"), 
                        Font, 
                        foreColor, 
                        selectionBounds.X + 440,
                        selectionBounds.Y,
                        200,
                        selectionBounds.Height);
                    
                    e.Graphics.uwfDrawString(
                        info.sizeStr, 
                        Font, 
                        foreColor, 
                        selectionBounds.X + 400,
                        selectionBounds.Y,
                        selectionBounds.Width - 412,
                        selectionBounds.Height,
                        ContentAlignment.MiddleRight);
#endif
                }
                
                
            }
        }

        internal class FileInfo
        {
#if IO_SUPPORTED
            private System.IO.FileInfo info;
            public System.IO.FileInfo Info
            {
                get
                {
                    if (IsDirectory)
                        return null;

                    if (info == null && System.IO.File.Exists(CurrentPath + Name))
                    {
                        info = new System.IO.FileInfo(CurrentPath + Name);
                        sizeStr = HumanizeBytes(info.Length);
                    }
                    
                    return info;
                }
            }

            public string sizeStr;
#endif

            public string CurrentPath;
            public bool IsDirectory;
            public string Name;

            private static readonly NumberFormatInfo numberFormatInfo = new NumberFormatInfo()
            {
                NumberGroupSeparator = " "
            };
            
            public override string ToString()
            {
                //if (IsDirectory) return "/ " + Name;
                return Name.Replace("\\", "");
            }
            
            private static string HumanizeBytes(long bytes)
            {
                if (bytes < 1024) return " 1 KB";

                return (bytes / 1024).ToString("N0", numberFormatInfo) + " KB";
            }
        }

        internal class FormFileInfo : Form
        {
            public FormFileInfo(string path)
            {
#if IO_SUPPORTED
                Size = new Drawing.Size(320, 120);
                Location = new Point(Screen.PrimaryScreen.WorkingArea.Width / 2 - Width / 2, Screen.PrimaryScreen.WorkingArea.Height / 2 - Height / 2);
                FormBorderStyle = FormBorderStyle.FixedSingle;
                Text = "Properties: ";

                if (System.IO.File.Exists(path) == false) return;

                var info = new System.IO.FileInfo(path);
                var bytes = info.Length;
                string sbytes = bytes.ToString() + " bytes";
                if (bytes >= 1000000000)
                    sbytes = (bytes / 1000000000) + " gb";
                if (bytes >= 1000000)
                    sbytes = (bytes / 1000000) + " mb";
                else if (bytes >= 1000)
                    sbytes = (bytes / 1000) + " kb";

                Label labelSize = new Label();
                labelSize.Location = new Point(16, 32);
                labelSize.Text = "Size: " + sbytes;
                Controls.Add(labelSize);

                Label labelCreationDate = new Label();
                labelCreationDate.Location = new Point(16, 64);
                labelCreationDate.Text = "Date creation: " + info.CreationTime.ToString("dd.MM.yyyy");
                Controls.Add(labelCreationDate);

                Label labelModDate = new Label();
                labelModDate.Location = new Point(16, 86);
                labelModDate.Text = "Date modified: " + info.LastWriteTime.ToString();
                Controls.Add(labelModDate);

                string fileName = System.IO.Path.GetFileName(path);
                Text += fileName;
#endif
            }
        }

        internal class PathTextBox : TextBox
        {
            private readonly FileDialog fileDialog;
            private string text;
            private bool updateButtons;

            public PathTextBox(FileDialog owner)
            {
                fileDialog = owner;
                Padding = new Padding(8, 0, 8, 0);
            }

            public override string Text
            {
                get { return text; }
                set
                {
                    if (text == value)
                        return;
                    
                    // TODO: reset text when directory not found or Escape button was pressed.
                    text = value;
                    if (text == null)
                        text = "";

                    OnTextChanged(EventArgs.Empty);
                }
            }

            protected override void OnGotFocus(EventArgs e)
            {
                base.OnGotFocus(e);
                DisposeButtons();
            }
            protected override void OnLostFocus(EventArgs e)
            {
                base.OnLostFocus(e);
                
                Refresh();
            }
            protected override void OnKeyDown(KeyEventArgs e)
            {
                base.OnKeyDown(e);

                if (e.KeyCode == Keys.Enter)
                    Parent.Focus();
            }
            protected override void OnPaint(PaintEventArgs e)
            {
                if (updateButtons)
                {
                    for (int i = Controls.Count - 1, x = 0; i >= 0; i--)
                    {
                        var b = Controls[i] as pathButton;
                        if (b == null) continue;

                        b.Width = (int) e.Graphics.MeasureString(b.Text, b.Font).Width + 23; // padding + arrowButton.
                        b.Location = new Point(x, 1);

                        x += b.Width;
                    }
                    updateButtons = false;
                }

                // Write text to buffer.
                var temp = text;
                if (Focused == false)
                    text = string.Empty;

                base.OnPaint(e);

                if (Focused == false)
                    text = temp; // Restore buffered text if needed.
            }
            public override void Refresh()
            {
                base.Refresh();
                
                UpdateButtons();
            }

            private void CreateButtons()
            {
#if IO_SUPPORTED
                var currentPath = text;
                if (string.IsNullOrEmpty(currentPath))
                    return;
                
                var directories = new List<IO.DirectoryInfo>();
                var cdir = new IO.DirectoryInfo(currentPath);
                directories.Add(cdir);

                var count = 0;
                while (count < 30)
                {
                    try
                    {
                        var dir = System.IO.Directory.GetParent(currentPath);
                        if (dir == null) break;

                        directories.Add(dir);
                        currentPath = dir.FullName;
                    }
                    catch (Exception)
                    {
                        // Fix: nullreference?? (Directory.GetParent("C:\\")). 
                    }

                    count++;
                }

                var allowedWidth = Width;
                float currentWidth = 0;
                int currentIndex;
                
                for (currentIndex = 0; currentIndex < directories.Count; currentIndex++)
                {
                    var dir = directories[currentIndex];
                    var estimatedWidth = Font.MeasureStringSimple(dir.Name).Width + 15;

                    currentWidth += estimatedWidth;
                    if (currentWidth >= allowedWidth)
                        break;

                    var button = new pathButton(dir.FullName);
                    button.Font = Font;
                    button.Height = Height - 2;
                    button.Width = (int)estimatedWidth;
                    button.Text = dir.Name;
                    button.Click += (sender, args) => fileDialog.fileRenderer.SetDirectory(button.Path, true);

                    Controls.Add(button);
                }

                updateButtons = true;
#endif
            }
            private void DisposeButtons()
            {
                for (int i = 0; i < Controls.Count; i++)
                {
                    var pb = Controls[i] as pathButton;
                    if (pb == null) continue;

                    pb.Dispose(); // TODO: cache + visible.
                    i--;
                }
            }
            private void UpdateButtons()
            {
                DisposeButtons();
                CreateButtons();
            }

            private class pathButton : Button
            {
                public pathButton(string path)
                {
                    Path = path;

                    BackColor = Color.Transparent;
                    uwfBorderColor = Color.Transparent;
                    ForeColor = Color.FromArgb(47, 47, 47);
                    uwfHoverColor = Color.FromArgb(229, 243, 251);
                    Padding = new Padding(4, 0, 4, 0);
                    TextAlign = ContentAlignment.MiddleLeft;

                    var arrowButton = new Button();
                    arrowButton.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
                    arrowButton.BackColor = BackColor;
                    arrowButton.uwfBorderColor = uwfBorderColor;
                    arrowButton.uwfHoverColor = uwfHoverColor;
                    arrowButton.Width = 15;
                    arrowButton.Height = Height;
                    arrowButton.Location = new Point(Width - arrowButton.Width, 0);
                    arrowButton.Text = "";
                    arrowButton.Image = ApplicationResources.Images.ArrowRight;
                    arrowButton.uwfImageColor = Color.Gray;
                    arrowButton.uwfImageHoverColor = Color.Gray;

                    Controls.Add(arrowButton);
                }

                public string Path { get; private set; }
            }
        }

        #endregion
    }
}
