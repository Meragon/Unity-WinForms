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
        private bool _group { get { return _groupControls.Count > 0; } }
        private Control _groupControlLast { get { return _groupControls[_groupControls.Count - 1]; } }
        private readonly List<Control> _groupControls = new List<Control>();
        private static readonly PointF _linePivot = new PointF();

        public static UnityEngine.Material DefaultMaterial { get; set; }
        public static IApiGraphics ApiGraphics { get; set; }
        public static bool GL_Lines { get; set; }
        public static bool NoFill { get; set; }
        public static bool NoRects { get; set; }
        public static bool NoStrings { get; set; }

        internal Control Control { get; set; }
        internal float FillRate { get; set; }
        internal Rectangle Group { get; set; }
        internal void GroupBegin(Control groupControl)
        {
            var c_position = new Point(Control.Location.X + Control.uwfOffset.X, Control.Location.Y + Control.uwfOffset.Y);//Control.PointToScreen(Point.Zero);
            GUI.BeginGroup(new Rect((c_position.X + Group.X), (c_position.Y + Group.Y), Group.Width, Group.Height));
            _groupControls.Add(groupControl);
        }
        internal void GroupEnd()
        {
            GUI.EndGroup();
            _groupControls.RemoveAt(_groupControls.Count - 1);
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
                if (font.UFont == null)
                    font.UFont = System.Windows.Forms.ApplicationBehaviour.Resources.Fonts.Find(f => f.fontNames[0] == font.Name);

                if (font.UFont != null)
                    style.font = font.UFont;
                else
                {
                    style.font = null;
                    UnityEngine.Debug.LogError("Font not found: " + font.Name);
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
                if (ApplicationBehaviour.Resources.Fonts.Count > 0)
                {
                    var _font = ApplicationBehaviour.Resources.Fonts[0];
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
            if (Control != null)
                ApiGraphics.FillRectangle(color, 0, 0, Control.Width, Control.Height);
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

            ApiGraphics.DrawImage(image, Color.White, x, y, width, height);
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
            if (pen.Color.A <= 0) return;

            if (GL_Lines)
            {
                GL.PushMatrix();
                //if (DefaultMaterial != null)
                //  DefaultMaterial.SetPass(0);
                //GL.LoadOrtho();

                GL.Begin(GL.LINES);
                GL.Color(pen.Color.ToUnityColor());

                // TODO: switch (pen.DashStyle) { ... }
                var loc = Control.PointToScreen(Point.Empty);

                GL.Vertex3(x1, y1, 0);
                GL.Vertex3(x2, y2, 0);

                GL.End();
                GL.PopMatrix();
                return;
            }

            float x = 0;
            float y = 0;
            float width = 0;
            float height = 0;

            if (x1 != x2 && y1 != y2)
            {
                Control.uwfBatches++;

                float xDiff = x2 - x1;
                float yDiff = y2 - y1;
                var angle = Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;

                uwfDrawTexture(ApplicationBehaviour.Resources.Images.Circle, x1, y1, (float)Math.Sqrt(xDiff * xDiff + yDiff * yDiff), pen.Width, pen.Color, (float)angle, _linePivot);
                return;
            }

            if (x1 == x2)
            {
                if (y1 > y2) y1 += pen.Width;
                else y2 += pen.Width;
                x = x1;
                width = pen.Width;
                if (y1 < y2)
                {
                    y = y1;
                    height = y2 - y1;
                }
                else
                {
                    y = y2;
                    height = y1 - y2;
                }
            }
            if (y1 == y2)
            {
                y = y1;
                height = pen.Width;
                if (x1 < x2)
                {
                    x = x1;
                    width = x2 - x1;
                }
                else
                {
                    x = x2;
                    width = x1 - x2;
                }
            }

            GUI.color = pen.Color.ToUnityColor();

            switch (pen.DashStyle)
            {
                case Drawing2D.DashStyle.Solid:
                    if (Control != null) Control.uwfBatches++;
                    GUI.DrawTexture(new Rect(x, y, width, height), System.Windows.Forms.ApplicationBehaviour.DefaultSprite);
                    break;
                case Drawing2D.DashStyle.Dash:
                    float dash_step = pen.Width * 6;
                    if (y1 == y2)
                        for (float i = 0; i < width; i += dash_step)
                        {
                            float dash_width = dash_step - 2;
                            if (i + dash_width > width)
                                dash_width = width - i;
                            if (Control != null) Control.uwfBatches++;
                            GUI.DrawTexture(new Rect(x + i, y, dash_width, pen.Width), System.Windows.Forms.ApplicationBehaviour.DefaultSprite);
                        }

                    if (x1 == x2)
                        for (float i = 0; i < height; i += dash_step)
                        {
                            float dash_height = dash_step - 2;
                            if (i + dash_height > height)
                                dash_height = height - i;
                            if (Control != null) Control.uwfBatches++;
                            GUI.DrawTexture(new Rect(x + width - pen.Width, y + i, pen.Width, dash_height), System.Windows.Forms.ApplicationBehaviour.DefaultSprite);
                        }
                    break;
            }
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
            if (DefaultMaterial != null)
                DefaultMaterial.SetPass(0);

            for (int i = 0; i < points.Length; i++)
            {
                if (i + 1 >= points.Length) break;

                GL.Begin(GL.LINES);
                GL.Color(pen.Color.ToUnityColor());

                GL.Vertex3(points[i].X, points[i].Y, 0);
                GL.Vertex3(points[i + 1].X, points[i + 1].Y, 0);

                GL.End();
            }
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
            if (NoRects) return;
            if (pen.Color.A <= 0 || pen.Width <= 0) return;
            GUI.color = pen.Color.ToUnityColor();

            switch (pen.DashStyle)
            {
                case Drawing2D.DashStyle.Solid:
                    if (Control != null)
                        Control.uwfBatches += 2;

                    GUI.DrawTexture(new Rect(x, y, width, pen.Width), System.Windows.Forms.ApplicationBehaviour.DefaultSprite);
                    GUI.DrawTexture(new Rect(x + width - pen.Width, y + pen.Width, pen.Width, height - pen.Width * 2), System.Windows.Forms.ApplicationBehaviour.DefaultSprite);
                    FillRate += width * pen.Width + pen.Width * (height - pen.Width * 2);
                    if (height > 1)
                    {
                        if (Control != null)
                            Control.uwfBatches++;
                        GUI.DrawTexture(new Rect(x, y + height - pen.Width, width, pen.Width), System.Windows.Forms.ApplicationBehaviour.DefaultSprite);
                        FillRate += width * pen.Width + pen.Width;
                    }
                    if (width > 1)
                    {
                        if (Control != null)
                            Control.uwfBatches++;
                        GUI.DrawTexture(new Rect(x, y + pen.Width, pen.Width, height - pen.Width * 2), System.Windows.Forms.ApplicationBehaviour.DefaultSprite);
                        FillRate += pen.Width * (height - pen.Width * 2);
                    }

                    break;
                case Drawing2D.DashStyle.Dash:
                    float dash_step = pen.Width * 6;
                    for (float i = 0; i < width; i += dash_step)
                    {
                        float dash_width = dash_step - 2;
                        if (i + dash_width > width)
                            dash_width = width - i;
                        if (Control != null)
                            Control.uwfBatches += 2;
                        GUI.DrawTexture(new Rect(x + i, y, dash_width, pen.Width), System.Windows.Forms.ApplicationBehaviour.DefaultSprite); // Top.
                        GUI.DrawTexture(new Rect(x + i, y + height - pen.Width, dash_width, pen.Width), System.Windows.Forms.ApplicationBehaviour.DefaultSprite); // Bottom.
                        FillRate += dash_width * pen.Width * 2;
                    }
                    for (float i = 0; i < height; i += dash_step)
                    {
                        float dash_height = dash_step - 2;
                        if (i + dash_height > height)
                            dash_height = height - i;
                        if (Control != null)
                            Control.uwfBatches += 2;
                        GUI.DrawTexture(new Rect(x + width - pen.Width, y + i, pen.Width, dash_height), System.Windows.Forms.ApplicationBehaviour.DefaultSprite); // Right.
                        GUI.DrawTexture(new Rect(x, y + i, pen.Width, dash_height), System.Windows.Forms.ApplicationBehaviour.DefaultSprite); // Left.
                        FillRate += pen.Width * dash_height * 2;
                    }
                    break;
            }
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
            uwfFillRectangle(brush, x, y, width, height);
        }
        public void FillRectangle(Brush brush, float x, float y, float width, float height)
        {
            uwfFillRectangle(brush, x, y, width, height);
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

        
        /// <summary>
        /// OnPaint call only.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        public SizeF MeasureString(string text, Font font)
        {
            return uwfMeasureStringStatic(text, font);
        }
        public SizeF MeasureString(string text, Font font, int width, StringFormat format)
        {
            return new SizeF() { Width = text.Length * 6, Height = font.Size };
        }

        #region Not original methods.
        public void uwfDrawImage(Image image, Color color, float x, float y, float width, float height)
        {
            ApiGraphics.DrawImage(image, color, x, y, width, height);
        }
        public void uwfDrawMesh(Mesh mesh, Point position, Quaternion rotation, Material mat)
        {
            mat.SetPass(0);
            UnityEngine.Graphics.DrawMeshNow(mesh, new Vector3(0, 0, 0), rotation);
        }
        public string uwfDrawPasswordField(string s, Font font, SolidBrush brush, float x, float y, float width, float height, HorizontalAlignment alignment)
        {
            if (Control == null) return s;
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
                if (System.Windows.Forms.ApplicationBehaviour.Resources != null)
                {
                    var _font = System.Windows.Forms.ApplicationBehaviour.Resources.Fonts.Find(f => f.fontNames[0] == font.Name);
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
                var _font = System.Windows.Forms.ApplicationBehaviour.Resources.Fonts.Find(f => f.fontNames[0] == "Arial");
                if (_font != null)
                    GUI.skin.textField.font = _font;
                GUI.skin.textField.fontSize = 12;
            }

            GUI.color = brush.Color.ToUnityColor();

            if (!_group)
            {
                var c_position = Control.PointToScreen(Point.Empty);
                return GUI.PasswordField(new Rect(c_position.X + x, c_position.Y + y, width, height), s, '*');
            }
            else
            {
                var c_position = Control.PointToScreen(Point.Empty);
                var g_position = _groupControlLast.PointToScreen(Point.Empty);
                var position = new PointF(c_position.X - g_position.X + x, c_position.Y - g_position.Y + y);

                return GUI.PasswordField(new Rect(position.X, position.Y, width, height), s, '*');
            }
        }
        public void uwfDrawRectangle(Color color, float x, float y, float width, float height)
        {
            if (NoRects) return;
            if (color.A <= 0) return;

            GUI.color = color.ToUnityColor();

            if (Control != null)
                Control.uwfBatches += 2;

            GUI.DrawTexture(new Rect(x, y, width, 1), System.Windows.Forms.ApplicationBehaviour.DefaultSprite);
            GUI.DrawTexture(new Rect(x + width - 1, y + 1, 1, height - 2), System.Windows.Forms.ApplicationBehaviour.DefaultSprite);
            FillRate += width + height - 2;
            if (height > 1)
            {
                if (Control != null)
                    Control.uwfBatches++;
                GUI.DrawTexture(new Rect(x, y + height - 1, width, 1), System.Windows.Forms.ApplicationBehaviour.DefaultSprite);
                FillRate += width * 1 + 1;
            }
            if (width > 1)
            {
                if (Control != null)
                    Control.uwfBatches++;
                GUI.DrawTexture(new Rect(x, y + 1, 1, height - 2), System.Windows.Forms.ApplicationBehaviour.DefaultSprite);
                FillRate += height - 2;
            }
        }
        public void uwfDrawString(string s, Font font, Brush brush, float x, float y, float width, float height, ContentAlignment alignment)
        {
            var solidBrush = brush as SolidBrush;
            if (solidBrush == null) return;

            uwfDrawString(s, font, solidBrush.Color, x, y, width, height, alignment);
        }
        public void uwfDrawString(string s, Font font, Color color, float x, float y, float width, float height, ContentAlignment alignment)
        {
            if (NoStrings) return;
            if (color.A <= 0) return;
            if (string.IsNullOrEmpty(s)) return;

            if (Control != null)
                Control.uwfBatches += 1;
            FillRate += width * height;

            ApiGraphics.DrawString(s, font, color, x, y, width, height, alignment);
        }
        public void uwfDrawString(string s, Font font, Color color, float x, float y)
        {
            uwfDrawString(s, font, color, x, y, 512, 64);
        }
        public void uwfDrawString(string s, Font font, Brush brush, float x, float y, float width, float height)
        {
            uwfDrawString(s, font, brush, x, y, width, height, new StringFormat());
        }
        public void uwfDrawString(string s, Font font, Color color, float x, float y, float width, float height)
        {
            uwfDrawString(s, font, color, x, y, width, height, ContentAlignment.BottomLeft);
        }
        public void uwfDrawString(string s, Font font, Color color, float x, float y, float width, float height, HorizontalAlignment alignment)
        {
            ContentAlignment ca = ContentAlignment.MiddleLeft;
            if (alignment == HorizontalAlignment.Center)
                ca = ContentAlignment.MiddleCenter;
            else if (alignment == HorizontalAlignment.Right)
                ca = ContentAlignment.MiddleRight;
            uwfDrawString(s, font, color, x, y, width, height, ca);
        }
        public void uwfDrawString(string s, Font font, Brush brush, float x, float y, float width, float height, HorizontalAlignment horizontalAlignment)
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
        public void uwfDrawString(string s, Font font, Brush brush, float x, float y, float width, float height, StringFormat format)
        {
            uwfDrawString(s, font, brush, x, y, width, height, format.ToContentAlignment());
        }
        public string uwfDrawTextArea(string s, Font font, SolidBrush brush, float x, float y, float width, float height)
        {
            return uwfDrawTextArea(s, font, brush.Color, x, y, width, height);
        }
        public string uwfDrawTextArea(string s, Font font, Color color, float x, float y, float width, float height)
        {
            if (Control == null) return s;
            if (s == null) s = "";

            Control.uwfBatches++;

            GUI.skin.textArea.alignment = TextAnchor.UpperLeft;

            GUI.color = color.ToUnityColor();
            //GUI.skin.textArea.hover.textColor = brush.Color.ToUColor();

            GUI_SetFont(GUI.skin.textArea, font);

            GUI.skin.textArea.hover.background = null;
            GUI.skin.textArea.active.background = null;
            GUI.skin.textArea.focused.background = null;
            GUI.skin.textArea.normal.background = null;

            if (Control.shouldFocus)
                uwfFocusNext();

            var val = GUI.TextArea(new Rect(x, y, width, height), s);

            if (Control.shouldFocus)
            {
                uwfFocus();
                Control.shouldFocus = false;
            }

            return val;
        }
        public string uwfDrawTextField(string s, Font font, SolidBrush brush, RectangleF layoutRectangle, HorizontalAlignment alignment)
        {
            return uwfDrawTextField(s, font, brush, layoutRectangle.X, layoutRectangle.Y, layoutRectangle.Width, layoutRectangle.Height, alignment);
        }
        public string uwfDrawTextField(string s, Font font, Color color, float x, float y, float width, float height, HorizontalAlignment alignment)
        {
            if (Control == null) return s;
            if (s == null) s = "";

            Control.uwfBatches++;

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

            if (Control.shouldFocus)
                uwfFocusNext();

            var val = GUI.TextField(new Rect(x, y, width, height), s);

            if (Control.shouldFocus)
            {
                uwfFocus();
                Control.shouldFocus = false;
            }

            return val;
        }
        public string uwfDrawTextField(string s, Font font, SolidBrush brush, float x, float y, float width, float height, HorizontalAlignment alignment)
        {
            return uwfDrawTextField(s, font, brush.Color, x, y, width, height, alignment);
        }
        public void uwfDrawTexture(Texture texture, RectangleF layoutRectangle)
        {
            uwfDrawTexture(texture, layoutRectangle.X, layoutRectangle.Y, layoutRectangle.Width, layoutRectangle.Height);
        }
        public void uwfDrawTexture(Texture texture, float x, float y, float width, float height)
        {
            uwfDrawTexture(texture, x, y, width, height, Color.White);
        }
        public void uwfDrawTexture(Texture texture, float x, float y, float width, float height, Color color)
        {
            uwfDrawTexture(texture, x, y, width, height, color, 0);
        }
        public void uwfDrawTexture(Texture texture, float x, float y, float width, float height, Color color, float angle)
        {
            uwfDrawTexture(texture, x, y, width, height, color, angle, new PointF(width / 2, height / 2));
        }
        public void uwfDrawTexture(Texture texture, float x, float y, float width, float height, Color color, float angle, PointF pivot)
        {
            if (texture == null) return;

            if (Control != null)
                Control.uwfBatches++;

            FillRate += width * height;

            GUI.color = color.ToUnityColor();

            if (angle != 0)
            {
                Matrix4x4 matrixBackup = GUI.matrix;
                GUIUtility.RotateAroundPivot(angle, new Vector2(x + pivot.X, y + pivot.Y));
                GUI.DrawTexture(new Rect(x, y, width, height), texture);
                GUI.matrix = matrixBackup;
            }
            else
                GUI.DrawTexture(new Rect(x, y, width, height), texture);
        }
        public void uwfDrawTexture(Texture texture, float x, float y, float width, float height, Material mat)
        {
            uwfDrawTexture(texture, x, y, width, height, Color.White, mat);
        }
        public void uwfDrawTexture(Texture texture, float x, float y, float width, float height, Color color, Material mat)
        {
            if (Control == null) return;

            Control.uwfBatches++;
            FillRate += width * height;

            mat.color = color.ToUnityColor();
            UnityEngine.Graphics.DrawTexture(new Rect(x, y, width, height), texture, mat);
        }
        public void uwfFillPolygonConvex(SolidBrush brush, PointF[] points)
        {
            if (points.Length < 3) return;

            if (DefaultMaterial != null)
            {
                DefaultMaterial.SetPass(0);
                DefaultMaterial.color = brush.Color.ToUnityColor();
            }

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
        public void uwfFillRectangle(Brush brush, float x, float y, float width, float height)
        {
            var solidBrush = brush as SolidBrush;
            if (solidBrush == null) return;

            uwfFillRectangle(solidBrush.Color, x, y, width, height);
        }
        public void uwfFillRectangle(Color color, Rectangle rect)
        {
            uwfFillRectangle(color, rect.X, rect.Y, rect.Width, rect.Height);
        }
        public void uwfFillRectangle(Color color, float x, float y, float width, float height)
        {
            uwfFillRectangle(color, x, y, width, height, null);
        }
        public void uwfFillRectangle(Color color, float x, float y, float width, float height, Material mat)
        {
            if (NoFill) return;
            if (color.A <= 0) return;

            if (Control != null)
                Control.uwfBatches += 1;
            FillRate += width * height;

            ApiGraphics.FillRectangle(color, x, y, width, height, mat);
        }
        public void uwfFillQuad(Color color, PointF[] points)
        {
            if (points == null || points.Length != 4) return;

            uwfFillQuad(color, points[0], points[1], points[2], points[3]);
        }
        public void uwfFillQuad(Color color, PointF p1, PointF p2, PointF p3, PointF p4)
        {
            GL.Begin(GL.QUADS);

            GL.Color(color.ToUnityColor());

            GL.Vertex3(p1.X, p1.Y, 0);
            GL.Vertex3(p2.X, p2.Y, 0);
            GL.Vertex3(p3.X, p3.Y, 0);
            GL.Vertex3(p4.X, p4.Y, 0);

            GL.End();
        }
        public void uwfFocus()
        {
            if (Control != null)
                UnityEngine.GUI.FocusControl(Control.GetHashCode().ToString("X2"));
        }
        public void uwfFocusNext()
        {
            if (Control != null)
                UnityEngine.GUI.SetNextControlName(Control.GetHashCode().ToString("X2"));
        }
        #endregion

        public static SizeF uwfMeasureStringStatic(string text, Font font)
        {
            int guiSkinFontSizeBuffer = GUI_SetFont(GUI.skin.label, font);

            var size = GUI.skin.label.CalcSize(new GUIContent(text));

            GUI.skin.label.fontSize = guiSkinFontSizeBuffer;

            return new SizeF(size.x, size.y);
        }
        public static SizeF uwfMeasureStringSimple(string text, Font font)
        {
            return new SizeF() { Width = text.Length * 8, Height = font.Size }; // fast but not accurate.
        }
    }
}
