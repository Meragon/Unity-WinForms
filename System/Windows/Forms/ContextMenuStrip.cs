using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public class ContextMenuStrip : ToolStripDropDownMenu
    {
        public ContextMenuStrip()
        {
            BackColor = Color.FromArgb(246, 246, 246);
            Context = true;
            ShadowBox = true;
        }
    }
}
