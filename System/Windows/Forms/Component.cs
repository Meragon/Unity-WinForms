using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Forms
{
    public class Component
    {
        public virtual void Dispose() { }
        protected virtual void Dispose(bool disposing) { }
    }
}
