namespace System.Drawing
{
    using System.Drawing.API;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    using Unity.API; // TODO: remove

    public sealed class Graphics : IDeviceContext, IDisposable
    {
        internal static IApiGraphics ApiGraphics
        {
            get { return ApiHolder.Graphics; }
        }

        public void Clear(Color color)
        {
            ApiGraphics.Clear(color);
        }
        public void Dispose()
        {
        }
        public void DrawCurve(Pen pen, PointF[] points, int segments = 32) // very slow.
        {
            if (points == null || points.Length <= 1) return;
            if (points.Length == 2)
            {
                DrawLine(pen, points[0].X, points[0].Y, points[1].X, points[1].Y);
                return;
            }

            var bPoints = GetBezierApproximation(points, segments); // decrease segments for better fps.
            for (int i = 0; i + 1 < bPoints.Length; i++)
            {
                var p1 = bPoints[i];
                var p2 = bPoints[i + 1];
                DrawLine(pen, p1.X, p1.Y, p2.X, p2.Y);
            }
        }
        public void DrawImage(Image image, Rectangle rect)
        {
            DrawImage(image, rect.X, rect.Y, rect.Width, rect.Height);
        }
        public void DrawImage(Image image, RectangleF rect)
        {
            DrawImage(image, rect.X, rect.Y, rect.Width, rect.Height);
        }
        public void DrawImage(Image image, int x, int y, int width, int height)
        {
            DrawImage(image, (float)x, (float)y, (float)width, (float)height);
        }
        public void DrawImage(Image image, float x, float y, float width, float height)
        {
            ApiGraphics.DrawImage(image, x, y, width, height, 0);
        }
        public void DrawLine(Pen pen, Point pt1, Point pt2)
        {
            DrawLine(pen, pt1.X, pt1.Y, pt2.X, pt2.Y);
        }
        public void DrawLine(Pen pen, PointF pt1, PointF pt2)
        {
            DrawLine(pen, pt1.X, pt1.Y, pt2.X, pt2.Y);
        }
        public void DrawLine(Pen pen, int x1, int y1, int x2, int y2)
        {
            DrawLine(pen, (float)x1, (float)y1, (float)x2, (float)y2);
        }
        public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
        {
            ApiGraphics.DrawLine(pen, x1, y1, x2, y2);
        }
        public void DrawLines(Pen pen, PointF[] points)
        {
            for (int i = 0; i < points.Length - 1; i++)
                DrawLine(pen, points[i], points[i + 1]);
        }
        public void DrawLines(Pen pen, Point[] points)
        {
            for (int i = 0; i < points.Length - 1; i++)
                DrawLine(pen, points[i], points[i + 1]);
        }
        public void DrawPolygon(Pen pen, Point[] points)
        {
            ApiGraphics.DrawPolygon(pen, points);
        }
        public void DrawRectangle(Pen pen, Rectangle rect)
        {
            DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
        }
        public void DrawRectangle(Pen pen, int x, int y, int width, int height)
        {
            DrawRectangle(pen, (float)x, (float)y, (float)width, (float)height);
        }
        public void DrawRectangle(Pen pen, float x, float y, float width, float height)
        {
            ApiGraphics.DrawRectangle(pen, x, y, width, height);
        }
        public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle)
        {
            uwfDrawString(s, font, brush, layoutRectangle.X, layoutRectangle.Y, layoutRectangle.Width, layoutRectangle.Height);
        }
        public void DrawString(string s, Font font, Brush brush, PointF point)
        {
            DrawString(s, font, brush, point.X, point.Y);
        }
        public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format)
        {
            uwfDrawString(s, font, brush, layoutRectangle.X, layoutRectangle.Y, layoutRectangle.Width, layoutRectangle.Height, format);
        }
        public void DrawString(string s, Font font, Brush brush, PointF point, StringFormat format)
        {
            DrawString(s, font, brush, point.X, point.Y, format);
        }
        public void DrawString(string s, Font font, Brush brush, float x, float y)
        {
            uwfDrawString(s, font, brush, x, y, 512, 64);
        }
        public void DrawString(string s, Font font, Brush brush, float x, float y, StringFormat format)
        {
            uwfDrawString(s, font, brush, x, y, 512, 64, format);
        }
        public void FillEllipse(SolidBrush brush, float x, float y, float width, float height)
        {
            uwfDrawImage(ApplicationResources.Images.Circle, brush.Color, x, y, width, height);
        }
        public void FillRectangle(Brush brush, Rectangle rect)
        {
            FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height);
        }
        public void FillRectangle(Brush brush, RectangleF rect)
        {
            FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height);
        }
        public void FillRectangle(Brush brush, int x, int y, int width, int height)
        {
            ApiGraphics.FillRectangle(brush, x, y, width, height);
        }
        public void FillRectangle(Brush brush, float x, float y, float width, float height)
        {
            ApiGraphics.FillRectangle(brush, x, y, width, height);
        }
        public void FillRectangles(Brush brush, RectangleF[] rects)
        {
            for (int i = 0; i < rects.Length; i++)
            {
                var r = rects[i];
                FillRectangle(brush, r);
            }
        }
        public void FillRectangles(Brush brush, Rectangle[] rects)
        {
            for (int i = 0; i < rects.Length; i++)
            {
                var r = rects[i];
                FillRectangle(brush, r);
            }
        }
        public SizeF MeasureString(string text, Font font)
        {
            return ApiGraphics.MeasureString(text, font);
        }

        internal static PointF[] GetBezierApproximation(PointF[] controlPoints, int outputSegmentCount)
        {
            if (outputSegmentCount <= 0) 
                return new PointF[] {};
            
            var points = new PointF[outputSegmentCount + 1];
            for (int i = 0; i <= outputSegmentCount; i++)
            {
                float t = (float) i / outputSegmentCount;
                points[i] = GetBezierPoint(t, controlPoints, 0, controlPoints.Length);
            }
            
            return points;
        }
        internal void GroupBegin(Control control)
        {
            var co = control.uwfOffset;
            var x = control.Location.X + co.X;
            var y = control.Location.Y + co.Y;
            var w = control.ClientSize.Width;
            var h = control.ClientSize.Height;

            ApiGraphics.BeginGroup(x, y, w, h);
        }
        internal void GroupEnd()
        {
            ApiGraphics.EndGroup();
        }

        #region Not original methods.
        internal void uwfDrawImage(Image image, Color color, float x, float y, float width, float height)
        {
            image.Color = color;
            ApiGraphics.DrawImage(image, x, y, width, height, 0);
        }
        internal void uwfDrawLine(Color color, int width, DashStyle style, float x1, float y1, float x2, float y2)
        {
            ApiGraphics.DrawLine(color, width, style, x1, y1, x2, y2);
        }
        internal string uwfDrawPasswordField(string s, Font font, Color color, float x, float y, float width, float height, HorizontalAlignment alignment)
        {
            // TODO: textBox.UseSystemPasswordChar.
            var unityApi = ApiGraphics as UnityGdi;
            if (unityApi != null)
                return unityApi.uwfDrawPasswordField(s, font, color, x, y, width, height, alignment);
            
            throw new NotImplementedException("uwfDrawPasswordField");
        }
        internal void uwfDrawString(string s, Font font, Brush brush, float x, float y, float width, float height, ContentAlignment alignment)
        {
            var solidBrush = brush as SolidBrush;
            if (solidBrush == null) return;

            uwfDrawString(s, font, solidBrush.Color, x, y, width, height, alignment);
        }
        internal void uwfDrawString(string s, Font font, Color color, float x, float y, float width, float height, ContentAlignment alignment)
        {
            if (color.A <= 0) return;
            if (string.IsNullOrEmpty(s)) return;

            ApiGraphics.DrawString(s, font, color, x, y, width, height, alignment);
        }
        internal void uwfDrawString(string s, Font font, Color color, float x, float y)
        {
            uwfDrawString(s, font, color, x, y, 512, 64);
        }
        internal void uwfDrawString(string s, Font font, Brush brush, float x, float y, float width, float height)
        {
            uwfDrawString(s, font, brush, x, y, width, height, new StringFormat());
        }
        internal void uwfDrawString(string s, Font font, Color color, float x, float y, float width, float height)
        {
            uwfDrawString(s, font, color, x, y, width, height, ContentAlignment.BottomLeft);
        }
        internal void uwfDrawString(string s, Font font, Color color, float x, float y, float width, float height, HorizontalAlignment alignment)
        {
            ContentAlignment ca = ContentAlignment.MiddleLeft;
            if (alignment == HorizontalAlignment.Center)
                ca = ContentAlignment.MiddleCenter;
            else if (alignment == HorizontalAlignment.Right)
                ca = ContentAlignment.MiddleRight;
            uwfDrawString(s, font, color, x, y, width, height, ca);
        }
        internal void uwfDrawString(string s, Font font, Brush brush, float x, float y, float width, float height, HorizontalAlignment horizontalAlignment)
        {
            ContentAlignment alignment;
            switch (horizontalAlignment)
            {
                default:
                    alignment = ContentAlignment.MiddleLeft;
                    break;
                case HorizontalAlignment.Center:
                    alignment = ContentAlignment.MiddleCenter;
                    break;
                case HorizontalAlignment.Right:
                    alignment = ContentAlignment.MiddleRight;
                    break;
            }
            uwfDrawString(s, font, brush, x, y, width, height, alignment);
        }
        internal void uwfDrawString(string s, Font font, Brush brush, float x, float y, float width, float height, StringFormat format)
        {
            uwfDrawString(s, font, brush, x, y, width, height, format.ToContentAlignment());
        }
        internal string uwfDrawTextArea(string s, Font font, Color color, float x, float y, float width, float height)
        {
            // TODO: TextBoxBase
            var unityApi = ApiGraphics as UnityGdi;
            if (unityApi != null)
                return unityApi.uwfDrawTextArea(s, font, color, x, y, width, height);

            throw new NotImplementedException("uwfDrawTextArea");
        }
        internal string uwfDrawTextField(string s, Font font, Color color, float x, float y, float width, float height, HorizontalAlignment alignment)
        {
            // TODO: TextBoxBase
            var unityApi = ApiGraphics as UnityGdi;
            if (unityApi != null)
                return unityApi.uwfDrawTextField(s, font, color, x, y, width, height, alignment);
            
            throw new NotImplementedException("uwfDrawTextField");
        }
        internal string uwfDrawTextField(string s, Font font, SolidBrush brush, float x, float y, float width, float height, HorizontalAlignment alignment)
        {
            return uwfDrawTextField(s, font, brush.Color, x, y, width, height, alignment);
        }
        internal void uwfFillRectangle(Color color, Rectangle rect)
        {
            uwfFillRectangle(color, rect.X, rect.Y, rect.Width, rect.Height);
        }
        internal void uwfFillRectangle(Color color, float x, float y, float width, float height)
        {
            uwfFillRectangle(color, x, y, width, height, null);
        }
        internal void uwfFillRectangle(Color color, float x, float y, float width, float height, object material)
        {
            if (color.A <= 0) return;

            ApiGraphics.FillRectangle(color, x, y, width, height, material);
        }
        internal void uwfFocus()
        {
            ApiGraphics.Focus();
        }
        internal void uwfFocusNext()
        {
            ApiGraphics.FocusNext();
        }
        #endregion

        private static PointF GetBezierPoint(float t, PointF[] controlPoints, int index, int count)
        {
            if (count == 1)
                return controlPoints[index];
            var P0 = GetBezierPoint(t, controlPoints, index, count - 1);
            var P1 = GetBezierPoint(t, controlPoints, index + 1, count - 1);
            return new PointF((1 - t) * P0.X + t * P1.X, (1 - t) * P0.Y + t * P1.Y);
        }
    }
}
