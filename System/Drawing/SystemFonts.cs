using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Drawing
{
    public sealed class SystemFonts
    {
        private static readonly Font defaultFont = new Font("Arial", 12);

        public static Font DefaultFont { get { return defaultFont; } }
    }
}
