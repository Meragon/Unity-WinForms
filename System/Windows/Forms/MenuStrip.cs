using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Forms
{
    [Serializable]
    public class MenuStrip : ToolStrip
    {
        public MenuStrip()
        {
            BorderColor = Drawing.Color.Transparent;
            Orientation = Forms.Orientation.Horizontal;
            Padding = new Padding(2);
            Size = new System.Drawing.Size(600, 24);
        }
    }
}
