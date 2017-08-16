namespace System.Windows.Forms
{
    using System.ComponentModel;
    using System.Drawing;

    [Serializable]
    public class Form : ContainerControl, IResizableControl
    {
        internal readonly Pen borderPen = new Pen(Color.White);
        internal readonly Pen innerBorderPen = new Pen(Color.FromArgb(214, 214, 214));
        internal bool dialog;
        internal Color uwfHeaderColor = Color.FromArgb(238, 238, 242);
        internal Font uwfHeaderFont;
        internal int uwfHeaderHeight = 24;
        internal Padding uwfHeaderPadding = new Padding(32, 0, 32, 0);
        internal ContentAlignment uwfHeaderTextAlign;
        internal Color uwfHeaderTextColor = Color.FromArgb(64, 64, 64);
        internal bool uwfMovable = true;

        protected internal Button uwfSizeGripRenderer;

        private const int RESIZE_OFFSET = 8;
        private static readonly Color shadowColor = Color.FromArgb(12, 64, 64, 64);
        private static Point nextLocation = new Point(128, 64);

        private Color backColor = SystemColors.Control;
        private Button closeButton;
        private Action<Form, DialogResult> dialogCallback;
        private MenuStrip mainMenuStrip;
        private bool windowMove;
        private Point windowMove_StartPosition;
        private ControlResizeTypes resizeType;
        private Size resizeOriginal;
        private Point resizePosition;
        private Point resizeDelta;
        private SizeGripStyle sizeGripStyle;
        private bool topMost;

        public Form()
        {
            ControlBox = true;
            Font = SystemFonts.uwfArial_14;
            FormBorderStyle = FormBorderStyle.Sizable;
            Location = nextLocation;
            MinimumSize = new Size(128, 48);
            Visible = false;

            uwfBorderColor = Color.FromArgb(192, 192, 192);
            uwfHeaderFont = Font;
            uwfHeaderTextAlign = ContentAlignment.MiddleLeft;
            uwfShadowBox = true;
            uwfShadowHandler = DrawShadow;
            uwfAppOwner.UpClick += Application_UpClick;
            uwfAppOwner.UpdateEvent += Application_UpdateEvent;

            var workingArea = Screen.PrimaryScreen.WorkingArea;
            nextLocation = new Point(nextLocation.X + 26, nextLocation.Y + 26);
            if (nextLocation.X + Width > workingArea.Width - 32)
                nextLocation = new Point(32, nextLocation.Y);
            if (nextLocation.Y + Height > workingArea.Height - 32)
                nextLocation = new Point(nextLocation.X, 32);
        }

        public event FormClosingEventHandler FormClosing = delegate { };
        public event EventHandler Shown = delegate { };

        public IButtonControl AcceptButton { get; set; }
        public override Color BackColor
        {
            get { return backColor; }
            set { backColor = value; }
        }
        public Button CloseButton { get { return closeButton; } }
        public bool ControlBox
        {
            get
            {
                if (CloseButton == null)
                    return false;
                return true;
            }
            set
            {
                if (value)
                {
                    if (CloseButton == null)
                        _MakeButtonClose();
                }
                else
                {
                    if (CloseButton != null)
                    {
                        int index = Controls.FindIndex(x => x == CloseButton);
                        Controls.RemoveAt(index);

                        CloseButton.Dispose();
                        closeButton = null;
                    }
                }
            }
        }
        public DialogResult DialogResult { get; set; }
        public bool IsModal { get { return uwfAppOwner.ModalForms.Contains(this); } }
        public FormBorderStyle FormBorderStyle { get; set; }
        public bool KeyPreview { get; set; }
        public MenuStrip MainMenuStrip { get { return mainMenuStrip; } set { mainMenuStrip = value; } }
        public SizeGripStyle SizeGripStyle
        {
            get { return sizeGripStyle; }
            set
            {
                if (sizeGripStyle == value) return;

                sizeGripStyle = value;
                if (value == SizeGripStyle.Show && uwfSizeGripRenderer != null && uwfSizeGripRenderer.IsDisposed == false)
                {
                    uwfSizeGripRenderer.Dispose();
                    uwfSizeGripRenderer = null;
                }
                else
                    _MakeButtonResize();
            }
        }
        public override string Text { get; set; }
        public bool TopMost
        {
            get { return topMost; }
            set
            {
                topMost = value;
                uwfAppOwner.Forms.Sort();
            }
        }

        internal Color uwfBorderColor
        {
            get { return borderPen.Color; }
            set { borderPen.Color = value; }
        }

        protected override Size DefaultSize
        {
            get { return new Size(300, 300); }
        }

        public void Close()
        {
            var fc_args = new FormClosingEventArgs(CloseReason.UserClosing, false);
            var oc_args = new CancelEventArgs(false);
            OnClosing(oc_args);
            if (oc_args.Cancel) return;
            FormClosing(this, fc_args);
            if (!fc_args.Cancel)
            {
                OnClosed(null);
                Dispose();
            }

            if (dialog && dialogCallback != null)
                dialogCallback.Invoke(this, DialogResult);
        }
        public virtual ControlResizeTypes GetResizeAt(Point mclient)
        {
            if (!(FormBorderStyle == FormBorderStyle.Sizable || FormBorderStyle == FormBorderStyle.SizableToolWindow)) return ControlResizeTypes.None;

            var r_type = ControlResizeTypes.None;

            // Left side.
            if (mclient.X < RESIZE_OFFSET)
            {
                r_type = ControlResizeTypes.Left;
                if (mclient.Y < RESIZE_OFFSET)
                    r_type = ControlResizeTypes.LeftUp;
                else if (mclient.Y > Height - RESIZE_OFFSET)
                    r_type = ControlResizeTypes.LeftDown;
            }
            else if (mclient.X > Width - RESIZE_OFFSET)
            {
                // Right side.
                r_type = ControlResizeTypes.Right;
                if (mclient.Y < RESIZE_OFFSET)
                    r_type = ControlResizeTypes.RightUp;
                else if (mclient.Y > Height - RESIZE_OFFSET)
                    r_type = ControlResizeTypes.RightDown;
            }
            else if (mclient.Y < RESIZE_OFFSET)
                r_type = ControlResizeTypes.Up;
            else if (mclient.Y > Height - RESIZE_OFFSET)
                r_type = ControlResizeTypes.Down;

            return r_type;
        }
        public void Hide()
        {
            Visible = false;
        }
        public void SetResize(ControlResizeTypes resize)
        {
            resizeType = resize;
            resizeDelta = MousePosition;
            resizeOriginal = Size;
            resizePosition = Location;

            switch (resize)
            {
                case ControlResizeTypes.None:
                    Application.activeResizeControl = null;
                    break;

                default:
                    Application.activeResizeControl = this;
                    break;
            }
        }
        public void Show(bool fShouldFocus = true)
        {
            Visible = true;

            int self = uwfAppOwner.Forms.FindIndex(x => x == this);
            if (self == -1)
                uwfAppOwner.Forms.Add(this);

            if (fShouldFocus)
            {
                Focus();
                _SelectFirstControl();
            }

            OnShown(EventArgs.Empty);
        }
        public DialogResult ShowDialog(Action<Form, DialogResult> onClosed = null)
        {
            dialog = true;
            dialogCallback = onClosed;

            Visible = true;

            int self = uwfAppOwner.ModalForms.FindIndex(x => x == this);
            if (self == -1)
                uwfAppOwner.ModalForms.Add(this);

            Focus();
            _SelectFirstControl();

            OnShown(EventArgs.Empty);

            return DialogResult;
        }

        internal virtual void Application_UpdateEvent()
        {
            #region ResizeComponent

            if (resizeType != ControlResizeTypes.None && (FormBorderStyle == FormBorderStyle.Sizable || FormBorderStyle == FormBorderStyle.SizableToolWindow))
            {
                int estimatedWidth = 0;
                int estimatedHeight = 0;

                switch (resizeType)
                {
                    case ControlResizeTypes.Right:
                        estimatedWidth = resizeOriginal.Width + (MousePosition.X - resizeDelta.X);
                        estimatedHeight = resizeOriginal.Height;
                        break;
                    case ControlResizeTypes.Down:
                        estimatedWidth = resizeOriginal.Width;
                        estimatedHeight = resizeOriginal.Height + (MousePosition.Y - resizeDelta.Y);
                        break;
                    case ControlResizeTypes.RightDown:
                        estimatedWidth = resizeOriginal.Width + (MousePosition.X - resizeDelta.X);
                        estimatedHeight = resizeOriginal.Height + (MousePosition.Y - resizeDelta.Y);
                        break;
                    case ControlResizeTypes.Left:
                        Location = new Point(resizePosition.X + (MousePosition.X - resizeDelta.X), resizePosition.Y);
                        estimatedWidth = resizeOriginal.Width + resizePosition.X - Location.X;
                        estimatedHeight = resizeOriginal.Height;
                        break;
                    case ControlResizeTypes.Up:
                        Location = new Point(resizePosition.X, resizePosition.Y + (MousePosition.Y - resizeDelta.Y));
                        estimatedWidth = resizeOriginal.Width;
                        estimatedHeight = resizeOriginal.Height + resizePosition.Y - Location.Y;
                        break;
                    case ControlResizeTypes.LeftUp:
                        Location = new Point(
                            resizePosition.X + (MousePosition.X - resizeDelta.X),
                            resizePosition.Y + (MousePosition.Y - resizeDelta.Y));
                        estimatedWidth = resizeOriginal.Width + resizePosition.X - Location.X;
                        estimatedHeight = resizeOriginal.Height + resizePosition.Y - Location.Y;
                        break;
                    case ControlResizeTypes.RightUp:
                        Location = new Point(resizePosition.X, resizePosition.Y + (MousePosition.Y - resizeDelta.Y));
                        estimatedWidth = resizeOriginal.Width + (MousePosition.X - resizeDelta.X);
                        estimatedHeight = resizeOriginal.Height + resizePosition.Y - Location.Y;
                        break;
                    case ControlResizeTypes.LeftDown:
                        Location = new Point(resizePosition.X + (MousePosition.X - resizeDelta.X), resizePosition.Y);
                        estimatedWidth = resizeOriginal.Width + resizePosition.X - Location.X;
                        estimatedHeight = resizeOriginal.Height + (MousePosition.Y - resizeDelta.Y);
                        break;
                }

                if (estimatedWidth < MinimumSize.Width)
                    estimatedWidth = MinimumSize.Width;
                if (estimatedHeight < MinimumSize.Height)
                    estimatedHeight = MinimumSize.Height;

                if (MaximumSize.Width > 0 && estimatedWidth > MaximumSize.Width)
                    estimatedWidth = MaximumSize.Width;
                if (MaximumSize.Height > 0 && estimatedHeight > MaximumSize.Height)
                    estimatedHeight = MaximumSize.Height;

                Size = new Size(estimatedWidth, estimatedHeight);


            }
            #endregion
        }
        internal virtual void Application_UpClick(object sender, MouseEventArgs e)
        {
            windowMove = false;
            resizeType = ControlResizeTypes.None;
            if (Application.activeResizeControl == this)
                Application.activeResizeControl = null;
        }

        protected override void Dispose(bool release_all)
        {
            uwfAppOwner.UpClick -= Application_UpClick;
            uwfAppOwner.UpdateEvent -= Application_UpdateEvent;

            if (IsModal == false)
                uwfAppOwner.Forms.Remove(this);
            else
                uwfAppOwner.ModalForms.Remove(this);
            base.Dispose(release_all);
        }
        protected virtual void OnClosed(EventArgs e)
        {
        }
        protected virtual void OnClosing(CancelEventArgs e)
        {
        }
        protected virtual void OnLoad(EventArgs e)
        {
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                resizeType = GetResizeAt(e.Location);
                SetResize(resizeType);

                if (resizeType == ControlResizeTypes.None)
                {
                    // Move then.
                    if (uwfMovable)
                        if (e.Location.Y < uwfHeaderHeight)
                        {
                            windowMove_StartPosition = e.Location;
                            windowMove = true;
                        }
                }
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (windowMove)
            {
                if (Parent == null)
                    Location = PointToScreen(e.Location).Subtract(windowMove_StartPosition);
                else
                    Location = Parent.PointToClient(PointToScreen(e.Location).Subtract(windowMove_StartPosition));
            }
            else
                GetResizeAt(e.Location);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;

            var headerHeight = uwfHeaderHeight;
            var headerPadding = uwfHeaderPadding;
            var width = Width;

            g.uwfFillRectangle(uwfHeaderColor, 0, 0, width, headerHeight);
            g.uwfDrawString(Text, uwfHeaderFont, uwfHeaderTextColor, headerPadding.Left, headerPadding.Top, width - headerPadding.Horizontal, headerHeight - headerPadding.Vertical, uwfHeaderTextAlign);
            g.uwfFillRectangle(BackColor, 0, headerHeight, width, Height - headerHeight);
        }
        protected virtual void OnShown(EventArgs e)
        {
            if (Shown != null)
                Shown(this, e);
        }
        protected override void uwfOnLatePaint(PaintEventArgs e)
        {
            base.uwfOnLatePaint(e);

            e.Graphics.DrawLine(innerBorderPen, 0, uwfHeaderHeight - 1, Width, uwfHeaderHeight - 1);
            e.Graphics.DrawRectangle(borderPen, 0, 0, Width, Height);
        }
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore(x, y, width, height, specified);
        }

        private void DrawShadow(PaintEventArgs e)
        {
            var loc = PointToScreen(Point.Empty);
            var locX = loc.X;
            var locY = loc.Y;
            var width = Width;
            var height = Height;
            var g = e.Graphics;

            g.uwfFillRectangle(shadowColor, locX - 3, locY, width + 6, height + 3);
            g.uwfFillRectangle(shadowColor, locX - 2, locY, width + 4, height + 2);
            g.uwfFillRectangle(shadowColor, locX - 1, locY - 1, width + 2, height + 2);
        }
        private void _MakeButtonClose()
        {
            closeButton = new formSystemButton();

            closeButton.Anchor = AnchorStyles.Right;
            closeButton.Text = "";
            if (uwfAppOwner.Resources != null && uwfAppOwner.Resources.Close != null)
                closeButton.Image = uwfAppOwner.Resources.Close;
            else
                closeButton.Text = "X";
            closeButton.Location = new Point(Width - 32, 1);
            closeButton.Name = "buttonClose";
            closeButton.BackColor = Color.FromArgb(0, 238, 238, 242);
            closeButton.Size = new Size(24, 16);
            closeButton.ForeColor = Color.FromArgb(64, 64, 64);

            closeButton.uwfBorderColor = Color.Transparent;
            closeButton.uwfBorderHoverColor = Color.Transparent;
            closeButton.uwfHoverColor = Color.FromArgb(64, 252, 252, 252);
            closeButton.uwfImageColor = Color.FromArgb(64, 64, 64);
            closeButton.uwfImageHoverColor = Color.FromArgb(128, 128, 128);

            closeButton.BringToFront();
            closeButton.Click += OnCloseButtonOnClick;

            Controls.Add(closeButton);
        }
        private void _MakeButtonResize()
        {
            var img = uwfAppOwner.Resources.FormResize;
            if (img == null) return;

            uwfSizeGripRenderer = new ResizeButton(this, img);
            uwfSizeGripRenderer.Location = new Point(Width - img.Width - 2, Height - img.Height - 2);
            Controls.Add(uwfSizeGripRenderer);
        }
        private void OnCloseButtonOnClick(object o, EventArgs e)
        {
            Close();
        }
        private void _SelectFirstControl()
        {
            for (int i = 0; i < Controls.Count; i++)
                if (Controls[i].CanSelect)
                {
                    Controls[i].Select();
                    return;
                }
        }

        private class formSystemButton : Button
        {
            public formSystemButton()
            {
                SetStyle(ControlStyles.Selectable, false);
            }
        }
    }
}
