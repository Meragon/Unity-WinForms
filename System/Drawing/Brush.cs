using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Drawing
{
    public abstract class Brush : ICloneable, IDisposable
    {
        public abstract object Clone();
        public void Dispose()
        {
            
        }
    }
}
