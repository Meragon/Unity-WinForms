using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public abstract class ListControl : Control
    {
        public abstract int SelectedIndex { get; set; }
        public object SelectedValue { get; set; }

        public event EventHandler SelectedValueChanged = delegate { };

        protected virtual void OnSelectedIndexChanged(EventArgs e)
        {
            OnSelectedValueChanged(EventArgs.Empty);    
        }
        protected virtual void OnSelectedValueChanged(EventArgs e)
        {
            SelectedValueChanged(this, EventArgs.Empty);
        }
    }
}
