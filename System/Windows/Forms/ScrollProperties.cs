namespace System.Windows.Forms
{
    public abstract class ScrollProperties
    {
        private readonly ScrollableControl parentControl;

        protected ScrollProperties(ScrollableControl container)
        {
            parentControl = container;
        }

        public bool Enabled { get; set; }
        public int LargeChange { get; set; }
        public int Maximum { get; set; }
        public int Minimum { get; set; }
        public int SmallChange { get; set; }
        public int Value { get; set; }
        public bool Visible { get; set; }

        protected ScrollableControl ParentControl { get { return parentControl; } }
    }
}
