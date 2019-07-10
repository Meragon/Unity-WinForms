namespace System.Windows.Forms
{
    using System.Drawing;
    using System.Linq;

    public class ScrollableControl : Control
    {
        internal HScrollBar hscroll; // In case you want to scroll manually.
        internal VScrollBar vscroll;

        internal event EventHandler uwfHScrollAdded; // For styling if necessary.
        internal event EventHandler uwfVScrollAdded;

        protected const int ScrollStateAutoScrolling = 0x0001;
        protected const int ScrollStateHScrollVisible = 0x0002;
        protected const int ScrollStateVScrollVisible = 0x0004;
        protected const int ScrollStateUserHasScrolled = 0x0008;
        protected const int ScrollStateFullDrag = 0x0010;

        private Rectangle displayRect = Rectangle.Empty;
        private int scrollState;

        private HScrollProperties horizontalScroll;
        private VScrollProperties verticalScroll;

        public ScrollableControl()
        {
            SetStyle(ControlStyles.ContainerControl, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, false);
            SetScrollState(ScrollStateAutoScrolling, false);
        }

        public virtual bool AutoScroll
        {
            get { return GetScrollState(ScrollStateAutoScrolling); }
            set
            {
                SetScrollState(ScrollStateAutoScrolling, value);
                PerformLayout();
            }
        }
        public override Rectangle DisplayRectangle
        {
            get
            {
                var rect = ClientRectangle;
                if (!displayRect.IsEmpty)
                {
                    rect.X = displayRect.X;
                    rect.Y = displayRect.Y;
                    if (HScroll)
                        rect.Width = displayRect.Width;

                    if (VScroll)
                        rect.Height = displayRect.Height;
                }
                return rect;
            }
        }
        public HScrollProperties HorizontalScroll
        {
            get
            {
                if (horizontalScroll == null)
                    horizontalScroll = new HScrollProperties(this);

                return horizontalScroll;
            }
        }
        public VScrollProperties VerticalScroll
        {
            get
            {
                if (verticalScroll == null)
                    verticalScroll = new VScrollProperties(this);

                return verticalScroll;
            }
        }

        protected bool HScroll
        {
            get { return GetScrollState(ScrollStateHScrollVisible); }
            set { SetScrollState(ScrollStateHScrollVisible, value); }
        }
        protected bool VScroll
        {
            get { return GetScrollState(ScrollStateVScrollVisible); }
            set { SetScrollState(ScrollStateVScrollVisible, value); }
        }

        internal void EnsureVisible(Control control)
        {
            if (control == null || control.Parent == null)
                return;

            var scrollable = control.Parent as ScrollableControl;
            if (scrollable == null)
            {
                EnsureVisible(control.Parent);
                return;
            }

            // Vertical.
            if (scrollable.vscroll != null)
            {
                var controlY = control.Location.Y;
                var hScrollHeight = 0;
                if (scrollable.hscroll != null)
                    hScrollHeight = scrollable.hscroll.Height;
                
                var goDown = controlY + control.Height > scrollable.vscroll.Value + scrollable.vscroll.LargeChange - hScrollHeight;
                var goUp = controlY < scrollable.vscroll.Value;
                
                var newValue = 0;

                if (goDown)
                    newValue = controlY + control.Height + hScrollHeight + 4 - scrollable.vscroll.LargeChange;
                else if (goUp)
                    newValue = controlY - 4;
                
                if (goDown || goUp)
                    scrollable.vscroll.Value = MathHelper.Clamp(newValue, 0, scrollable.vscroll.Maximum);
            }

            // Horizontal.
            if (scrollable.hscroll != null)
            {
                var controlX = control.Location.X;
                var vScrollWidth = 0;
                if (scrollable.vscroll != null)
                    vScrollWidth = scrollable.vscroll.Width;
                
                var goLeft = controlX < scrollable.hscroll.Value;
                var goRight = controlX + control.Width > scrollable.hscroll.Value + scrollable.hscroll.LargeChange - vScrollWidth;
                
                var newValue = 0;

                if (goRight)
                    newValue = controlX + control.Width + vScrollWidth + 4 - scrollable.hscroll.LargeChange;
                else if (goLeft)
                    newValue = controlX - 4;    
                
                if (goRight || goLeft)
                    scrollable.hscroll.Value = MathHelper.Clamp(newValue, 0, scrollable.hscroll.Maximum);
            }
        }
        internal void Native_EnableScrollBar(bool enable, int orientation)
        {
            var oriV = orientation == NativeMethods.SB_VERT;
            var oriH = orientation == NativeMethods.SB_HORZ;

            if (enable)
            {
                if (oriV && vscroll == null)
                {
                    vscroll = new VScrollBar();
                    vscroll.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
                    vscroll.uwfSystem = true;
                    vscroll.ValueChanged += Scroll_ValueChanged;

                    Controls.Add(vscroll);

                    uwfOnVScrollAdded(EventArgs.Empty);
                }

                if (oriH && hscroll == null)
                {
                    hscroll = new HScrollBar();
                    hscroll.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
                    hscroll.uwfSystem = true;
                    hscroll.ValueChanged += Scroll_ValueChanged;

                    Controls.Add(hscroll);

                    uwfOnHScrollAdded(EventArgs.Empty);
                }
            }
            else
            {
                if (oriV && vscroll != null)
                {
                    vscroll.Value = 0; // Reset view.
                    vscroll.ValueChanged -= Scroll_ValueChanged;
                    vscroll.Dispose();
                    vscroll = null;
                }

                if (oriH && hscroll != null)
                {
                    hscroll.Value = 0; // Reset view.
                    hscroll.ValueChanged -= Scroll_ValueChanged;
                    hscroll.Dispose();
                    hscroll = null;
                }
            }
        }
        internal ScrollBar GetScrollBar(int orientation)
        {
            var oriV = orientation == NativeMethods.SB_VERT;
            var oriH = orientation == NativeMethods.SB_HORZ;

            if (oriV) return vscroll;
            if (oriH) return hscroll;

            return null;
        }
        internal void UpdateControlsOffsets()
        {
            var hValue = 0;
            var vValue = 0;

            if (hscroll != null)
                hValue = hscroll.Value;

            if (vscroll != null)
                vValue = vscroll.Value;

            for (int i = 0; i < Controls.Count; i++)
            {
                var item = Controls[i];
                if (item.uwfSystem)
                    continue;

                item.uwfOffset = new Point(-hValue, -vValue);
            }
        }
        internal void UpdateScrolls()
        {
            var autoScroll = (scrollState & ScrollStateAutoScrolling) != 0;
            if (autoScroll == false)
            {
                Native_EnableScrollBar(false, NativeMethods.SB_VERT);
                Native_EnableScrollBar(false, NativeMethods.SB_HORZ);
                return;
            }

            var controlRect = GetControlsRect();
            var bottom = controlRect.Height;
            var left = controlRect.X;
            var right = controlRect.Width;
            var top = controlRect.Y;
            
            var enableHScroll = left < 0 || right > Width;
            var enableVScroll = top < 0 || bottom > Height;

            Native_EnableScrollBar(enableHScroll, NativeMethods.SB_HORZ);
            Native_EnableScrollBar(enableVScroll, NativeMethods.SB_VERT);

            var hMaximum = right > Width ? right : Width;
            var vMaximum = bottom > Height ? bottom : Height;

            if (vscroll != null) hMaximum += vscroll.Width + 2;
            if (hscroll != null) vMaximum += hscroll.Height + 2;

            if (vscroll != null)
            {
                vscroll.Maximum = vMaximum;
                vscroll.Minimum = top < 0 ? top : 0;
                vscroll.LargeChange = Height; 
            }

            if (hscroll != null)
            {
                hscroll.Maximum = hMaximum;
                hscroll.Minimum = left < 0 ? left : 0;
                hscroll.LargeChange = Width; 
            }

            UpdateScrollRects();
        }

        protected internal virtual void uwfOnHScrollAdded(EventArgs e)
        {
            var handler = uwfHScrollAdded;
            if (handler != null)
                handler(this, e);
        }
        protected internal virtual void uwfOnVScrollAdded(EventArgs e)
        {
            var handler = uwfVScrollAdded;
            if (handler != null)
                handler(this, e);
        }
        protected internal override void uwfOnLatePaint(PaintEventArgs e)
        {
            if (vscroll == null || hscroll == null)
                return;

            // Fill rect between two scrollbars.
            var rx = hscroll.Location.X + hscroll.Width;
            var ry = vscroll.Location.Y + vscroll.Height;
            var rw = Width - rx;
            var rh = Height - ry;

            e.Graphics.uwfFillRectangle(vscroll.BackColor, rx, ry, rw, rh);
        }

        protected bool GetScrollState(int bit)
        {
            return (bit & scrollState) == bit;
        }
        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            UpdateScrolls();
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            ScrollBar scroll = vscroll;
            if (scroll == null)
                scroll = hscroll;

            if (scroll != null)
                scroll.Value -= e.Delta;
        }
        protected void SetScrollState(int bit, bool value)
        {
            if (value)
                scrollState |= bit;
            else
                scrollState &= ~bit;
        }

        private Rectangle GetControlsRect()
        {
            var bottom = 0;
            var left = 0;
            var right = 0;
            var top = 0;
            
            for (int i = 0; i < Controls.Count; i++)
            {
                var item = Controls[i];
                if (item.uwfSystem)
                    continue;

                var itemBottom = item.Location.Y + item.Height;
                if (itemBottom > bottom)
                    bottom = itemBottom;

                var itemLeft = item.Location.X;
                if (itemLeft < left)
                    left = itemLeft;
                
                var itemRight = item.Location.X + item.Width;
                if (itemRight > right)
                    right = itemRight;

                var itemTop = item.Location.Y;
                if (itemTop < top)
                    top = itemTop;
            }

            return new Rectangle(left, top, right, bottom);
        }
        private void Scroll_ValueChanged(object sender, EventArgs e)
        {
            UpdateControlsOffsets();
        }
        private void UpdateScrollRects()
        {
            var hRightOffset = 0;
            var vBottomOffset = 0;
            var vTopOffset = 0;

            var form = this as Form;
            if (form != null)
            {
                vTopOffset += form.uwfHeaderHeight;

                if (form.uwfSizeGripRenderer != null)
                {
                    form.ResetGripRendererLocation();

                    var gripOriginLocation = form.uwfSizeGripRenderer.Location;
                    if (vscroll != null && hscroll == null)
                        gripOriginLocation.Offset(-vscroll.Width, 0);
                    if (hscroll != null && vscroll == null)
                        gripOriginLocation.Offset(0, -hscroll.Height);
                    
                    form.uwfSizeGripRenderer.Location = gripOriginLocation;
                }
            }

            if (vscroll != null && hscroll != null)
            {
                hRightOffset += 14;
                vBottomOffset += 14;
            }

            if (vscroll != null)
            {
                vscroll.Location = new Point(Width - vscroll.Width - 1, vTopOffset - 1);
                vscroll.Height = Height - vTopOffset - vBottomOffset - 2;
            }

            if (hscroll != null)
            {
                hscroll.Location = new Point(1, Height - hscroll.Height - 1);
                hscroll.Width = Width - hRightOffset - 2;
            }
        }
    }
}
