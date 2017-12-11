namespace System.Windows.Forms
{
    using System.Drawing;

    public enum MouseButtons
    {
        None = 0,
        Left = 1048576,
        Right = 2097152,
        Middle = 4194304,
        XButton1 = 8388608,
        XButton2 = 16777216,
    }

    public class MouseEventArgs : EventArgs
    {
        public MouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta)
        {
            Button = button;
            Clicks = clicks;
            X = x;
            Y = y;
            Location = new Point(X, Y);
            Delta = delta;
        }

        internal MouseEventArgs()
        {
        }

        public MouseButtons Button { get; internal set; }
        public int Clicks { get; internal set; }
        public Point Location { get; internal set; }
        public int X { get; internal set; }
        public int Y { get; internal set; }
        public int Delta { get; internal set; }
    }
}
