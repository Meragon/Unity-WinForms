namespace System.Windows.Forms
{
    using System.Drawing;

    public class DrawItemEventArgs : EventArgs
    {
        private readonly Color backColor;
        private readonly DrawItemState state;
        private readonly Color foreColor;

        public DrawItemEventArgs(Graphics graphics, Font font, Rectangle rect, int index, DrawItemState state)
        {
            Graphics = graphics;
            Font = font;
            Bounds = rect;
            Index = index;
            this.state = state;
        }
        public DrawItemEventArgs(Graphics graphics, Font font, Rectangle rect, int index, DrawItemState state,
                                 Color foreColor, Color backColor) : this(graphics, font, rect, index, state)
        {
            this.foreColor = foreColor;
            this.backColor = backColor;
        }

        public Color BackColor
        {
            get
            {
                return backColor;
            }
        }
        public Rectangle Bounds { get; private set; }
        public Font Font { get; private set; }
        public Color ForeColor
        {
            get
            {
                return foreColor;
            }
        }
        public Graphics Graphics { get; private set; }
        public int Index { get; private set; }
        public DrawItemState State
        {
            get { return state; }
        }

        public virtual void DrawBackground()
        {
            Graphics.uwfFillRectangle(BackColor, Bounds);
        }
        public virtual void DrawFocusRectangle()
        {

        }
    }
}
