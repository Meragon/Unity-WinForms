using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    [Serializable]
    public class Form : ContainerControl
    {
        private Button _closeButton;
        private Action<Form, DialogResult> _dialogCallback;
        private MenuStrip _mainMenuStrip;
        private static Point _nextLocation = new Point(128, 64);
        private bool _windowMove = false;
        private Point _windowMoveDelta;

        private DNDResizeType resizeType;
        private Size _resizeOriginal;
        private Point _resizePosition;
        private Point _resizeDelta;
        private int _resizeOffset = 8;
        private DNDResizeType _resizeShow;
        private float _resizeAlpha;
        private bool _toggleEditor = true;

        internal bool dialog;

        public IButtonControl AcceptButton { get; set; }
        public Color BorderColor { get; set; }
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
        public bool ResizeIcon { get; set; }
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
        public bool TopMost { get; set; }

        public Form()
        {
            HeaderHeight = 24;
            HeaderPadding = new Padding(32, 0, 32, 0);

            BackColor = Color.FromArgb(238, 238, 242);
            BorderColor = Color.FromArgb(204, 206, 219);
            //BorderColor = Color.FromArgb(155, 159, 185);
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
            resizeType = DNDResizeType.None;
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
        private void Owner_UpdateEvent()
        {
            #region Resize
            int estimatedWidth = 0;
            int estimatedHeight = 0;

            if (resizeType != DNDResizeType.None && Resizable)
            {
                switch (resizeType)
                {
                    case DNDResizeType.Right:
                        estimatedWidth = _resizeOriginal.Width + (MousePosition.X - _resizeDelta.X);
                        estimatedHeight = _resizeOriginal.Height;
                        break;
                    case DNDResizeType.Down:
                        estimatedWidth = _resizeOriginal.Width;
                        estimatedHeight = _resizeOriginal.Height + (MousePosition.Y - _resizeDelta.Y);
                        break;
                    case DNDResizeType.RightDown:
                        estimatedWidth = _resizeOriginal.Width + (MousePosition.X - _resizeDelta.X);
                        estimatedHeight = _resizeOriginal.Height + (MousePosition.Y - _resizeDelta.Y);
                        break;
                    case DNDResizeType.Left:
                        Location = new Point(_resizePosition.X + (MousePosition.X - _resizeDelta.X), _resizePosition.Y);
                        estimatedWidth = _resizeOriginal.Width + _resizePosition.X - Location.X;
                        estimatedHeight = _resizeOriginal.Height;
                        break;
                    case DNDResizeType.Up:
                        Location = new Point(_resizePosition.X, _resizePosition.Y + (MousePosition.Y - _resizeDelta.Y));
                        estimatedWidth = _resizeOriginal.Width;
                        estimatedHeight = _resizeOriginal.Height + _resizePosition.Y - Location.Y;
                        break;
                    case DNDResizeType.LeftUp:
                        Location = new Point(
                            _resizePosition.X + (MousePosition.X - _resizeDelta.X),
                            _resizePosition.Y + (MousePosition.Y - _resizeDelta.Y));
                        estimatedWidth = _resizeOriginal.Width + _resizePosition.X - Location.X;
                        estimatedHeight = _resizeOriginal.Height + _resizePosition.Y - Location.Y;
                        break;
                    case DNDResizeType.RightUp:
                        Location = new Point(_resizePosition.X, _resizePosition.Y + (MousePosition.Y - _resizeDelta.Y));
                        estimatedWidth = _resizeOriginal.Width + (MousePosition.X - _resizeDelta.X);
                        estimatedHeight = _resizeOriginal.Height + _resizePosition.Y - Location.Y;
                        break;
                    case DNDResizeType.LeftDown:
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
                //OnResize(new Point(_sizeBefore.Width - Size.Width, _sizeBefore.Height - Size.Height));


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
        public void Hide()
        {
            Visible = false;
        }
        public DNDResizeType ResizeMouseAt(MouseEventArgs e)
        {
            DNDResizeType r_type = DNDResizeType.None;
            if (Resizable)
            {
                if (ResizeIcon == false)
                {
                    // Resize.
                    r_type = DNDResizeType.None;

                    // Left side.
                    if (e.X < _resizeOffset)
                    {
                        r_type = DNDResizeType.Left;
                        if (e.Y < _resizeOffset)
                            r_type = DNDResizeType.LeftUp;
                        else if (e.Y > Height - _resizeOffset)
                            r_type = DNDResizeType.LeftDown;
                    }
                    else if (e.X > Width - _resizeOffset)
                    {
                        // Right side.
                        r_type = DNDResizeType.Right;
                        if (e.Y < _resizeOffset)
                            r_type = DNDResizeType.RightUp;
                        else if (e.Y > Height - _resizeOffset)
                            r_type = DNDResizeType.RightDown;
                    }
                    else if (e.Y < _resizeOffset)
                        r_type = DNDResizeType.Up;
                    else if (e.Y > Height - _resizeOffset)
                        r_type = DNDResizeType.Down;
                }
                else
                {
                    var resizeIcon = ApplicationBehaviour.Resources.Images.FormResize;
                    if (resizeIcon != null)
                    {
                        if (new Rectangle(Width - resizeIcon.width - 4, Height - resizeIcon.height - 4, resizeIcon.width + 4, resizeIcon.height + 4).Contains(e.Location))
                            r_type = DNDResizeType.RightDown;
                    }
                }
            }
            return r_type;
        }
        public void SetResize(DNDResizeType resize)
        {
            resizeType = resize;
            _resizeDelta = MousePosition;
            _resizeOriginal = Size;
            _resizePosition = PointToScreen(Point.Zero);
        }
        public void Show()
        {
            Visible = true;
            int self = Owner.Forms.FindIndex(x => x == this);
            if (self == -1)
                Owner.Forms.Add(this);
            Focus();
            _SelectFirstControl();
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
                resizeType = ResizeMouseAt(e);

                if (resizeType != DNDResizeType.None)
                {
                    _resizeDelta = MousePosition;
                    _resizeOriginal = Size;
                    _resizePosition = PointToScreen(Point.Zero);
                }
                else
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
            {
                Location = PointToScreen(e.Location) - _windowMoveDelta;
            }
            else
            {
                _resizeShow = ResizeMouseAt(e);
            }

        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.FillRectangle(new SolidBrush(HeaderColor), 0, 0, Width, HeaderHeight);
            g.DrawString(Text, HeaderFont, new SolidBrush(HeaderTextColor), HeaderPadding.Left, HeaderPadding.Top, Width - HeaderPadding.Right - HeaderPadding.Left, HeaderHeight - HeaderPadding.Bottom - HeaderPadding.Top, HeaderTextAlign);
            g.FillRectangle(new SolidBrush(BackColor), 0, HeaderHeight, Width, Height - HeaderHeight);

            if (Resizable && ResizeIcon && ApplicationBehaviour.Resources.Images.FormResize != null)
            {
                var resizeIcon = ApplicationBehaviour.Resources.Images.FormResize;
                g.DrawTexture(ApplicationBehaviour.Resources.Images.FormResize, Width - resizeIcon.width - 4, Height - resizeIcon.height - 4, resizeIcon.width, resizeIcon.height);
            }
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

                Editor.EndGroup();
            }
            Editor.EndVertical();

            return control;
        }
        protected override void OnLatePaint(PaintEventArgs e)
        {
            base.OnLatePaint(e);

            e.Graphics.DrawRectangle(new Pen(BorderColor), 0, 0, Width, Height);
            /*if (!AlwaysFocused && !Focused)
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(64, 180, 180, 180)), 0, 0, Width, Height);*/

            var g = e.Graphics;

            #region Show resize.
            if (HighlightResizeBorders && !ResizeIcon)
            {
                if (ClientRectangle.Contains(PointToClient(MousePosition)))
                {
                    if (_resizeShow == DNDResizeType.None)
                        _resizeAlpha = MathHelper.FloatLerp(_resizeAlpha, 0, 1);
                    else
                        _resizeAlpha = MathHelper.FloatLerp(_resizeAlpha, 255, 1);
                }
                else
                    _resizeAlpha = MathHelper.FloatLerp(_resizeAlpha, 0, 1);

                //SolidBrush resizeBrush = new SolidBrush(Color.FromArgb((int)_resizeAlpha, 8, 122, 204));
                if (_resizeAlpha > 0)
                {
                    switch (_resizeShow)
                    {
                        case DNDResizeType.Right:
                            //g.FillRectangle(resizeBrush, Width - _resizeOffset, 0, _resizeOffset, Height); 
                            g.DrawTexture(
                                ApplicationBehaviour.Resources.Images.ArrowRight,
                                Width - ApplicationBehaviour.Resources.Images.ArrowRight.width,
                                Height / 2 - ApplicationBehaviour.Resources.Images.ArrowRight.height / 2,
                                ApplicationBehaviour.Resources.Images.ArrowRight.width,
                                ApplicationBehaviour.Resources.Images.ArrowRight.height,
                                Color.FromArgb((int)_resizeAlpha, Color.White));
                            break;
                        case DNDResizeType.RightDown:
                            g.DrawTexture(
                                ApplicationBehaviour.Resources.Images.ArrowRight,
                                Width - ApplicationBehaviour.Resources.Images.ArrowRight.width,
                                Height / 2 - ApplicationBehaviour.Resources.Images.ArrowRight.height / 2,
                                ApplicationBehaviour.Resources.Images.ArrowRight.width,
                                ApplicationBehaviour.Resources.Images.ArrowRight.height,
                                Color.FromArgb((int)_resizeAlpha, Color.White)
                                );
                            g.DrawTexture(
                                ApplicationBehaviour.Resources.Images.ArrowDown,
                                Width / 2 - ApplicationBehaviour.Resources.Images.ArrowDown.width / 2,
                                Height - ApplicationBehaviour.Resources.Images.ArrowDown.height,
                                ApplicationBehaviour.Resources.Images.ArrowDown.width,
                                ApplicationBehaviour.Resources.Images.ArrowDown.height,
                                Color.FromArgb((int)_resizeAlpha, Color.White)
                                );
                            //g.FillRectangle(resizeBrush, Width - _resizeOffset, 0, _resizeOffset, Height - _resizeOffset);
                            //g.FillRectangle(resizeBrush, 0, Height - _resizeOffset, Width, _resizeOffset);
                            break;
                        case DNDResizeType.Down:
                            g.DrawTexture(
                                ApplicationBehaviour.Resources.Images.ArrowDown,
                                Width / 2 - ApplicationBehaviour.Resources.Images.ArrowDown.width / 2,
                                Height - ApplicationBehaviour.Resources.Images.ArrowDown.height,
                                ApplicationBehaviour.Resources.Images.ArrowDown.width,
                                ApplicationBehaviour.Resources.Images.ArrowDown.height,
                                Color.FromArgb((int)_resizeAlpha, Color.White)
                                );
                            //g.FillRectangle(resizeBrush, 0, Height - _resizeOffset, Width, _resizeOffset); 
                            break;
                        case DNDResizeType.LeftDown:
                            g.DrawTexture(
                                ApplicationBehaviour.Resources.Images.ArrowLeft,
                                0,
                                Height / 2 - ApplicationBehaviour.Resources.Images.ArrowLeft.height / 2,
                                ApplicationBehaviour.Resources.Images.ArrowLeft.width,
                                ApplicationBehaviour.Resources.Images.ArrowLeft.height,
                                Color.FromArgb((int)_resizeAlpha, Color.White)
                                );
                            g.DrawTexture(
                                ApplicationBehaviour.Resources.Images.ArrowDown,
                                Width / 2 - ApplicationBehaviour.Resources.Images.ArrowDown.width / 2,
                                Height - ApplicationBehaviour.Resources.Images.ArrowDown.height,
                                ApplicationBehaviour.Resources.Images.ArrowDown.width,
                                ApplicationBehaviour.Resources.Images.ArrowDown.height,
                                Color.FromArgb((int)_resizeAlpha, Color.White)
                                );
                            //g.FillRectangle(resizeBrush, 0, 0, _resizeOffset, Height - _resizeOffset);
                            //g.FillRectangle(resizeBrush, 0, Height - _resizeOffset, Width, _resizeOffset);
                            break;
                        case DNDResizeType.Left:
                            g.DrawTexture(
                                ApplicationBehaviour.Resources.Images.ArrowLeft,
                                0,
                                Height / 2 - ApplicationBehaviour.Resources.Images.ArrowLeft.height / 2,
                                ApplicationBehaviour.Resources.Images.ArrowLeft.width,
                                ApplicationBehaviour.Resources.Images.ArrowLeft.height,
                                Color.FromArgb((int)_resizeAlpha, Color.White)
                                );
                            //g.FillRectangle(resizeBrush, 0, 0, _resizeOffset, Height); 
                            break;
                        case DNDResizeType.LeftUp:
                            g.DrawTexture(
                                ApplicationBehaviour.Resources.Images.ArrowLeft,
                                0,
                                Height / 2 - ApplicationBehaviour.Resources.Images.ArrowLeft.height / 2,
                                ApplicationBehaviour.Resources.Images.ArrowLeft.width,
                                ApplicationBehaviour.Resources.Images.ArrowLeft.height,
                                Color.FromArgb((int)_resizeAlpha, Color.White)
                                );
                            g.DrawTexture(
                                ApplicationBehaviour.Resources.Images.ArrowUp,
                                Width / 2 - ApplicationBehaviour.Resources.Images.ArrowUp.width / 2,
                                0,
                                ApplicationBehaviour.Resources.Images.ArrowUp.width,
                                ApplicationBehaviour.Resources.Images.ArrowUp.height,
                                Color.FromArgb((int)_resizeAlpha, Color.White)
                                );
                            //g.FillRectangle(resizeBrush, 0, _resizeOffset, _resizeOffset, Height - _resizeOffset);
                            //g.FillRectangle(resizeBrush, 0, 0, Width, _resizeOffset);
                            break;
                        case DNDResizeType.Up:
                            g.DrawTexture(
                                ApplicationBehaviour.Resources.Images.ArrowUp,
                                Width / 2 - ApplicationBehaviour.Resources.Images.ArrowUp.width / 2,
                                0,
                                ApplicationBehaviour.Resources.Images.ArrowUp.width,
                                ApplicationBehaviour.Resources.Images.ArrowUp.height,
                                Color.FromArgb((int)_resizeAlpha, Color.White)
                                );
                            //g.FillRectangle(resizeBrush, 0, 0, Width, _resizeOffset); 
                            break;
                        case DNDResizeType.RightUp:
                            g.DrawTexture(
                                ApplicationBehaviour.Resources.Images.ArrowRight,
                                Width - ApplicationBehaviour.Resources.Images.ArrowRight.width,
                                Height / 2 - ApplicationBehaviour.Resources.Images.ArrowRight.height / 2,
                                ApplicationBehaviour.Resources.Images.ArrowRight.width,
                                ApplicationBehaviour.Resources.Images.ArrowRight.height,
                                Color.FromArgb((int)_resizeAlpha, Color.White));
                            g.DrawTexture(
                                ApplicationBehaviour.Resources.Images.ArrowUp,
                                Width / 2 - ApplicationBehaviour.Resources.Images.ArrowUp.width / 2,
                                0,
                                ApplicationBehaviour.Resources.Images.ArrowUp.width,
                                ApplicationBehaviour.Resources.Images.ArrowUp.height,
                                Color.FromArgb((int)_resizeAlpha, Color.White)
                                );
                            //g.FillRectangle(resizeBrush, Width - _resizeOffset, _resizeOffset, _resizeOffset, Height - _resizeOffset);
                            //g.FillRectangle(resizeBrush, 0, 0, Width, _resizeOffset);
                            break;
                    }
                }
            }
            #endregion
        }

        public enum DNDResizeType
        {
            None,

            Right,
            Down,
            Left,
            Up,

            RightDown,
            LeftDown,
            LeftUp,
            RightUp
        }
    }

    public delegate void FormClosingEventHandler(object sender, FormClosingEventArgs e);
}
