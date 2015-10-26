using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel_Mini;
using System.Drawing;

namespace System.Windows.Forms
{
    public class Form : ContainerControl
    {
        private Button _closeButton;
        private bool _windowMove = false;
        private Point _windowMoveDelta;

        private DNDResizeType resizeType;
        private Size _resizeOriginal;
        private Point _resizePosition;
        private Point _resizeDelta;
        private int _resizeOffset = 8;
        private DNDResizeType _resizeShow;
        private float _resizeAlpha;

#if UNITY_EDITOR
        private bool _toggleEditor = true;
#endif

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
        public Color HeaderColor { get; set; }
        public int HeaderHeight { get; set; }
        public Padding HeaderPadding { get; set; }
        public Color HeaderTextColor { get; set; }
        public ContentAlignment HeaderTextAlign { get; set; }
        public bool HighlightResizeBorders { get; set; }
        public MenuStrip MainMenuStrip { get; set; }
        public bool Movable { get; set; }
        public bool Resizable { get; set; }
        public override string Text { get; set; }

        public Form()
        {
            HeaderHeight = 24;
            HeaderPadding = new Padding(32, 0, 32, 0);

            BackColor = Color.FromArgb(238, 238, 242);
            BorderColor = Color.FromArgb(204, 206, 219);
            //BorderColor = Color.FromArgb(155, 159, 185);
            Font = new Font("Arial", 14);
            Location = new Point(64, 64);
            HeaderColor = Color.FromArgb(238, 238, 242);
            HeaderTextColor = Color.FromArgb(64, 64, 64);
            HeaderTextAlign = ContentAlignment.MiddleLeft;
            ControlBox = true;
            MinimumSize = new Drawing.Size(128, 48);
            Movable = true;
            Resizable = true;
            ShadowBox = true;
            Size = new Size(334, 260);
        }

        private void _MakeButtonClose()
        {
            _closeButton = new Button();

            CloseButton.Anchor = AnchorStyles.Right;
            CloseButton.Image = Application.Resources.Reserved.Close;
            CloseButton.HoverColor = System.Drawing.Color.FromArgb(252, 252, 252);
            CloseButton.HoverBorderColor = System.Drawing.Color.Transparent;
            CloseButton.Location = new Point(Width - 32, 1);
            CloseButton.Name = "buttonClose";
            CloseButton.NormalColor = System.Drawing.Color.FromArgb(238, 238, 242);
            CloseButton.NormalBorderColor = System.Drawing.Color.Transparent;
            CloseButton.Size = new System.Drawing.Size(24, 16);
            CloseButton.Text = "";
            CloseButton.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            CloseButton.TopMost = this.TopMost;

            CloseButton.BringToFront();
            CloseButton.Click += (o, e) => { Close(); };

            Controls.Add(CloseButton);
        }
        private DNDResizeType _ResizeMouseAt(MouseEventArgs e)
        {
            DNDResizeType r_type = DNDResizeType.None;
            if (Resizable)
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
            return r_type;
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
        }
        public void Hide()
        {
            Visible = false;
        }
        public void Show()
        {
            Visible = true;
            int self = Application.Controls.FindIndex(x => x == this);
            if (self == -1)
            {
                Application.Controls.Add(this);
                if (Controls != null)
                    for (int i = 0; i < Controls.Count; i++)
                        Application.Controls.Add(Controls[i]);
            }
        }
        public DialogResult ShowDialog()
        {
            return Forms.DialogResult.Cancel;
        }

        public event FormClosingEventHandler FormClosing = delegate { };

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
            Focus();

