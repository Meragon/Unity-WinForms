using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public class TabPage : Panel
    {
        public int ImageIndex { get; set; }
        public string ImageKey { get; set; }

        public TabPage()
        {
            BackColor = Color.White;
        }
        public TabPage(string text)
        {
            Text = text;
        }
    }
}
