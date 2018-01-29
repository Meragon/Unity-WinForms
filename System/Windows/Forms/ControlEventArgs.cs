namespace System.Windows.Forms
{
    public delegate void ControlEventHandler(object sender, ControlEventArgs e);

    public class ControlEventArgs : EventArgs
    {
        public ControlEventArgs(Control control)
        {
            Control = control;
        }

        public Control Control { get; private set; }
    }
}