            if (e.Button == MouseButtons.Left)
            {
                resizeType = _ResizeMouseAt(e);

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
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _windowMove = false;

            resizeType = DNDResizeType.None;
            /*Batched = false;
            foreach (var c in Controls)
                c.Batched = false;*/
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
                _resizeShow = _ResizeMouseAt(e);
            }

        }
        protected override void OnPaint(PaintEventArgs e)
        {

            Graphics g = e.Graphics;

            #region Resize
            if (resizeType != DNDResizeType.None && Resizable)
            {
                switch (resizeType)
                {
                    case DNDResizeType.Right:
                        Size = new Size(_resizeOriginal.Width + (MousePosition.X - _resizeDelta.X), _resizeOriginal.Height);
                        break;
                    case DNDResizeType.Down:
                        Size = new Size(_resizeOriginal.Width, _resizeOriginal.Height + (MousePosition.Y - _resizeDelta.Y));
                        break;
                    case DNDResizeType.RightDown:
                        Size = new Size(
                            _resizeOriginal.Width + (MousePosition.X - _resizeDelta.X),
                            _resizeOriginal.Height + (MousePosition.Y - _resizeDelta.Y));
                        break;
                    case DNDResizeType.Left:
                        Location = new Point(_resizePosition.X + (MousePosition.X - _resizeDelta.X), _resizePosition.Y);
                        Size = new Size(_resizeOriginal.Width + _resizePosition.X - Location.X, _resizeOriginal.Height);
                        break;
                    case DNDResizeType.Up:
                        Location = new Point(_resizePosition.X, _resizePosition.Y + (MousePosition.Y - _resizeDelta.Y));
                        Size = new Size(_resizeOriginal.Width, _resizeOriginal.Height + _resizePosition.Y - Location.Y);
                        break;
                    case DNDResizeType.LeftUp:
                        Location = new Point(
                            _resizePosition.X + (MousePosition.X - _resizeDelta.X),
                            _resizePosition.Y + (MousePosition.Y - _resizeDelta.Y));
                        Size = new Size(
                            _resizeOriginal.Width + _resizePosition.X - Location.X,
                            _resizeOriginal.Height + _resizePosition.Y - Location.Y);
                        break;
                    case DNDResizeType.RightUp:
                        Location = new Point(_resizePosition.X, _resizePosition.Y + (MousePosition.Y - _resizeDelta.Y));
                        Size = new Size(
                            _resizeOriginal.Width + (MousePosition.X - _resizeDelta.X),
                            _resizeOriginal.Height + _resizePosition.Y - Location.Y);
                        break;
                    case DNDResizeType.LeftDown:
                        Location = new Point(_resizePosition.X + (MousePosition.X - _resizeDelta.X), _resizePosition.Y);
                        Size = new Size(
                            _resizeOriginal.Width + _resizePosition.X - Location.X,
                            _resizeOriginal.Height + (MousePosition.Y - _resizeDelta.Y));
                        break;
                }
                if (Size.Width < MinimumSize.Width)
                    Size = new Size(MinimumSize.Width, Size.Height);
                if (Size.Height < MinimumSize.Height)
                    Size = new Size(Size.Width, MinimumSize.Height);
                if (MaximumSize.Width != 0 && Size.Width > MaximumSize.Width)
                    Size = new Size(MaximumSize.Width, Size.Height);
                if (MaximumSize.Height != 0 && Size.Height > MaximumSize.Height)
                    Size = new Size(Size.Width, MaximumSize.Height);
                //OnResize(new Point(_sizeBefore.Width - Size.Width, _sizeBefore.Height - Size.Height));


            }
            #endregion

            // Draw header.
            //if (ShadowBox)
            //    g.FillRectangle(new SolidBrush(Color.FromArgb(64, 64, 64, 64)), _shadowDistance.X, _shadowDistance.Y, Width, HeaderHeight);


            //if (ShadowBox)
            //    g.FillRectangle(new SolidBrush(Color.FromArgb(64, 64, 64, 64)), _shadowDistance.X, _shadowDistance.Y + HeaderHeight, Width, Height - HeaderHeight);

            g.FillRectangle(new SolidBrush(HeaderColor), 0, 0, Width, HeaderHeight);
            g.DrawString(Text, Font, new SolidBrush(HeaderTextColor), HeaderPadding.Left, HeaderPadding.Top, Width - HeaderPadding.Right - HeaderPadding.Left, HeaderHeight - HeaderPadding.Bottom - HeaderPadding.Top, HeaderTextAlign);
            g.FillRectangle(new SolidBrush(BackColor), 0, HeaderHeight, Width, Height - HeaderHeight);


            base.OnPaint(e);
        }
        protected override object OnPaintEditor(float width)
        {
            var control = base.OnPaintEditor(width);

#if UNITY_EDITOR
            Editor.BeginVertical();
            Editor.NewLine(1);

            _toggleEditor = Editor.Foldout("Form", _toggleEditor);
            if (_toggleEditor)
            {
                Editor.BeginGroup(width - 24);

                var editorBorderColor = Editor.ColorField("      BorderColor", BorderColor);
                if (editorBorderColor.Changed) BorderColor = editorBorderColor;

                var editorControlBox = Editor.BooleanField("      ControlBox", ControlBox);
                if (editorControlBox.Changed) ControlBox = editorControlBox;

                var editorHeaderColor = Editor.ColorField("      HeaderColor", HeaderColor);
                if (editorHeaderColor.Changed) HeaderColor = editorHeaderColor;

                var editorHeaderHeight = Editor.IntField("      HeaderHeight", HeaderHeight);
                if (editorHeaderHeight.Changed) HeaderHeight = editorHeaderHeight.Value[0];

                var editorHeaderTextColor = Editor.ColorField("      HeaderTextColor", HeaderTextColor);
                if (editorHeaderTextColor.Changed) HeaderTextColor = editorHeaderTextColor;

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
#endif

            return control;
        }
        protected override void OnLatePaint(PaintEventArgs e)
        {
            base.OnLatePaint(e);

            e.Graphics.DrawRectangle(new Pen(BorderColor), 0, 0, Width, Height);
            /*if (!AlwaysFocused && !Focused)
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(64, 180, 180, 180)), 0, 0, Width, Height);*/

            var g = e.Graphics;

            // Show resize.
            if (HighlightResizeBorders)
            {
                if (ClientRectangle.Contains(PointToClient(MousePosition)))
                {
                    if (_resizeShow == DNDResizeType.None)
                        _resizeAlpha = MathCustom.FloatLerp(_resizeAlpha, 0, 1);
                    else
                        _resizeAlpha = MathCustom.FloatLerp(_resizeAlpha, 255, 1);
                }
                else
                    _resizeAlpha = MathCustom.FloatLerp(_resizeAlpha, 0, 1);

                //SolidBrush resizeBrush = new SolidBrush(Color.FromArgb((int)_resizeAlpha, 8, 122, 204));
                if (_resizeAlpha > 0)
                {
                    switch (_resizeShow)
                    {
                        case DNDResizeType.Right:
                            //g.FillRectangle(resizeBrush, Width - _resizeOffset, 0, _resizeOffset, Height); 
                            g.DrawTexture(
                                Application.Resources.Reserved.ArrowRight,
                                Width - Application.Resources.Reserved.ArrowRight.width,
                                Height / 2 - Application.Resources.Reserved.ArrowRight.height / 2,
                                Application.Resources.Reserved.ArrowRight.width,
                                Application.Resources.Reserved.ArrowRight.height, 
                                Color.FromArgb((int)_resizeAlpha, Color.White));
                            break;
                        case DNDResizeType.RightDown:
                            g.DrawTexture(
                                Application.Resources.Reserved.ArrowRight,
                                Width - Application.Resources.Reserved.ArrowRight.width,
                                Height / 2 - Application.Resources.Reserved.ArrowRight.height / 2,
                                Application.Resources.Reserved.ArrowRight.width,
                                Application.Resources.Reserved.ArrowRight.height, 
                                Color.FromArgb((int)_resizeAlpha, Color.White)
                                );
                            g.DrawTexture(
                                Application.Resources.Reserved.ArrowDown,
                                Width / 2 - Application.Resources.Reserved.ArrowDown.width / 2,
                                Height - Application.Resources.Reserved.ArrowDown.height,
                                Application.Resources.Reserved.ArrowDown.width,
                                Application.Resources.Reserved.ArrowDown.height,
                                Color.FromArgb((int)_resizeAlpha, Color.White)
                                );
                            //g.FillRectangle(resizeBrush, Width - _resizeOffset, 0, _resizeOffset, Height - _resizeOffset);
                            //g.FillRectangle(resizeBrush, 0, Height - _resizeOffset, Width, _resizeOffset);
                            break;
                        case DNDResizeType.Down:
                            g.DrawTexture(
                                Application.Resources.Reserved.ArrowDown,
                                Width / 2 - Application.Resources.Reserved.ArrowDown.width / 2,
                                Height - Application.Resources.Reserved.ArrowDown.height,
                                Application.Resources.Reserved.ArrowDown.width,
                                Application.Resources.Reserved.ArrowDown.height,
                                Color.FromArgb((int)_resizeAlpha, Color.White)
                                );
                            //g.FillRectangle(resizeBrush, 0, Height - _resizeOffset, Width, _resizeOffset); 
                            break;
                        case DNDResizeType.LeftDown:
                            g.DrawTexture(
                                Application.Resources.Reserved.ArrowLeft,
                                0,
                                Height / 2 - Application.Resources.Reserved.ArrowLeft.height / 2,
                                Application.Resources.Reserved.ArrowLeft.width,
                                Application.Resources.Reserved.ArrowLeft.height,
                                Color.FromArgb((int)_resizeAlpha, Color.White)
                                );
                            g.DrawTexture(
                                Application.Resources.Reserved.ArrowDown,
                                Width / 2 - Application.Resources.Reserved.ArrowDown.width / 2,
                                Height - Application.Resources.Reserved.ArrowDown.height,
                                Application.Resources.Reserved.ArrowDown.width,
                                Application.Resources.Reserved.ArrowDown.height,
                                Color.FromArgb((int)_resizeAlpha, Color.White)
                                );
                            //g.FillRectangle(resizeBrush, 0, 0, _resizeOffset, Height - _resizeOffset);
                            //g.FillRectangle(resizeBrush, 0, Height - _resizeOffset, Width, _resizeOffset);
                            break;
                        case DNDResizeType.Left:
                            g.DrawTexture(
                                Application.Resources.Reserved.ArrowLeft,
                                0,
                                Height / 2 - Application.Resources.Reserved.ArrowLeft.height / 2,
                                Application.Resources.Reserved.ArrowLeft.width,
                                Application.Resources.Reserved.ArrowLeft.height,
                                Color.FromArgb((int)_resizeAlpha, Color.White)
                                );
                            //g.FillRectangle(resizeBrush, 0, 0, _resizeOffset, Height); 
                            break;
                        case DNDResizeType.LeftUp:
                            g.DrawTexture(
                                Application.Resources.Reserved.ArrowLeft,
                                0,
                                Height / 2 - Application.Resources.Reserved.ArrowLeft.height / 2,
                                Application.Resources.Reserved.ArrowLeft.width,
                                Application.Resources.Reserved.ArrowLeft.height,
                                Color.FromArgb((int)_resizeAlpha, Color.White)
                                );
                            g.DrawTexture(
                                Application.Resources.Reserved.ArrowUp,
                                Width / 2 - Application.Resources.Reserved.ArrowUp.width / 2,
                                0,
                                Application.Resources.Reserved.ArrowUp.width,
                                Application.Resources.Reserved.ArrowUp.height,
                                Color.FromArgb((int)_resizeAlpha, Color.White)
                                );
                            //g.FillRectangle(resizeBrush, 0, _resizeOffset, _resizeOffset, Height - _resizeOffset);
                            //g.FillRectangle(resizeBrush, 0, 0, Width, _resizeOffset);
                            break;
                        case DNDResizeType.Up:
                            g.DrawTexture(
                                Application.Resources.Reserved.ArrowUp,
                                Width / 2 - Application.Resources.Reserved.ArrowUp.width / 2,
                                0,
                                Application.Resources.Reserved.ArrowUp.width,
                                Application.Resources.Reserved.ArrowUp.height,
                                Color.FromArgb((int)_resizeAlpha, Color.White)
                                );
                            //g.FillRectangle(resizeBrush, 0, 0, Width, _resizeOffset); 
                            break;
                        case DNDResizeType.RightUp:
                            g.DrawTexture(
                                Application.Resources.Reserved.ArrowRight,
                                Width - Application.Resources.Reserved.ArrowRight.width,
                                Height / 2 - Application.Resources.Reserved.ArrowRight.height / 2,
                                Application.Resources.Reserved.ArrowRight.width,
                                Application.Resources.Reserved.ArrowRight.height,
                                Color.FromArgb((int)_resizeAlpha, Color.White));
                            g.DrawTexture(
                                Application.Resources.Reserved.ArrowUp,
                                Width / 2 - Application.Resources.Reserved.ArrowUp.width / 2,
                                0,
                                Application.Resources.Reserved.ArrowUp.width,
                                Application.Resources.Reserved.ArrowUp.height,
                                Color.FromArgb((int)_resizeAlpha, Color.White)
                                );
                            //g.FillRectangle(resizeBrush, Width - _resizeOffset, _resizeOffset, _resizeOffset, Height - _resizeOffset);
                            //g.FillRectangle(resizeBrush, 0, 0, Width, _resizeOffset);
                            break;
                    }
                }
            }
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
