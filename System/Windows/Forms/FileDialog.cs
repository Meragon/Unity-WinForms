using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public abstract class FileDialog : Form
    {
        public static Bitmap FolderNavBack { get; set; }
        public static Bitmap FolderNavUp { get; set; }

        internal FileRender fileRender;

        protected Button buttonOk;
        protected Button buttonCancel;
        protected Button buttonBack;
        protected Button buttonUp;

        protected TextBox textBoxPath;
        protected Label labelFilename;
        protected TextBox textBoxFile;
        protected ComboBox comboFilter;

        public string FileName { get; set; }
        public string Filter { get; set; }
        public Bitmap ImageFile { get; set; }
        public Bitmap ImageFolder { get; set; }

        private string currentFilter
        {
            get
            {
                if (comboFilter.SelectedIndex < 0) return "*.*";

                var fs = Filter.Split('|');
                return fs[comboFilter.SelectedIndex * 2 + 1];
            }
        }

        public FileDialog()
        {
#if !UNITY_STANDALONE
            throw new NotSupportedException();
#endif
            Filter = "All files|*.*";
            MinimumSize = new Drawing.Size(240, 240);
            Size = new Drawing.Size(540, 320);
            Text = "File Dialog";

            buttonBack = new Button();
            buttonBack.BackgroundImageLayout = ImageLayout.Center;
            buttonBack.Enabled = false;
            buttonBack.Font = new Drawing.Font("Arial", 16, FontStyle.Bold);
            buttonBack.Image = FolderNavBack;
            buttonBack.Location = new Point(8, HeaderHeight);
            buttonBack.NormalColor = Color.Transparent;
            buttonBack.NormalBorderColor = Color.Transparent;
            buttonBack.Size = new Size(22, 22);
            if (buttonBack.Image == null) buttonBack.Text = "◀";
            buttonBack.Click += (sender, args) =>
            {
                buttonBack.Enabled = fileRender.Back() && fileRender.prevPathes.Count > 0;
                textBoxPath.Text = fileRender.currentPath;
            };
            Controls.Add(buttonBack);

            buttonUp = new Button();
            buttonUp.BackgroundImageLayout = ImageLayout.Center;
            buttonUp.Font = new Drawing.Font("Arial", 16, FontStyle.Bold);
            buttonUp.Image = FolderNavUp;
            buttonUp.Location = new Point(buttonBack.Location.X + buttonBack.Width + 8, buttonBack.Location.Y);
            buttonUp.NormalColor = Color.Transparent;
            buttonUp.NormalBorderColor = Color.Transparent;
            buttonUp.Size = new Drawing.Size(22, 22);
            if (buttonUp.Image == null) buttonUp.Text = "▲";
            buttonUp.Click += (sender, args) =>
            {
                fileRender.Up();
            };
            Controls.Add(buttonUp);

            textBoxPath = new TextBox();
            textBoxPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxPath.Font = new Drawing.Font("Arial", 14);
            textBoxPath.Location = new Point(buttonUp.Location.X + buttonUp.Width + 8, buttonUp.Location.Y);
            textBoxPath.Size = new Drawing.Size(Width - 8 - textBoxPath.Location.X, buttonBack.Height);
            Controls.Add(textBoxPath);

            buttonCancel = new Button();
            buttonCancel.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            buttonCancel.Location = new Point(Width - 8 - buttonCancel.Width, Height - 8 - buttonCancel.Height);
            buttonCancel.Text = "Cancel";
            buttonCancel.Click += (sender, args) =>
            {
                DialogResult = Forms.DialogResult.Cancel;
                Close();
            };
            Controls.Add(buttonCancel);

            buttonOk = new Button();
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonOk.Location = new Point(buttonCancel.Location.X - buttonOk.Width - 8, buttonCancel.Location.Y);
            buttonOk.Text = "Ok";
            buttonOk.Click += (sender, args) => { OpenFile(); };
            Controls.Add(buttonOk);

            labelFilename = new Label();
            labelFilename.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            labelFilename.Location = new Point(8, buttonOk.Location.Y - 30);
            labelFilename.Size = new Drawing.Size(64, 22);
            labelFilename.Text = "File: ";
            labelFilename.TextAlign = ContentAlignment.MiddleRight;
            Controls.Add(labelFilename);

            textBoxFile = new TextBox();
            textBoxFile.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBoxFile.Location = new Point(labelFilename.Location.X + labelFilename.Width, labelFilename.Location.Y);
            textBoxFile.Size = new Drawing.Size(Width - 24 - (buttonOk.Width + 8 + buttonCancel.Width) - labelFilename.Width, 22);
            Controls.Add(textBoxFile);

            comboFilter = new ComboBox();
            comboFilter.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            comboFilter.Size = new Drawing.Size(buttonOk.Width + 8 + buttonCancel.Width, 22);
            comboFilter.Location = new Point(Width - 8 - comboFilter.Width, textBoxFile.Location.Y);
            Controls.Add(comboFilter);

            fileRender = new FileRender(this);
            fileRender.Anchor = AnchorStyles.All;
            fileRender.Location = new Point(8, buttonBack.Location.Y + buttonBack.Height + 8);
            fileRender.Name = "fileRender";
            fileRender.Size = new Drawing.Size(Width - 16, textBoxFile.Location.Y - buttonBack.Location.Y - buttonBack.Height - 16);
            fileRender.OnDirectoryChanged += () =>
            {
                if (fileRender.prevPathes.Count > 0)
                    buttonBack.Enabled = true;
                textBoxPath.Text = fileRender.currentPath;
            };
            fileRender.OnSelectedFileChanged += (file) =>
            {
                textBoxFile.Text = file.ToString();
            };
            fileRender.OnFileOpen += (file) =>
            {
                OpenFile();
            };
            Controls.Add(fileRender);

            textBoxPath.Text = fileRender.currentPath;
            textBoxPath.TextChanged += (sender, args) =>
            {
                fileRender.SetDirectory(textBoxPath.Text);
            };

            fileRender.filesTree.NodeMouseClick += filesTree_NodeMouseClick;
        }

        protected virtual void filesTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
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
                itemProperties.Click += (sender2, args) => { new FormFileInfo(fileRender.currentPath + "/" + textBoxFile.Text).Show(); };
                contextMenu.Items.Add(itemProperties);

                contextMenu.Show(null, MousePosition);
            }
        }

        protected override void OnKeyPress(KeyEventArgs e)
        {
            base.OnKeyPress(e);
            if (e.KeyCode == UnityEngine.KeyCode.Escape)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }
        public override DialogResult ShowDialog()
        {
            Focus();
            var fs = Filter.Split('|');
            for (int i = 0; i < fs.Length; i += 2)
                comboFilter.Items.Add(fs[i]);

            if (comboFilter.Items.Count > 0)
            {
                comboFilter.SelectedIndex = 0;
                comboFilter.SelectedIndexChanged += (sender, args) =>
                {
                    fileRender.SetDirectory(fileRender.currentPath);
                };
            }

            fileRender.SetDirectory(fileRender.currentPath);
            Show();
            return Forms.DialogResult.None;
        }

        protected void OpenFile()
        {
            FileName = fileRender.currentPath + "/" + textBoxFile.Text;
            DialogResult = Forms.DialogResult.OK;
            Close();
        }

        internal class FileRender : Control
        {
            private FileDialog _owner;

            public string currentPath;
            public List<string> prevPathes;

            private FileInfo[] currentFiles;
            internal TreeView filesTree;

            public FileRender(FileDialog owner)
            {
                _owner = owner;

                prevPathes = new List<string>();

                filesTree = new TreeView();
                filesTree.Anchor = AnchorStyles.All;
                filesTree.Size = new Drawing.Size(Width, Height);
                filesTree.SelectedNodeChanged += filesTree_SelectedNodeChanged;
                filesTree.NodeMouseDoubleClick += filesTree_NodeMouseDoubleClick;
                Controls.Add(filesTree);

                Bitmap folderImage = _owner.ImageFolder != null ? _owner.ImageFolder : GenDefaultFolderImage();
                Bitmap fileImage = _owner.ImageFile != null ? _owner.ImageFile : GenDefaultFileImage();

                filesTree.ImageList = new ImageList();
                filesTree.ImageList.Images.Add(folderImage);
                filesTree.ImageList.Images.Add(fileImage);

                currentPath = UnityEngine.Application.dataPath;
            }

            private static Bitmap GenDefaultFileImage()
            {
                Bitmap fileImage = new Bitmap(16, 16);
                for (int i = 0; i < fileImage.Width; i++)
                    for (int k = 0; k < fileImage.Height; k++)
                        fileImage.SetPixel(i, k, Color.Transparent);

                for (int i = 0; i < 2; i++) fileImage.SetPixel(i, 1, Color.Transparent);
                for (int i = 2; i < 10; i++) fileImage.SetPixel(i, 1, Color.FromArgb(255, 180, 180, 180));
                for (int i = 10; i < 13; i++) fileImage.SetPixel(i, 1, Color.Transparent);
                for (int i = 13; i < 14; i++) fileImage.SetPixel(i, 1, Color.Transparent);
                for (int i = 0; i < 2; i++) fileImage.SetPixel(i, 2, Color.Transparent);
                for (int i = 2; i < 3; i++) fileImage.SetPixel(i, 2, Color.FromArgb(255, 180, 180, 180));
                for (int i = 3; i < 8; i++) fileImage.SetPixel(i, 2, Color.FromArgb(255, 233, 233, 233));
                for (int i = 8; i < 9; i++) fileImage.SetPixel(i, 2, Color.FromArgb(255, 180, 180, 180));
                for (int i = 9; i < 10; i++) fileImage.SetPixel(i, 2, Color.FromArgb(255, 233, 233, 233));
                for (int i = 10; i < 11; i++) fileImage.SetPixel(i, 2, Color.FromArgb(255, 180, 180, 180));
                for (int i = 11; i < 13; i++) fileImage.SetPixel(i, 2, Color.Transparent);
                for (int i = 13; i < 14; i++) fileImage.SetPixel(i, 2, Color.Transparent);
                for (int i = 0; i < 2; i++) fileImage.SetPixel(i, 3, Color.Transparent);
                for (int i = 2; i < 3; i++) fileImage.SetPixel(i, 3, Color.FromArgb(255, 180, 180, 180));
                for (int i = 3; i < 8; i++) fileImage.SetPixel(i, 3, Color.FromArgb(255, 233, 233, 233));
                for (int i = 8; i < 9; i++) fileImage.SetPixel(i, 3, Color.FromArgb(255, 180, 180, 180));
                for (int i = 9; i < 11; i++) fileImage.SetPixel(i, 3, Color.FromArgb(255, 233, 233, 233));
                for (int i = 11; i < 12; i++) fileImage.SetPixel(i, 3, Color.FromArgb(255, 180, 180, 180));
                for (int i = 12; i < 13; i++) fileImage.SetPixel(i, 3, Color.Transparent);
                for (int i = 13; i < 14; i++) fileImage.SetPixel(i, 3, Color.Transparent);
                for (int i = 0; i < 2; i++) fileImage.SetPixel(i, 4, Color.Transparent);
                for (int i = 2; i < 3; i++) fileImage.SetPixel(i, 4, Color.FromArgb(255, 180, 180, 180));
                for (int i = 3; i < 8; i++) fileImage.SetPixel(i, 4, Color.FromArgb(255, 233, 233, 233));
                for (int i = 8; i < 9; i++) fileImage.SetPixel(i, 4, Color.FromArgb(255, 180, 180, 180));
                for (int i = 9; i < 12; i++) fileImage.SetPixel(i, 4, Color.FromArgb(255, 233, 233, 233));
                for (int i = 12; i < 13; i++) fileImage.SetPixel(i, 4, Color.FromArgb(255, 180, 180, 180));
                for (int i = 13; i < 14; i++) fileImage.SetPixel(i, 4, Color.Transparent);
                for (int i = 0; i < 2; i++) fileImage.SetPixel(i, 5, Color.Transparent);
                for (int i = 2; i < 3; i++) fileImage.SetPixel(i, 5, Color.FromArgb(255, 180, 180, 180));
                for (int i = 3; i < 8; i++) fileImage.SetPixel(i, 5, Color.FromArgb(255, 233, 233, 233));
                for (int i = 8; i < 13; i++) fileImage.SetPixel(i, 5, Color.FromArgb(255, 180, 180, 180));
                for (int i = 13; i < 14; i++) fileImage.SetPixel(i, 5, Color.Transparent);
                for (int i = 0; i < 2; i++) fileImage.SetPixel(i, 6, Color.Transparent);
                for (int i = 2; i < 3; i++) fileImage.SetPixel(i, 6, Color.FromArgb(255, 180, 180, 180));
                for (int i = 3; i < 12; i++) fileImage.SetPixel(i, 6, Color.FromArgb(255, 233, 233, 233));
                for (int i = 12; i < 13; i++) fileImage.SetPixel(i, 6, Color.FromArgb(255, 180, 180, 180));
                for (int i = 13; i < 14; i++) fileImage.SetPixel(i, 6, Color.Transparent);
                for (int i = 0; i < 2; i++) fileImage.SetPixel(i, 7, Color.Transparent);
                for (int i = 2; i < 3; i++) fileImage.SetPixel(i, 7, Color.FromArgb(255, 180, 180, 180));
                for (int i = 3; i < 12; i++) fileImage.SetPixel(i, 7, Color.FromArgb(255, 233, 233, 233));
                for (int i = 12; i < 13; i++) fileImage.SetPixel(i, 7, Color.FromArgb(255, 180, 180, 180));
                for (int i = 13; i < 14; i++) fileImage.SetPixel(i, 7, Color.Transparent);
                for (int i = 0; i < 2; i++) fileImage.SetPixel(i, 8, Color.Transparent);
                for (int i = 2; i < 3; i++) fileImage.SetPixel(i, 8, Color.FromArgb(255, 180, 180, 180));
                for (int i = 3; i < 12; i++) fileImage.SetPixel(i, 8, Color.FromArgb(255, 233, 233, 233));
                for (int i = 12; i < 13; i++) fileImage.SetPixel(i, 8, Color.FromArgb(255, 180, 180, 180));
                for (int i = 13; i < 14; i++) fileImage.SetPixel(i, 8, Color.Transparent);
                for (int i = 0; i < 2; i++) fileImage.SetPixel(i, 9, Color.Transparent);
                for (int i = 2; i < 3; i++) fileImage.SetPixel(i, 9, Color.FromArgb(255, 180, 180, 180));
                for (int i = 3; i < 12; i++) fileImage.SetPixel(i, 9, Color.FromArgb(255, 233, 233, 233));
                for (int i = 12; i < 13; i++) fileImage.SetPixel(i, 9, Color.FromArgb(255, 180, 180, 180));
                for (int i = 13; i < 14; i++) fileImage.SetPixel(i, 9, Color.Transparent);
                for (int i = 0; i < 2; i++) fileImage.SetPixel(i, 10, Color.Transparent);
                for (int i = 2; i < 3; i++) fileImage.SetPixel(i, 10, Color.FromArgb(255, 180, 180, 180));
                for (int i = 3; i < 12; i++) fileImage.SetPixel(i, 10, Color.FromArgb(255, 233, 233, 233));
                for (int i = 12; i < 13; i++) fileImage.SetPixel(i, 10, Color.FromArgb(255, 180, 180, 180));
                for (int i = 13; i < 14; i++) fileImage.SetPixel(i, 10, Color.Transparent);
                for (int i = 0; i < 2; i++) fileImage.SetPixel(i, 11, Color.Transparent);
                for (int i = 2; i < 3; i++) fileImage.SetPixel(i, 11, Color.FromArgb(255, 180, 180, 180));
                for (int i = 3; i < 12; i++) fileImage.SetPixel(i, 11, Color.FromArgb(255, 233, 233, 233));
                for (int i = 12; i < 13; i++) fileImage.SetPixel(i, 11, Color.FromArgb(255, 180, 180, 180));
                for (int i = 13; i < 14; i++) fileImage.SetPixel(i, 11, Color.Transparent);
                for (int i = 0; i < 2; i++) fileImage.SetPixel(i, 12, Color.Transparent);
                for (int i = 2; i < 3; i++) fileImage.SetPixel(i, 12, Color.FromArgb(255, 180, 180, 180));
                for (int i = 3; i < 12; i++) fileImage.SetPixel(i, 12, Color.FromArgb(255, 233, 233, 233));
                for (int i = 12; i < 13; i++) fileImage.SetPixel(i, 12, Color.FromArgb(255, 180, 180, 180));
                for (int i = 13; i < 14; i++) fileImage.SetPixel(i, 12, Color.Transparent);
                for (int i = 0; i < 2; i++) fileImage.SetPixel(i, 13, Color.Transparent);
                for (int i = 2; i < 3; i++) fileImage.SetPixel(i, 13, Color.FromArgb(255, 180, 180, 180));
                for (int i = 3; i < 12; i++) fileImage.SetPixel(i, 13, Color.FromArgb(255, 233, 233, 233));
                for (int i = 12; i < 13; i++) fileImage.SetPixel(i, 13, Color.FromArgb(255, 180, 180, 180));
                for (int i = 13; i < 14; i++) fileImage.SetPixel(i, 13, Color.Transparent);
                for (int i = 0; i < 2; i++) fileImage.SetPixel(i, 14, Color.Transparent);
                for (int i = 2; i < 13; i++) fileImage.SetPixel(i, 14, Color.FromArgb(255, 180, 180, 180));
                for (int i = 13; i < 14; i++) fileImage.SetPixel(i, 14, Color.Transparent);

                fileImage.Apply();
                return fileImage;
            }
            private static Bitmap GenDefaultFolderImage()
            {
                Bitmap folderImage = new Bitmap(16, 16);
                for (int i = 0; i < folderImage.Width; i++)
                    for (int k = 0; k < folderImage.Height; k++)
                        folderImage.SetPixel(i, k, Color.Transparent);

                for (int i = 0; i < 3; i++) folderImage.SetPixel(i, 0, Color.Transparent);
                for (int i = 3; i < 13; i++) folderImage.SetPixel(i, 0, Color.Transparent);
                for (int i = 13; i < 15; i++) folderImage.SetPixel(i, 0, Color.Transparent);
                for (int i = 0; i < 1; i++) folderImage.SetPixel(i, 1, Color.Transparent);
                for (int i = 0; i < 3; i++) folderImage.SetPixel(i, 2, Color.Transparent);
                for (int i = 3; i < 12; i++) folderImage.SetPixel(i, 2, Color.FromArgb(255, 218, 191, 127));
                for (int i = 0; i < 3; i++) folderImage.SetPixel(i, 3, Color.Transparent);
                for (int i = 3; i < 4; i++) folderImage.SetPixel(i, 3, Color.FromArgb(255, 218, 191, 127));
                for (int i = 4; i < 9; i++) folderImage.SetPixel(i, 3, Color.FromArgb(255, 248, 239, 215));
                for (int i = 9; i < 10; i++) folderImage.SetPixel(i, 3, Color.FromArgb(255, 218, 191, 127));
                for (int i = 10; i < 11; i++) folderImage.SetPixel(i, 3, Color.FromArgb(255, 237, 219, 179));
                for (int i = 11; i < 12; i++) folderImage.SetPixel(i, 3, Color.FromArgb(255, 218, 191, 127));
                for (int i = 0; i < 3; i++) folderImage.SetPixel(i, 4, Color.Transparent);
                for (int i = 3; i < 4; i++) folderImage.SetPixel(i, 4, Color.FromArgb(255, 218, 191, 127));
                for (int i = 4; i < 9; i++) folderImage.SetPixel(i, 4, Color.FromArgb(255, 248, 239, 215));
                for (int i = 9; i < 10; i++) folderImage.SetPixel(i, 4, Color.FromArgb(255, 218, 191, 127));
                for (int i = 10; i < 11; i++) folderImage.SetPixel(i, 4, Color.FromArgb(255, 237, 219, 179));
                for (int i = 11; i < 12; i++) folderImage.SetPixel(i, 4, Color.FromArgb(255, 218, 191, 127));
                for (int i = 0; i < 3; i++) folderImage.SetPixel(i, 5, Color.Transparent);
                for (int i = 3; i < 4; i++) folderImage.SetPixel(i, 5, Color.FromArgb(255, 218, 191, 127));
                for (int i = 4; i < 9; i++) folderImage.SetPixel(i, 5, Color.FromArgb(255, 248, 239, 215));
                for (int i = 9; i < 10; i++) folderImage.SetPixel(i, 5, Color.FromArgb(255, 218, 191, 127));
                for (int i = 10; i < 11; i++) folderImage.SetPixel(i, 5, Color.FromArgb(255, 237, 219, 179));
                for (int i = 11; i < 12; i++) folderImage.SetPixel(i, 5, Color.FromArgb(255, 218, 191, 127));
                for (int i = 0; i < 3; i++) folderImage.SetPixel(i, 6, Color.Transparent);
                for (int i = 3; i < 4; i++) folderImage.SetPixel(i, 6, Color.FromArgb(255, 218, 191, 127));
                for (int i = 4; i < 9; i++) folderImage.SetPixel(i, 6, Color.FromArgb(255, 248, 239, 215));
                for (int i = 9; i < 10; i++) folderImage.SetPixel(i, 6, Color.FromArgb(255, 218, 191, 127));
                for (int i = 10; i < 11; i++) folderImage.SetPixel(i, 6, Color.FromArgb(255, 237, 219, 179));
                for (int i = 11; i < 12; i++) folderImage.SetPixel(i, 6, Color.FromArgb(255, 218, 191, 127));
                for (int i = 0; i < 3; i++) folderImage.SetPixel(i, 7, Color.Transparent);
                for (int i = 3; i < 4; i++) folderImage.SetPixel(i, 7, Color.FromArgb(255, 218, 191, 127));
                for (int i = 4; i < 9; i++) folderImage.SetPixel(i, 7, Color.FromArgb(255, 248, 239, 215));
                for (int i = 9; i < 10; i++) folderImage.SetPixel(i, 7, Color.FromArgb(255, 218, 191, 127));
                for (int i = 10; i < 11; i++) folderImage.SetPixel(i, 7, Color.FromArgb(255, 237, 219, 179));
                for (int i = 11; i < 12; i++) folderImage.SetPixel(i, 7, Color.FromArgb(255, 218, 191, 127));
                for (int i = 0; i < 3; i++) folderImage.SetPixel(i, 8, Color.Transparent);
                for (int i = 3; i < 4; i++) folderImage.SetPixel(i, 8, Color.FromArgb(255, 218, 191, 127));
                for (int i = 4; i < 9; i++) folderImage.SetPixel(i, 8, Color.FromArgb(255, 248, 239, 215));
                for (int i = 9; i < 10; i++) folderImage.SetPixel(i, 8, Color.FromArgb(255, 218, 191, 127));
                for (int i = 10; i < 11; i++) folderImage.SetPixel(i, 8, Color.FromArgb(255, 237, 219, 179));
                for (int i = 11; i < 13; i++) folderImage.SetPixel(i, 8, Color.FromArgb(255, 218, 191, 127));
                for (int i = 0; i < 3; i++) folderImage.SetPixel(i, 9, Color.Transparent);
                for (int i = 3; i < 4; i++) folderImage.SetPixel(i, 9, Color.FromArgb(255, 218, 191, 127));
                for (int i = 4; i < 9; i++) folderImage.SetPixel(i, 9, Color.FromArgb(255, 248, 239, 215));
                for (int i = 9; i < 10; i++) folderImage.SetPixel(i, 9, Color.FromArgb(255, 218, 191, 127));
                for (int i = 10; i < 12; i++) folderImage.SetPixel(i, 9, Color.FromArgb(255, 237, 219, 179));
                for (int i = 12; i < 13; i++) folderImage.SetPixel(i, 9, Color.FromArgb(255, 218, 191, 127));
                for (int i = 0; i < 3; i++) folderImage.SetPixel(i, 10, Color.Transparent);
                for (int i = 3; i < 4; i++) folderImage.SetPixel(i, 10, Color.FromArgb(255, 218, 191, 127));
                for (int i = 4; i < 9; i++) folderImage.SetPixel(i, 10, Color.FromArgb(255, 248, 239, 215));
                for (int i = 9; i < 10; i++) folderImage.SetPixel(i, 10, Color.FromArgb(255, 218, 191, 127));
                for (int i = 10; i < 12; i++) folderImage.SetPixel(i, 10, Color.FromArgb(255, 237, 219, 179));
                for (int i = 12; i < 13; i++) folderImage.SetPixel(i, 10, Color.FromArgb(255, 218, 191, 127));
                for (int i = 0; i < 3; i++) folderImage.SetPixel(i, 11, Color.Transparent);
                for (int i = 3; i < 4; i++) folderImage.SetPixel(i, 11, Color.FromArgb(255, 218, 191, 127));
                for (int i = 4; i < 9; i++) folderImage.SetPixel(i, 11, Color.FromArgb(255, 248, 239, 215));
                for (int i = 9; i < 10; i++) folderImage.SetPixel(i, 11, Color.FromArgb(255, 218, 191, 127));
                for (int i = 10; i < 12; i++) folderImage.SetPixel(i, 11, Color.FromArgb(255, 237, 219, 179));
                for (int i = 12; i < 13; i++) folderImage.SetPixel(i, 11, Color.FromArgb(255, 218, 191, 127));
                for (int i = 0; i < 3; i++) folderImage.SetPixel(i, 12, Color.Transparent);
                for (int i = 3; i < 12; i++) folderImage.SetPixel(i, 12, Color.FromArgb(255, 218, 191, 127));

                folderImage.Apply();
                return folderImage;
            }

            void filesTree_SelectedNodeChanged(object sender, TreeViewEventArgs e)
            {
                if (e.Node != null)
                    OnSelectedFileChanged((FileInfo)e.Node.Tag);
            }
            void filesTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
            {
                var fInfo = (FileInfo)e.Node.Tag;
                if (fInfo.IsDirectory)
                {
                    prevPathes.Add(currentPath);
                    currentPath += fInfo.Name;
                    SetDirectory(currentPath);
                    OnDirectoryChanged();
                }
                else
                {
                    OnFileOpen(fInfo);
                }
            }

            public bool Back()
            {
                if (prevPathes.Count == 0) return false;

                currentPath = prevPathes.Last();
                prevPathes.RemoveAt(prevPathes.Count - 1);
                SetDirectory(currentPath);
                return true;
            }
            public void SetDirectory(string path)
            {
#if UNITY_STANDALONE
                if (path.Length <= 2) return;
                if (System.IO.Directory.Exists(path) == false) return;

                currentPath = path.Replace("\\", "/");

                var files = System.IO.Directory.GetFiles(currentPath, _owner.currentFilter).Select(f => f.Substring(currentPath.Length)).ToArray();
                var dirs = System.IO.Directory.GetDirectories(currentPath).Select(f => f.Substring(currentPath.Length)).ToArray();

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
                    TreeNode fileNode = new TreeNode(currentFiles[i].ToString());
                    fileNode.Tag = currentFiles[i];
                    if (currentFiles[i].IsDirectory)
                        fileNode.ImageIndex = 0;
                    else
                        fileNode.ImageIndex = 1;

                    filesTree.Nodes.Add(fileNode);
                }
                filesTree.ExpandAll();

                OnDirectoryChanged();
#endif
            }
            public void Up()
            {
#if UNITY_STANDALONE
                var parent = System.IO.Directory.GetParent(currentPath);
                if (parent.Exists == false) return;

                prevPathes.Add(currentPath);
                SetDirectory(parent.FullName);
#endif
            }

            public event DirectoryChangedHandler OnDirectoryChanged = delegate { };
            public event FileHandler OnFileOpen = delegate { };
            public event FileHandler OnSelectedFileChanged = delegate { };

            public delegate void DirectoryChangedHandler();
            public delegate void FileHandler(FileInfo file);
        }

        internal class FileInfo
        {
            public bool IsDirectory { get; set; }
            public string Name { get; set; }

            public override string ToString()
            {
                //if (IsDirectory) return "/ " + Name;
                return Name.Replace("\\", "");
            }
        }

        internal class FormFileInfo : Form
        {
            public FormFileInfo(string path)
            {
#if UNITY_STANDALONE
                Size = new Drawing.Size(320, 120);
                Location = new Point(Screen.PrimaryScreen.WorkingArea.Width / 2 - Width / 2, Screen.PrimaryScreen.WorkingArea.Height / 2 - Height / 2);
                Resizable = false;
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
    }
}
