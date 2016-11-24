using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public sealed class Cursor
    {
        private static Cursor current;
        private static Cursor currentSystem;

        private Bitmap image;

        public static Cursor Current
        {
            get { return current; }
            set
            {
                if (current == value) return;

                current = value;
                if (current == null)
                    current = Cursors.Default;

                UnityEngine.Cursor.visible = IsVisible;
            }
        }
        internal static Cursor CurrentSystem
        {
            get { return currentSystem; }
            set
            {
                if (currentSystem == value) return;

                currentSystem = value;

                UnityEngine.Cursor.visible = IsVisible;
            }
        }
        internal static bool IsVisible
        {
            get
            {
                bool cursorVisible = true;
                if (CurrentSystem != null)
                    cursorVisible = currentSystem.image.uTexture == null;
                else
                    cursorVisible = current.image.uTexture == null;
                return cursorVisible;
            }
        }
        public Point HotSpot { get; set; }
        public static Point Position
        {
            get { return Control.MousePosition; }
        }
        public Size Size { get; private set; }

        public Cursor(Bitmap resource)
        {
            this.HotSpot = new Point();
            this.Size = new Size(32, 32);

            image = resource;
        }

        public void SetHotspot(PointPositionTypes x, PointPositionTypes y)
        {
            int pX = 0;
            int pY = 0;

            switch (x)
            {
                case PointPositionTypes.Middle:
                    pX -= Size.Width / 2;
                    break;
                case PointPositionTypes.End:
                    pX -= Size.Width;
                    break;
            }

            switch (y)
            {
                case PointPositionTypes.Middle:
                    pY -= Size.Height / 2;
                    break;
                case PointPositionTypes.End:
                    pY -= Size.Height;
                    break;
            }

            HotSpot = new Point(pX, pY);
        }
        public void Draw(Graphics g, Rectangle targetRect)
        {
            g.DrawImage(image, Position.X + HotSpot.X, Position.Y + HotSpot.Y, Size.Width, Size.Height);
        }

        public enum PointPositionTypes
        {
            Start,
            Middle,
            End
        }
    }
}
