namespace System.Windows.Forms
{
    using System.Drawing;

    public abstract class ScrollBar : Control
    {
        internal Button addButton;
        internal Button subtractButton;
        
        internal Color uwfScrollColor = SystemColors.ScrollBar;
        internal Color uwfScrollHoverColor = Color.FromArgb(166, 166, 166);

        protected ScrollOrientation scrollOrientation;

        private readonly int minScrollSize = 17;

        private int       largeChange = 10;
        private int       maximum     = 100;
        private bool      scrollCanDrag;
        private Color     scrollCurrentColor;
        private float     scrollCurrentColorA;
        private float     scrollCurrentColorR;
        private float     scrollCurrentColorG;
        private float     scrollCurrentColorB;
        private Color     scrollDestinationColor;
        private Point     scrollDragStartLocation;
        private Point     scrollDragRectOffset;
        private bool      scrollDraging;
        private Rectangle scrollRect;
        private int       value       = 0;

        protected ScrollBar()
        {
            Minimum = 0;
            SmallChange = 1;
            BackColor = Color.FromArgb(240, 240, 240);
            TabStop = false;
            
            scrollCurrentColor = uwfScrollColor;
            scrollCurrentColorA = scrollCurrentColor.A;
            scrollCurrentColorR = scrollCurrentColor.R;
            scrollCurrentColorG = scrollCurrentColor.G;
            scrollCurrentColorB = scrollCurrentColor.B;

            var backColor = SystemColors.Control;
            var backHoverColor = Color.FromArgb(218, 218, 218);
            var borderColor = Color.Transparent;
            var borderHoverColor = Color.Transparent;
            var imageColor = Color.FromArgb(96, 96, 96);
            var imageHoverColor = Color.Black;

            addButton = new RepeatButton();
            addButton.uwfBorderHoverColor = borderHoverColor;
            addButton.uwfBorderDisableColor = borderColor;
            addButton.uwfHoverColor = backHoverColor;
            addButton.uwfImageColor = imageColor;
            addButton.uwfImageHoverColor = imageHoverColor;
            addButton.uwfBorderColor = borderColor;
            addButton.BackColor = backColor;
            addButton.Click += (s, a) => { DoScroll(ScrollEventType.SmallIncrement); };
            Controls.Add(addButton);

            subtractButton = new RepeatButton();
            subtractButton.uwfBorderHoverColor = borderHoverColor;
            subtractButton.uwfBorderDisableColor = borderColor;
            subtractButton.uwfHoverColor = backHoverColor;
            subtractButton.uwfImageColor = imageColor;
            subtractButton.uwfImageHoverColor = imageHoverColor;
            subtractButton.uwfBorderColor = borderColor;
            subtractButton.BackColor = backColor;
            subtractButton.Click += (s, a) => { DoScroll(ScrollEventType.SmallDecrement); };
            Controls.Add(subtractButton);

            MouseHook.MouseUp += Owner_UpClick;
            Application.UpdateEvent += ApplicationOnUpdateEvent;
        }

        public event ScrollEventHandler Scroll;
        public event EventHandler ValueChanged;

        public int LargeChange
        {
            get { return Math.Min(largeChange, maximum - Minimum + 1); }
            set
            {
                largeChange = value;
                UpdateScrollRect();
            }
        }
        public int Maximum
        {
            get { return maximum; }
            set
            {
                maximum = value;
                UpdateScrollRect();
            }
        }
        public int Minimum { get; set; }
        public int SmallChange { get; set; }
        public int Value
        {
            get { return value; }
            set
            {
                if (this.value == value)
                    return;

                this.value = value;
                if (this.value > Maximum)
                    this.value = Maximum;
                if (this.value < Minimum)
                    this.value = Minimum;

                UpdateScrollRect();
                OnValueChanged(EventArgs.Empty);
            }
        }

        public override void Refresh()
        {
            base.Refresh();

            UpdateScrollRect();
        }

