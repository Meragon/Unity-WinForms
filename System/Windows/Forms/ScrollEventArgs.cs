namespace System.Windows.Forms
{
    public class ScrollEventArgs : EventArgs
    {
        public ScrollEventArgs(ScrollEventType type, int newValue)
        {
            Type = type;
            NewValue = newValue;
        }
        public ScrollEventArgs(ScrollEventType type, int oldValue, int newValue)
        {
            Type = type;
            this.OldValue = oldValue;
            NewValue = newValue;
        }
        public ScrollEventArgs(ScrollEventType type, int newValue, ScrollOrientation scroll)
        {
            Type = type;
            NewValue = newValue;
            ScrollOrientation = scroll;
        }
        public ScrollEventArgs(ScrollEventType type, int oldValue, int newValue, ScrollOrientation scroll)
        {
            Type = type;
            this.OldValue = oldValue;
            NewValue = newValue;
            ScrollOrientation = scroll;
        }

        public int NewValue { get; set; }
        public int OldValue { get; private set; }
        public ScrollOrientation ScrollOrientation { get; private set; }
        public ScrollEventType Type { get; private set; }
    }
}
