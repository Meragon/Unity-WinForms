using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel_Mini
{
    public class CancelEventArgs : EventArgs
    {
        public CancelEventArgs()
        {

        }
        public CancelEventArgs(bool cancel)
        {
            Cancel = cancel;
        }

        public bool Cancel { get; set; }
    }
}
