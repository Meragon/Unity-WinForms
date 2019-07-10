namespace System.Windows.Forms
{
    public class KeyEventArgs : EventArgs
    {
        private static object[] keyValuesRaw;

        private readonly Keys keyData;
        private Keys keyCode;
        private bool keyCodeCached;
        private bool suppressKeyPress;

        public KeyEventArgs(Keys keyData)
        {
            this.keyData = keyData;
        }
        internal KeyEventArgs()
        {

        }

        public virtual bool Alt
        {
            get { return (keyData & Keys.Alt) == Keys.Alt; }
        }
        public bool Control
        {
            get { return (keyData & Keys.Control) == Keys.Control; }
        }
        public bool Handled { get; set; }
        public Keys KeyCode
        {
            get
            {
                if (!keyCodeCached)
                {
                    var keys = keyData & Keys.KeyCode;
                    keyCode = !IsKeyDefined(keys) ? Keys.None : keys;
                    keyCodeCached = true;
                }

                return keyCode;
            }
        }
        public int KeyValue
        {
            get { return (int)(keyData & Keys.KeyCode); }
        }
        public Keys KeyData
        {
            get { return keyData; }
        }
        public Keys Modifiers
        {
            get { return keyData & Keys.Modifiers; }
        }
        public virtual bool Shift
        {
            get { return (keyData & Keys.Shift) == Keys.Shift; }
        }
        public bool SuppressKeyPress
        {
            get { return suppressKeyPress; }
            set
            {
                suppressKeyPress = value;
                Handled = value;
            }
        }

        // Less allocation, will only work with integer types, like typeof(Keys).
        internal static bool IsKeyDefined(Keys value)
        {
            if (keyValuesRaw == null)
            {
                var fields = typeof(Keys).GetFields(
                    Reflection.BindingFlags.Public |
                    Reflection.BindingFlags.NonPublic |
                    Reflection.BindingFlags.Static);

                keyValuesRaw = new object[fields.Length];
                for (int i = 0; i < fields.Length; i++)
                    keyValuesRaw[i] = fields[i].GetRawConstantValue();
            }

            var keyValuesRawLength = keyValuesRaw.Length;
            for (int i = 0; i < keyValuesRawLength; i++)
                if ((Keys)keyValuesRaw[i] == value)
                    return true;
            
            return false;
        }
    }
}
