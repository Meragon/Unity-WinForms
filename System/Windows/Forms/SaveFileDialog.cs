using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Forms
{
    public class SaveFileDialog : FileDialog
    {
        public SaveFileDialog()
        {
            Text = "Save as";

            buttonOk.Text = "Save";
        }
    }
}