        internal void DoScroll(ScrollEventType type)
        {
            int newValue = value;
            int oldValue = value;

            switch (type)
            {
                case ScrollEventType.First:
                    newValue = Minimum;
                    break;

                case ScrollEventType.Last:
                    newValue = maximum - LargeChange + 1;
                    break;

                case ScrollEventType.SmallDecrement:
                    newValue = Math.Max(value - SmallChange, Minimum);
                    break;

                case ScrollEventType.SmallIncrement:
                    newValue = Math.Min(value + SmallChange, maximum - LargeChange + 1);
                    break;

                case ScrollEventType.LargeDecrement:
                    newValue = Math.Max(value - LargeChange, Minimum);
                    break;

                case ScrollEventType.LargeIncrement:
                    newValue = Math.Min(value + LargeChange, maximum - LargeChange + 1);
                    break;

                case ScrollEventType.ThumbPosition:
                case ScrollEventType.ThumbTrack:

                    // not implemented yet.
                    break;
            }

            UpdateScrollRect();

            var se = new ScrollEventArgs(type, oldValue, newValue, scrollOrientation);
            OnScroll(se);
            Value = se.NewValue;
        }
        internal void UpdateScrollRect()
        {
            // WIN_API Random calculations.

            if (largeChange > maximum)
            {
                scrollRect = new Rectangle();
                addButton.Enabled = false;
                subtractButton.Enabled = false;
                return;
            }

            addButton.Enabled = true;
            subtractButton.Enabled = true;

            float sx = 0;
            float sy = 0;
            float sw = 0;
            float sh = 0;

            // Total range for scroll bar.
            float scrollLength = 0;
            if (scrollOrientation == ScrollOrientation.HorizontalScroll)
                scrollLength = addButton.Location.X - subtractButton.Location.X - subtractButton.Width;
            else
                scrollLength = addButton.Location.Y - subtractButton.Location.Y - subtractButton.Height;

            int valueRange = (maximum - Minimum);
            float barSize = minScrollSize;
            if (largeChange > 0)
                barSize = (float)scrollLength / ((float)valueRange / largeChange);
            if (barSize >= scrollLength)
                barSize = scrollLength - 7;
            if (barSize < minScrollSize)
                barSize = minScrollSize;
            /*
            Example:
                this.Width = 134;
                addButton.Width = 17;
                subtractButton.Width = 17;

                scrollLength = 134 - 17 - 17 = 100;
                maximum = 400;
                minimum = 0;
                largeChange = 100; // 
                estimatedScrollWidth = 100 / ((400 - 0) / 100) = 100 / (4) = 25;
            */
            scrollLength -= barSize; // Adjusted range for scroll bar, depending on size.

            float valueK = (float)(Value - Minimum) / (valueRange - largeChange + 1);
            float scrollPos = scrollLength * valueK;

            if (scrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                sx = subtractButton.Location.X + subtractButton.Width + scrollPos;
                sy = 0;
                sw = barSize;
                sh = Height;

                scrollRect = new Rectangle((int)sx, (int)sy, (int)sw, (int)sh);

                if (sx + sw > addButton.Location.X && sw < scrollLength + barSize)
                {
                    sx = addButton.Location.X - sw;
                    scrollRect = new Rectangle((int)sx, (int)sy, (int)sw, (int)sh);
                    UpdateValueAtScrollRect();
                }
                else if (sw > scrollLength + barSize)
                    scrollRect = new Rectangle(0, 0, 17, (int)sh);
            }
            else
            {
                sx = 0;
                sy = subtractButton.Location.Y + subtractButton.Height + scrollPos;
                sw = Width;
                sh = barSize;

                scrollRect = new Rectangle((int)sx, (int)sy, (int)sw, (int)sh);

                if (sy + sh > addButton.Location.Y && sh < scrollLength + barSize)
                {
                    sy = addButton.Location.Y - sh;
                    scrollRect = new Rectangle((int)sx, (int)sy, (int)sw, (int)sh);
                    UpdateValueAtScrollRect();
                }
                else if (sh > scrollLength + barSize)
                    scrollRect = new Rectangle(0, 0, (int)sw, 17);
            }
        }

