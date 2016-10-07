using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Forms
{
    public interface IButtonControl
    {
        DialogResult DialogResult { get; set; }

        void NotifyDefault(bool value);
        void PerformClick();
    }
}
