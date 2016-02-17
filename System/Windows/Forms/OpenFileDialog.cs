using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Forms
{
    public class OpenFileDialog : FileDialog
    {
        public OpenFileDialog()
        {
            Text = "Open file";

            buttonOk.Text = "Open";
        }
    }
}
