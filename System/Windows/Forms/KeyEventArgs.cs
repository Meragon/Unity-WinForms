namespace System.Windows.Forms
{
    public class KeyEventArgs : EventArgs
    {
        private readonly Keys keyData;
        private Keys keyCode;
        private bool keyCodeCached;
        private bool handled;
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
        public bool Handled
        {
            get { return handled; }
            set { handled = value; }
        }
        public Keys KeyCode
        {
            get
            {
                if (keyCodeCached == false)
                {
                    var keys = keyData & Keys.KeyCode;
                    keyCode = !Enum.IsDefined(typeof(Keys), keys) ? Keys.None : keys;
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
                handled = value;
            }
        }

        public UnityEngine.KeyCode uwfKeyCode { get; set; }
        public UnityEngine.EventModifiers uwfModifiers { get; set; }
    }
}