        protected override void Dispose(bool release_all)
        {
            MouseHook.MouseUp -= Owner_UpClick;
            Application.UpdateEvent -= ApplicationOnUpdateEvent;

            base.Dispose(release_all);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            
            if (Parent != null)
                Parent.RaiseOnKeyDown(e);
        }
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            
            if (Parent != null)
                Parent.RaiseOnKeyPress(e);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            
            if (Parent != null)
                Parent.RaiseOnKeyUp(e);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (scrollRect.Contains(e.Location))
            {
                scrollCanDrag = true;
                scrollDragStartLocation = e.Location;
                scrollDragRectOffset = e.Location.Subtract(scrollRect.Location);
            }
            else
            {
                scrollCanDrag = false;
                scrollDraging = false;
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (scrollDraging) return;

            var sEvent = ScrollEventType.LargeIncrement;
            if (scrollRect.Contains(e.Location)) return;

            if (scrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                if (scrollRect.X + scrollRect.Width / 2 > e.Location.X)
                    sEvent = ScrollEventType.LargeDecrement;
            }
            else
            {
                if (scrollRect.Y + scrollRect.Height / 2 > e.Location.Y)
                    sEvent = ScrollEventType.LargeDecrement;
            }

            DoScroll(sEvent);
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            var wheelDelta = e.Delta / 4;

            bool scrolled = false;

            while (wheelDelta != 0)
            {
                if (wheelDelta > 0)
                {
                    DoScroll(ScrollEventType.SmallDecrement);
                    wheelDelta -= SmallChange;
                    scrolled = true;
                    if (wheelDelta <= 0)
                        break;
                }
                else
                {
                    DoScroll(ScrollEventType.SmallIncrement);
                    wheelDelta += SmallChange;
                    scrolled = true;
                    if (wheelDelta >= 0)
                        break;
                }
            }

            if (scrolled)
                DoScroll(ScrollEventType.EndScroll);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (uwfHovered || scrollDraging)
                scrollDestinationColor = uwfScrollHoverColor;
            else
                scrollDestinationColor = uwfScrollColor;
            MathHelper.ColorLerp(scrollDestinationColor, 4, ref scrollCurrentColorA, ref scrollCurrentColorR, ref scrollCurrentColorG, ref scrollCurrentColorB);
            scrollCurrentColor = Color.FromArgb((int)scrollCurrentColorA, (int)scrollCurrentColorR, (int)scrollCurrentColorR, (int)scrollCurrentColorB);


            if (scrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                int backX = subtractButton.Location.X + subtractButton.Width;
                e.Graphics.uwfFillRectangle(BackColor, backX, 0, addButton.Location.X - backX, Height);
            }
            else
            {
                int backY = subtractButton.Location.Y + subtractButton.Height;
                e.Graphics.uwfFillRectangle(BackColor, 0, backY, Width, addButton.Location.Y - backY);
            }
            
            if (Enabled)
                e.Graphics.uwfFillRectangle(scrollCurrentColor, scrollRect.X, scrollRect.Y, scrollRect.Width, scrollRect.Height);
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateScrollRect();
        }
        protected virtual void OnScroll(ScrollEventArgs se)
        {
            var scroll = Scroll;
            if (scroll != null)
                scroll(this, se);
        }
        protected virtual void OnValueChanged(EventArgs e)
        {
            var valueChanged = ValueChanged;
            if (valueChanged != null)
                valueChanged(this, e);
        }

        private void Owner_UpClick(object sender, MouseEventArgs e)
        {
            if (scrollDraging)
                UpdateScrollRect();

            scrollDraging = false;
            scrollCanDrag = false;
        }
        private void ApplicationOnUpdateEvent()
        {
            var mclient = PointToClient(MousePosition);

            // TODO: move back to OnMouseMove.
            if (scrollCanDrag && mclient.Distance(scrollDragStartLocation) > 2)
            {
                scrollCanDrag = false;
                scrollDraging = true;
            }

            if (scrollDraging)
            {
                int sX = scrollRect.X;
                int sY = scrollRect.Y;
                if (scrollOrientation == ScrollOrientation.HorizontalScroll)
                {
                    sX = mclient.X - scrollDragRectOffset.X;
                    if (sX < subtractButton.Location.X + subtractButton.Width)
                        sX = subtractButton.Location.X + subtractButton.Width;
                    if (sX + scrollRect.Width > addButton.Location.X)
                        sX = addButton.Location.X - scrollRect.Width;
                }
                else
                {
                    sY = mclient.Y - scrollDragRectOffset.Y;
                    if (sY < subtractButton.Location.Y + subtractButton.Height)
                        sY = subtractButton.Location.Y + subtractButton.Height;
                    if (sY + scrollRect.Height > addButton.Location.Y)
                        sY = addButton.Location.Y - scrollRect.Height;
                }
                scrollRect = new Rectangle(sX, sY, scrollRect.Width, scrollRect.Height);

                UpdateValueAtScrollRect();
            }
        }
        private void UpdateValueAtScrollRect()
        {
            var scrollLength = 0f;
            if (scrollOrientation == ScrollOrientation.HorizontalScroll)
                scrollLength = addButton.Location.X - subtractButton.Location.X - subtractButton.Width;
            else
                scrollLength = addButton.Location.Y - subtractButton.Location.Y - subtractButton.Height;

            int valueRange = maximum - Minimum;
            float barSize = minScrollSize;
            if (largeChange > 0)
                barSize = scrollLength / ((float)valueRange / largeChange);
            if (barSize >= scrollLength)
                barSize = scrollLength - 7;
            if (barSize < minScrollSize)
                barSize = minScrollSize;

            scrollLength -= barSize; // Adjusted range for scroll bar, depending on size.

            if (scrollLength <= 0) return;

            long currentLength;
            if (scrollOrientation == ScrollOrientation.HorizontalScroll)
                currentLength = scrollRect.X - subtractButton.Location.X - subtractButton.Width;
            else
                currentLength = scrollRect.Y - subtractButton.Location.Y - subtractButton.Height;

            value = (int)((valueRange - largeChange + 1) * currentLength / scrollLength);
            value = MathHelper.Clamp(Minimum + value, Minimum, maximum);
            
            OnValueChanged(EventArgs.Empty);
        }
    }
}
