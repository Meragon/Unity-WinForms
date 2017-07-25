namespace System.Windows.Forms
{
    public enum DragDropEffects
    {
        Scroll = -2147483648,
        All = -2147483645,
        None = 0,
        Copy = 1,
        Move = 2,
        Link = 4,
    }

    public class DragEventArgs : EventArgs
    {
        public DragEventArgs(IDataObject data, int keyState, int x, int y, DragDropEffects allowedEffect, DragDropEffects effect)
        {
            this.Data = data;
            this.KeyState = keyState;
            this.X = x;
            this.Y = y;
            this.AllowedEffect = allowedEffect;
            this.Effect = effect;
        }

        public DragDropEffects AllowedEffect { get; private set; }
        public IDataObject Data { get; private set; }
        public DragDropEffects Effect { get; private set; }
        public int KeyState { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
    }
}
