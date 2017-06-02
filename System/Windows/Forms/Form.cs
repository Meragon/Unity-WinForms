using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    [Serializable]
    public class Form : ContainerControl, IResizableControl
    {
        protected readonly Pen borderPen;

        private Button _closeButton;
        private Action<Form, DialogResult> _dialogCallback;
        private MenuStrip _mainMenuStrip;
        private static Point nextLocation = new Point(128, 64);
        private bool _windowMove = false;
        private Point _windowMove_StartPosition;
        
        private ControlResizeTypes resizeType;
        private Size _resizeOriginal;
        private Point _resizePosition;
        private Point _resizeDelta;
        private const int _resizeOffset = 8;
        private SizeGripStyle sizeGripStyle;
        protected Button uwfSizeGripRenderer;
        private bool _topMost;

        internal bool dialog;

        public IButtonControl AcceptButton { get; set; }
        public Color BorderColor
        {
            get { return borderPen.Color; }
            set { borderPen.Color = value; }
        }
        public Button CloseButton { get { return _closeButton; } }
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
                        _closeButton = null;
                    }
                }
            }
        }
        public DialogResult DialogResult { get; set; }
        public bool IsModal { get { return uwfAppOwner.ModalForms.Contains(this); } }
        public Color HeaderColor { get; set; }
        public Font HeaderFont { get; set; }
        public int HeaderHeight { get; set; }
        public Padding HeaderPadding { get; set; }
        public Color HeaderTextColor { get; set; }
        public ContentAlignment HeaderTextAlign { get; set; }
        public bool KeyPreview { get; set; }
        public MenuStrip MainMenuStrip { get { return _mainMenuStrip; } set { _mainMenuStrip = value; } }
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
            get { return _topMost; }
            set
            {
                _topMost = value;
                uwfAppOwner.Forms.Sort();
            }
        }

        public bool uwfMovable { get; set; }
        public bool uwfResizable { get; set; }

        public Form()
        {
            borderPen = new Pen(Color.White);

            HeaderHeight = 24;
            HeaderPadding = new Padding(32, 0, 32, 0);

            BackColor = Color.FromArgb(238, 238, 242);
            BorderColor = Color.FromArgb(204, 206, 219);
            Font = new Font("Arial", 14);
            Location = nextLocation;
            HeaderColor = Color.FromArgb(238, 238, 242);
            HeaderFont = Font;
            HeaderTextColor = Color.FromArgb(64, 64, 64);
            HeaderTextAlign = ContentAlignment.MiddleLeft;
            ControlBox = true;
            MinimumSize = new Drawing.Size(128, 48);
            uwfMovable = true;
            uwfResizable = true;
            uwfShadowBox = true;
            Size = new Size(334, 260);
            Visible = false;

            uwfAppOwner.UpClick += _Application_UpClick;
            uwfAppOwner.UpdateEvent += Owner_UpdateEvent;

            nextLocation = new Point(nextLocation.X + 26, nextLocation.Y + 26);
            if (nextLocation.X + Width > Screen.PrimaryScreen.WorkingArea.Width - 32)
                nextLocation = new Point(32, nextLocation.Y);
            if (nextLocation.Y + Height > Screen.PrimaryScreen.WorkingArea.Height - 32)
                nextLocation = new Point(nextLocation.X, 32);
        }

        private void _Application_UpClick(object sender, MouseEventArgs e)
        {
            _windowMove = false;
            resizeType = ControlResizeTypes.None;
            if (Application.activeResizeControl == this)
                Application.activeResizeControl = null;
        }
        private void _MakeButtonClose()
        {
            _closeButton = new formSystemButton();

            CloseButton.Anchor = AnchorStyles.Right;
            CloseButton.Text = "";
            if (ApplicationBehaviour.Resources != null && ApplicationBehaviour.GdiImages.Close != null)
                CloseButton.Image = ApplicationBehaviour.GdiImages.Close;
            else
                CloseButton.Text = "X";
            CloseButton.HoverColor = System.Drawing.Color.FromArgb(252, 252, 252);
            CloseButton.BorderHoverColor = System.Drawing.Color.Transparent;
            CloseButton.Location = new Point(Width - 32, 1);
            CloseButton.Name = "buttonClose";
            CloseButton.BackColor = System.Drawing.Color.FromArgb(238, 238, 242);
            CloseButton.BorderColor = System.Drawing.Color.Transparent;
            CloseButton.Size = new System.Drawing.Size(24, 16);
            CloseButton.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            CloseButton.ImageColor = Color.FromArgb(64, 64, 64);
            CloseButton.ImageHoverColor = Color.FromArgb(128, 128, 128);

            CloseButton.BringToFront();
            CloseButton.Click += (o, e) => { Close(); };

            Controls.Add(CloseButton);
        }
        private void _MakeButtonResize()
        {
            var img = ApplicationBehaviour.GdiImages.FormResize;
            if (img == null) return;

            uwfSizeGripRenderer = new ResizeButton(this, img);
            uwfSizeGripRenderer.Location = new Point(Width - img.Width - 2, Height - img.Height - 2);
            Controls.Add(uwfSizeGripRenderer);
        }
        private void Owner_UpdateEvent()
        {
            #region ResizeComponent

            if (resizeType != ControlResizeTypes.None && uwfResizable)
            {
                int estimatedWidth = 0;
                int estimatedHeight = 0;

                switch (resizeType)
                {
                    case ControlResizeTypes.Right:
                        estimatedWidth = _resizeOriginal.Width + (MousePosition.X - _resizeDelta.X);
                        estimatedHeight = _resizeOriginal.Height;
                        break;
                    case ControlResizeTypes.Down:
                        estimatedWidth = _resizeOriginal.Width;
                        estimatedHeight = _resizeOriginal.Height + (MousePosition.Y - _resizeDelta.Y);
                        break;
                    case ControlResizeTypes.RightDown:
                        estimatedWidth = _resizeOriginal.Width + (MousePosition.X - _resizeDelta.X);
                        estimatedHeight = _resizeOriginal.Height + (MousePosition.Y - _resizeDelta.Y);
                        break;
                    case ControlResizeTypes.Left:
                        Location = new Point(_resizePosition.X + (MousePosition.X - _resizeDelta.X), _resizePosition.Y);
                        estimatedWidth = _resizeOriginal.Width + _resizePosition.X - Location.X;
                        estimatedHeight = _resizeOriginal.Height;
                        break;
                    case ControlResizeTypes.Up:
                        Location = new Point(_resizePosition.X, _resizePosition.Y + (MousePosition.Y - _resizeDelta.Y));
                        estimatedWidth = _resizeOriginal.Width;
                        estimatedHeight = _resizeOriginal.Height + _resizePosition.Y - Location.Y;
                        break;
                    case ControlResizeTypes.LeftUp:
                        Location = new Point(
                            _resizePosition.X + (MousePosition.X - _resizeDelta.X),
                            _resizePosition.Y + (MousePosition.Y - _resizeDelta.Y));
                        estimatedWidth = _resizeOriginal.Width + _resizePosition.X - Location.X;
                        estimatedHeight = _resizeOriginal.Height + _resizePosition.Y - Location.Y;
                        break;
                    case ControlResizeTypes.RightUp:
                        Location = new Point(_resizePosition.X, _resizePosition.Y + (MousePosition.Y - _resizeDelta.Y));
                        estimatedWidth = _resizeOriginal.Width + (MousePosition.X - _resizeDelta.X);
                        estimatedHeight = _resizeOriginal.Height + _resizePosition.Y - Location.Y;
                        break;
                    case ControlResizeTypes.LeftDown:
                        Location = new Point(_resizePosition.X + (MousePosition.X - _resizeDelta.X), _resizePosition.Y);
                        estimatedWidth = _resizeOriginal.Width + _resizePosition.X - Location.X;
                        estimatedHeight = _resizeOriginal.Height + (MousePosition.Y - _resizeDelta.Y);
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
        private void _SelectFirstControl()
        {
            for (int i = 0; i < Controls.Count; i++)
                if (Controls[i].CanSelect)
                {
                    Controls[i].Select();
                    return;
                }
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

            if (dialog && _dialogCallback != null)
                _dialogCallback.Invoke(this, this.DialogResult);
        }
        public virtual ControlResizeTypes GetResizeAt(Point mclient)
        {
            if (!uwfResizable) return ControlResizeTypes.None;

            var r_type = ControlResizeTypes.None;

            // Left side.
            if (mclient.X < _resizeOffset)
            {
                r_type = ControlResizeTypes.Left;
                if (mclient.Y < _resizeOffset)
                    r_type = ControlResizeTypes.LeftUp;
                else if (mclient.Y > Height - _resizeOffset)
                    r_type = ControlResizeTypes.LeftDown;
            }
            else if (mclient.X > Width - _resizeOffset)
            {
                // Right side.
                r_type = ControlResizeTypes.Right;
                if (mclient.Y < _resizeOffset)
                    r_type = ControlResizeTypes.RightUp;
                else if (mclient.Y > Height - _resizeOffset)
                    r_type = ControlResizeTypes.RightDown;
            }
            else if (mclient.Y < _resizeOffset)
                r_type = ControlResizeTypes.Up;
            else if (mclient.Y > Height - _resizeOffset)
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
            _resizeDelta = MousePosition;
            _resizeOriginal = Size;
            _resizePosition = Location;

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

            Shown(this, null);
        }
        public DialogResult ShowDialog(Action<Form, DialogResult> onClosed = null)
        {
            dialog = true;
            _dialogCallback = onClosed;

            Visible = true;

            int self = uwfAppOwner.ModalForms.FindIndex(x => x == this);
            if (self == -1)
                uwfAppOwner.ModalForms.Add(this);

            Focus();
            _SelectFirstControl();

            Shown(this, null);

            return this.DialogResult;
        }

        public event FormClosingEventHandler FormClosing = delegate { };
        public event EventHandler Shown = delegate { };

        protected override void Dispose(bool release_all)
        {
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
                        if (e.Location.Y < HeaderHeight)
                        {
                            _windowMove_StartPosition = e.Location;
                            _windowMove = true;
                        }
                }
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_windowMove)
            {
                if (Parent == null)
                    Location = PointToScreen(e.Location).Subtract(_windowMove_StartPosition);
                else
                    Location = Parent.PointToClient(PointToScreen(e.Location).Subtract(_windowMove_StartPosition));
            }
            else
                GetResizeAt(e.Location);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;

            g.uwfFillRectangle(HeaderColor, 0, 0, Width, HeaderHeight);
            g.uwfDrawString(Text, HeaderFont, HeaderTextColor, HeaderPadding.Left, HeaderPadding.Top, Width - HeaderPadding.Right - HeaderPadding.Left, HeaderHeight - HeaderPadding.Bottom - HeaderPadding.Top, HeaderTextAlign);
            g.uwfFillRectangle(BackColor, 0, HeaderHeight, Width, Height - HeaderHeight);
        }
        protected override void OnLatePaint(PaintEventArgs e)
        {
            base.OnLatePaint(e);

            e.Graphics.DrawRectangle(borderPen, 0, 0, Width, Height);
        }
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore(x, y, width, height, specified);
        }

        private class formSystemButton : Button
        {
            public formSystemButton()
            {
                SetStyle(ControlStyles.Selectable, false);
            }
        }
    }

    public delegate void FormClosingEventHandler(object sender, FormClosingEventArgs e);
}
