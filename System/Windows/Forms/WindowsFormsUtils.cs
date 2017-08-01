namespace System.Windows.Forms
{
    internal sealed class WindowsFormsUtils
    {
        public static bool SafeCompareStrings(string string1, string string2, bool ignoreCase)
        {
            return string1 != null && string2 != null && string1.Length == string2.Length && string.Compare(string1, string2, ignoreCase) == 0;
        }
    }
}
