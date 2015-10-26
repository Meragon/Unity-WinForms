using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Drawing
{
    public sealed class StringFormat
    {
        public StringAlignment Alignment { get; set; }
        public StringAlignment LineAlignment { get; set; }
    }

    public enum StringAlignment
    {
        Near = 0,
        Center = 1,
        Far = 2,
    }
}
