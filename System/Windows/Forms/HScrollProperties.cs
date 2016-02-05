using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Forms
{
    [Serializable]
    public class HScrollProperties : ScrollProperties
    {
        public HScrollProperties(ScrollableControl container) : base(container)
        {

        }
    }
}
