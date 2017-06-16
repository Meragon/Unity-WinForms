using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Forms.Design
{
    internal static class DesignerHelper
    {
        public static ControlDesigner GetDesigner(this Control c)
        {
            return new ControlDesigner(c);
        }
    }
}
