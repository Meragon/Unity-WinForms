#if UNITY_STANDALONE_WIN
#define KEYBOARD_LAYOUT_SUPPORTED
#endif

namespace System.Windows.Forms
{
    internal static class SafeNativeMethods
    {
#if KEYBOARD_LAYOUT_SUPPORTED
        
        [System.Runtime.InteropServices.DllImport("user32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        [System.Runtime.Versioning.ResourceExposure(System.Runtime.Versioning.ResourceScope.None)]
        public static extern IntPtr GetKeyboardLayout(int dwLayout);
        
#endif
    }
}