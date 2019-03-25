namespace System.Windows.Forms
{
    using System.ComponentModel;
    using System.Drawing;

    public abstract class ToolStripItem : Component
    {
        internal bool boundsChanged;
        internal bool locationChanged;
        internal Color disabledForeColor = SystemColors.InactiveCaption;
        internal Color hoverColor = Color.FromArgb(196, 225, 255);
        internal Pen selectPen = new Pen(Color.FromArgb(51, 153, 255));

        private Color backColor = Color.Empty;
        private Rectangle bounds = Rectangle.Empty;
        private ToolStrip owner;
        private Padding padding;
        private bool paddingExists;
        private ToolStrip parent;
        private bool stateAutoSize = true;
        private bool stateAutoToolTip = true;
        private bool stateContstructing;
        private bool statePressed;
        private bool stateSelected;
        private bool stateVisible = true;
        private string text;
        private ContentAlignment textAlign = ContentAlignment.MiddleCenter;
        private string toolTipText;

        protected ToolStripItem()
        {
            stateContstructing = true;

            BackgroundImageLayout = ImageLayout.Stretch; // tile default?
            Enabled = true;
            Font = SystemFonts.uwfArial_12;
            ForeColor = Control.defaultForeColor;
            Size = DefaultSize;

            uwfImageColor = Color.White;

            stateContstructing = false;
        }
        protected ToolStripItem(string text, Image image, EventHandler onClick) : this(text, image, onClick, null)
        {
        }
        protected ToolStripItem(string text, Image image, EventHandler onClick, string name) : this()
        {
            Image = image;
            Name = name;
            Text = text;
            if (onClick != null)
                Click += onClick;
        }

        public event EventHandler AvailableChanged;
        public event EventHandler BackColorChanged;
        public event EventHandler Click;
        public event EventHandler LocationChanged;
        public event EventHandler TextChanged;
        public event EventHandler VisibleChanged;

        public bool Available
        {
            get { return stateVisible; }
            set { SetVisibleCore(value); }
        }
        public bool AutoSize
        {
            get { return stateAutoSize; }
            set { stateAutoSize = value; }
        }
        public bool AutoToolTip
        {
            get { return stateAutoToolTip; }
            set { stateAutoToolTip = value; }
        }
        public virtual Color BackColor
        {
            get
            {
                var c = RawBackColor;
                if (c.IsEmpty == false)
                    return c;

                var p = ParentInternal;
                if (p != null)
                    return p.BackColor;

                return Color.Transparent; // Control.DefaultColor?
            }
            set
            {
                if (backColor == value || value.IsEmpty)
                    return;

                backColor = value;
                OnBackColorChanged(EventArgs.Empty);
            }
        }
        public virtual Image BackgroundImage { get; set; }
        public virtual ImageLayout BackgroundImageLayout { get; set; }
        public virtual Rectangle Bounds { get { return bounds; } }
        public virtual bool CanSelect { get { return true; } }
        public virtual bool Enabled { get; set; }
        public virtual Font Font { get; set; }
        public virtual Color ForeColor { get; set; }
        public int Height
        {
            get { return Bounds.Height; }
            set
            {
                var currentBounds = Bounds;
                SetBounds(currentBounds.X, currentBounds.Y, currentBounds.Width, value);
            }
        }
        public virtual Image Image { get; set; }
        public bool IsOnDropDown
        {
            get
            {
                if (ParentInternal != null)
                    return ParentInternal.IsDropDown;

                if (Owner != null && Owner.IsDropDown)
                    return true;

                return false;
            }
        }
        public string Name { get; set; }
        public ToolStrip Owner
        {
            get { return owner; }
            set
            {
                if (owner == value)
                    return;

                if (owner != null)
                    owner.Items.Remove(this);
                if (value != null)
                    value.Items.Add(this);
            }
        }
        public ToolStripItem OwnerItem
        {
            get
            {
                ToolStripDropDown currentParent = null;
                var parentInternal = ParentInternal;
                if (parentInternal != null)
                    currentParent = parentInternal as ToolStripDropDown;
                else if (Owner != null)
                    currentParent = Owner as ToolStripDropDown;

                if (currentParent != null)
                    return currentParent.OwnerItem;
                return null;
            }
        }
        public virtual Padding Padding
        {
            get
            {
                if (paddingExists)
                    return padding;
                return DefaultPadding;
            }
            set
            {
                padding = value;
                paddingExists = true;
            }
        }
        public virtual bool Pressed { get { return false; } }
        public virtual bool Selected { get { return stateSelected; } }
        public Size Size
        {
            get { return Bounds.Size; }
            set
            {
                var currentBounds = Bounds;
                currentBounds.Size = value;
                SetBounds(currentBounds);
            }
        }
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
        public virtual ContentAlignment TextAlign
        {
            get { return textAlign; }
            set { textAlign = value; }
        }
        public string ToolTipText
        {
            get
            {
                if (AutoToolTip == false || string.IsNullOrEmpty(toolTipText) == false)
                    return toolTipText;

                string toolText = Text;
                if (WindowsFormsUtils.ContainsMnemonic(toolText))
                    toolText = string.Join("", toolText.Split('&'));
                
                return toolText;
            }
            set { toolTipText = value; }
        }
        public bool Visible
        {
            get { return ParentInternal != null && ParentInternal.Visible && Available; }
            set { SetVisibleCore(value); }
        }
        public int Width
        {
            get { return Bounds.Width; }
            set
            {
                var currentBounds = Bounds;
                SetBounds(currentBounds.X, currentBounds.Y, value, currentBounds.Height);
            }
        }

        internal ToolStrip ParentInternal
        {
            get { return parent; }
            set
            {
                if (parent == value) return;

                var oldParent = parent;
                parent = value;
                OnParentChanged(oldParent, value);
            }
        }
        internal Color RawBackColor
        {
            get { return backColor; }
        }
        internal Color uwfImageColor { get; set; }

        protected internal ToolStrip Parent
        {
            get { return ParentInternal; }
            set { ParentInternal = value; }
        }
        protected virtual Padding DefaultPadding
        {
            get { return Padding.Empty; }
        }
        protected virtual Size DefaultSize
        {
            get { return new Size(23, 23); }
        }

        public void Invalidate()
        {
            if (this.ParentInternal != null)
                ParentInternal.Invalidate(this.Bounds, true);
        }
        public void Select()
        {
            if (CanSelect == false)
                return;

            stateSelected = true;
        }
        public override string ToString()
        {
            var text = Text;
            if (string.IsNullOrEmpty(text) == false)
                return text;
            return base.ToString();
        }

        internal void Push(bool push)
        {
            if (!CanSelect || !Enabled)
                return;

            statePressed = push;
            if (Available)
                Invalidate();
        }
        internal void RaiseClick()
        {
            OnClick(EventArgs.Empty);
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
        internal void RaiseOnMouseMove(MouseEventArgs mea)
        {
            OnMouseMove(mea);
        }
        internal void RaiseOnMouseUp(MouseEventArgs e)
        {
            OnMouseUp(e);
        }
        internal void RaiseOnPaint(PaintEventArgs e)
        {
            OnPaint(e);
        }
        internal void SetBounds(int x, int y, int width, int height)
        {
            SetBounds(new Rectangle(x, y, width, height));
        }
        internal void SetOwner(ToolStrip newOwner)
        {
            if (owner == newOwner) return;

            owner = newOwner;
            if (newOwner == null)
                ParentInternal = null;
        }
        internal virtual void Unselect()
        {
            if (stateSelected == false)
                return;

            stateSelected = false;

            if (Available && ParentInternal != null)
                ParentInternal.NotifySelectionChange(this);
        }
        internal void UpdateBounds(int x, int y)
        {
            var currentOwner = Owner;
            if (currentOwner == null)
                return;

            var currentBounds = Bounds;
            var nx = 0;
            var ny = y;
            var nw = currentBounds.Width;
            var nh = currentBounds.Height;
            if (currentOwner.Orientation == Orientation.Vertical)
            {
                nx = 2; // Border + 1 pixel offset (padding?).
                ny = y;
                if (AutoSize)
                    nh = DefaultSize.Height + Padding.Vertical + 1; // Dunno where 1 pixel is missing.
            }
            else
            {
                nx = x;
                if (AutoSize)
                    nh = Height;
            }

            SetBounds(nx, ny, nw, nh);
        }

        protected internal virtual void DrawImage(Graphics graphics)
        {
            var image = Image;

            // TODO: image align.
            if (image != null)
            {
                var imageColor = Enabled ? uwfImageColor : Color.Gray;
                var rect = Bounds;
                graphics.uwfDrawImage(
                    image,
                    imageColor,
                    rect.X + rect.Width / 2 - image.Width / 2,
                    rect.Y + rect.Height / 2 - image.Height / 2,
                    image.Width,
                    image.Height);
            }
        }
        protected internal virtual int GetEstimatedWidth(Graphics graphics) // OnPaint only.
        {
            // ToolStripSeparator?
            if (string.IsNullOrEmpty(Text))
                return 0; 
            
            return (int)graphics.MeasureString(Text, Font).Width;
        }
        protected internal virtual void SetBounds(Rectangle nbounds)
        {
            var oldBounds = bounds;
            bounds = nbounds;

            if (stateContstructing)
                return;

            if (bounds != oldBounds)
                OnBoundsChanged();

            if (bounds.Location != oldBounds.Location)
                OnLocationChanged(EventArgs.Empty);
        }

        protected override void Dispose(bool disposing)
        {
        }
        protected virtual void OnAvailableChanged(System.EventArgs e)
        {
            var handler = AvailableChanged;
            if (handler != null)
                handler(this, e);
        }
        protected virtual void OnBackColorChanged(EventArgs e)
        {
            var handler = BackColorChanged;
            if (handler != null)
                handler(this, e);
        }
        protected virtual void OnBoundsChanged()
        {
            boundsChanged = true;
        }
        protected virtual void OnClick(EventArgs e)
        {
            var handler = Click;
            if (handler != null)
                handler(this, e);
        }
        protected virtual void OnLocationChanged(EventArgs e)
        {
            var handler = LocationChanged;
            if (handler != null)
                handler(this, e);
        }
        protected virtual void OnMouseDown(MouseEventArgs e)
        {
        }
        protected virtual void OnMouseEnter(EventArgs e)
        {
            Select();
        }
        protected virtual void OnMouseHover(EventArgs e)
        {
        }
        protected virtual void OnMouseLeave(EventArgs e)
        {
            Unselect();
        }
        protected virtual void OnMouseMove(MouseEventArgs mea)
        {
        }
        protected virtual void OnMouseUp(MouseEventArgs e)
        {
            if (!Enabled) return;

            OnClick(EventArgs.Empty);
        }
        protected virtual void OnPaint(PaintEventArgs e)
        {
            var localBackColor = stateSelected && Enabled ? hoverColor : BackColor;
            var rect = Bounds;
            var graphics = e.Graphics;
            var textColor = Enabled ? ForeColor : disabledForeColor;

            // Text left offset for vertical ToolStrip.
            var leftOffset = 0;
            if (Owner != null && Owner.Orientation == Orientation.Vertical)
                leftOffset = Owner.Padding.Left;

            // Paint back + backImage.
            if (BackgroundImage != null)
                ControlPaint.DrawBackgroundImage(graphics, BackgroundImage, localBackColor, BackgroundImageLayout, rect, rect);
            else
                e.Graphics.uwfFillRectangle(localBackColor, rect);

            // Draw border for selected state.
            if (stateSelected && Enabled)
                graphics.DrawRectangle(selectPen, rect);

            // Image.
            DrawImage(graphics);

            // Text.
            graphics.uwfDrawString(Text, Font, textColor, rect.X + leftOffset, rect.Y, rect.Width - leftOffset, rect.Height, TextAlign);
            ////graphics.DrawRectangle(new Pen(Color.Red), rect); // Debug.
        }
        protected virtual void OnParentChanged(ToolStrip oldParent, ToolStrip newParent)
        {
        }
        protected virtual void OnTextChanged(EventArgs e)
        {
            var handler = TextChanged;
            if (handler != null)
                handler(this, e);

            boundsChanged = true;
        }
        protected virtual void OnVisibleChanged(EventArgs e)
        {
            var handler = VisibleChanged;
            if (handler != null)
                handler(this, e);
        }
        protected virtual void SetVisibleCore(bool visible)
        {
            if (stateVisible == visible)
                return;

            stateVisible = visible;
            Unselect();
            Push(false);

            OnAvailableChanged(EventArgs.Empty);
            OnVisibleChanged(EventArgs.Empty);
        }
    }
}
