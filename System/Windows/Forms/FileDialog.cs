using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using UnityEngine;
using Color = System.Drawing.Color;
using FontStyle = System.Drawing.FontStyle;

namespace System.Windows.Forms
{
    public abstract class FileDialog : Form
    {
        protected static Size savedFormSize = new Size(720, 400);

        public static Bitmap FolderNavBack { get; set; }
        public static Bitmap FolderNavRefresh { get; set; }
        public static Bitmap FolderNavUp { get; set; }

        internal FileRenderer fileRenderer;

        private readonly bool handleFormSize;

        protected Button buttonOk;
        protected Button buttonCancel;
        protected Button buttonBack;
        protected Button buttonUp;
        protected Button buttonRefresh;
        protected ComboBox comboFilter;
        protected string currentFilter
        {
            get
            {
                if (comboFilter.SelectedIndex < 0) return "*.*";

                var fs = Filter.Split('|');
                return fs[comboFilter.SelectedIndex * 2 + 1];
            }
        }
        protected Label labelFilename;
        protected TextBox textBoxPath;
        protected TextBox textBoxFilename;

        public string FileName { get; set; }
        public string Filter { get; set; }
        public Bitmap ImageFile { get; set; }
        public Bitmap ImageFolder { get; set; }

        protected FileDialog()
        {
#if !UNITY_STANDALONE && !UNITY_ANDROID
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

            #region Button Back.
            buttonBack = new Button();
            buttonBack.BackgroundImageLayout = ImageLayout.Center;
            buttonBack.Enabled = false;
            buttonBack.Font = new Drawing.Font("Arial", 16, FontStyle.Bold);
            buttonBack.Image = FolderNavBack;
            buttonBack.Location = new Point(Padding.Left, uwfHeaderHeight + Padding.Top);
            buttonBack.BackColor = Color.Transparent;
            buttonBack.uwfBorderColor = Color.Transparent;
            buttonBack.Size = new Size(22, 22);
            if (buttonBack.Image == null) buttonBack.Text = "◀";
            buttonBack.Click += (sender, args) => ButtonBack();
            Controls.Add(buttonBack);

            ToolTip buttonBackTooltip = new ToolTip();
            buttonBackTooltip.SetToolTip(buttonBack, "Back (ALT + Left Arrow)");
            #endregion;

            #region Button Up.
            buttonUp = new Button();
            buttonUp.BackgroundImageLayout = ImageLayout.Center;
            buttonUp.Font = new Drawing.Font("Arial", 16, FontStyle.Bold);
            buttonUp.Image = FolderNavUp;
            buttonUp.Location = new Point(buttonBack.Location.X + buttonBack.Width + 8, buttonBack.Location.Y);
            buttonUp.BackColor = Color.Transparent;
            buttonUp.uwfBorderColor = Color.Transparent;
            buttonUp.Size = new Drawing.Size(22, 22);
            if (buttonUp.Image == null) buttonUp.Text = "▲";
            buttonUp.Click += (sender, args) => ButtonUp();
            Controls.Add(buttonUp);

            ToolTip buttonUpTooltip = new ToolTip();
            buttonUpTooltip.SetToolTip(buttonUp, "Up (ALT + Up Arrow)");
            #endregion

            #region Button Refresh.
            buttonRefresh = new Button();
            buttonRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonRefresh.Image = FolderNavRefresh;
            buttonRefresh.uwfImageColor = Color.FromArgb(64, 64, 64);
            buttonRefresh.uwfImageHoverColor = buttonRefresh.uwfImageColor;
            buttonRefresh.BackColor = Color.Transparent;
            buttonRefresh.uwfBorderColor = Color.Transparent;
            buttonRefresh.Size = new Size(22, 22);
            buttonRefresh.Location = new Point(Width - Padding.Right - buttonRefresh.Width, buttonUp.Location.Y);
            buttonRefresh.Click += (sender, args) => ButtonRefresh();
            if (buttonRefresh.Image == null) buttonRefresh.Text = "R";
            Controls.Add(buttonRefresh);

            ToolTip buttonRefreshTooltip = new ToolTip();
            buttonRefreshTooltip.SetToolTip(buttonRefresh, "Refresh (F5)");
            #endregion

            #region Textbox Path.
            textBoxPath = new PathTextBox(this);
            textBoxPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxPath.Font = new Drawing.Font("Arial", 14);
            textBoxPath.Location = new Point(buttonUp.Location.X + buttonUp.Width + 8, buttonUp.Location.Y);
            textBoxPath.Size = new Drawing.Size(Width - textBoxPath.Location.X - Padding.Right - buttonRefresh.Width - 8, buttonBack.Height);
            Controls.Add(textBoxPath);
            #endregion

            #region Button Cancel.
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
            #endregion

            #region Button Ok.
            buttonOk = new Button();
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonOk.Location = new Point(buttonCancel.Location.X - buttonOk.Width - 8, buttonCancel.Location.Y);
            buttonOk.Text = "Ok";
            buttonOk.Click += (sender, args) => { OpenFile(); };
            Controls.Add(buttonOk);
            #endregion

            #region Label Filename.
            labelFilename = new Label();
            labelFilename.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            labelFilename.Location = new Point(8, buttonOk.Location.Y - 30);
            labelFilename.Size = new Drawing.Size(64, 22);
            labelFilename.Text = "File: ";
            labelFilename.TextAlign = ContentAlignment.MiddleRight;
            Controls.Add(labelFilename);
            #endregion

            #region Textbox Filename.
            textBoxFilename = new TextBox();
            textBoxFilename.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBoxFilename.Location = new Point(labelFilename.Location.X + labelFilename.Width, labelFilename.Location.Y);
            textBoxFilename.Size = new Drawing.Size(Width - 32 - (buttonOk.Width + 8 + buttonCancel.Width) - labelFilename.Width, 22);
            Controls.Add(textBoxFilename);
            #endregion

            #region Combobox Filter.
            comboFilter = new ComboBox();
            comboFilter.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            comboFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            comboFilter.Size = new Drawing.Size(buttonOk.Width + 8 + buttonCancel.Width, 22);
            comboFilter.Location = new Point(Width - Padding.Right - comboFilter.Width, textBoxFilename.Location.Y);
            Controls.Add(comboFilter);
            #endregion

            #region File Render.
            fileRenderer = new FileRenderer(this);
            fileRenderer.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            fileRenderer.Location = new Point(Padding.Left, buttonBack.Location.Y + buttonBack.Height + 8);
            fileRenderer.Name = "fileRenderer";
            fileRenderer.Size = new Drawing.Size(Width - Padding.Left - Padding.Right, textBoxFilename.Location.Y - buttonBack.Location.Y - buttonBack.Height - 16);
            fileRenderer.OnDirectoryChanged += () =>
            {
                if (fileRenderer.prevPathes.Count > 0)
                    buttonBack.Enabled = true;
                textBoxPath.Text = fileRenderer.currentPath;
            };
            fileRenderer.OnSelectedFileChanged += (file) =>
            {
                textBoxFilename.Text = file.ToString();
            };
            fileRenderer.OnFileOpen += (file) =>
            {
                OpenFile();
            };
            Controls.Add(fileRenderer);
            #endregion

            textBoxPath.Text = fileRenderer.currentPath;
            textBoxPath.TextChanged += (sender, args) =>
            {
                fileRenderer.SetDirectory(textBoxPath.Text);
            };

            fileRenderer.filesTree.NodeMouseClick += filesTree_NodeMouseClick;

            this.AcceptButton = buttonOk;
            this.Shown += FileDialog_Shown;
        }

