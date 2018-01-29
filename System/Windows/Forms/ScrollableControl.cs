namespace System.Windows.Forms
{
    using System.Drawing;
    using System.Linq;

    public class ScrollableControl : Control
    {
        protected const int ScrollStateAutoScrolling = 0x0001;
        protected const int ScrollStateHScrollVisible = 0x0002;
        protected const int ScrollStateVScrollVisible = 0x0004;
        protected const int ScrollStateUserHasScrolled = 0x0008;
        protected const int ScrollStateFullDrag = 0x0010;

        private Rectangle displayRect = Rectangle.Empty;
        private int scrollState;

        private HScrollBar hscroll;
        private VScrollBar vscroll;

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
            set { SetScrollState(ScrollStateAutoScrolling, value); }
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
                }

                if (oriH && hscroll == null)
                {
                    hscroll = new HScrollBar();
                    hscroll.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
                    hscroll.uwfSystem = true;
                    hscroll.ValueChanged += Scroll_ValueChanged;

                    Controls.Add(hscroll);
                }
            }
            else
            {
                if (oriV && vscroll != null)
                {
                    vscroll.ValueChanged -= Scroll_ValueChanged;
                    vscroll.Dispose();
                    vscroll = null;
                }

                if (oriH && hscroll != null)
                {
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

        protected bool GetScrollState(int bit)
        {
            return (bit & scrollState) == bit;
        }
        protected override void OnLayout(object levent)
        {
            base.OnLayout(levent);

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

        private int GetControlsBottom()
        {
            var bottom = 0;
            for (int i = 0; i < Controls.Count; i++)
            {
                var item = Controls[i];
                if (item.uwfSystem)
                    continue;

                var itemBottom = item.Location.Y + item.Height + 1;
                if (itemBottom > bottom)
                    bottom = itemBottom;
            }

            return bottom;
        }
        private int GetControlsRight()
        {
            var right = 0;
            for (int i = 0; i < Controls.Count; i++)
            {
                var item = Controls[i];
                if (item.uwfSystem)
                    continue;

                var itemRight = item.Location.X + item.Width + 1;
                if (itemRight > right)
                    right = itemRight;
            }

            return right;
        }
        private void Scroll_ValueChanged(object sender, EventArgs e)
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
        private void UpdateScrolls()
        {
            var autoScroll = (scrollState & ScrollStateAutoScrolling) != 0;
            if (autoScroll == false)
            {
                HorizontalScroll.UpdateScrollInfo();
                VerticalScroll.UpdateScrollInfo();
                UpdateScrollRects();
                return;
            }

            var bottom = GetControlsBottom();
            var right = GetControlsRight();

            var enableHScroll = right > Width;
            var enableVScroll = bottom > Height;

            Native_EnableScrollBar(enableHScroll, NativeMethods.SB_HORZ);
            Native_EnableScrollBar(enableVScroll, NativeMethods.SB_VERT);

            var hMaximum = right;
            var vMaximum = bottom;

            if (vscroll != null) hMaximum += vscroll.Width + 2;
            if (hscroll != null) vMaximum += hscroll.Height + 2;

            if (vscroll != null)
            {
                vscroll.Maximum = vMaximum;
                vscroll.LargeChange = Height; // - hscroll.Height
            }

            if (hscroll != null)
            {
                hscroll.Maximum = hMaximum;
                hscroll.LargeChange = Width; // - vscroll.Width
            }

            UpdateScrollRects();
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
                    if (vscroll != null && hscroll != null)
                    {
                        hRightOffset += 14; // img.Width + bottomRight offset
                        vBottomOffset += 14;
                    }

                    form.uwfSizeGripRenderer.Location = gripOriginLocation;
                }
            }

            if (vscroll != null)
            {
                vscroll.Location = new Point(Width - vscroll.Width, vTopOffset);
                vscroll.Height = Height - vTopOffset - vBottomOffset;
            }

            if (hscroll != null)
            {
                hscroll.Location = new Point(0, Height - hscroll.Height);
                hscroll.Width = Width - hRightOffset;
            }
        }
    }
}
