#define UNITY_GDI

namespace System.Drawing
{
    /// <summary>
    /// You can copy this to your native windows forms application.
    /// </summary>
    public static class DrawingHelper
    {
        public static Point Add(this Point p1, int val)
        {
            return new Point(p1.X + val, p1.Y + val);
        }
        public static Point Add(this Point p1, Point p2)
        {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }
        public static Point Add(this Point p1, PointF p2)
        {
            return new Point(p1.X + (int)p2.X, p1.Y + (int)p2.Y);
        }
        public static void Apply(this Bitmap bitmap)
        {
#if UNITY_GDI
            bitmap.Texture.Apply();
#endif
        }
        public static int Distance(this Point p1, Point to)
        {
            return (int)Math.Sqrt(Math.Pow(to.X - p1.X, 2) + Math.Pow(to.Y - p1.Y, 2));
        }
        public static Point Divide(this Point p1, int val)
        {
            return new Point(p1.X / val, p1.Y / val);
        }
        public static Point Divide(this Point p1, Point p2)
        {
            return new Point(p1.X / p2.X, p1.Y / p2.Y);
        }
        public static Point Divide(this Point p1, PointF p2)
        {
            return new Point(p1.X / (int)p2.X, p1.Y / (int)p2.Y);
        }
        public static SizeF MeasureStringSimple(this Font font, string text)
        {
            return new SizeF() { Width = text.Length * 8, Height = font.Size }; // fast, but not accurate.
        }
        public static Point Multiply(this Point p1, int val)
        {
            return new Point(p1.X * val, p1.Y * val);
        }
        public static Point Multiply(this Point p1, Point p2)
        {
            return new Point(p1.X * p2.X, p1.Y * p2.Y);
        }
        public static Point Multiply(this Point p1, PointF p2)
        {
            return new Point(p1.X * (int)p2.X, p1.Y * (int)p2.Y);
        }
        public static Point Subtract(this Point p1, int val)
        {
            return new Point(p1.X - val, p1.Y - val);
        }
        public static Point Subtract(this Point p1, Point p2)
        {
            return new Point(p1.X - p2.X, p1.Y - p2.Y);
        }
        public static Point Subtract(this Point p1, PointF p2)
        {
            return new Point(p1.X - (int)p2.X, p1.Y - (int)p2.Y);
        }
        public static ContentAlignment ToContentAlignment(this StringFormat format)
        {
            ContentAlignment alignment = ContentAlignment.TopLeft;
            switch (format.Alignment)
            {
                case StringAlignment.Near:
                    if (format.LineAlignment == StringAlignment.Near)
                        alignment = ContentAlignment.TopLeft;
                    else if (format.LineAlignment == StringAlignment.Center)
                        alignment = ContentAlignment.MiddleLeft;
                    else alignment = ContentAlignment.BottomLeft;
                    break;
                case StringAlignment.Center:
                    if (format.LineAlignment == StringAlignment.Near)
                        alignment = ContentAlignment.TopCenter;
                    else if (format.LineAlignment == StringAlignment.Center)
                        alignment = ContentAlignment.MiddleCenter;
                    else alignment = ContentAlignment.BottomCenter;
                    break;
                case StringAlignment.Far:
                    if (format.LineAlignment == StringAlignment.Near)
                        alignment = ContentAlignment.TopRight;
                    else if (format.LineAlignment == StringAlignment.Center)
                        alignment = ContentAlignment.MiddleRight;
                    else alignment = ContentAlignment.BottomRight;
                    break;
            }

            return alignment;
        }
    }
}
