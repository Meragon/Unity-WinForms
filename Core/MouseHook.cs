namespace System.Windows.Forms
{
    /// <summary>
    /// implement your windows hook here.
    /// </summary>
    internal static class MouseHook
    {
        // usable by childs, ex: ToolStripItem will invoke MouseUp then parent will invoke same event,
        // which will prevent unity designer to paint child fields and properties.
        public static bool preventNextMouseUpInvoke; 

        public static event MouseEventHandler MouseDown;
        [Obsolete("not supported yet")]
        public static event MouseEventHandler MouseMove;
        public static event MouseEventHandler MouseUp;
        [Obsolete("not supported yet")]
        public static event MouseEventHandler MouseWheel;
        
        public static void RaiseMouseDown(object sender, MouseEventArgs args)
        {
            var handler = MouseDown;
            if (handler != null)
                handler(sender, args);
        }
        public static void RaiseMouseMove(object sender, MouseEventArgs args)
        {
#pragma warning disable 618
            var handler = MouseMove;
#pragma warning restore 618
            if (handler != null)
                handler(sender, args);
        }
        public static void RaiseMouseWheel(object sender, MouseEventArgs args)
        {
#pragma warning disable 618
            var handler = MouseWheel;
#pragma warning restore 618
            if (handler != null)
                handler(sender, args);
        }
        public static void RaiseMouseUp(object sender, MouseEventArgs args)
        {
            if (preventNextMouseUpInvoke == false)
            {
                var handler = MouseUp;
                if (handler != null)
                    handler(sender, args);
            }
            else
                preventNextMouseUpInvoke = false;
        }
    }
}
