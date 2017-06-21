using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace System.Windows.Forms
{
    /// <summary>
    /// implement your windows hook here.
    /// </summary>
    public static class MouseHook
    {
        public static event MouseEventHandler MouseDown = delegate { };
        [Obsolete("not supported yet")]
        public static event MouseEventHandler MouseMove = delegate { };
        public static event MouseEventHandler MouseUp = delegate { };
        [Obsolete("not supported yet")]
        public static event MouseEventHandler MouseWheel = delegate { };

        public static void RaiseMouseDown(object sender, MouseEventArgs args)
        {
            MouseDown(sender, args);
        }
        public static void RaiseMouseMove(object sender, MouseEventArgs args)
        {
            MouseMove(sender, args);
        }
        public static void RaiseMouseWheel(object sender, MouseEventArgs args)
        {
            MouseWheel(sender, args);
        }
        public static void RaiseMouseUp(object sender, MouseEventArgs args)
        {
            MouseUp(sender, args);
        }
    }
}
