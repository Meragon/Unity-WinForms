using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Forms
{
    public class DateRangeEventArgs : EventArgs
    {
        public DateTime End { get; private set; }
        public DateTime Start { get; private set; }

        public DateRangeEventArgs(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }
    }
}
