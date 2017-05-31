using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Drawing
{
    public sealed class SystemBrushes
    {
        public static Brush AppWorkspace
        {
            get { return new SolidBrush(SystemColors.AppWorkspace); }
        }
    }
}
