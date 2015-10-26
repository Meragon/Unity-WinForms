using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public class PaintEventArgs
    {
        public Rectangle ClipRectangle { get; set; }
        public Graphics Graphics { get; set;  }
    }
}