        protected virtual void ButtonBack()
        {
            buttonBack.Enabled = fileRenderer.Back() && fileRenderer.prevPathes.Count > 0;
            textBoxPath.Text = fileRenderer.currentPath;
        }
        protected virtual void ButtonUp()
        {
            fileRenderer.Up();
        }
        protected virtual void ButtonRefresh()
        {
            fileRenderer.SetDirectory(fileRenderer.currentPath);
        }
        private void FileDialog_Shown(object sender, EventArgs e)
        {
            var fs = Filter.Split('|');
            for (int i = 0; i < fs.Length; i += 2)
                comboFilter.Items.Add(fs[i]);

            if (comboFilter.Items.Count > 0)
            {
                comboFilter.SelectedIndex = 0;
                comboFilter.SelectedIndexChanged += (s, a) =>
                {
                    fileRenderer.SetDirectory(fileRenderer.currentPath);
                };
            }

            fileRenderer.SetDirectory(fileRenderer.currentPath);
            fileRenderer.filesTree.Focus();

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
                itemProperties.Click += (sender2, args) => { new FormFileInfo(fileRenderer.currentPath + "/" + textBoxFilename.Text).Show(); };
                contextMenu.Items.Add(itemProperties);

                contextMenu.Show(null, MousePosition);
            }
        }
        protected override void OnKeyPress(KeyEventArgs e)
        {
            base.OnKeyPress(e);
            if (e.KeyCode == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }

            // Next folder.
            if (e.KeyCode == Keys.Return)
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

            e.Graphics.DrawLine(borderPen, 1, uwfHeaderHeight, Width - 1, uwfHeaderHeight);
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (handleFormSize)
                savedFormSize = Size;
        }
        protected void OpenFile()
        {
            FileName = fileRenderer.currentPath + "/" + textBoxFilename.Text;
            DialogResult = Forms.DialogResult.OK;
            Close();
        }

