namespace System.Windows.Forms
{
    using System.Drawing;

    public sealed class Cursor
    {
        private static Cursor current;
        private static Cursor currentSystem;

        private readonly Bitmap image;

        public Cursor(Bitmap resource)
        {
            HotSpot = new Point();
            Size = new Size(32, 32);

            image = resource;
        }

        public enum PointPositionTypes
        {
            Start,
            Middle,
            End
        }

        public static Cursor Current
        {
            get { return current; }
            set
            {
                if (current == value) return;

                current = value;
                if (current == null)
                    current = Cursors.Default;

                ApiHolder.Input.CursorVisible = IsVisible;
            }
        }
        public static Point Position
        {
            get { return Control.MousePosition; }
        }

        public Point HotSpot { get; set; }
        public Size Size { get; private set; }

        internal static Cursor CurrentSystem
        {
            get { return currentSystem; }
            set
            {
                if (currentSystem == value) return;

                currentSystem = value;

                ApiHolder.Input.CursorVisible = IsVisible;
            }
        }
        internal static bool IsVisible
        {
            get
            {
                bool cursorVisible = true;
                if (CurrentSystem != null)
                    cursorVisible = currentSystem.image == null || currentSystem.image.Texture == null;
                else
                    cursorVisible = current == null || current.image == null || current.image.Texture == null;
                return cursorVisible;
            }
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
            g.DrawImage(
                image, 
                targetRect.X + HotSpot.X / Application.ScaleX, 
                targetRect.Y + HotSpot.Y / Application.ScaleY, 
                targetRect.Width, 
                targetRect.Height);
        }
    }
}
