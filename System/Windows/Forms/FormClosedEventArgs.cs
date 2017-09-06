namespace System.Windows.Forms
{
    public class FormClosedEventArgs : EventArgs
    {
        public FormClosedEventArgs(CloseReason closeReason)
        {
            CloseReason = closeReason;
        }

        public CloseReason CloseReason { get; private set; }
    }
}
