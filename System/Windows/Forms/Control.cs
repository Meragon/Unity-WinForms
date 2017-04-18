using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
    [Serializable]
    public class Control : Component, IDisposable
    {
        protected static Color defaultShadowColor = Color.FromArgb(12, 64, 64, 64);

        public static Application DefaultController { get; set; }
        public static Point MousePosition
        {
            get
            {
                return new Point(
                    (int)(UnityEngine.Input.mousePosition.x / Application.ScaleX),
                    (int)((UnityEngine.Screen.height - UnityEngine.Input.mousePosition.y) / Application.ScaleY));
            }
        }
        
        private bool _toggleEditor = true;
        private bool _toggleFont;
        private bool _toggleControls;
        private bool _toggleSource;

        private bool _uwfContext;
        private ControlCollection _controls;
        internal bool selected;
        internal static Control lastSelected;
        private Point _location;
        private int _height;
        internal bool hovered;
        internal bool mouseEntered;
        internal bool shouldFocus;
        private bool _visible;
        private int _width;

        public virtual bool AllowDrop { get; set; }
        public bool AlwaysFocused { get; set; }
        public virtual AnchorStyles Anchor { get; set; }
        public virtual bool AutoSize { get; set; }
        public Color BackColor { get; set; }
        public virtual ImageLayout BackgroundImageLayout { get; set; }
        public Rectangle Bounds
        {
            get { return new Rectangle(Location.X, Location.Y, Width, Height); }
            set
            {
                Location = new Point(value.X, value.Y);
                Size = new Size(value.Width, value.Height);
            }
        }
        public bool CanSelect { get; set; }
        public Rectangle ClientRectangle { get { return new Rectangle(0, 0, Width, Height); } }
        public ControlCollection Controls { get { return _controls; } set { _controls = value; } }
        public virtual Rectangle DisplayRectangle { get { return ClientRectangle; } }
        public bool Disposing { get; private set; }
        public bool Enabled { get; set; }
        public bool Focused { get { return selected; } }
        public virtual Font Font { get; set; }
        public Color ForeColor { get; set; }
        public int Height
        {
            get { return _height; }
            set
            {
                var delta = new Point(0, _height - value);
                _height = value;
                RaiseOnResize(delta);
            }
        }
        public bool Hovered { get { return hovered; } }
        public bool IsDisposed { get; private set; }
        public int Left
        {
            get { return this.Location.X; }
            set { Location = new Point(value, Location.Y); }
        }
        public Point Location
        {
            get { return _location; }
            set
            {
                _location = value;
                OnLocationChanged(null);
            }
        }
        public virtual Size MaximumSize { get; set; }
        public virtual Size MinimumSize { get; set; }
        public string Name { get; set; }
        public Padding Padding { get; set; }
        public Control Parent { get; set; }
        public Size Size
        {
            get
            {
                return new Size(Width, Height);
            }
            set
            {
                int widthBuffer = _width;
                int heightBuffer = _height;

                _width = value.Width;
                _height = value.Height;

                RaiseOnResize(new Point(widthBuffer - value.Width, heightBuffer - value.Height));
            }
        }
        public int TabIndex { get; set; }
        public bool TabStop { get; set; }
        public object Tag { get; set; }
        public virtual string Text { get; set; }
        public int Top
        {
            get { return this.Location.Y; }
            set { Location = new Point(Location.X, value); }
        }
        public bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                VisibleChanged(this, new EventArgs());
            }
        }
        public int Width
        {
            get { return _width; }
            set
            {
                var delta = new Point(_width - value, 0);
                _width = value;
                RaiseOnResize(delta);
            }
        }

        public Application UWF_AppOwner { get; internal set; }
        public bool UWF_AutoGroup { get; set; } // GUI.BeingGroup(...) & GUI.EndGroup()
        public int UWF_Batches { get; internal set; }
        public virtual bool UWF_Context // Close on click control.
        {
            get { return _uwfContext; }
            set
            {
                if (_uwfContext != value)
                {
                    _uwfContext = value;
                    if (_uwfContext)
                        UWF_AppOwner.Contexts.Add(this);
                    else
                        UWF_AppOwner.Contexts.Remove(this);
                }
            }
        }
        public bool UWF_PreventChildDisposing { get; set; }
        public bool UWF_ShadowBox { get; set; }
        public DrawHandler UWF_ShadowHandler { get; set; }

        internal Point UWF_Offset { get; set; }
        internal string UWF_Source { get; set; }

        public Control()
        {
            if (Parent != null && Parent.UWF_AppOwner != null)
                Parent.UWF_AppOwner.Run(this);
            else if (DefaultController != null)
                DefaultController.Run(this);

            Anchor = AnchorStyles.Left | AnchorStyles.Top;
            Controls = new ControlCollection(this);
            Enabled = true;
            Font = SystemFonts.DefaultFont;
            ForeColor = Color.Black;
            TabIndex = -1;
            TabStop = true;
            UWF_AutoGroup = true;
            _visible = true;

#if UNITY_EDITOR
            var stackTrace = UnityEngine.StackTraceUtility.ExtractStackTrace();
            UWF_Source = stackTrace;
#endif
        }

        private void ParentResized(Point delta)
        {
            bool an_right = (Anchor & AnchorStyles.Right) == AnchorStyles.Right;
            bool an_bottom = (Anchor & AnchorStyles.Bottom) == AnchorStyles.Bottom;
            bool an_left = (Anchor & AnchorStyles.Left) == AnchorStyles.Left;
            bool an_top = (Anchor & AnchorStyles.Top) == AnchorStyles.Top;

            if (Anchor != AnchorStyles.None)
            {
                int prevWidth = _width;
                int prevHeight = _height;

                if (an_right)
                    Location = new Point(Location.X - delta.X, Location.Y);
                else
                    delta = new Point(0, delta.Y);
                if (an_bottom)
                    Location = new Point(Location.X, Location.Y - delta.Y);
                else
                    delta = new Point(delta.X, 0);
                if (an_left)
                {
                    if (an_right)
                    {
                        Location = new Point(Location.X + delta.X, Location.Y);
                        _width = Size.Width - delta.X;
                        delta = new Point(delta.X, delta.Y);
                    }
                }
                if (an_top)
                {
                    if (an_bottom)
                    {
                        Location = new Point(Location.X, Location.Y + delta.Y);
                        _height = Size.Height - delta.Y;
                        delta = new Point(delta.X, delta.Y);
                    }
                }

                // Reset delta for childs.
                if (prevWidth - _width == 0)
                    delta = new Point(0, delta.Y);
                if (prevHeight - _height == 0)
                    delta = new Point(delta.X, 0);

                if ((an_left && an_right) || (an_top && an_bottom))
                    RaiseOnResize(delta);
            }
        }

        public void BringToFront()
        {
            if (AlwaysFocused) return;

            if (Parent != null)
            {
                Parent.Controls.Remove(this);
                Parent.Controls.Add(this);
            }

            var form = this as Form ?? Application.GetRootControl(this) as Form;
            if (form != null)
            {
                if (this.UWF_AppOwner.Forms.Contains(form))
                {
                    this.UWF_AppOwner.Forms.Remove(form);
                    this.UWF_AppOwner.Forms.Add(form);
                }
                else if (form.IsModal)
                {
                    this.UWF_AppOwner.ModalForms.Remove(form);
                    this.UWF_AppOwner.ModalForms.Add(form);
                }
            }
        }
        public new virtual void Dispose()
        {
            if (IsDisposed) return;

            Disposing = true;
            OnDisposing(this, EventArgs.Empty);

            if (UWF_PreventChildDisposing == false)
            {
                for (; Controls.Count > 0;)
                    Controls[0].Dispose();

                Controls.Clear();
            }

            if (Parent != null)
            {
                int self = Parent.Controls.FindIndex(x => x == this);
                if (self > -1)
                    Parent.Controls.RemoveAt(self);
            }

            if (UWF_Context)
                UWF_AppOwner.Contexts.Remove(this);

            Disposed(this, null);
            IsDisposed = true;
        }
        public DragDropEffects DoDragDrop(object data, DragDropEffects allowedEffects, DragDropRenderHandler render = null)
        {
            Application.DoDragDrop(data, allowedEffects, render);
            return DragDropEffects.None; // TODO: ?
        }
        public bool Focus()
        {
            return FocusInternal();
        }
        public void Invalidate()
        {

        }
        public void Invalidate(Rectangle rc)
        {
            // Dunno.
        }
        public Point PointToClient(Point p)
        {
            return PointToClient(new PointF(p.X, p.Y));
        }
        public PointF PointToClient(PointF p)
        {
            if (Parent != null)
                p = Parent.PointToClient(p);

            p.X -= Location.X + UWF_Offset.X;
            p.Y -= Location.Y + UWF_Offset.Y;
            return p;
        }
        public Point PointToScreen(Point p)
        {
            if (Parent != null)
                p = Parent.PointToScreen(p);

            p.X += Location.X + UWF_Offset.X;
            p.Y += Location.Y + UWF_Offset.Y;
            return p;
        }
        public virtual void Refresh()
        {
            
        }
        public void ResumeLayout()
        {

        }
        public void Select()
        {
            if (CanSelect == false || lastSelected == this) return;

            if (lastSelected != null && lastSelected.AlwaysFocused == false)
            {
                lastSelected.selected = false;
                lastSelected.InvokeLostFocus(lastSelected, EventArgs.Empty);
            }

            selected = true;
            lastSelected = this;
        }
        public void SuspendLayout()
        {
            // dunno.
        }

        protected void InvokeGotFocus(Control toInvoke, EventArgs e)
        {
            if (toInvoke == null) return;
            toInvoke.OnGotFocus(e);
        }
        protected void InvokeLostFocus(Control toInvoke, EventArgs e)
        {
            if (toInvoke == null)
                return;
            toInvoke.OnLostFocus(e);
        }
        protected virtual void OnClick(EventArgs e)
        {
            Click(this, e);
        }
        protected virtual void OnDoubleClick(EventArgs e)
        {

        }
        protected virtual void OnDragDrop(DragEventArgs drgevent)
        {

        }
        protected virtual void OnDragEnter(DragEventArgs drgevent)
        {

        }
        protected virtual void OnDragLeave(EventArgs e)
        {

        }
        protected virtual void OnGotFocus(EventArgs e)
        {
            if (Parent != null)
                Parent.UWF_ChildGotFocus(this);

            GotFocus(this, e);
        }
        protected virtual void OnKeyDown(KeyEventArgs e)
        {

        }
        protected virtual void OnKeyPress(KeyEventArgs e)
        {

        }
        protected virtual void OnKeyUp(KeyEventArgs e)
        {

        }
        protected virtual void OnLatePaint(PaintEventArgs e)
        {

        }
        protected virtual void OnLocationChanged(EventArgs e)
        {
            LocationChanged(this, EventArgs.Empty);
        }
        protected virtual void OnLostFocus(EventArgs e)
        {
            LostFocus(this, e);
        }
        protected virtual void OnMouseClick(MouseEventArgs e)
        {
            Click(this, e);
        }
        protected virtual void OnMouseDoubleClick(MouseEventArgs e)
        {

        }
        protected virtual void OnMouseDown(MouseEventArgs e)
        {
            Focus();
            MouseDown(this, e);
        }
        protected virtual void OnMouseEnter(EventArgs e)
        {
            MouseEnter(this, e);
            if (Enabled)
                hovered = true;
        }
        protected virtual void OnMouseHover(EventArgs e)
        {
            MouseHover(this, e);
        }
        protected virtual void OnMouseLeave(EventArgs e)
        {
            MouseLeave(this, e);
            hovered = false;
        }
        protected virtual void OnMouseMove(MouseEventArgs e)
        {

        }
        protected virtual void OnMouseUp(MouseEventArgs e)
        {

        }
        protected virtual void OnMouseWheel(MouseEventArgs e)
        {
            // TODO: should I raise in parent?
            // remove base call if you have stack overflow.
            if (Parent != null)
                Parent.RaiseOnMouseWheel(e);
        }
        protected virtual void OnPaint(PaintEventArgs e)
        {

        }
        protected virtual void OnPaintBackground(PaintEventArgs pevent)
        {

        }
        protected virtual void OnResize(Point delta)
        {


        }
        protected virtual void OnTextChanged(EventArgs e)
        {
            TextChanged(this, e);
        }

        protected virtual void UWF_ChildGotFocus(Control child)
        {
            if (Parent == null) return;

            Parent.UWF_ChildGotFocus(child);
        }
        protected virtual object UWF_OnPaintEditor(float width)
        {
            System.Windows.Forms.Control controlToSet = null;

            Editor.BeginGroup(width - 24);

            _toggleEditor = Editor.Foldout("Control (" + GetType().Name + ")", _toggleEditor);
            if (_toggleEditor)
            {
                var editorAllowDrop = Editor.BooleanField("AllowDrop", this.AllowDrop);
                if (editorAllowDrop.Changed) this.AllowDrop = editorAllowDrop;

                var editorAlwaysFocused = Editor.BooleanField("AlwaysFocused", this.AlwaysFocused);
                if (editorAlwaysFocused.Changed) this.AlwaysFocused = editorAlwaysFocused;

                var editorAnchor = Editor.EnumField("Anchor", this.Anchor);
                if (editorAnchor.Changed) this.Anchor = (AnchorStyles)editorAnchor.Value;

                var editorAutoSize = Editor.BooleanField("AutoSize", this.AutoSize);
                if (editorAutoSize.Changed) this.AutoSize = editorAutoSize;

                Editor.ColorField("BackColor", this.BackColor, (c) => { this.BackColor = c; });

                var editorBackgroundImageLayout = Editor.EnumField("BackgroundImageLayout", this.BackgroundImageLayout);
                if (editorBackgroundImageLayout.Changed) this.BackgroundImageLayout = (ImageLayout)editorBackgroundImageLayout.Value;

                var editorCanSelect = Editor.BooleanField("CanSelect", CanSelect);
                if (editorCanSelect.Changed)
                    CanSelect = editorCanSelect.Value;

                Editor.Label("ClientRectangle", this.ClientRectangle);

                if (this.Controls != null && this.Controls.Count > 0)
                {
                    _toggleControls = Editor.Foldout("Controls (" + this.Controls.Count.ToString() + ")", _toggleControls);
                    if (_toggleControls)
                    {
                        Editor.BeginGroup(width - 32);
                        for (int i = 0; i < this.Controls.Count; i++)
                        {
                            var childControl = this.Controls[i];
                            if (Editor.Button(childControl.Name, childControl.GetType().Name))
                                controlToSet = childControl;
                        }
                        Editor.EndGroup();
                    }
                }

                Editor.Label("DisplayRectangle", this.DisplayRectangle);
                Editor.Label("Disposing", this.Disposing);

                var editorEnabled = Editor.BooleanField("Enabled", this.Enabled);
                if (editorEnabled.Changed) Enabled = editorEnabled;

                Editor.Label("Focused", this.Focused);

                _toggleFont = Editor.Foldout("Font", _toggleFont);
                if (_toggleFont)
                {
                    int selectedFont = -1;
                    float size = 12;
                    if (this.Font != null)
                        size = this.Font.Size;
                    System.Drawing.FontStyle style = System.Drawing.FontStyle.Regular;
                    if (this.Font != null)
                        style = this.Font.Style;
                    List<string> fonts = new List<string>();
                    for (int i = 0; i < System.Windows.Forms.ApplicationBehaviour.Resources.Fonts.Count; i++)
                    {
                        fonts.Add(System.Windows.Forms.ApplicationBehaviour.Resources.Fonts[i].fontNames[0]);
                        if (this.Font != null && this.Font.Name.ToLower() == System.Windows.Forms.ApplicationBehaviour.Resources.Fonts[i].fontNames[0].ToLower())
                            selectedFont = i;
                    }

                    var editorFont = Editor.Popup("      Font", selectedFont, fonts.ToArray());
                    if (editorFont.Changed)
                        this.Font = new System.Drawing.Font(System.Windows.Forms.ApplicationBehaviour.Resources.Fonts[editorFont].fontNames[0], size);

                    var editorFontSize = Editor.Slider("      Size", size, 8, 128);
                    if (editorFontSize.Changed)
                    {
                        if (this.Font == null)
                            this.Font = new System.Drawing.Font("Arial", editorFontSize);
                        else
                            this.Font = new System.Drawing.Font(this.Font.Name, editorFontSize);
                    }

                    var editorFontStyle = Editor.EnumField("      Style", style);
                    if (editorFontStyle.Changed)
                    {
                        if (this.Font == null)
                            this.Font = new System.Drawing.Font("Arial", editorFontSize, (System.Drawing.FontStyle)editorFontStyle.Value);
                        else
                            this.Font = new System.Drawing.Font(this.Font.Name, editorFontSize, (System.Drawing.FontStyle)editorFontStyle.Value);
                    }
                }

                Editor.ColorField("ForeColor", this.ForeColor, (c) => { this.ForeColor = c; });

                var editorHeight = Editor.Slider("Height", this.Height, 0, 4096);
                if (editorHeight.Changed) Height = (int)editorHeight.Value;

                Editor.Label("Hovered", this.Hovered);

                Editor.Label("IsDisposed", this.IsDisposed.ToString());

                var editorLocation = Editor.IntField("Location", this.Location.X, this.Location.Y);
                if (editorLocation.Changed) Location = new Point(editorLocation.Value[0], editorLocation.Value[1]);

                var editorMaximumSize = Editor.IntField("MaximumSize", this.MaximumSize.Width, this.MaximumSize.Height);
                if (editorMaximumSize.Changed) this.MaximumSize = new Drawing.Size(editorMaximumSize.Value[0], editorMaximumSize.Value[1]);

                var editorMinimumSize = Editor.IntField("MinimumSize", this.MinimumSize.Width, this.MinimumSize.Height);
                if (editorMinimumSize.Changed) this.MinimumSize = new Drawing.Size(editorMinimumSize.Value[0], editorMinimumSize.Value[1]);

                var editorName = Editor.TextField("Name", Name ?? "");
                if (editorName.Changed) this.Name = editorName;

                var editorPadding = Editor.IntField("Padding", this.Padding.Left, this.Padding.Top, this.Padding.Right, this.Padding.Bottom);
                if (editorPadding.Changed)
                    Padding = new Forms.Padding(editorPadding.Value[0], editorPadding.Value[3], editorPadding.Value[2], editorPadding.Value[1]);

                if (this.Parent != null)
                {
                    if (Editor.Button("Parent", this.Parent.GetType().Name))
                        controlToSet = this.Parent;
                }
                else
                    Editor.Label("Parent", "null");

                var editorSize = Editor.IntField("Size", this.Size.Width, this.Size.Height);
                if (editorSize.Changed) this.Size = new Drawing.Size(editorSize.Value[0], editorSize.Value[1]);
                
                var editorTabIndex = Editor.Slider("TabIndex", this.TabIndex, 0, 255);
                if (editorTabIndex.Changed) this.TabIndex = (int)editorTabIndex.Value;

                string tagText = "null";
                if (Tag != null)
                    tagText = string.Format("[{0}] {1}", Tag.GetType().Name, Tag.ToString());
                Editor.Label("Tag", tagText);

                var editorText = Editor.TextField("Text", Text ?? "");
                if (editorText.Changed) this.Text = editorText;

                Editor.Label("Type", this.GetType());

                var editorVisible = Editor.BooleanField("Visible", this.Visible);
                if (editorVisible.Changed) this.Visible = editorVisible;

                var editorWidth = Editor.Slider("Width", this.Width, 0, 4096);
                if (editorWidth.Changed) this.Width = (int)editorWidth.Value;

                Editor.NewLine(1);

                Editor.Label("UWF_Batches", UWF_Batches);
                Editor.Label("UWF_Context", this.UWF_Context);
                Editor.Label("UWF_Offset", UWF_Offset);

                var editorShadowBox = Editor.BooleanField("UWF_ShadowBox", this.UWF_ShadowBox);
                if (editorShadowBox.Changed) UWF_ShadowBox = editorShadowBox;

                _toggleSource = Editor.Foldout("UWF_Source", _toggleSource);
                if (_toggleSource) Editor.Label(this.UWF_Source);

                Editor.NewLine(1);

                if (Editor.Button("BringToFront()")) BringToFront();
                if (Editor.Button("Dispose()")) Dispose();
                if (Editor.Button("Focus()")) Focus();
                if (Editor.Button("Refresh()")) Refresh();
                if (Editor.Button("Select()")) Select();
            }

            Editor.EndGroup();

            return controlToSet;
        }

        internal Form FindFormInternal()
        {
            Control cur = this;
            while (cur != null && !(cur is Form))
                cur = cur.Parent;

            return (Form)cur;
        }
        internal virtual bool FocusInternal()
        {
            Select();

            var form = this as Form ?? Application.GetRootControl(this) as Form;
            if (form != null) form.BringToFront();

            shouldFocus = true;

            OnGotFocus(EventArgs.Empty);
            return true; // TODO: CanFocus.
        }
        internal void RaiseOnDragDrop(DragEventArgs drgevent)
        {
            OnDragDrop(drgevent);
        }
        internal void RaiseOnDragEnter(DragEventArgs drgevent)
        {
            OnDragEnter(drgevent);
        }
        internal void RaiseOnDragLeave(EventArgs e)
        {
            OnDragLeave(e);
        }
        internal void RaiseOnMouseClick(MouseEventArgs e)
        {
            OnMouseClick(e);
        }
        internal void RaiseOnMouseDoubleClick(MouseEventArgs e)
        {
            OnMouseDoubleClick(e);
            OnDoubleClick(e);
        }
        internal void RaiseOnMouseDown(MouseEventArgs e)
        {
            OnMouseDown(e);
        }
        internal void RaiseOnMouseEnter(EventArgs e)
        {
            OnMouseEnter(e);
        }
        internal void RaiseOnMouseHover(EventArgs e)
        {
            OnMouseHover(e);
        }
        internal void RaiseOnMouseLeave(EventArgs e)
        {
            OnMouseLeave(e);
        }
        internal void RaiseOnMouseMove(MouseEventArgs e)
        {
            OnMouseMove(e);
        }
        internal void RaiseOnMouseUp(MouseEventArgs e)
        {
            OnMouseUp(e);
            MouseUp(this, e);

            if (UWF_AppOwner != null && ApplicationBehaviour.ShowControlProperties && Application.ShowCallback != null)
                Application.ShowCallback.Invoke(this);
        }
        internal void RaiseOnMouseWheel(MouseEventArgs e)
        {
            OnMouseWheel(e);
        }
        internal void RaiseOnKeyDown(KeyEventArgs e)
        {
            OnKeyDown(e);
            KeyDown(this, e);
        }
        internal void RaiseOnKeyPress(KeyEventArgs e)
        {
            OnKeyPress(e);
        }
        internal void RaiseOnKeyUp(KeyEventArgs e)
        {
            OnKeyUp(e);
            KeyUp(this, e);
        }
        internal void RaiseOnPaint(PaintEventArgs e)
        {
            e.Graphics.Control = this;
            UWF_Batches = 0;

            if (UWF_ShadowBox)
            {
                if (UWF_ShadowHandler == null)
                {
                    UWF_ShadowHandler = (pArgs) =>
                    {
                        var psLoc = PointToScreen(Point.Zero);
                        int shX = psLoc.X + 6;
                        int shY = psLoc.Y + 6;
                        var shadowColor = defaultShadowColor;
                        pArgs.Graphics.FillRectangle(shadowColor, shX + 6, shY + 6, Width - 12, Height - 12);
                        pArgs.Graphics.FillRectangle(shadowColor, shX + 5, shY + 5, Width - 10, Height - 10);
                        pArgs.Graphics.FillRectangle(shadowColor, shX + 4, shY + 4, Width - 8, Height - 8);
                        pArgs.Graphics.FillRectangle(shadowColor, shX + 3, shY + 3, Width - 6, Height - 6);
                        pArgs.Graphics.FillRectangle(shadowColor, shX + 2, shY + 2, Width - 4, Height - 4);
                    };
                }

                UWF_ShadowHandler.Invoke(e);
            }

            if (UWF_AutoGroup)
            {
                int gx = e.ClipRectangle.X;
                int gy = e.ClipRectangle.Y;

                if (gx != 0 && gy != 9) // Reset?
                    e.ClipRectangle = new Rectangle(0, 0, Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);

                e.Graphics.Group = new Drawing.Rectangle(gx, gy, Width, Height);
                e.Graphics.GroupBegin(this);
            }
            OnPaintBackground(e);
            OnPaint(e);
            if (Controls != null)
                for (int i = 0; i < Controls.Count; i++)
                {
                    var childControl = Controls[i];
                    if (Application.ControlIsVisible(childControl) == false) continue;

                    var currentAbspos = childControl.PointToScreen(Point.Zero);
                    if (currentAbspos.X + childControl.Width < 0 || currentAbspos.X > Screen.PrimaryScreen.WorkingArea.Width ||
                    currentAbspos.Y + childControl.Height < 0 || currentAbspos.Y > Screen.PrimaryScreen.WorkingArea.Height)
                        continue;

                    e.Graphics.Control = childControl;
                    childControl.RaiseOnPaint(e);
                }
            e.Graphics.Control = this;
            if (Application.Debug)
            {
                e.Graphics.DrawString(GetType().Name, Font, Brushes.White, 3, 1, 256, 32);
                e.Graphics.DrawString(GetType().Name, Font, Brushes.White, 5, 3, 256, 32);
                e.Graphics.DrawString(GetType().Name, Font, Brushes.DarkRed, 4, 2, 256, 32);
                e.Graphics.DrawRectangle(Pens.DarkRed, 0, 0, Width, Height);
            }
            OnLatePaint(e);
            if (UWF_AutoGroup)
                e.Graphics.GroupEnd();

            e.Graphics.Control = null;
        }
        internal object RaiseOnPaintEditor(float width)
        {
            return UWF_OnPaintEditor(width);
        }
        internal void RaiseOnResize(Point delta)
        {
            if (delta != Point.Zero)
                if (Controls != null)
                    for (int i = 0; i < Controls.Count; i++)
                        Controls[i].ParentResized(delta);

            OnResize(delta);
            Resize(this, null);
        }
        internal void UWF_AddjustSizeToScreen(Size delta)
        {
            ParentResized(new Point(delta.Width, delta.Height));
        }

        public event EventHandler Click = delegate { };
        public new event EventHandler Disposed = delegate { };
        public event EventHandler GotFocus = delegate { };
        public event EventHandler LocationChanged = delegate { };
        public event EventHandler LostFocus = delegate { };
        public event KeyEventHandler KeyDown = delegate { };
        public event KeyEventHandler KeyUp = delegate { };
        public event MouseEventHandler MouseDown = delegate { };
        public event EventHandler MouseEnter = delegate { };
        public event EventHandler MouseHover = delegate { };
        public event EventHandler MouseLeave = delegate { };
        public event MouseEventHandler MouseUp = delegate { };
        public event EventHandler OnDisposing = delegate { };
        public event EventHandler Resize = delegate { };
        public event EventHandler TextChanged = delegate { };
        public event EventHandler VisibleChanged = delegate { };

        public delegate void DrawHandler(PaintEventArgs e);
        public delegate void ResizeHandler(Point delta);

        public class ControlCollection : IEnumerator<Control>, IEnumerable<Control>, IList
        {
            private readonly List<Control> _items = new List<Control>();
            private readonly Control _owner;

            public ControlCollection(Control owner)
            {
                _owner = owner;
            }

            public virtual Control this[int index] { get { return _items[index]; } }

            public int Count { get { return _items.Count; } }
            public Control Owner { get { return _owner; } }

            object IEnumerator.Current
            {
                get { return _items.GetEnumerator().Current; }
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return _items.GetEnumerator();
            }
            bool IEnumerator.MoveNext()
            {
                return _items.GetEnumerator().MoveNext();
            }
            public virtual void Add(Control value)
            {
                _items.Add(value);
                value.Parent = _owner;
            }
            public virtual void AddRange(Control[] controls)
            {
                foreach (var c in controls)
                {
                    _items.Add(c);
                    c.Parent = _owner;
                }
            }
            public void Clear()
            {
                _items.Clear();
            }
            public bool Contains(Control control)
            {
                return _items.Contains(control);
            }
            public Control Current
            {
                get { return _items.GetEnumerator().Current; }
            }
            public void Dispose()
            {
                _items.GetEnumerator().Dispose();
            }
            public Control Find(Predicate<Control> match)
            {
                return _items.Find(match);
            }
            public List<Control> FindAll(Predicate<Control> match)
            {
                return _items.FindAll(match);
            }
            public int FindIndex(Predicate<Control> match)
            {
                return _items.FindIndex(match);
            }
            public IEnumerator<Control> GetEnumerator()
            {
                return _items.GetEnumerator();
            }
            public int IndexOf(Control control)
            {
                return _items.IndexOf(control);
            }
            public void Insert(int index, Control value)
            {
                _items.Insert(index, value);
            }
            public virtual void Remove(Control item)
            {
                _items.Remove(item);
            }
            public void RemoveAt(int index)
            {
                _items.RemoveAt(index);
            }
            public void Reset()
            {

            }

            bool IList.IsFixedSize
            {
                get
                {
                    return false;
                }
            }
            bool IList.IsReadOnly
            {
                get
                {
                    return false;
                }
            }
            public bool IsSynchronized
            {
                get
                {
                    return false;
                }
            }
            public object SyncRoot
            {
                get
                {
                    return null;
                }
            }
            object IList.this[int index]
            {
                get
                {
                    return _items[index];
                }
                set
                {
                    if (value is Control)
                        _items[index] = (Control)value;
                }
            }

            int IList.Add(object value)
            {
                var control = value as Control;
                if (control != null)
                {
                    Add(control);
                    return _items.Count - 1;
                }
                return -1;
            }
            bool IList.Contains(object value)
            {
                var control = value as Control;
                if (control != null)
                    return _items.Contains(control);
                return false;
            }
            int IList.IndexOf(object value)
            {
                var control = value as Control;
                if (control != null)
                    return _items.IndexOf(control);
                return -1;
            }
            void IList.Insert(int index, object value)
            {
                var control = value as Control;
                if (control != null)
                    _items.Insert(index, control);
            }
            void IList.Remove(object value)
            {
                var control = value as Control;
                if (control != null)
                    _items.Remove(control);
            }
            public void CopyTo(Array array, int index)
            {
                var controls = array as Control[];
                if (controls != null)
                    _items.CopyTo(controls, index);
            }
        }
    }

    public delegate void DragDropRenderHandler(Graphics g);
}