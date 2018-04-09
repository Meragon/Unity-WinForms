namespace System.Windows.Forms
{
    internal static class KeyboardHook
    {
        public static event KeyEventHandler KeyDown;
        public static event KeyEventHandler KeyUp;

        public static void RaiseKeyDown(object sender, KeyEventArgs e)
        {
            var handler = KeyDown;
            if (handler != null)
                handler(sender, e);
        }
        public static void RaiseKeyUp(object sender, KeyEventArgs e)
        {
            var handler = KeyUp;
            if (handler != null)
                handler(sender, e);
        }
    }
}
