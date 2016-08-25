using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// Additional methods for designing and managing controls.
    /// </summary>
    public static class ControlHelper
    {
        /// <summary>
        /// Sets button's back and border colors for normal and hover state to specified color.
        /// </summary>
        /// <param name="button"></param>
        public static void ClearColor(this Button button, Color clearColor)
        {
            button.NormalColor = clearColor;
            button.NormalBorderColor = clearColor;
            button.HoverColor = clearColor;
            button.HoverBorderColor = clearColor;
        }
    }
}
