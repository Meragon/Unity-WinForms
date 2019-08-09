namespace System.Windows.Forms
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;

    public class Control : Component, IDropTarget
    {
        internal static readonly Color defaultShadowColor = Color.FromArgb(12, 0, 0, 0);
        internal static readonly Color defaultForeColor = Color.FromArgb(64, 64, 64); // SystemColors.ControlText is too dark.
        internal static Control lastSelected;

        internal bool selected;
        internal bool hovered;
        internal bool mouseEntered;
        internal bool shouldFocus;

        internal Application uwfAppOwner;
        internal bool uwfAutoGroup; // Used by GUI.BeginGroup & GUI.EndGroup. Allows to paint controls using their relevant position Rect(0, 0, Width, Height).
        internal bool uwfCanDrawTabDots = true; // Means that Control will draw dots on Tab focus if possible.
        internal bool uwfDrawTabDots; // If 'Tab' key was pressed on any form, Control should be marked to draw dots. Work in progress.
        internal readonly Pen uwfTabPen = new Pen(Color.Black) { DashStyle = DashStyle.Dot };
        internal bool uwfShadowBox;
        internal DrawHandler uwfShadowHandler;
        internal Point uwfOffset; // Position for scrolling. Affects rendering.
        internal bool uwfSystem; // Indicates that Control can be handled by OS (like scrollbars).
        
        private AnchorStyles anchor = AnchorStyles.Top | AnchorStyles.Left;
        private Color backColor = SystemColors.Control;
        private int clientHeight;
        private int clientWidth;
        private ControlCollection controls;
        private ControlStyles controlStyle;
        private CreateParams createParams;
        private bool enabled;
        private Font font = SystemFonts.uwfArial_12;
        private Color foreColor = defaultForeColor;
        private int height;
        private bool fuwfContext;
        private Control parent;
        private string text;
        private bool visible;
        private int width;
        private int x, y;

        public Control()
        {
            if (parent != null && parent.uwfAppOwner != null)
                parent.uwfAppOwner.Run(this);
            else if (uwfDefaultController != null)
                uwfDefaultController.Run(this);

            uwfAutoGroup = true;

            Enabled = true;
            Padding = DefaultPadding;
            TabIndex = -1;
            TabStop = true;
            Visible = true;

            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.StandardClick | ControlStyles.Selectable | ControlStyles.StandardDoubleClick |
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UseTextForAccessibility,
                true);

            var defaultSize = DefaultSize;
            width = defaultSize.Width;
            height = defaultSize.Height;
            clientWidth = width;
            clientHeight = height;
        }

        public delegate void DrawHandler(PaintEventArgs e);
        public delegate void ResizeHandler(Point delta);

        public event EventHandler AutoSizeChanged;
        public event EventHandler BackColorChanged;
        public event EventHandler Click;
        public event EventHandler ClientSizeChanged;
        public event ControlEventHandler ControlAdded;
        public event ControlEventHandler ControlRemoved;
        public event EventHandler DoubleClick;
        public event DragEventHandler DragDrop;
        public event DragEventHandler DragEnter;
        public event EventHandler DragLeave;
        public event EventHandler EnabledChanged;
        public event EventHandler ForeColorChanged;
        public event EventHandler GotFocus;
        public event KeyEventHandler KeyDown;
        public event KeyPressEventHandler KeyPress;
        public event KeyEventHandler KeyUp;
        public event LayoutEventHandler Layout;
        public event EventHandler LocationChanged;
        public event EventHandler LostFocus;
        public event MouseEventHandler MouseClick;
        public event MouseEventHandler MouseDoubleClick;
        public event MouseEventHandler MouseDown;
        public event EventHandler MouseEnter;
        public event EventHandler MouseHover;
        public event EventHandler MouseLeave;
        public event MouseEventHandler MouseMove;
        public event MouseEventHandler MouseUp;
        public event EventHandler ParentChanged;
        public event EventHandler Resize;
        public event EventHandler SizeChanged;
        public event EventHandler TextChanged;
        public event EventHandler VisibleChanged;

        public static Color DefaultBackColor
        {
            get { return SystemColors.Control; }
        }
        public static Point MousePosition
        {
            get { return ApiHolder.System.MousePosition; }
        }

        public virtual bool AllowDrop { get; set; }
        public bool AlwaysFocused { get; set; }
        public virtual AnchorStyles Anchor
        {
            get { return anchor; }
            set { anchor = value; }
        }
        public virtual bool AutoSize { get; set; }
        public virtual Color BackColor
        {
            get { return backColor; }
            set
            {
                if (backColor == value)
                    return;
                
                backColor = value;
                OnBackColorChanged(EventArgs.Empty);
            }
        }
        public virtual Image BackgroundImage { get; set; }
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
        public bool CanSelect
        {
            get { return CanSelectCore(); }
        }
        public Rectangle ClientRectangle { get { return new Rectangle(0, 0, clientWidth, clientHeight); } }
        public Size ClientSize
        {
            get { return new Size(clientWidth, clientHeight); }
            set { Size = value; }
        }
        public ControlCollection Controls
        {
            get
            {
                if (controls == null)
                    controls = CreateControlsInstance();

                return controls;
            }
        }
        public virtual Rectangle DisplayRectangle { get { return ClientRectangle; } }
        public bool Disposing { get; private set; }
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled == value)
                    return;

                enabled = value;

                OnEnabledChanged(EventArgs.Empty);
            }
        }
        public bool Focused { get { return selected; } }
        public virtual Font Font
        {
            get { return font; }
            set { font = value; }
        }
        public virtual Color ForeColor
        {
            get { return foreColor; }
            set
            {
                if (foreColor == value)
                    return;
                
                foreColor = value;
                OnForeColorChanged(EventArgs.Empty);
            }
        }
        public int Height
        {
            get { return height; }
            set { SetBounds(x, y, width, value, BoundsSpecified.Height); }
        }
        public bool IsDisposed { get; private set; }
        public int Left
        {
            get { return Location.X; }
            set { Location = new Point(value, Location.Y); }
        }
        public Point Location
        {
            get { return new Point(x, y); }
            set { SetBounds(value.X, value.Y, width, height, BoundsSpecified.Location); }
        }
        public virtual Size MaximumSize { get; set; }
        public virtual Size MinimumSize { get; set; }
        public string Name { get; set; }
        public Padding Padding { get; set; }
        public Control Parent
        {
            get { return parent; }
            set
            {
                if (parent == value)
                    return;

                if (value != null)
                    value.Controls.Add(this);
                else
                    parent.Controls.Remove(this);
            }
        }
        public Size Size
        {
            get { return new Size(Width, Height); }
            set { SetBounds(x, y, value.Width, value.Height, BoundsSpecified.Size); }
        }
        public int TabIndex { get; set; }
        public bool TabStop { get; set; }
        public object Tag { get; set; }
        public virtual string Text
        {
            get { return text; }
            set
            {
                if (text == value)
                    return;

                text = value;
                OnTextChanged(EventArgs.Empty);
            }
        }
        public int Top
        {
            get { return Location.Y; }
            set { Location = new Point(Location.X, value); }
        }
        public bool Visible
        {
            get { return visible; }
            set { SetVisibleCore(value); }
        }
        public int Width
        {
            get { return width; }
            set { SetBounds(x, y, value, height, BoundsSpecified.Width); }
        }

        internal static Application uwfDefaultController { get; set; }
        internal virtual bool uwfContext // Close on click control.
        {
            get { return fuwfContext; }
            set
            {
                if (fuwfContext == value) return;
                
                fuwfContext = value;
                
                if (fuwfContext)
                    uwfAppOwner.Contexts.Add(this);
                else
                    uwfAppOwner.Contexts.Remove(this);
            }
        }
        internal bool uwfHovered { get { return hovered; } }

        protected virtual CreateParams CreateParams
        {
            get
            {
                if (createParams == null)
                    createParams = new CreateParams();

                createParams.Caption = text;
                createParams.X = x;
                createParams.Y = y;
                createParams.Width = width;
                createParams.Height = height;

                return createParams;
            }
        }
        protected virtual Size DefaultSize
        {
            get { return Size.Empty; }
        }
        protected virtual Padding DefaultPadding
        {
            get { return Padding.Empty; }
        }

        public void BringToFront()
        {
            if (AlwaysFocused) return;

            if (parent != null && 
                parent.controls.Count > 0 && 
                parent.controls[parent.controls.Count - 1] != this)
            {
                var parentControls = parent.controls;
                parentControls.Remove(this);
                parentControls.Add(this);
            }

            var form = this as Form ?? Application.GetRootControl(this) as Form;
            if (form != null)
            {
                Form.ActiveForm = form;
                
                var forms = uwfAppOwner.Forms;
                if (forms.Contains(form))
                {
                    forms.Remove(form);
                    forms.Add(form);
                }
                else if (form.IsModal)
                {
                    var modalForms = uwfAppOwner.ModalForms;
                    modalForms.Remove(form);
                    modalForms.Add(form);
                }
            }
        }
        public DragDropEffects DoDragDrop(object data, DragDropEffects allowedEffects)
        {
            Application.DoDragDrop(data, allowedEffects);
            return allowedEffects; // TODO: ?
        }
        public bool Focus()
        {
            return FocusInternal();
        }
        public void Hide()
        {
            Visible = false;
        }
        public virtual void Invalidate()
        {
        }
        public virtual void Invalidate(Rectangle rc)
        {
        }
        public virtual void Invalidate(Rectangle rc, bool invalidateChildren)
        {
        }
        public void PerformLayout()
        {
            OnLayout(null);
        }
        public Point PointToClient(Point p)
        {
            if (parent != null)
                p = parent.PointToClient(p);

            var localuwfOffset = uwfOffset;

            p.Offset(-x - localuwfOffset.X, -y - localuwfOffset.Y);
            return p;
        }
        public Point PointToScreen(Point p)
        {
            if (parent != null)
                p = parent.PointToScreen(p);

            var localuwfOffset = uwfOffset;

            p.Offset(x + localuwfOffset.X, y + localuwfOffset.Y);
            return p;
        }
        public virtual void Refresh()
        {
        }
        public void ResumeLayout()
        {
        }
        public void ResumeLayout(bool performLayout)
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
        public void SendToBack()
        {
            if (parent != null)
                parent.Controls.SetChildIndex(this, 0);
        }
        public void SetBounds(int argX, int argY, int argWidth, int argHeight)
        {
            if (x != argX || y != argY || (width != argWidth || height != argHeight))
                SetBoundsCore(argX, argY, argWidth, argHeight, BoundsSpecified.All);
        }
        public void SetBounds(int argX, int argY, int argWidth, int argHeight, BoundsSpecified specified)
        {
            if ((specified & BoundsSpecified.X) == BoundsSpecified.None)
                argX = x;
            if ((specified & BoundsSpecified.Y) == BoundsSpecified.None)
                argY = y;
            if ((specified & BoundsSpecified.Width) == BoundsSpecified.None)
                argWidth = width;
            if ((specified & BoundsSpecified.Height) == BoundsSpecified.None)
                argHeight = height;
            if (x != argX || y != argY || (width != argWidth || height != argHeight))
                SetBoundsCore(argX, argY, argWidth, argHeight, specified);
        }
        public void Show()
        {
            Visible = true;
        }
        public void SuspendLayout()
        {
            // dunno.
        }

        void IDropTarget.OnDragEnter(DragEventArgs e)
        {
            OnDragEnter(e);
        }
        void IDropTarget.OnDragDrop(DragEventArgs e)
        {
            OnDragDrop(e);
        }
        void IDropTarget.OnDragLeave(EventArgs e)
        {
            OnDragLeave(e);
        }
        void IDropTarget.OnDragOver(DragEventArgs e)
        {
            OnDragOver(e);
        }

        internal virtual void AssignParent(Control value)
        {
            parent = value;
            OnParentChanged(EventArgs.Empty);
        }
        internal virtual bool CanSelectCore()
        {
            if ((controlStyle & ControlStyles.Selectable) != ControlStyles.Selectable)
                return false;
            for (Control control = this; control != null; control = control.parent)
            {
                if (!control.Enabled || !control.Visible)
                    return false;
            }
            return true;
        }
        internal DragDropEffects DoDragDrop(object data, DragDropEffects allowedEffects, DragDropRenderHandler render)
        {
            Application.DoDragDrop(data, allowedEffects, render);
            return allowedEffects; // TODO: ?
        }
        internal Form FindFormInternal()
        {
            Control cur = this;
            while (cur != null && !(cur is Form))
                cur = cur.parent;

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
        internal void PaintBackground(PaintEventArgs e, Rectangle rectangle)
        {
            PaintBackground(e, rectangle, this.BackColor, Point.Empty);
        }
        internal void PaintBackground(PaintEventArgs e, Rectangle rectangle, Color backColor, Point scrollOffset)
        {
            var bImage = BackgroundImage;
            if (bImage != null) // Fill back & draw image.
                ControlPaint.DrawBackgroundImage(e.Graphics, bImage, backColor, BackgroundImageLayout, ClientRectangle, rectangle);
            else if (backColor.A > 0) // Fill back.
                PaintBackColor(e, rectangle, backColor);
        }
        internal void PerformLayout(Control affectedControl, string affectedProperty)
        {
            OnLayout(new LayoutEventArgs(affectedControl, affectedProperty));
        }
        internal virtual void RaiseOnDragDrop(DragEventArgs drgevent)
        {
            OnDragDrop(drgevent);
        }
        internal virtual void RaiseOnDragEnter(DragEventArgs drgevent)
        {
            OnDragEnter(drgevent);
        }
        internal virtual void RaiseOnDragLeave(EventArgs e)
        {
            OnDragLeave(e);
        }
        internal virtual void RaiseOnLostFocus(EventArgs e)
        {
            OnLostFocus(e);
        }
        internal virtual void RaiseOnMouseClick(MouseEventArgs e)
        {
            OnMouseClick(e);
            OnClick(e);
        }
        internal virtual void RaiseOnMouseDoubleClick(MouseEventArgs e)
        {
            OnMouseDoubleClick(e);
            OnDoubleClick(e);
        }
        internal virtual void RaiseOnMouseDown(MouseEventArgs e)
        {
            OnMouseDown(e);
        }
        internal virtual void RaiseOnMouseEnter(EventArgs e)
        {
            OnMouseEnter(e);
        }
        internal virtual void RaiseOnMouseHover(EventArgs e)
        {
            OnMouseHover(e);
        }
        internal virtual void RaiseOnMouseLeave(EventArgs e)
        {
            OnMouseLeave(e);
        }
        internal virtual void RaiseOnMouseMove(MouseEventArgs e)
        {
            OnMouseMove(e);
        }
        internal virtual void RaiseOnMouseUp(MouseEventArgs e)
        {
            OnMouseUp(e);
        }
        internal virtual void RaiseOnMouseWheel(MouseEventArgs e)
        {
            OnMouseWheel(e);
        }
        internal virtual void RaiseOnKeyDown(KeyEventArgs e)
        {
            OnKeyDown(e);
        }
        internal virtual void RaiseOnKeyPress(KeyPressEventArgs e)
        {
            OnKeyPress(e);
        }
        internal virtual void RaiseOnKeyUp(KeyEventArgs e)
        {
            OnKeyUp(e);
        }
        internal virtual void RaiseOnPaint(PaintEventArgs e)
        {
            if (uwfShadowBox)
            {
                if (uwfShadowHandler == null)
                    PaintShadow(e, this);
                else
                    uwfShadowHandler(e);
            }

            if (uwfAutoGroup)
                e.Graphics.GroupBegin(this);

            OnPaintBackground(e);
            OnPaint(e);

            if (controls != null)
            {
                for (int i = 0; i < controls.Count; i++)
                {
                    var childControl = controls[i];
                    if (Application.ControlIsVisible(childControl) == false) continue;

                    var currentAbspos = childControl.PointToScreen(Point.Empty);
                    var currentAbsposX = currentAbspos.X;
                    var currentAbsposY = currentAbspos.Y;

                    if (currentAbsposX + childControl.width < 0 ||
                        currentAbsposX > Screen.width ||
                        currentAbsposY + childControl.height < 0 ||
                        currentAbsposY > Screen.height)
                        continue;

                    childControl.RaiseOnPaint(e);
                }
            }

            uwfOnLatePaint(e);

            if (uwfAutoGroup)
                e.Graphics.GroupEnd();
        }
        internal virtual Size SizeFromClientSize(int argWidth, int argHeight)
        {
            return new Size(argWidth, argHeight);
        }
        internal virtual void uwfAddjustSizeToScreen(Size delta)
        {
            ParentResized(new Point(delta.Width, delta.Height));
        }

        protected internal virtual void uwfChildGotFocus(Control child)
        {
            if (parent == null) return;

            parent.uwfChildGotFocus(child);
        }
        protected internal virtual void uwfOnLatePaint(PaintEventArgs e)
        {
        }
        protected internal virtual Point uwfShadowPointToScreen(Point p)
        {
            // See Form.
            if (parent != null)
                p = parent.uwfShadowPointToScreen(p);

            var localuwfOffset = uwfOffset;

            p.Offset(x + localuwfOffset.X, y + localuwfOffset.Y);
            return p;
        }

        protected virtual ControlCollection CreateControlsInstance()
        {
            return new ControlCollection(this);
        }
        protected override void Dispose(bool release_all)
        {
            if (IsDisposed) return;

            Disposing = true;

            if (release_all && Controls.IsReadOnly == false)
            {
                while (Controls.Count > 0)
                    Controls[0].Dispose();

                Controls.Clear();
            }

            if (parent != null)
            {
                int self = parent.Controls.FindIndex(control => control == this);
                if (self > -1)
                    parent.Controls.RemoveAt(self);
            }

            if (uwfContext)
                uwfAppOwner.Contexts.Remove(this);

            IsDisposed = true;

            base.Dispose(release_all);
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
        protected virtual void OnAutoSizeChanged(EventArgs e)
        {
            var autoSizeChanged = AutoSizeChanged;
            if (autoSizeChanged != null)
                autoSizeChanged(this, e);
        }
        protected virtual void OnBackColorChanged(EventArgs e)
        {
            var handler = BackColorChanged;
            if (handler != null)
                handler(this, e);
        }
        protected virtual void OnClick(EventArgs e)
        {
            var click = Click;
            if (click != null)
                click(this, e);
        }
        protected virtual void OnClientSizeChanged(EventArgs e)
        {
            var clientSizeChanged = ClientSizeChanged;
            if (clientSizeChanged != null)
                clientSizeChanged(this, e);
        }
        protected virtual void OnControlAdded(ControlEventArgs e)
        {
            var handler = ControlAdded;
            if (handler != null)
                handler(this, e);
        }
        protected virtual void OnControlRemoved(ControlEventArgs e)
        {
            var handler = ControlRemoved;
            if (handler != null)
                handler(this, e);
        }
        protected virtual void OnDoubleClick(EventArgs e)
        {
            var doubleClick = DoubleClick;
            if (doubleClick != null)
                doubleClick(this, e);
        }
        protected virtual void OnDragDrop(DragEventArgs drgevent)
        {
            var dragDrop = DragDrop;
            if (dragDrop != null)
                dragDrop(this, drgevent);
        }
        protected virtual void OnDragEnter(DragEventArgs drgevent)
        {
            var dragEnter = DragEnter;
            if (dragEnter != null)
                dragEnter(this, drgevent);
        }
        protected virtual void OnDragLeave(EventArgs e)
        {
            var dragLeave = DragLeave;
            if (dragLeave != null)
                dragLeave(this, e);
        }
        protected virtual void OnDragOver(DragEventArgs drgevent)
        {
            throw new NotImplementedException();
        }
        protected virtual void OnEnabledChanged(EventArgs e)
        {
            var handler = EnabledChanged;
            if (handler != null)
                handler(this, e);
        }
        protected virtual void OnForeColorChanged(EventArgs e)
        {
            var handler = ForeColorChanged;
            if (handler != null)
                handler(this, e);
        }
        protected virtual void OnGotFocus(EventArgs e)
        {
            if (parent != null)
                parent.uwfChildGotFocus(this);

            var gotFocus = GotFocus;
            if (gotFocus != null)
                gotFocus(this, e);
        }
        protected virtual void OnKeyDown(KeyEventArgs e)
        {
            var keyDown = KeyDown;
            if (keyDown != null)
                keyDown(this, e);
        }
        protected virtual void OnKeyPress(KeyPressEventArgs e)
        {
            var keyPress = KeyPress;
            if (keyPress != null)
                keyPress(this, e);
        }
        protected virtual void OnKeyUp(KeyEventArgs e)
        {
            var keyUp = KeyUp;
            if (keyUp != null)
                keyUp(this, e);
        }
        protected virtual void OnLayout(LayoutEventArgs e)
        {
            var handler = Layout;
            if (handler != null)
                handler(this, e);
        }
        protected virtual void OnLocationChanged(EventArgs e)
        {
            var locationChanged = LocationChanged;
            if (locationChanged != null)
                locationChanged(this, EventArgs.Empty);
            
            var scrollableParent = parent as ScrollableControl;
            if (scrollableParent != null)
                scrollableParent.UpdateScrolls();
        }
        protected virtual void OnLostFocus(EventArgs e)
        {
            var lostFocus = LostFocus;
            if (lostFocus != null)
                lostFocus(this, e);
        }
        protected virtual void OnMouseClick(MouseEventArgs e)
        {
            var mouseClick = MouseClick;
            if (mouseClick != null)
                mouseClick(this, e);
        }
        protected virtual void OnMouseDoubleClick(MouseEventArgs e)
        {
            var mouseDoubleClick = MouseDoubleClick;
            if (mouseDoubleClick != null)
                mouseDoubleClick(this, e);
        }
        protected virtual void OnMouseDown(MouseEventArgs e)
        {
            Focus();

            var mouseDown = MouseDown;
            if (mouseDown != null)
                mouseDown(this, e);
        }
        protected virtual void OnMouseEnter(EventArgs e)
        {
            var mouseEnter = MouseEnter;
            if (mouseEnter != null)
                mouseEnter(this, e);

            if (Enabled)
                hovered = true;
        }
        protected virtual void OnMouseHover(EventArgs e)
        {
            var mouseHover = MouseHover;
            if (mouseHover != null)
                mouseHover(this, e);
        }
        protected virtual void OnMouseLeave(EventArgs e)
        {
            var mouseLeave = MouseLeave;
            if (mouseLeave != null)
                mouseLeave(this, e);

            hovered = false;
        }
        protected virtual void OnMouseMove(MouseEventArgs e)
        {
            var handler = MouseMove;
            if (handler != null) handler(this, e);
        }
        protected virtual void OnMouseUp(MouseEventArgs e)
        {
            var mouseUp = MouseUp;
            if (mouseUp != null)
                mouseUp(this, e);
        }
        protected virtual void OnMouseWheel(MouseEventArgs e)
        {
            // TODO: should I raise in parent?
            // remove base call if you have stack overflow.
            if (parent != null)
                parent.RaiseOnMouseWheel(e);
        }
        protected virtual void OnPaint(PaintEventArgs e)
        {
        }
        protected virtual void OnPaintBackground(PaintEventArgs e)
        {
            PaintBackground(e, ClientRectangle);
        }
        protected virtual void OnParentChanged(EventArgs e)
        {
            var handler = ParentChanged;
            if (handler != null)
                handler(this, e);
        }
        protected virtual void OnResize(EventArgs e)
        {
            var resize = Resize;
            if (resize != null)
                resize(this, e);

            PerformLayout();
        }
        protected virtual void OnSizeChanged(EventArgs e)
        {
            OnResize(EventArgs.Empty);

            var scrollableParent = parent as ScrollableControl;
            if (scrollableParent != null)
                scrollableParent.UpdateScrolls();

            var sizeChanged = SizeChanged;
            if (sizeChanged != null)
                sizeChanged(this, e);
        }
        protected virtual void OnTextChanged(EventArgs e)
        {
            var textChanged = TextChanged;
            if (textChanged != null)
                textChanged(this, e);
        }
        protected virtual void OnVisibleChanged(EventArgs e)
        {
            var visibleChanged = VisibleChanged;
            if (visibleChanged != null)
                visibleChanged(this, e);
        }
        protected virtual void SetBoundsCore(int argX, int argY, int argWidth, int argHeight, BoundsSpecified specified)
        {
            // Maximum size.
            if (MaximumSize.Width > 0 && argWidth > MaximumSize.Width)
                argWidth = MaximumSize.Width;

            if (MaximumSize.Height > 0 && argHeight > MaximumSize.Height)
                argHeight = MaximumSize.Height;

            // Minimum size.
            if (MinimumSize.Width > 0 && argWidth < MinimumSize.Width)
                argWidth = MinimumSize.Width;

            if (MinimumSize.Height > 0 && argHeight < MinimumSize.Height)
                argHeight = MinimumSize.Height;
            
            if (x == argX && y == argY && width == argWidth && height == argHeight)
                return;

            if (parent != null)
                parent.SuspendLayout();

            UpdateBounds(argX, argY, argWidth, argHeight);

            if (parent != null)
                parent.ResumeLayout(true);
        }
        protected virtual void SetClientSizeCore(int argX, int argY)
        {
            Size = SizeFromClientSize(argX, argY);
            clientWidth = argX;
            clientHeight = argY;
            OnClientSizeChanged(EventArgs.Empty);
        }
        protected void SetStyle(ControlStyles flag, bool value)
        {
            controlStyle = value ? controlStyle | flag : controlStyle & ~flag;
        }
        protected virtual void SetVisibleCore(bool value)
        {
            if (visible == value)
                return;

            visible = value;
            OnVisibleChanged(EventArgs.Empty);
        }
        protected virtual Size SizeFromClientSize(Size clientSize)
        {
            return SizeFromClientSize(clientSize.Width, clientSize.Height);
        }
        protected void UpdateBounds(int argX, int argY, int argWidth, int argHeight)
        {
            int cWidth = argWidth;
            int cHeight = argHeight;
            
            UpdateBounds(argX, argY, argWidth, argHeight, cWidth, cHeight);
        }
        protected void UpdateBounds(int argX, int argY, int argWidth, int argHeight, int argClientWidth, int argClientHeight)
        {
            int widthBuffer = width;
            int heightBuffer = height;

            bool locationFlag = x != argX || y != argY;
            bool sizeFlag = Width != argWidth || Height != argHeight || clientWidth != argClientWidth || clientHeight != argClientHeight;

            x = argX;
            y = argY;
            width = argWidth;
            height = argHeight;
            clientWidth = argClientWidth;
            clientHeight = argClientHeight;

            if (locationFlag)
                OnLocationChanged(EventArgs.Empty);
            if (!sizeFlag)
                return;

            var delta = new Point(widthBuffer - argWidth, heightBuffer - argHeight);
            ResizeChilds(delta);

            OnSizeChanged(EventArgs.Empty);
            OnClientSizeChanged(EventArgs.Empty);

            PerformLayout();
        }

        private static void PaintBackColor(PaintEventArgs e, Rectangle rectangle, Color backColor)
        {
            e.Graphics.uwfFillRectangle(backColor, rectangle);
        }
        private static void PaintShadow(PaintEventArgs pArgs, Control c)
        {
            var psLoc = c.uwfShadowPointToScreen(Point.Empty);
            int shX = psLoc.X + 6;
            int shY = psLoc.Y + 6;
            var shadowColor = defaultShadowColor;
            var localWidth = c.Width;
            var localHeight = c.Height;
            var graphics = pArgs.Graphics;
            graphics.uwfFillRectangle(shadowColor, shX + 6, shY + 6, localWidth - 12, localHeight - 12);
            graphics.uwfFillRectangle(shadowColor, shX + 5, shY + 5, localWidth - 10, localHeight - 10);
            graphics.uwfFillRectangle(shadowColor, shX + 4, shY + 4, localWidth - 8, localHeight - 8);
            graphics.uwfFillRectangle(shadowColor, shX + 3, shY + 3, localWidth - 6, localHeight - 6);
            graphics.uwfFillRectangle(shadowColor, shX + 2, shY + 2, localWidth - 4, localHeight - 4);
        }

        private void ParentResized(Point delta)
        {
            if (Anchor == AnchorStyles.None) return;

            bool an_right = (Anchor & AnchorStyles.Right) == AnchorStyles.Right;
            bool an_bottom = (Anchor & AnchorStyles.Bottom) == AnchorStyles.Bottom;
            bool an_left = (Anchor & AnchorStyles.Left) == AnchorStyles.Left;
            bool an_top = (Anchor & AnchorStyles.Top) == AnchorStyles.Top;

            int nextX = x;
            int nextY = y;
            int nextWidth = width;
            int nextHeight = height;

            if (an_right)
                nextX -= delta.X;
            else
                delta = new Point(0, delta.Y);

            if (an_bottom)
                nextY -= delta.Y;
            else
                delta = new Point(delta.X, 0);

            if (an_left)
            {
                if (an_right)
                {
                    nextX += delta.X;
                    nextWidth -= delta.X;
                }
            }
            if (an_top)
            {
                if (an_bottom)
                {
                    nextY += delta.Y;
                    nextHeight -= delta.Y;
                }
            }

            UpdateBounds(nextX, nextY, nextWidth, nextHeight);
        }
        private void ResizeChilds(Point delta)
        {
            if (delta == Point.Empty) return;
            if (Controls == null) return;

            for (int i = 0; i < Controls.Count; i++)
                Controls[i].ParentResized(delta);
        }

        public class ControlCollection : IList, ICloneable
        {
            private readonly List<Control> items = new List<Control>();
            private readonly Control owner;

            private int lastAccessedIndex = -1;
            private object syncRoot;

            public ControlCollection(Control owner)
            {
                this.owner = owner;
            }

            public Control Current
            {
                get { return items.GetEnumerator().Current; }
            }
            public int Count { get { return items.Count; } }
            public Control Owner { get { return owner; } }
            bool IList.IsFixedSize
            {
                get
                {
                    return false;
                }
            }
            public virtual bool IsReadOnly { get { return false; } }
            bool IList.IsReadOnly
            {
                get
                {
                    return IsReadOnly;
                }
            }
            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }
            object ICollection.SyncRoot
            {
                get
                {
                    if (syncRoot == null)
                        syncRoot = new object();

                    return syncRoot;
                }
            }

            object IList.this[int index]
            {
                get
                {
                    return items[index];
                }
                set
                {
                    if (value is Control)
                        items[index] = (Control)value;
                }
            }
            public virtual Control this[int index] { get { return items[index]; } }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return items.GetEnumerator();
            }
            public virtual void Add(Control value)
            {
                if (value == null)
                    return;

                items.Add(value);
                value.AssignParent(owner);

                owner.PerformLayout();
                owner.OnControlAdded(new ControlEventArgs(value));
            }
            public virtual void AddRange(Control[] controls)
            {
                foreach (var c in controls)
                    Add(c);
            }
            public virtual void Clear()
            {
                while (Count != 0)
                    RemoveAt(Count - 1);
            }
            public bool Contains(Control control)
            {
                return items.Contains(control);
            }
            public void CopyTo(Array array, int index)
            {
                var controls = array as Control[];
                if (controls != null)
                    items.CopyTo(controls, index);
            }
            public Control Find(Predicate<Control> match)
            {
                return items.Find(match);
            }
            public List<Control> FindAll(Predicate<Control> match)
            {
                return items.FindAll(match);
            }
            public int FindIndex(Predicate<Control> match)
            {
                return items.FindIndex(match);
            }
            public int GetChildIndex(Control child)
            {
                return GetChildIndex(child, true);
            }
            public virtual int GetChildIndex(Control child, bool throwException)
            {
                var index = IndexOf(child);
                if (index == -1 && throwException)
                    throw new ArgumentException("child");
                return index;
            }
            public IEnumerator<Control> GetEnumerator()
            {
                return items.GetEnumerator();
            }
            public int IndexOf(Control control)
            {
                return items.IndexOf(control);
            }
            public virtual int IndexOfKey(string key)
            {
                if (string.IsNullOrEmpty(key))
                    return -1;

                if (IsValidIndex(lastAccessedIndex))
                    if (WindowsFormsUtils.SafeCompareStrings(this[lastAccessedIndex].Name, key, true))
                        return lastAccessedIndex;

                for (int i = 0; i < Count; i++)
                {
                    if (!WindowsFormsUtils.SafeCompareStrings(this[i].Name, key, true)) continue;

                    lastAccessedIndex = i;
                    return i;
                }

                lastAccessedIndex = -1;
                return -1;
            }
            public void Insert(int index, Control value)
            {
                items.Insert(index, value);

                owner.PerformLayout();
                owner.OnControlAdded(new ControlEventArgs(value));
            }
            public virtual void Remove(Control item)
            {
                if (item == null)
                    return;

                items.Remove(item);
                owner.PerformLayout();
                owner.OnControlRemoved(new ControlEventArgs(item));
            }
            public void RemoveAt(int index)
            {
                Remove(this[index]);
            }
            public virtual void RemoveByKey(string key)
            {
                int index = IndexOfKey(key);
                if (IsValidIndex(index))
                    RemoveAt(index);
            }
            public void Reset()
            {
            }
            public virtual void SetChildIndex(Control child, int newIndex)
            {
                SetChildIndexInternal(child, newIndex);
            }

            internal virtual void SetChildIndexInternal(Control child, int newIndex)
            {
                if (child == null)
                    throw new ArgumentNullException("child");

                items.Remove(child);
                items.Insert(newIndex, child);
            }

            int IList.Add(object value)
            {
                var control = value as Control;
                if (control != null)
                {
                    Add(control);
                    return items.Count - 1;
                }
                return -1;
            }
            bool IList.Contains(object value)
            {
                var control = value as Control;
                if (control != null)
                    return items.Contains(control);
                return false;
            }
            int IList.IndexOf(object value)
            {
                var control = value as Control;
                if (control != null)
                    return items.IndexOf(control);
                return -1;
            }
            void IList.Insert(int index, object value)
            {
                var control = value as Control;
                if (control != null)
                    items.Insert(index, control);
            }
            void IList.Remove(object value)
            {
                var control = value as Control;
                if (control != null)
                    items.Remove(control);
            }
            object ICloneable.Clone()
            {
                var cc = new ControlCollection(owner);

                cc.items.AddRange(items);
                return cc;
            }

            private bool IsValidIndex(int index)
            {
                return index >= 0 && index < Count;
            }
        }
    }
}