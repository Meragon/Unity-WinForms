namespace System.Windows.Forms
{
    using System.Drawing;

    public enum TickStyle 
    {
        None = 0,
        TopLeft = 1,
        BottomRight = 2,
        Both = 3,
    }
    
    public class TrackBar : Control
    {
        private int         largeChange = 5;
        private int         maximum     = 10;
        private int         minimum     = 0;
        private Orientation orientation = Orientation.Horizontal;
        private int         requestedDim;
        private int         smallChange   = 1;
        private int         value         = 0;
        
        // GUI
        private Pen backBarBorderPen = new Pen(Color.FromArgb(214, 214, 214));
        private Color backBarColor = Color.FromArgb(231, 234, 234);
        private int backBarWH = 4;
        private Padding padding = new Padding(8, 2, 8, 2);
        private Color sliderBorderColor = Color.FromArgb(172, 172, 172);
        private Color sliderBorderColorHovered = Color.FromArgb(126, 180, 234);
        private Pen sliderBorderPen = new Pen(Color.Transparent);
        private Color sliderColor = Color.FromArgb(236, 236, 236);
        private Color sliderColorHovered = Color.FromArgb(228, 240, 252);
        private bool sliderHovered;
        private int sliderLength = 10; // Use sliderHeight for vertical orientation.
        private int sliderHeight = 21; // ^
        private int sliderX;
        private int sliderY;
        private bool sliderDragStarted;
        private int tickWH = 4;
        private Pen tickPen = new Pen(Color.FromArgb(196, 196, 196));

        public TrackBar()
        {
            TickStyle = TickStyle.BottomRight;
            TickFrequency = 1;
            requestedDim = PreferredDimension;
            
            MouseHook.MouseUp += MouseHookOnMouseUp;
        }
        
        public event EventHandler ValueChanged;
        
        public override Color ForeColor
        {
            get { return SystemColors.WindowText; }
            set { }
        }
        public int LargeChange
        {
            get { return largeChange; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");
                
                largeChange = value;
            }
        }
        public int Maximum
        {
            get { return maximum; }
            set
            {
                if (maximum == value)
                    return;

                if (minimum > value)
                    minimum = value;
                
                SetRange(minimum, value);
            }
        }
        public int Minimum
        {
            get { return minimum; }
            set
            {
                if (minimum == value)
                    return;

                if (maximum < value)
                    maximum = value;
                
                SetRange(value, maximum);
            }
        }
        /// <summary>
        /// Vertical orientation uses Minimum value at top and Maximum at bottom, it's seems
        /// more logical to me. Can be changed in future.
        /// </summary>
        public Orientation Orientation
        {
            get { return orientation; }
            set
            {
                if (orientation == value)
                    return;
                
                orientation = value;
                if (orientation == Orientation.Horizontal)
                    Width = requestedDim;
                else
                    Height = requestedDim;
                
                SetBoundsCore(Location.X, Location.Y, Height, Width, BoundsSpecified.All);
                AdjustSize();
            }
        }
        public int SmallChange
        {
            get { return smallChange; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");
                
                smallChange = value;
            }
        }
        public int TickFrequency { get; set; }
        public TickStyle TickStyle { get; set; }
        public int Value
        {
            get { return value; }
            set
            {
                if (this.value == value)
                    return;
                
                if (value < minimum || value > maximum)
                    throw new ArgumentOutOfRangeException("value");

                this.value = value;
                SetTrackBarPosition();
                OnValueChanged(EventArgs.Empty);
            }
        }

        protected override Size DefaultSize
        {
            get { return new Size(104, PreferredDimension); }
        }

        private int PreferredDimension
        {
            get
            {
                int cyhscroll = 17;

                return (cyhscroll * 8) / 3;
            }
        }

        public void SetRange(int minValue, int maxValue)
        {
            if (minimum == minValue && maximum == maxValue) 
                return;
            
            if (minValue > maxValue)
                maxValue = minValue;

            minimum = minValue;
            maximum = maxValue;

            value = MathHelper.Clamp(value, minimum, maximum);
            
            SetTrackBarPosition();
        }
        public override string ToString()
        {
            return base.ToString() + ", Minimum: " + Minimum + ", Maximum: " + Maximum + ", Value: " + Value;
        }

        protected override void Dispose(bool release_all)
        {
            base.Dispose(release_all);
            
            MouseHook.MouseUp -= MouseHookOnMouseUp;
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (!sliderDragStarted)
            {
                var sliderAreaClicked = IsPointOverSliderArea(e.Location);
                if (sliderAreaClicked)
                {
                    bool descrease;
                    if (Orientation == Orientation.Horizontal)
                        descrease = e.Location.X < sliderX + sliderLength / 2;
                    else
                        descrease = e.Location.Y < sliderY + sliderLength / 2;

                    var newValue = Value;
                    if (descrease)
                        newValue -= LargeChange;
                    else
                        newValue += LargeChange;

                    SetNewValue(newValue);
                }
            }
            
            base.OnMouseClick(e);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            sliderDragStarted = IsPointOverSlider(e.Location);
        }
        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);

            var mclient = PointToClient(MousePosition);

            sliderHovered = IsPointOverSlider(mclient);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (sliderDragStarted)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    var relativeX = e.X - padding.Left;
                    var barWidth = Width - padding.Horizontal - sliderLength;
                    var coef = (float)relativeX / barWidth;
                    var newValue = (int)((Maximum - Minimum) * coef); // Transform coefficient to value.

                    SetNewValue(newValue);
                }
                else
                {
                    var relativeY = e.Y - padding.Left;
                    var barWidth = Height - padding.Horizontal - sliderLength;
                    var coef = (float)relativeY / barWidth;
                    var newValue = (int)((Maximum - Minimum) * coef); // Transform coefficient to value.

                    SetNewValue(newValue);
                }
            }
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            
            if (e.Button != MouseButtons.None)
                return;

            var scrollLines = SystemInformation.MouseWheelScrollLines;
            if (scrollLines == -1)
                scrollLines = TickFrequency;
            
            if (e.Delta > 0)
                SetNewValue(Value - scrollLines);
            else if (e.Delta < 0)
                SetNewValue(Value + scrollLines);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.KeyCode)
            {
                case Keys.Down:
                case Keys.Right:
                    SetNewValue(Value + SmallChange);
                    break;
                
                case Keys.Up:
                case Keys.Left:
                    SetNewValue(Value - SmallChange);
                    break;
                
                case Keys.PageDown:
                    SetNewValue(Value + LargeChange);
                    break;
                case Keys.PageUp:
                    SetNewValue(Value - LargeChange);
                    break;
                
                case Keys.End:
                    SetNewValue(Maximum);
                    break;
                case Keys.Home:
                    SetNewValue(Minimum);
                    break;
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            int backBarX = 0;
            int backBarY = 0;
            int backBarW = 0;
            int backBarH = 0;
            
            if (Orientation == Orientation.Horizontal)
            {
                backBarX = padding.Left;
                backBarH = backBarWH;
                backBarW = Width - padding.Horizontal;
                
                sliderLength = 10;
                sliderHeight = 21;

                var disPerValue = (float)(backBarW - sliderLength) / (Maximum - Minimum);
                sliderX = (int)(backBarX + value * disPerValue);
                
                float ticks = maximum;
                float tickDistance = backBarW - sliderLength;
                if (TickFrequency > 0)
                {
                    ticks /= TickFrequency;
                    tickDistance /= ticks;
                }
                int ticksStartX = backBarX + sliderLength / 2;
                int ticksStartY1 = padding.Top + 3;
                int ticksTotalWidth = backBarW - sliderLength;
                
                switch (TickStyle)
                {
                    case TickStyle.Both:
                        backBarY = (PreferredDimension - backBarWH - padding.Vertical) / 2 + 1;
                        sliderY = padding.Top + tickWH + 4;
                        
                        DrawTicksH(e.Graphics, ticksStartX, ticksStartY1, ticksTotalWidth, ticks, tickDistance);
                        DrawTicksH(e.Graphics, ticksStartX, sliderY + sliderHeight + 2, ticksTotalWidth, ticks, tickDistance);
                        break;
                    case TickStyle.BottomRight:
                        sliderHeight -= 2;
                        backBarY = (sliderHeight - backBarWH) / 2 + padding.Top;
                        sliderY = padding.Top;
                        
                        DrawTicksH(e.Graphics, ticksStartX, sliderY + sliderHeight + 1, ticksTotalWidth, ticks, tickDistance);
                        break;
                    case TickStyle.None:
                        sliderHeight -= 2;
                        backBarY = (sliderHeight - backBarWH) / 2 + padding.Top;
                        sliderY = padding.Top;
                        break;
                    case TickStyle.TopLeft:
                        backBarY = (sliderHeight - backBarWH) / 2 + padding.Top + tickWH + 4;
                        sliderY = padding.Top + tickWH + 4;
                        
                        DrawTicksH(e.Graphics, ticksStartX, ticksStartY1, ticksTotalWidth, ticks, tickDistance);
                        break;
                }
            }
            else
            {
                backBarY = padding.Left;
                backBarW = backBarWH;
                backBarH = Height - padding.Horizontal;
                
                sliderLength = 10;
                sliderHeight = 21;
                
                var disPerValue = (float)(backBarH - sliderLength) / (Maximum - Minimum);
                sliderY = (int)(backBarY + value * disPerValue);

                float ticks = maximum;
                float tickDistance = backBarH - sliderLength;
                if (TickFrequency > 0)
                {
                    ticks /= TickFrequency;
                    tickDistance /= ticks;
                }
                int ticksStartX = padding.Top + 3;
                int ticksStartY1 = backBarY + sliderLength / 2;
                int ticksTotalWidth = backBarH - sliderLength;
                
                switch (TickStyle)
                {
                    case TickStyle.Both:
                        backBarX = (PreferredDimension - backBarWH - padding.Vertical) / 2 + 1;
                        sliderX = padding.Top + tickWH + 4;
                        
                        DrawTicksV(e.Graphics, ticksStartX, ticksStartY1, ticksTotalWidth, ticks, tickDistance);
                        DrawTicksV(e.Graphics, sliderX + sliderHeight + 2, ticksStartY1, ticksTotalWidth, ticks, tickDistance);
                        break;
                    case TickStyle.BottomRight:
                        sliderHeight -= 2;
                        backBarX = (sliderHeight - backBarWH) / 2 + padding.Top;
                        sliderX = padding.Top;
                        
                        DrawTicksV(e.Graphics, sliderX + sliderHeight + 1, ticksStartY1, ticksTotalWidth, ticks, tickDistance);
                        break;
                    case TickStyle.None:
                        sliderHeight -= 2;
                        backBarX = (sliderHeight - backBarWH) / 2 + padding.Top;
                        sliderX = padding.Top;
                        break;
                    case TickStyle.TopLeft:
                        backBarX = (sliderHeight - backBarWH) / 2 + padding.Top + tickWH + 4;
                        sliderX = padding.Top + tickWH + 4;
                        
                        DrawTicksV(e.Graphics, ticksStartX, ticksStartY1, ticksTotalWidth, ticks, tickDistance);
                        break;
                }
            }
            
            DrawBackBar(e.Graphics, backBarX, backBarY, backBarW, backBarH);
            
            if (Orientation == Orientation.Horizontal)
                DrawSlider(e.Graphics, sliderX, sliderY, sliderLength, sliderHeight);
            else
                DrawSlider(e.Graphics, sliderX, sliderY, sliderHeight, sliderLength);

            if (uwfCanDrawTabDots && uwfDrawTabDots)
                e.Graphics.DrawRectangle(uwfTabPen, 0, 0, Width, Height);
        }
        protected virtual void OnValueChanged(EventArgs e)
        {
            var handler = ValueChanged;
            if (handler != null) 
                handler(this, e);
        }
        protected override void SetBoundsCore(int argX, int argY, int argWidth, int argHeight, BoundsSpecified specified)
        {
            requestedDim = orientation == Orientation.Horizontal ? argHeight : argWidth;

            if (AutoSize)
            {
                if (orientation == Orientation.Horizontal)
                {
                    if ((specified & BoundsSpecified.Height) != BoundsSpecified.None)
                        argHeight = PreferredDimension;
                }
                else
                {
                    if ((specified & BoundsSpecified.Width) != BoundsSpecified.None)
                        Width = PreferredDimension;
                }
            }
            
            base.SetBoundsCore(argX, argY, argWidth, argHeight, specified);
        }

        private void AdjustSize()
        {
            var dim = requestedDim;
            if (Orientation == Orientation.Horizontal)
                Height = AutoSize ? PreferredDimension : dim;
            else
                Width = AutoSize ? PreferredDimension : dim;

            requestedDim = dim;
        }
        private void DrawBackBar(Graphics g, int x, int y, int width, int height)
        {
            if (width == 0 || height == 0)
                return;
            
            g.uwfFillRectangle(backBarColor, x + 1, y + 1, width - 2, height - 2);
            g.DrawRectangle(backBarBorderPen, x, y, width, height);
        }
        private void DrawSlider(Graphics g, int x, int y, int width, int height)
        {
            if (width == 0 || height == 0)
                return;

            var highlightSlider = sliderHovered || sliderDragStarted;
            
            sliderBorderPen.Color = highlightSlider ? sliderBorderColorHovered : sliderBorderColor;
            
            g.uwfFillRectangle(highlightSlider ? sliderColorHovered : sliderColor, x + 1, y + 1, width - 2, height - 2);
            g.DrawRectangle(sliderBorderPen, x, y, width, height);
        }
        private void DrawTicksH(Graphics g, int x, int y, int width, float count, float distance)
        {
            var tickWHLineFix = tickWH - 1; // g.DrawLine is not pixel perfect.
            
            // Big ones.
            g.DrawLine(tickPen, x, y, x, y + tickWHLineFix);
            g.DrawLine(tickPen, x + width, y, x + width, y + tickWHLineFix);
            
            // Other.
            for (int i = 0; i < count; i++)
            {
                var tx = x + distance * i;
                if (tx > x + width)
                    continue;
                
                g.DrawLine(tickPen, tx, y + 1, tx, y + tickWHLineFix);
            }
        }
        private void DrawTicksV(Graphics g, int x, int y, int width, float count, float distance)
        {
            var tickWHLineFix = tickWH - 1; // g.DrawLine is not pixel perfect.
            
            // Big ones.
            g.DrawLine(tickPen, x, y, x + tickWHLineFix, y);
            g.DrawLine(tickPen, x, y + width, x + tickWHLineFix, y + width);
            
            // Other.
            for (int i = 0; i < count; i++)
            {
                var ty = y + distance * i;
                if (ty > y + width)
                    continue;
                
                g.DrawLine(tickPen, x + 1, ty, x + tickWHLineFix, ty);
            }
        }
        private bool IsPointOverSlider(Point p)
        {
            if (Orientation == Orientation.Horizontal)
                return new Rectangle(sliderX, sliderY, sliderLength, sliderHeight).Contains(p);
            
            return new Rectangle(sliderX, sliderY, sliderHeight, sliderLength).Contains(p);
        }
        private bool IsPointOverSliderArea(Point p)
        {
            if (Orientation == Orientation.Horizontal)
                return new Rectangle(padding.Left, sliderY, Width - padding.Horizontal, sliderHeight).Contains(p);
            
            return new Rectangle(sliderX, padding.Left, sliderHeight, Height - padding.Horizontal).Contains(p);
        }
        private void MouseHookOnMouseUp(object sender, MouseEventArgs e)
        {
            sliderDragStarted = false;
        }
        private void SetNewValue(int newValue)
        {
            Value = MathHelper.Clamp(newValue, Minimum, Maximum);
        }
        private void SetTrackBarPosition()
        {
        }
    }
}