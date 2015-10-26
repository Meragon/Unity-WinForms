using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Forms
{
    public class ToolStripItemClickedEventArgs : EventArgs
    {
        private ToolStripItem _clickedItem;

        public ToolStripItemClickedEventArgs(ToolStripItem clickedItem)
        {
            _clickedItem = clickedItem;
        }

        public ToolStripItem ClickedItem { get { return _clickedItem; } }
    }
}
