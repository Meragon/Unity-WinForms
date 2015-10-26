using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Forms
{
    public abstract class ScrollProperties
    {
        private ScrollableControl _parentControl;

        protected ScrollProperties(ScrollableControl container)
        {
            _parentControl = container;
        }

        public bool Enabled { get; set; }
        public int LargeChange { get; set; }
        public int Maximum { get; set; }
        public int Minimum { get; set; }
        protected ScrollableControl ParentControl { get { return _parentControl; } }
        public int SmallChange { get; set; }
        public int Value { get; set; }
        public bool Visible { get; set; }
    }
}
