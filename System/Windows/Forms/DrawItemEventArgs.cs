using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace System.Windows.Forms
{
    public class DrawItemEventArgs : EventArgs
    {
        private readonly Color backColor;
        private readonly DrawItemState state;
        private readonly Color foreColor;

        public Color BackColor
        {
            get
            {
                return this.backColor;
            }
        }
        public Rectangle Bounds { get; private set; }
        public Font Font { get; private set; }
        public Color ForeColor
        {
            get
            {
                return this.foreColor;
            }
        }

        public Graphics Graphics { get; private set; }
        public int Index { get; private set; }
        public DrawItemState State
        {
            get { return state; }
        }

        public DrawItemEventArgs(Graphics graphics, Font font, Rectangle rect, int index, DrawItemState state)
        {
            this.Graphics = graphics;
            this.Font = font;
            this.Bounds = rect;
            this.Index = index;
            this.state = state;
        }
        public DrawItemEventArgs(Graphics graphics, Font font, Rectangle rect, int index, DrawItemState state,
            Color foreColor, Color backColor) : this(graphics, font, rect, index, state)
        {
            this.foreColor = foreColor;
            this.backColor = backColor;
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
