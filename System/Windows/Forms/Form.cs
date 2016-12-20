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
        private Pen _borderPen;
        private Button _closeButton;
        private Action<Form, DialogResult> _dialogCallback;
        private MenuStrip _mainMenuStrip;
        private static Point _nextLocation = new Point(128, 64);
        private bool _windowMove = false;
        private Point _windowMoveDelta;

        private bool resizeActive;
        private ControlResizeTypes resizeType;
        private Size _resizeOriginal;
        private Point _resizePosition;
        private Point _resizeDelta;
        private const int _resizeOffset = 8;
        private ControlResizeTypes _resizeShow;
        private float _resizeAlpha;
        protected Button resizeButton;
        private bool resizeIcon;
        private bool _toggleEditor = true;
        private bool _topMost;

        internal bool dialog;

        public IButtonControl AcceptButton { get; set; }
        public Color BorderColor
        {
            get { return _borderPen.Color; }
            set
            {
                if (_borderPen == null)
                    _borderPen = new Pen(value);
                else
                    _borderPen.Color = value;
            }
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
        public bool IsModal { get { return Owner.ModalForms.Contains(this); } }
        public Color HeaderColor { get; set; }
        public Font HeaderFont { get; set; }
        public int HeaderHeight { get; set; }
        public Padding HeaderPadding { get; set; }
        public Color HeaderTextColor { get; set; }
        public ContentAlignment HeaderTextAlign { get; set; }
        public bool HighlightResizeBorders { get; set; }
        public bool KeyPreview { get; set; }
        public MenuStrip MainMenuStrip { get { return _mainMenuStrip; } set { _mainMenuStrip = value; } }
        public bool Movable { get; set; }
        public bool Resizable { get; set; }
        public bool ResizeIcon
        {
            get { return resizeIcon; }
            set
            {
                if (resizeIcon == value) return;

                resizeIcon = value;
                if (value == false && resizeButton != null && resizeButton.IsDisposed == false)
                {
                    resizeButton.Dispose();
                    resizeButton = null;
                }
                else
                    _MakeButtonResize();
            }
        }
        public new Size Size
        {
            get { return base.Size; }
            set
            {
                base.Size = value;
                if (_nextLocation.X + value.Width > Screen.PrimaryScreen.WorkingArea.Width - 32)
                    _nextLocation = new Point(64, _nextLocation.Y);
                if (_nextLocation.Y + value.Height > Screen.PrimaryScreen.WorkingArea.Height - 32)
                    _nextLocation = new Point(_nextLocation.X, 32);
            }
        }
        public override string Text { get; set; }
        public bool TopMost
        {
            get { return _topMost; }
            set
            {
                _topMost = value;
                Owner.Forms.Sort();
            }
        }

        public Form()
        {
            HeaderHeight = 24;
            HeaderPadding = new Padding(32, 0, 32, 0);

            BackColor = Color.FromArgb(238, 238, 242);
            BorderColor = Color.FromArgb(204, 206, 219);
            //BorderColor = Color.FromArgb(155, 159, 185);
            CanSelect = true;
            Font = new Font("Arial", 14);
            Location = _nextLocation;
            HeaderColor = Color.FromArgb(238, 238, 242);
            HeaderFont = Font;
            HeaderTextColor = Color.FromArgb(64, 64, 64);
            HeaderTextAlign = ContentAlignment.MiddleLeft;
            ControlBox = true;
            MinimumSize = new Drawing.Size(128, 48);
            Movable = true;
            Resizable = true;
            ShadowBox = true;
            Size = new Size(334, 260);
            Visible = false;

            Owner.UpClick += _Application_UpClick;
            Owner.UpdateEvent += Owner_UpdateEvent;

            _nextLocation = new Point(_nextLocation.X + 26, _nextLocation.Y + 26);
            if (_nextLocation.X + Width > Screen.PrimaryScreen.WorkingArea.Width - 32)
                _nextLocation = new Point(32, _nextLocation.Y);
            if (_nextLocation.Y + Height > Screen.PrimaryScreen.WorkingArea.Height - 32)
                _nextLocation = new Point(_nextLocation.X, 32);
        }

        private void _Application_UpClick(object sender, MouseEventArgs e)
        {
            _windowMove = false;
            resizeType = ControlResizeTypes.None;
            if (Application.activeResizeControl == this)
                Application.activeResizeControl = null;

            resizeActive = false;
        }
        private void _MakeButtonClose()
        {
            _closeButton = new Button();

            CloseButton.Anchor = AnchorStyles.Right;
            CloseButton.CanSelect = false;
            CloseButton.Text = "";
            if (ApplicationBehaviour.Resources != null && ApplicationBehaviour.Resources.Images.Close != null)
                CloseButton.Image = ApplicationBehaviour.Resources.Images.Close;
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

            CloseButton.BringToFront();
            CloseButton.Click += (o, e) => { Close(); };

            Controls.Add(CloseButton);
        }
        private void _MakeButtonResize()
        {
            var img = ApplicationBehaviour.Resources.Images.FormResize;
            if (img == null) return;

            resizeButton = new ResizeButton(this, img);
            resizeButton.Location = new Point(Width - img.width - 2, Height - img.height - 2);
            Controls.Add(resizeButton);
        }
        private void Owner_UpdateEvent()
        {
            #region Resize
            int estimatedWidth = 0;
            int estimatedHeight = 0;

            if (resizeType != ControlResizeTypes.None && Resizable)
            {
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
        public ControlResizeTypes GetResizeAt(Point location)
        {
            if (!Resizable || ResizeIcon) return ControlResizeTypes.None;

            var r_type = ControlResizeTypes.None;

            // Left side.
            if (location.X < _resizeOffset)
            {
                r_type = ControlResizeTypes.Left;
                if (location.Y < _resizeOffset)
                    r_type = ControlResizeTypes.LeftUp;
                else if (location.Y > Height - _resizeOffset)
                    r_type = ControlResizeTypes.LeftDown;
            }
            else if (location.X > Width - _resizeOffset)
            {
                // Right side.
                r_type = ControlResizeTypes.Right;
                if (location.Y < _resizeOffset)
                    r_type = ControlResizeTypes.RightUp;
                else if (location.Y > Height - _resizeOffset)
                    r_type = ControlResizeTypes.RightDown;
            }
            else if (location.Y < _resizeOffset)
                r_type = ControlResizeTypes.Up;
            else if (location.Y > Height - _resizeOffset)
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
            _resizePosition = PointToScreen(Point.Zero);

            switch (resize)
            {
                case ControlResizeTypes.None:
                    resizeActive = false;
                    Application.activeResizeControl = null;
                    break;

                default:
                    resizeActive = true;
                    Application.activeResizeControl = this;
                    break;
            }
        }
        public void Show(bool shouldFocus = true)
        {
            Visible = true;
            int self = Owner.Forms.FindIndex(x => x == this);
            if (self == -1)
                Owner.Forms.Add(this);
            if (shouldFocus)
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
            int self = Owner.ModalForms.FindIndex(x => x == this);
            if (self == -1)
            {
                Owner.ModalForms.Add(this);
                Shown(this, null);
            }
            Focus();
            _SelectFirstControl();

            return this.DialogResult;
        }

        public event FormClosingEventHandler FormClosing = delegate { };
        public event EventHandler Shown = delegate { };

        public override void Dispose()
        {
            if (IsModal == false)
                Owner.Forms.Remove(this);
            else
                Owner.ModalForms.Remove(this);
            base.Dispose();
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
                    if (Movable)
                        if (e.Location.Y < HeaderHeight)
                        {
                            _windowMoveDelta = e.Location;
                            _windowMove = true;
                        }
                }
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_windowMove)
                Location = PointToScreen(e.Location) - _windowMoveDelta;
            else
                _resizeShow = GetResizeAt(e.Location);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.FillRectangle(HeaderColor, 0, 0, Width, HeaderHeight);
            g.DrawString(Text, HeaderFont, HeaderTextColor, HeaderPadding.Left, HeaderPadding.Top, Width - HeaderPadding.Right - HeaderPadding.Left, HeaderHeight - HeaderPadding.Bottom - HeaderPadding.Top, HeaderTextAlign);
            g.FillRectangle(BackColor, 0, HeaderHeight, Width, Height - HeaderHeight);
        }
        protected override object OnPaintEditor(float width)
        {
            var control = base.OnPaintEditor(width);

            Editor.BeginVertical();
            Editor.NewLine(1);

            _toggleEditor = Editor.Foldout("Form", _toggleEditor);
            if (_toggleEditor)
            {
                Editor.BeginGroup(width - 24);

                Editor.ColorField("      BorderColor", BorderColor, (c) => { BorderColor = c; });

                var editorControlBox = Editor.BooleanField("      ControlBox", ControlBox);
                if (editorControlBox.Changed) ControlBox = editorControlBox;

                Editor.ColorField("      HeaderColor", HeaderColor, (c) => { HeaderColor = c; });

                var editorHeaderHeight = Editor.IntField("      HeaderHeight", HeaderHeight);
                if (editorHeaderHeight.Changed) HeaderHeight = editorHeaderHeight.Value[0];

                Editor.ColorField("      HeaderTextColor", HeaderTextColor, (c) => { HeaderTextColor = c; });

                var editorHeaderTextFormat = Editor.EnumField("      HeaderTextFormat", HeaderTextAlign);
                if (editorHeaderTextFormat.Changed) HeaderTextAlign = (System.Drawing.ContentAlignment)editorHeaderTextFormat.Value;

                var editorHighlightResizeBorders = Editor.BooleanField("      HighlightResizeBorders", HighlightResizeBorders);
                if (editorHighlightResizeBorders.Changed) HighlightResizeBorders = editorHighlightResizeBorders;

                var editorMovable = Editor.BooleanField("      Movable", Movable);
                if (editorMovable.Changed) Movable = editorMovable;

                var editorResizable = Editor.BooleanField("      Resizable", Resizable);
                if (editorResizable.Changed) Resizable = editorResizable;

                var editorTopMost = Editor.BooleanField("      TopMost", TopMost);
                if (editorTopMost.Changed) TopMost = editorTopMost.Value;

                Editor.EndGroup();
            }
            Editor.EndVertical();

            return control;
        }
        protected override void OnLatePaint(PaintEventArgs e)
        {
            base.OnLatePaint(e);

            e.Graphics.DrawRectangle(_borderPen, 0, 0, Width, Height);
        }
    }

    public delegate void FormClosingEventHandler(object sender, FormClosingEventArgs e);
}
