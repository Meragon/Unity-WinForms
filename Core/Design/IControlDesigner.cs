using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Forms.Design
{
    public interface IControlDesigner
    {
        Control Control { get; }

        object Draw(int width, int height);
    }
}
