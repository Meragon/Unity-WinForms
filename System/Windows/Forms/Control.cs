namespace System.Windows.Forms
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms.Design;

    [Serializable]
    public class Control : Component
    {
        internal static Control lastSelected;

        internal bool selected;
        internal bool hovered;
        internal bool mouseEntered;
        internal bool shouldFocus;

        internal Application uwfAppOwner;
        internal bool uwfAutoGroup;
        internal int uwfBatches;
        internal bool uwfShadowBox;
        internal DrawHandler uwfShadowHandler;
        internal IControlDesigner uwfDesigner;
        internal Point uwfOffset;

        protected static Color defaultShadowColor = Color.FromArgb(12, 0, 0, 0);

        private readonly ControlCollection controls;

        private AnchorStyles anchor = AnchorStyles.Top | AnchorStyles.Left;
        private int clientHeight;
        private int clientWidth;
        private ControlStyles controlStyle;
        private Font font = SystemFonts.uwfArial_12;
        private int height;
        private bool fuwfContext;
        private bool visible;
        private int width;
        private int x, y;

        public Control()
        {
            if (Parent != null && Parent.uwfAppOwner != null)
                Parent.uwfAppOwner.Run(this);
            else if (uwfDefaultController != null)
                uwfDefaultController.Run(this);

            controls = new ControlCollection(this);
            Enabled = true;
            ForeColor = Color.Black;
            TabIndex = -1;
            TabStop = true;
            uwfAutoGroup = true;
            visible = true;

            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.StandardClick | ControlStyles.Selectable | ControlStyles.StandardDoubleClick | 
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UseTextForAccessibility,
                true);

            var defaultSize = DefaultSize;
            width = defaultSize.Width;
            height = defaultSize.Height;
        }

        public delegate void DrawHandler(PaintEventArgs e);
        public delegate void ResizeHandler(Point delta);

        public event EventHandler Click = delegate { };
        public event EventHandler ClientSizeChanged = delegate { };
        public new event EventHandler Disposed = delegate { };
        public event DragEventHandler DragDrop = delegate { };
        public event DragEventHandler DragEnter = delegate { };
        public event EventHandler DragLeave = delegate { };
        public event EventHandler GotFocus = delegate { };
        public event EventHandler LocationChanged = delegate { };
        public event EventHandler LostFocus = delegate { };
        public event KeyEventHandler KeyDown = delegate { };
        public event KeyPressEventHandler KeyPress = delegate { };
        public event KeyEventHandler KeyUp = delegate { };
        public event MouseEventHandler MouseClick = delegate { };
        public event MouseEventHandler MouseDown = delegate { };
        public event EventHandler MouseEnter = delegate { };
        public event EventHandler MouseHover = delegate { };
        public event EventHandler MouseLeave = delegate { };
        public event MouseEventHandler MouseUp = delegate { };
        public event EventHandler Resize = delegate { };
        public event EventHandler SizeChanged = delegate { };
        public event EventHandler TextChanged = delegate { };
        public event EventHandler VisibleChanged = delegate { };

        public event KeyEventHandler uwfKeyPress = delegate { };

        public static Application uwfDefaultController { get; set; }
        public static Point MousePosition
        {
            get
            {
                return new Point(
                    (int)(UnityEngine.Input.mousePosition.x / Application.ScaleX),
                    (int)((UnityEngine.Screen.height - UnityEngine.Input.mousePosition.y) / Application.ScaleY));
            }
        }

        public virtual bool AllowDrop { get; set; }
        public bool AlwaysFocused { get; set; }
        public virtual AnchorStyles Anchor
        {
            get { return anchor; }
            set { anchor = value; }
        }
        public virtual bool AutoSize { get; set; }
        public virtual Color BackColor { get; set; }
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
        public ControlCollection Controls { get { return controls; } }
        public virtual Rectangle DisplayRectangle { get { return ClientRectangle; } }
        public bool Disposing { get; private set; }
        public bool Enabled { get; set; }
        public bool Focused { get { return selected; } }
        public virtual Font Font
        {
            get { return font; }
            set { font = value; }
        }
        public virtual Color ForeColor { get; set; }
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
        public Control Parent { get; set; }
        public Size Size
        {
            get { return new Size(Width, Height); }
            set { SetBounds(x, y, value.Width, value.Height, BoundsSpecified.Size); }
        }
        public int TabIndex { get; set; }
        public bool TabStop { get; set; }
        public object Tag { get; set; }
        public virtual string Text { get; set; }
        public int Top
        {
            get { return Location.Y; }
            set { Location = new Point(Location.X, value); }
        }
        public bool Visible
        {
            get { return visible; }
            set
            {
                bool changed = visible != value;
                visible = value;
                if (changed)
                    VisibleChanged(this, new EventArgs());
            }
        }
        public int Width
        {
            get { return width; }
            set { SetBounds(x, y, value, height, BoundsSpecified.Width); }
        }

        internal virtual bool uwfContext // Close on click control.
        {
            get { return fuwfContext; }
            set
            {
                if (fuwfContext != value)
                {
                    fuwfContext = value;
                    if (fuwfContext)
                        uwfAppOwner.Contexts.Add(this);
                    else
                        uwfAppOwner.Contexts.Remove(this);
                }
            }
        }
        internal bool uwfHovered { get { return hovered; } }

        protected virtual Size DefaultSize
        {
            get { return Size.Empty; }
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
                if (uwfAppOwner.Forms.Contains(form))
                {
                    uwfAppOwner.Forms.Remove(form);
                    uwfAppOwner.Forms.Add(form);
                }
                else if (form.IsModal)
                {
                    uwfAppOwner.ModalForms.Remove(form);
                    uwfAppOwner.ModalForms.Add(form);
                }
            }
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
        public void PerformLayout()
        {
        }
        public Point PointToClient(Point p)
        {
            var parent = Parent;
            if (parent != null)
                p = parent.PointToClient(p);

            var location = Location;
            var localuwfOffset = uwfOffset;

            p.Offset(-location.X - localuwfOffset.X, -location.Y - localuwfOffset.Y);
            return p;
        }
        public Point PointToScreen(Point p)
        {
            var parent = Parent;
            if (parent != null)
                p = parent.PointToScreen(p);

            var location = Location;
            var localuwfOffset = uwfOffset;

            p.Offset(location.X + localuwfOffset.X, location.Y + localuwfOffset.Y);
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
        public void SuspendLayout()
        {
            // dunno.
        }

        internal virtual bool CanSelectCore()
        {
            if ((controlStyle & ControlStyles.Selectable) != ControlStyles.Selectable)
                return false;
            for (Control control = this; control != null; control = control.Parent)
            {
                if (!control.Enabled || !control.Visible)
                    return false;
            }
            return true;
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
            OnClick(e);
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
        internal void RaiseOnKeyPress(KeyPressEventArgs e)
        {
            OnKeyPress(e); // TODO: KeyPressEventArgs?
            KeyPress(this, e);

            uwfKeyPress(this, e.uwfKeyArgs);
        }
        internal void RaiseOnKeyUp(KeyEventArgs e)
        {
            OnKeyUp(e);
            KeyUp(this, e);
        }
        internal void RaiseOnPaint(PaintEventArgs e)
        {
            uwfBatches = 0;

            if (uwfShadowBox)
            {
                if (uwfShadowHandler == null)
                {
                    uwfShadowHandler = (pArgs) =>
                    {
                        var psLoc = PointToScreen(Point.Empty);
                        int shX = psLoc.X + 6;
                        int shY = psLoc.Y + 6;
                        var shadowColor = defaultShadowColor;
                        var localWidth = Width;
                        var localHeight = Height;
                        pArgs.Graphics.uwfFillRectangle(shadowColor, shX + 6, shY + 6, localWidth - 12, localHeight - 12);
                        pArgs.Graphics.uwfFillRectangle(shadowColor, shX + 5, shY + 5, localWidth - 10, localHeight - 10);
                        pArgs.Graphics.uwfFillRectangle(shadowColor, shX + 4, shY + 4, localWidth - 8, localHeight - 8);
                        pArgs.Graphics.uwfFillRectangle(shadowColor, shX + 3, shY + 3, localWidth - 6, localHeight - 6);
                        pArgs.Graphics.uwfFillRectangle(shadowColor, shX + 2, shY + 2, localWidth - 4, localHeight - 4);
                    };
                }

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
        internal Size SizeFromClientSize(int argWidth, int argHeight)
        {
            return new Size(argWidth, argHeight);
        }
        internal void uwfAddjustSizeToScreen(Size delta)
        {
            ParentResized(new Point(delta.Width, delta.Height));
        }

        protected override void Dispose(bool release_all)
        {
            if (IsDisposed) return;

            Disposing = true;

            if (release_all)
            {
                for (; Controls.Count > 0;)
                    Controls[0].Dispose();

                Controls.Clear();
            }

            if (Parent != null)
            {
                int self = Parent.Controls.FindIndex(control => control == this);
                if (self > -1)
                    Parent.Controls.RemoveAt(self);
            }

            if (uwfContext)
                uwfAppOwner.Contexts.Remove(this);

            Disposed(this, EventArgs.Empty);
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
        protected virtual void OnClick(EventArgs e)
        {
            Click(this, e);
        }
        protected virtual void OnClientSizeChanged(EventArgs e)
        {
            ClientSizeChanged(this, e);
        }
        protected virtual void OnDoubleClick(EventArgs e)
        {
        }
        protected virtual void OnDragDrop(DragEventArgs drgevent)
        {
            DragDrop(this, drgevent);
        }
        protected virtual void OnDragEnter(DragEventArgs drgevent)
        {
            DragEnter(this, drgevent);
        }
        protected virtual void OnDragLeave(EventArgs e)
        {
            DragLeave(this, e);
        }
        protected virtual void OnGotFocus(EventArgs e)
        {
            if (Parent != null)
                Parent.uwfChildGotFocus(this);

            GotFocus(this, e);
        }
        protected virtual void OnKeyDown(KeyEventArgs e)
        {
        }
        protected virtual void OnKeyPress(KeyPressEventArgs e)
        {
        }
        protected virtual void OnKeyUp(KeyEventArgs e)
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
            MouseClick(this, e);
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
            MouseUp(this, e);
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
        protected virtual void OnResize(EventArgs e)
        {
            Resize(this, e);
        }
        protected virtual void OnTextChanged(EventArgs e)
        {
            TextChanged(this, e);
        }
        protected virtual void SetBoundsCore(int argX, int argY, int argWidth, int argHeight, BoundsSpecified specified)
        {
            if (x == argX && y == argY && (width == argWidth && height == argHeight))
                return;

            if (Parent != null)
                Parent.SuspendLayout();

            UpdateBounds(argX, argY, argWidth, argHeight);

            if (Parent != null)
                Parent.ResumeLayout(true);
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
        protected virtual void OnSizeChanged(EventArgs e)
        {
            OnResize(EventArgs.Empty);
            SizeChanged(this, e);
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
        }

        protected virtual void uwfChildGotFocus(Control child)
        {
            if (Parent == null) return;

            Parent.uwfChildGotFocus(child);
        }
        protected virtual void uwfOnLatePaint(PaintEventArgs e)
        {
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
                items.Add(value);
                value.Parent = owner;
            }
            public virtual void AddRange(Control[] controls)
            {
                foreach (var c in controls)
                {
                    items.Add(c);
                    c.Parent = owner;
                }
            }
            public void Clear()
            {
                items.Clear();
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
            public IEnumerator<Control> GetEnumerator()
            {
                return items.GetEnumerator();
            }
            public int IndexOf(Control control)
            {
                return items.IndexOf(control);
            }
            public void Insert(int index, Control value)
            {
                items.Insert(index, value);
            }
            public virtual void Remove(Control item)
            {
                items.Remove(item);
            }
            public void RemoveAt(int index)
            {
                items.RemoveAt(index);
            }
            public void Reset()
            {
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
        }
    }

    public delegate void DragDropRenderHandler(Graphics g);
}