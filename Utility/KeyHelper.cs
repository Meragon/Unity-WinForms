namespace System.Windows.Forms
{
    public static class KeyHelper
    {
        /// <summary>
        /// Get last char from UnityEngine.Input.inputString.
        /// </summary>
        /// <returns>\0 if no chars was found</returns>
        public static char GetLastInputChar()
        {
            int len = UnityEngine.Input.inputString.Length;
            if (len == 0) return '\0';
            return UnityEngine.Input.inputString[len - 1];
        }
    }
}
