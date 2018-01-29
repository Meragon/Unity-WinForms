namespace System.Windows.Forms
{
    using System.Drawing;

    /*
     * I don't really know how this class supposed to be used with a ScrollableControl,
     * especially when you have 'scrollableControl.AutoScroll = true'. It's like doing nothing.
     * So, most of the logic will be held inside a ScrollableControl.
     */

    public abstract class ScrollProperties
    {
        internal int minimum = 0;
        internal int maximum = 100;
        internal bool maximumSetExternally;
        internal int largeChange = 10;
        internal bool largeChangeSetExternally;
        internal int smallChange = 1;
        internal bool smallChangeSetExternally;
        internal int value = 0;
        internal bool visible = false;

        private readonly ScrollableControl parent;
        private bool enabled = true;

        protected ScrollProperties(ScrollableControl container)
        {
            parent = container;
        }

        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (parent.AutoScroll)
                    return;

                if (value == enabled) return;

                enabled = value;
                EnableScroll(value);
            }
        }
        public int LargeChange
        {
            get { return Math.Min(largeChange, maximum - minimum + 1); }
            set
            {
                if (largeChange == value)
                    return;

                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");

                largeChange = value;
                largeChangeSetExternally = true;
                UpdateScrollInfo();
            }
        }
        public int Maximum
        {
            get { return maximum; }
            set
            {
                if (parent.AutoScroll)
                    return;

                if (maximum == value)
                    return;

                if (minimum > value)
                    minimum = value;

                if (value < this.value)
                    Value = value;

                maximumSetExternally = true;
                UpdateScrollInfo();
            }
        }
        public int Minimum
        {
            get { return minimum; }
            set
            {
                if (parent.AutoScroll)
                    return;

                if (minimum == value)
                    return;

                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");

                if (maximum < value)
                    maximum = value;

                if (value > this.value)
                    this.value = value;

                minimum = value;
                UpdateScrollInfo();
            }
        }
        public int SmallChange
        {
            get { return Math.Min(smallChange, LargeChange); }
            set
            {
                if (smallChange == value)
                    return;

                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");

                smallChange = value;
                smallChangeSetExternally = true;
                UpdateScrollInfo();
            }
        }
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
                UpdateScrollInfo();
            }
        }
        public bool Visible
        {
            get { return visible; }
            set
            {
                if (parent.AutoScroll)
                    return;

                if (visible == value)
                    return;

                visible = value;
                UpdateScrollInfo();
            }
        }

        internal abstract int PageSize
        {
            get;
        }
        internal abstract int Orientation
        {
            get;
        }
        internal abstract int HorizontalDisplayPosition
        {
            get;
        }
        internal abstract int VerticalDisplayPosition
        {
            get;
        }

        protected ScrollableControl ParentControl
        {
            get { return parent; }
        }
        
        internal void UpdateScrollInfo()
        {
            var scrollbar = ParentControl.GetScrollBar(Orientation);
            if (scrollbar == null)
                return;

            scrollbar.Minimum = minimum;
            scrollbar.Maximum = maximum;
            scrollbar.SmallChange = smallChange;
            scrollbar.LargeChange = parent.AutoScroll ? PageSize : LargeChange;
        }

        private void EnableScroll(bool enable)
        {
            ParentControl.Native_EnableScrollBar(enable, Orientation);
        }
    }
}
