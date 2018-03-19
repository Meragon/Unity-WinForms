namespace System.Windows.Forms
{
    using System.Drawing;

    [Serializable]
    public class Form : ContainerControl, IResizableControl
    {
        internal readonly Pen borderPen = new Pen(defaultFormBorderColor);
        internal readonly Pen innerBorderPen = new Pen(defaultFormInnerBorderColor);
        internal bool dialog;
        internal bool formMoving;
        internal Color uwfHeaderColor = defaultFormHeaderColor;
        internal Font uwfHeaderFont;
        internal int uwfHeaderHeight = 24;
        internal Padding uwfHeaderPadding = defaultFormPadding;
        internal ContentAlignment uwfHeaderTextAlign;
        internal Color uwfHeaderTextColor = defaultFormHeaderTextColor;
        internal bool uwfMovable = true;

        protected internal Button uwfSizeGripRenderer;

        #region Private vars.

        private const int RESIZE_OFFSET = 8;
        private static readonly Color defaultFormBorderColor = Color.FromArgb(192, 192, 192);
        private static readonly Color defaultFormInnerBorderColor = Color.FromArgb(214, 214, 214);
        private static readonly Color defaultFormHeaderColor = Color.FromArgb(238, 238, 242);
        private static readonly Color defaultFormHeaderTextColor = Color.FromArgb(64, 64, 64);
        private static readonly Padding defaultFormPadding = new Padding(32, 0, 32, 0);
        private static readonly Color defaultFormShadowColor = Color.FromArgb(12, 64, 64, 64);
        private static Point nextLocation = new Point(156, 156);

        private Color backColor = SystemColors.Control;
        private formSystemButton closeButton;
        private Action<Form, DialogResult> dialogCallback;
        private MenuStrip mainMenuStrip;
        private MdiClient mdiClient;
        private Form mdiParent;
        private Point windowMove_StartPosition;
        private Form owner;
        private ControlResizeTypes resizeType;
        private Size resizeOriginal;
        private Point resizePosition;
        private Point resizeDelta;
        private SizeGripStyle sizeGripStyle;
        private FormStartPosition startPosition = FormStartPosition.WindowsDefaultLocation;
        private bool topMost;

        #endregion

        public Form()
        {
            ControlBox = true;
            Font = SystemFonts.uwfArial_14;
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimumSize = new Size(128, 48);
            Visible = false;

            uwfHeaderFont = Font;
            uwfHeaderTextAlign = ContentAlignment.MiddleLeft;
            uwfShadowBox = true;
            uwfShadowHandler = DrawShadow;

            Application.UpdateEvent += Application_UpdateEvent;
            MouseHook.MouseUp += Application_UpClick;
        }

        public event FormClosedEventHandler FormClosed;
        public event FormClosingEventHandler FormClosing;
        public event EventHandler Shown;

        public IButtonControl AcceptButton { get; set; }
        public override Color BackColor
        {
            get { return backColor; }
            set { backColor = value; }
        }
        public IButtonControl CancelButton { get; set; }
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
        public bool IsMdiChild
        {
            get { return mdiParent != null; }
        }
        public bool IsMdiContainer
        {
            get { return mdiClient != null; }
            set
            {
                if (IsMdiContainer == value)
                    return;

                if (value)
                {
                    mdiClient = new MdiClient();
                    mdiClient.Location = new Point(0, uwfHeaderHeight);
                    mdiClient.Size = new Size(Width, Height - uwfHeaderHeight);
                    Controls.Add(mdiClient);
                }
                else
                {
                    mdiClient.Dispose();
                    mdiClient = null;
                }
            }
        }
        public bool IsModal { get { return uwfAppOwner.ModalForms.Contains(this); } }
        public FormBorderStyle FormBorderStyle { get; set; }
        public bool KeyPreview { get; set; }
        public MenuStrip MainMenuStrip { get { return mainMenuStrip; } set { mainMenuStrip = value; } }
        public Form MdiParent
        {
            get { return mdiParent; }
            set
            {
                if (mdiParent == value && (value != null || Parent == null))
                    return;

                if (value == null)
                    Parent = null;
                else
                {
                    mdiParent = value;
                    Parent = value.MdiClient;
                }
            }
        }
        public Form Owner
        {
            get { return owner; }
            set { owner = value; }
        }
        public SizeGripStyle SizeGripStyle
        {
            get { return sizeGripStyle; }
            set
            {
                if (sizeGripStyle == value) return;

                sizeGripStyle = value;
                if (value == SizeGripStyle.Hide)
                {
                    if (uwfSizeGripRenderer != null && uwfSizeGripRenderer.IsDisposed == false)
                        uwfSizeGripRenderer.Dispose();
                    uwfSizeGripRenderer = null;
                }
                else
                    _MakeButtonResize();

                PerformLayout();
            }
        }
        public FormStartPosition StartPosition
        {
            get { return startPosition; }
            set { startPosition = value; }
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

        internal MdiClient MdiClient
        {
            get { return mdiClient; }
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
            CloseInternal(CloseReason.UserClosing);
        }
        public virtual ControlResizeTypes GetResizeAt(Point mclient)
        {
#pragma warning disable 618
            if (!(FormBorderStyle == FormBorderStyle.Sizable || FormBorderStyle == FormBorderStyle.SizableToolWindow)) return ControlResizeTypes.None;
#pragma warning restore 618

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
        public void Show()
        {
            if (Visible)
                return;

            PlaceAtStartPosition(startPosition);

            Visible = true;


            if (MdiParent == null && uwfAppOwner.Forms.Contains(this) == false)
                uwfAppOwner.Forms.Add(this);

            TryFocus();

            OnShown(EventArgs.Empty);
        }
        public void Show(Form owner) // original: public void Show(IWin32Window owner)
        {
            this.owner = owner;

            Show();
        }
        public DialogResult ShowDialog(Action<Form, DialogResult> onClosed = null)
        {
            return ShowDialog(null, onClosed);
        }
        public DialogResult ShowDialog(Form owner, Action<Form, DialogResult> onClosed = null)
        {
            this.owner = owner;

            PlaceAtStartPosition(startPosition);

            dialog = true;
            dialogCallback = onClosed;

            Visible = true;

            int self = uwfAppOwner.ModalForms.FindIndex(x => x == this);
            if (self == -1)
                uwfAppOwner.ModalForms.Add(this);

            TryFocus();

            OnShown(EventArgs.Empty);

            return DialogResult;
        }

        internal virtual void Application_UpdateEvent()
        {
            // ResizeComponent.
#pragma warning disable 618
            if (resizeType != ControlResizeTypes.None && (FormBorderStyle == FormBorderStyle.Sizable || FormBorderStyle == FormBorderStyle.SizableToolWindow))
#pragma warning restore 618
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
        }
        internal virtual void Application_UpClick(object sender, MouseEventArgs e)
        {
            if (formMoving)
            {
                // Fix location here.
                if (Location.Y < 0) // Preventing Form to go over top of rendering window and mdiclient parent.
                    Location = new Point(Location.X, 0);
            }

            formMoving = false;
            resizeType = ControlResizeTypes.None;
            if (Application.activeResizeControl == this)
                Application.activeResizeControl = null;
        }
        internal void CloseInternal(CloseReason closeReason)
        {
            var closingEventArgs = new FormClosingEventArgs(closeReason, false);

            OnFormClosing(closingEventArgs);

            if (!closingEventArgs.Cancel)
            {
                var closedEventArgs = new FormClosedEventArgs(closeReason);
                OnFormClosed(closedEventArgs);
                Dispose();
            }

            // Invoke dialog result action.
            if (dialog && dialogCallback != null)
                dialogCallback.Invoke(this, DialogResult);
        }
        internal void ResetGripRendererLocation()
        {
            if (uwfSizeGripRenderer == null)
                return;

            uwfSizeGripRenderer.Location = new Point(Width - 12, Height - 12); // TODO: img error: Internal_GetWidth.
        }

        protected internal override void uwfOnLatePaint(PaintEventArgs e)
        {
            base.uwfOnLatePaint(e);

            var g = e.Graphics;
            var headerHeight = uwfHeaderHeight;
            var headerPadding = uwfHeaderPadding;
            var width = Width;

            g.uwfFillRectangle(uwfHeaderColor, 0, 0, width, headerHeight);
            g.uwfDrawString(Text, uwfHeaderFont, uwfHeaderTextColor, headerPadding.Left, headerPadding.Top, width - headerPadding.Horizontal, headerHeight - headerPadding.Vertical, uwfHeaderTextAlign);

            // System controls.
            if (closeButton != null)
            {
                closeButton.formPainting = true;
                closeButton.RaiseOnPaint(e);
                closeButton.formPainting = false;
            }

            g.DrawLine(innerBorderPen, 0, uwfHeaderHeight - 1, width, uwfHeaderHeight - 1);
            g.DrawRectangle(borderPen, 0, 0, width, Height);
        }

        protected override Control.ControlCollection CreateControlsInstance()
        {
            return new ControlCollection(this);
        }
        protected override void Dispose(bool release_all)
        {
            MouseHook.MouseUp -= Application_UpClick;
            Application.UpdateEvent -= Application_UpdateEvent;

            if (IsModal == false)
                uwfAppOwner.Forms.Remove(this);
            else
                uwfAppOwner.ModalForms.Remove(this);
            base.Dispose(release_all);
        }
        protected virtual void OnFormClosed(FormClosedEventArgs e)
        {
            var formClosed = FormClosed;
            if (formClosed != null)
                formClosed(this, e);
        }
        protected virtual void OnFormClosing(FormClosingEventArgs e)
        {
            var formClosing = FormClosing;
            if (formClosing != null)
                formClosing(this, e);
        }
        protected virtual void OnLoad(EventArgs e)
        {
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Enter && AcceptButton != null)
                AcceptButton.PerformClick();
            if (e.KeyCode == Keys.Escape && CancelButton != null)
                CancelButton.PerformClick();
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
                            formMoving = true;
                        }
                }
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (formMoving)
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
            var width = Width;

            // Background color.
            g.uwfFillRectangle(BackColor, 0, headerHeight, width, Height - headerHeight);
        }
        protected virtual void OnShown(EventArgs e)
        {
            var shown = Shown;
            if (shown != null)
                shown(this, e);
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

            g.uwfFillRectangle(defaultFormShadowColor, locX - 3, locY, width + 6, height + 3);
            g.uwfFillRectangle(defaultFormShadowColor, locX - 2, locY, width + 4, height + 2);
            g.uwfFillRectangle(defaultFormShadowColor, locX - 1, locY - 1, width + 2, height + 2);
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
            closeButton.uwfSystem = true;

            closeButton.BringToFront();
            closeButton.Click += OnCloseButtonOnClick;

            Controls.Add(closeButton);
        }
        private void _MakeButtonResize()
        {
            var img = uwfAppOwner.Resources.FormResize;
            if (img == null) return;

            uwfSizeGripRenderer = new ResizeButton(this, img);
            uwfSizeGripRenderer.uwfSystem = true;

            Controls.Add(uwfSizeGripRenderer);

            ResetGripRendererLocation();
        }
        private void OnCloseButtonOnClick(object o, EventArgs e)
        {
            Close();
        }
        private void PlaceAtStartPosition(FormStartPosition formStartPosition)
        {
            var workingSize = Screen.PrimaryScreen.WorkingArea.Size;

            if (MdiParent != null)
                workingSize = MdiParent.MdiClient.Size;

            switch (formStartPosition)
            {
                case FormStartPosition.CenterParent:
                    {
                        if (owner != null)
                        {
                            var ex = owner.Location.X + (owner.Width - Width) / 2;
                            var ey = owner.Location.Y + (owner.Height - Height) / 2;

                            if (ex < 0) ex = 0;
                            if (ey < 0) ey = 0;

                            Location = new Point(ex, ey);
                        }
                        else

                            // Use default position.
                            PlaceAtStartPosition(FormStartPosition.WindowsDefaultLocation);
                    }
                    break;
                case FormStartPosition.CenterScreen:
                    {
                        var ex = (workingSize.Width - Width) / 2;
                        var ey = (workingSize.Height - Height) / 2;

                        if (ex < 0) ex = 0;
                        if (ey < 0) ey = 0;

                        Location = new Point(ex, ey);
                    }
                    break;
#pragma warning disable 618
                case FormStartPosition.WindowsDefaultBounds: // TODO: Need size calculation algorithm.
#pragma warning restore 618
                case FormStartPosition.WindowsDefaultLocation:
                    {
                        var ex = nextLocation.X;
                        var ey = nextLocation.Y;

                        if (ex + Width > workingSize.Width)
                            ex = workingSize.Width - Width;
                        if (ey + Height > workingSize.Height)
                            ey = workingSize.Height - Height;

                        if (ex < 0) ex = 0;
                        if (ey < 0) ey = 0;

                        Location = new Point(ex, ey);

                        nextLocation = new Point(nextLocation.X + 26, nextLocation.Y + 26);
                        if (nextLocation.X >= 260) nextLocation = new Point(26, nextLocation.Y);
                        if (nextLocation.Y >= 260) nextLocation = new Point(nextLocation.X, 26);
                    }
                    break;
            }
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
        private bool TryFocus()
        {
            // Check if we should focus form.
            if ((CreateParams.ExStyle & (NativeMethods.WS_DISABLED | NativeMethods.WS_EX_TOOLWINDOW)) == 0)
            {
                Focus();
                _SelectFirstControl();

                return true;
            }

            return false;
        }

        private new class ControlCollection : Control.ControlCollection
        {
            private readonly Form owner;

            public ControlCollection(Form owner) : base(owner)
            {
                this.owner = owner;
            }

            public override void Add(Control value)
            {
                var client = value as MdiClient;
                if (client != null && owner.mdiClient == null)
                    owner.mdiClient = client;

                base.Add(value);

                if (client != null)
                    client.SendToBack();
            }
            public override void Remove(Control item)
            {
                if (item == owner.mdiClient)
                    owner.mdiClient = null;

                base.Remove(item);
            }
        }

        private class formSystemButton : Button
        {
            internal bool formPainting;

            public formSystemButton()
            {
                SetStyle(ControlStyles.Selectable, false);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                if (formPainting)
                    base.OnPaint(e);
            }
        }
    }
}
