using UnityEngine;
using System;
using System.Collections.Generic;
using System.Drawing.API;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Unity.API;

namespace System.Drawing
{
    public sealed class Graphics : IDeviceContext, IDisposable
    {
        internal static IApiGraphics ApiGraphics { get; set; }

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

        private static PointF[] GetBezierApproximation(PointF[] controlPoints, int outputSegmentCount)
        {
            if (outputSegmentCount <= 0) return null;
            PointF[] points = new PointF[outputSegmentCount + 1];
            for (int i = 0; i <= outputSegmentCount; i++)
            {
                float t = (float)i / outputSegmentCount;
                points[i] = GetBezierPoint(t, controlPoints, 0, controlPoints.Length);
            }
            return points;
        }
        private static PointF GetBezierPoint(float t, PointF[] controlPoints, int index, int count)
        {
            if (count == 1)
                return controlPoints[index];
            var P0 = GetBezierPoint(t, controlPoints, index, count - 1);
            var P1 = GetBezierPoint(t, controlPoints, index + 1, count - 1);
            return new PointF((1 - t) * P0.X + t * P1.X, (1 - t) * P0.Y + t * P1.Y);
        }
        private static int GUI_SetFont(GUIStyle style, Font font)
        {
            int guiSkinFontSizeBuffer = GUI.skin.label.fontSize;
            if (font != null)
            {
                if (font.UFont == null && Unity.API.ApplicationBehaviour.gResources != null && Unity.API.ApplicationBehaviour.gResources.Fonts != null)
                {
                    var fonts = Unity.API.ApplicationBehaviour.gResources.Fonts;
                    for (int i = 0; i < fonts.Count; i++)
                        if (fonts[i].fontNames[0] == font.Name)
                        {
                            font.UFont = fonts[i];
                            break;
                        }
                }

                if (font.UFont != null)
                    style.font = font.UFont;
                else
                {
                    style.font = null;
#if UNITY_EDITOR
                    UnityEngine.Debug.LogError("Font not found: " + font.Name);
#endif
                }

                style.fontSize = (int)(font.Size);
                bool styleBold = (font.Style & FontStyle.Bold) == FontStyle.Bold;
                bool styleItalic = (font.Style & FontStyle.Italic) == FontStyle.Italic;
                if (styleBold)
                {
                    if (styleItalic)
                        style.fontStyle = UnityEngine.FontStyle.BoldAndItalic;
                    else
                        style.fontStyle = UnityEngine.FontStyle.Bold;
                }
                else if (styleItalic)
                    style.fontStyle = UnityEngine.FontStyle.Italic;
                else style.fontStyle = UnityEngine.FontStyle.Normal;
            }
            else
            {
                if (ApplicationBehaviour.gResources.Fonts.Count > 0)
                {
                    var _font = ApplicationBehaviour.gResources.Fonts[0];
                    if (_font != null)
                        style.font = _font;
                    style.fontSize = (int)(12);
                    style.fontStyle = UnityEngine.FontStyle.Normal;
                }
            }
            return guiSkinFontSizeBuffer;
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
                DrawLine(pen, bPoints[i].X, bPoints[i].Y, bPoints[i + 1].X, bPoints[i + 1].Y);
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
            if (image == null) return;

            ApiGraphics.DrawImage(image, Color.White, x, y, width, height, 0);
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
            uwfDrawImage(ApplicationBehaviour.GdiImages.Circle, brush.Color, x, y, width, height);
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

        #region Not original methods.
        internal void uwfDrawImage(Image image, Color color, float x, float y, float width, float height)
        {
            ApiGraphics.DrawImage(image, color, x, y, width, height, 0);
        }
        internal string uwfDrawPasswordField(string s, Font font, SolidBrush brush, float x, float y, float width, float height, HorizontalAlignment alignment)
        {
            if (s == null) s = "";

            GUI.skin.textField.alignment = TextAnchor.UpperLeft;
            switch (alignment)
            {
                case HorizontalAlignment.Center:
                    GUI.skin.textField.alignment = TextAnchor.MiddleCenter;
                    break;
                default:
                    GUI.skin.textField.alignment = TextAnchor.MiddleLeft;
                    break;
                case HorizontalAlignment.Right:
                    GUI.skin.textField.alignment = TextAnchor.MiddleRight;
                    break;
            }

            if (font != null)
            {
                if (Unity.API.ApplicationBehaviour.gResources != null)
                {
                    var _font = Unity.API.ApplicationBehaviour.gResources.Fonts.Find(f => f.fontNames[0] == font.Name);
                    if (_font != null)
                        GUI.skin.textField.font = _font;
                    else
                        GUI.skin.textField.font = null;
                }
                GUI.skin.textField.fontSize = (int)font.Size;
                bool styleBold = (font.Style & FontStyle.Bold) == FontStyle.Bold;
                bool styleItalic = (font.Style & FontStyle.Italic) == FontStyle.Italic;
                if (styleBold)
                {
                    if (styleItalic)
                        GUI.skin.textField.fontStyle = UnityEngine.FontStyle.BoldAndItalic;
                    else
                        GUI.skin.textField.fontStyle = UnityEngine.FontStyle.Bold;
                }
                else if (styleItalic)
                    GUI.skin.textField.fontStyle = UnityEngine.FontStyle.Italic;
                else GUI.skin.textField.fontStyle = UnityEngine.FontStyle.Normal;
            }
            else
            {
                var _font = Unity.API.ApplicationBehaviour.gResources.Fonts.Find(f => f.fontNames[0] == "Arial");
                if (_font != null)
                    GUI.skin.textField.font = _font;
                GUI.skin.textField.fontSize = 12;
            }

            GUI.color = brush.Color.ToUnityColor();

            return GUI.PasswordField(new Rect(x, y, width, height), s, '*');
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
            ContentAlignment alignment = ContentAlignment.MiddleLeft;
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
        internal string uwfDrawTextArea(string s, Font font, SolidBrush brush, float x, float y, float width, float height)
        {
            return uwfDrawTextArea(s, font, brush.Color, x, y, width, height);
        }
        internal string uwfDrawTextArea(string s, Font font, Color color, float x, float y, float width, float height)
        {
            if (s == null) s = "";

            GUI.skin.textArea.alignment = TextAnchor.UpperLeft;

            GUI.color = color.ToUnityColor();
            //GUI.skin.textArea.hover.textColor = brush.Color.ToUColor();

            GUI_SetFont(GUI.skin.textArea, font);

            GUI.skin.textArea.hover.background = null;
            GUI.skin.textArea.active.background = null;
            GUI.skin.textArea.focused.background = null;
            GUI.skin.textArea.normal.background = null;

            return GUI.TextArea(new Rect(x, y, width, height), s);
        }
        internal string uwfDrawTextField(string s, Font font, Color color, float x, float y, float width, float height, HorizontalAlignment alignment)
        {
            if (s == null) s = "";

            GUI.skin.textField.alignment = TextAnchor.UpperLeft;
            switch (alignment)
            {
                case HorizontalAlignment.Center:
                    GUI.skin.textField.alignment = TextAnchor.MiddleCenter;
                    break;
                default:
                    GUI.skin.textField.alignment = TextAnchor.MiddleLeft;
                    break;
                case HorizontalAlignment.Right:
                    GUI.skin.textField.alignment = TextAnchor.MiddleRight;
                    break;
            }

            GUI_SetFont(GUI.skin.textField, font);

            GUI.color = color.ToUnityColor();
            GUI.skin.textField.hover.background = null;
            GUI.skin.textField.active.background = null;
            GUI.skin.textField.focused.background = null;
            GUI.skin.textField.normal.background = null;

            return GUI.TextField(new Rect(x, y, width, height), s);
        }
        internal string uwfDrawTextField(string s, Font font, SolidBrush brush, float x, float y, float width, float height, HorizontalAlignment alignment)
        {
            return uwfDrawTextField(s, font, brush.Color, x, y, width, height, alignment);
        }
        internal void uwfDrawTexture(Texture texture, float x, float y, float width, float height, Material mat = null)
        {
            uwfDrawTexture(texture, x, y, width, height, Color.White, mat);
        }
        internal void uwfDrawTexture(Texture texture, float x, float y, float width, float height, Color color, Material mat = null)
        {
            if (texture == null) return;
            if (mat != null)
                mat.color = color.ToUnityColor();
            else
                GUI.color = color.ToUnityColor();
            UnityEngine.Graphics.DrawTexture(new Rect(x, y, width, height), texture, mat);
        }
        internal void uwfFillPolygonConvex(SolidBrush brush, PointF[] points)
        {
            if (points.Length < 3) return;

            GL.Begin(GL.TRIANGLES);

            var color = brush.Color.ToUnityColor();
            GL.Color(color);

            for (int i = 1; i + 1 < points.Length; i += 1)
            {
                GL.Vertex3(points[0].X, points[0].Y, 0);
                GL.Vertex3(points[i].X, points[i].Y, 0);
                GL.Vertex3(points[i + 1].X, points[i + 1].Y, 0);
            }

            GL.End();
        }
        internal void uwfFillRectangle(Color color, Rectangle rect)
        {
            uwfFillRectangle(color, rect.X, rect.Y, rect.Width, rect.Height);
        }
        internal void uwfFillRectangle(Color color, float x, float y, float width, float height)
        {
            uwfFillRectangle(color, x, y, width, height, null);
        }
        internal void uwfFillRectangle(Color color, float x, float y, float width, float height, Material mat)
        {
            if (color.A <= 0) return;

            ApiGraphics.FillRectangle(color, x, y, width, height, mat);
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
    }
}