        #region classes

        internal class FileRenderer : Control
        {
            private readonly FileDialog _owner;
            private string fromFolder;

            public string currentPath;
            public List<string> prevPathes;

            private FileInfo[] currentFiles;
            internal TreeView filesTree;

            public FileRenderer(FileDialog owner)
            {
                _owner = owner;

                prevPathes = new List<string>();

                filesTree = new TreeView();
                filesTree.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
                filesTree.BorderColor = Color.LightGray;
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
                Next();
            }

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
                    OnDirectoryChanged();
                }
                else
                {
                    OnFileOpen(fInfo);
                }
            }
            public void SetDirectory(string path, bool addPrevPath = false)
            {
#if UNITY_STANDALONE
                if (addPrevPath)
                    prevPathes.Add(currentPath);

                if (path.Length <= 2) return;
                if (System.IO.Directory.Exists(path) == false) return;

                currentPath = path.Replace("\\", "/");

                string[] files = null;
                if (_owner.currentFilter == "*.*")
                    files = System.IO.Directory.GetFiles(currentPath, "*.*").Select(f => f.Substring(currentPath.Length)).ToArray();
                else
                    files = System.IO.Directory.GetFiles(currentPath, "*.*").Where(f => _owner.currentFilter.Contains(System.IO.Path.GetExtension(f).ToLower())).Select(f => f.Substring(currentPath.Length)).ToArray();
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
                    var file = currentFiles[i];
                    var fileNode = new TreeNode(file.ToString());
                    fileNode.Tag = file;
                    fileNode.ImageIndex = file.IsDirectory ? 0 : 1;

                    filesTree.Nodes.Add(fileNode);
                    if (fileNode.Text == fromFolder)
                        filesTree.SelectedNode = fileNode;
                }
                filesTree.ExpandAll();

                OnDirectoryChanged();

                fromFolder = null;
#endif
            }
            public void Up()
            {
#if UNITY_STANDALONE
                var parent = System.IO.Directory.GetParent(currentPath);
                if (parent.Exists == false) return;

                prevPathes.Add(currentPath);
                fromFolder = System.IO.Path.GetFileName(currentPath);
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
                uwfResizable = false;
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
                    var changed = text != value;
                    text = value;
                    if (text == null)
                        text = "";

                    if (changed)
                    {
                        DisposeButtons();
                        CreateButtons();
                        OnTextChanged(EventArgs.Empty);
                    }
                }
            }

            private void CreateButtons()
            {
#if UNITY_STANDALONE
                var directories = new List<IO.DirectoryInfo>();
                var currentPath = text;
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

            protected override void OnGotFocus(EventArgs e)
            {
                base.OnGotFocus(e);
                DisposeButtons();
            }
            protected override void OnLostFocus(EventArgs e)
            {
                base.OnLostFocus(e);
                CreateButtons();
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

            private class pathButton : Button
            {
                public string Path { get; private set; }

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
                    arrowButton.Image = Unity.API.ApplicationBehaviour.GdiImages.ArrowRight;
                    arrowButton.uwfImageColor = Color.Gray;
                    arrowButton.uwfImageHoverColor = Color.Gray;

                    Controls.Add(arrowButton);
                }
            }
        }

        #endregion
    }
}
