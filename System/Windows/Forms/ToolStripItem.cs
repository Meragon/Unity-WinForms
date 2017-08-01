namespace System.Windows.Forms
{
    using System.ComponentModel;
    using System.Drawing;

    public abstract class ToolStripItem : Component
    {
        protected Pen selectPen = new Pen(Color.Transparent);

        private bool hovered;
        private Color hoverColor;

        protected ToolStripItem()
        {
            Enabled = true;
            Font = SystemFonts.uwfArial_12;
            ForeColor = Color.FromArgb(64, 64, 64);
            HoverColor = Color.FromArgb(64, 200, 200, 200);
            HoverPadding = new Size(2, 2);
            ImageColor = Color.White;
            Name = "toolStripItem";
            Padding = new Forms.Padding(8, 0, 8, 0);
            Size = new Drawing.Size(160, 24);
            TextAlign = ContentAlignment.MiddleLeft;
        }
        protected ToolStripItem(string text) : this()
        {
            Text = text;
        }

        public event EventHandler Click = delegate { };

        public virtual Color BackColor { get; set; }
        public virtual Rectangle Bounds { get { return new Rectangle(0, 0, Width, Height); } }
        public virtual bool Enabled { get; set; }
        public virtual Font Font { get; set; }
        public virtual Color ForeColor { get; set; }
        public int Height { get; set; }
        public bool Hovered { get { return hovered; } }
        public Color HoverColor
        {
            get { return hoverColor; }
            set
            {
                hoverColor = value;
                selectPen.Color = HoverColor - Color.FromArgb(0, 64, 64, 64);
            }
        }
        public Size HoverPadding { get; set; }
        public virtual Bitmap Image { get; set; }
        public Color ImageColor { get; set; }
        public string Name { get; set; }
        public ToolStrip Owner { get; set; }
        public ToolStripItem OwnerItem { get; internal set; }
        public virtual Padding Padding { get; set; }
        public virtual bool Pressed { get { return false; } }
        public virtual bool Selected { get; internal set; }
        public Size Size { get { return new Size(Width, Height); } set { Width = value.Width; Height = value.Height; } }
        public virtual string Text { get; set; }
        public virtual ContentAlignment TextAlign { get; set; }
        public bool Visible { get; set; }
        public int Width { get; set; }

        internal bool JustVisual { get; set; }

        protected internal ToolStrip Parent { get; set; }

        internal void RaiseClick()
        {
            Click(this, null);
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
        internal void ResetSelected()
        {
            if (Parent.uwfContext && Owner.OwnerItem != null)
            {
                var parent = Owner.OwnerItem.Owner;
                while (true)
                {
                    if (parent == null) break;

                    if (!parent.uwfContext)
                    {
                        parent.ResetSelected();
                        break;
                    }

                    if (parent.OwnerItem != null)
                        parent = parent.OwnerItem.Owner;
                    else
                        break;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
        }
        protected virtual void OnMouseDown(MouseEventArgs e)
        {
        }
        protected virtual void OnMouseEnter(EventArgs e)
        {
            hovered = true;
        }
        protected virtual void OnMouseHover(EventArgs e)
        {
        }
        protected virtual void OnMouseLeave(EventArgs e)
        {
            hovered = false;
        }
        protected virtual void OnMouseMove(MouseEventArgs mea)
        {
        }
        protected virtual void OnMouseUp(MouseEventArgs e)
        {
            if (!Enabled) return;
            Click(this, null);
            if (Parent.Parent == null)
                Parent.Dispose();
        }
        protected virtual void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            var ex = e.ClipRectangle.X;
            var ey = e.ClipRectangle.Y;
            var ew = e.ClipRectangle.Width;
            var eh = e.ClipRectangle.Height;

            g.uwfFillRectangle(BackColor, ex, ey, ew, eh);
            if (Image != null)
                g.uwfDrawImage(Image, ImageColor, ex + 10, ey + 4, 12, 12);
            if (Enabled)
            {
                var rect = Rectangle.Inflate(e.ClipRectangle, -HoverPadding.Width, -HoverPadding.Height);
                if (!Selected)
                {
                    if (hovered)
                    {
                        g.uwfFillRectangle(HoverColor, rect);
                        g.DrawRectangle(selectPen, rect);
                    }
                }
                else
                {
                    if (Owner.Orientation == Orientation.Horizontal)
                        g.uwfFillRectangle(Color.FromArgb(246, 246, 246), e.ClipRectangle);
                    else
                    {
                        g.uwfFillRectangle(HoverColor, rect);
                        g.DrawRectangle(selectPen, rect);
                    }
                }
            }

            Color textColor = Enabled ? ForeColor : ForeColor + Color.FromArgb(0, 100, 100, 100);
            if (Selected && Enabled) textColor = Color.FromArgb(64, 64, 64);

            if (Parent.Orientation == Orientation.Horizontal)
            {
                g.uwfDrawString(Text, Font, textColor, ex, ey, ew, eh, TextAlign);
            }
            else
                g.uwfDrawString(Text, Font, textColor, ex + 40, ey, ew, eh, TextAlign);
        }
    }
}
