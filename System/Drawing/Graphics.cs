using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace System.Drawing
{
    public sealed class Graphics
    {
        private bool _group { get { return _groupControls.Count > 0; } }
        private Control _groupControlLast { get { return _groupControls[_groupControls.Count - 1]; } }
        private List<Control> _groupControls = new List<Control>();

        public static bool GL_Lines { get; set; }
        public static bool NoFill { get; set; }
        public static bool NoRects { get; set; }
        public static bool NoStrings { get; set; }

        internal Control Control { get; set; }
        internal Rectangle Group { get; set; }
        internal void GroupBegin(Control groupControl)
        {
            var c_position = Control.Location + Control.Offset;//Control.PointToScreen(Point.Zero);
            GUI.BeginGroup(new Rect((c_position.X + Group.X), (c_position.Y + Group.Y), Group.Width, Group.Height));
            _groupControls.Add(groupControl);
        }
        internal void GroupEnd()
        {
            GUI.EndGroup();
            _groupControls.RemoveAt(_groupControls.Count - 1);
        }

        public void Clear(System.Drawing.Color color)
        {
        }
        public void DrawLine(Pen pen, int x1, int y1, int x2, int y2)
        {
            DrawLine(pen, (float)x1, (float)y1, (float)x2, (float)y2);
        }
        public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
        {
            float x = 0;
            float y = 0;
            float width = 0;
            float height = 0;

            if (x1 == x2 && y1 == y2)
                return;

            if (x1 == x2)
            {
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

            if (!GL_Lines)
                DrawRectangle(pen, x, y, width, height);
            else
            {
                GL.Begin(GL.LINES);
                GL.Color(pen.Color.ToUColor());

                // TODO: switch (pen.DashStyle) { ... }
                
                if (!_group)
                {
                    Point c_position = Point.Empty;
                    if (Control != null)
                        c_position = Control.PointToScreen(Point.Zero);

                    GL.Vertex3(c_position.X + x1, c_position.Y + y1, 0);
                    GL.Vertex3(c_position.X + x2, c_position.Y + y2, 0);
                }
                else
                {
                    Point c_position = Point.Empty;
                    if (Control != null)
                        c_position = Control.PointToScreen(Point.Zero);
                    var g_position = _groupControlLast.PointToScreen(Point.Zero);

                    GL.Vertex3(c_position.X - g_position.X + x1, c_position.Y - g_position.Y + y1, 0);
                    GL.Vertex3(c_position.X - g_position.X + x2, c_position.Y - g_position.Y + y2, 0);
                }

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
            if (pen.Color == Color.Transparent) return;
            GUI.color = pen.Color.ToUColor();

            /*if (Control.Batched)
            {
                GUI.color = UnityEngine.Color.white;
                GUI.DrawTexture(new Rect(x, y, width, height), Control.BatchedTexture);
                return;
            }*/

            if (!_group)
            {
                Point c_position = Point.Empty;
                if (Control != null)
                    c_position = Control.PointToScreen(Point.Zero);
                // Top.
                GUI.DrawTexture(new Rect(c_position.X + x, c_position.Y + y, width, pen.Width), System.Windows.Forms.Application.DefaultSprite);
                // Right.
                GUI.DrawTexture(new Rect(c_position.X + x + width - pen.Width, c_position.Y + y + pen.Width, pen.Width, height - pen.Width * 2), System.Windows.Forms.Application.DefaultSprite);
                // Bottom.
                if (height > 1)
                    GUI.DrawTexture(new Rect(c_position.X + x, c_position.Y + y + height - pen.Width, width, pen.Width), System.Windows.Forms.Application.DefaultSprite);
                // Left.
                if (width > 1)
                    GUI.DrawTexture(new Rect(c_position.X + x, c_position.Y + y + pen.Width, pen.Width, height - pen.Width * 2), System.Windows.Forms.Application.DefaultSprite);
            }
            else
            {
                Point c_position = Point.Empty;
                if (Control != null)
                    c_position = Control.PointToScreen(Point.Zero);
                var g_position = _groupControlLast.PointToScreen(Point.Zero);
                var position = c_position - g_position + new PointF(x, y);

                switch (pen.DashStyle)
                {
                    case Drawing2D.DashStyle.Solid:
                        /*if (width == 0 || height == 0) return;
                        if (width < 0 || height < 0)
                        {
                            if (width < 0)
                            {
                                x += width;
                                width *= -1;
                            }
                            if (height < 0)
                            {
                                y += height;
                                height *= -1;
                            }
                        }


                        //UnityEngine.Debug.Log(width.ToString() + " " + height.ToString());
                        //return;

                        // Batching
                        UnityEngine.Texture2D tex = new Texture2D((int)width, (int)height);
                        for (int i = 0; i < tex.height; i++)
                            for (int k = 0; k < tex.width; k++)
                                tex.SetPixel(k, i, new UnityEngine.Color(1, 1, 1, 0));

                        var ucolor = pen.Color.ToUColor();
                        for (int i = 0; i < tex.width; i++)
                        {
                            for (int k = 0; k < pen.Width && k < tex.height; k++)
                                tex.SetPixel(i, k, ucolor);
                            for (int k = tex.height - 1; k > 0 && k > tex.height - 1 - pen.Width; k--)
                                tex.SetPixel(i, k, ucolor);
                        }

                        for (int i = 0; i < tex.height; i++)
                        {
                            for (int k = 0; k < pen.Width && k < tex.width; k++)
                                tex.SetPixel(k, i, ucolor);
                            for (int k = tex.width - 1; k > 0 && k > tex.width - 1 - pen.Width; k--)
                                tex.SetPixel(k, i, ucolor);
                        }

                        tex.Apply();
                        Control.BatchedTexture = tex;*/

                        GUI.DrawTexture(new Rect(position.X, position.Y, width, pen.Width), System.Windows.Forms.Application.DefaultSprite);
                        GUI.DrawTexture(new Rect(position.X + width - pen.Width, position.Y + pen.Width, pen.Width, height - pen.Width * 2), System.Windows.Forms.Application.DefaultSprite);
                        if (height > 1)
                            GUI.DrawTexture(new Rect(position.X, position.Y + height - pen.Width, width, pen.Width), System.Windows.Forms.Application.DefaultSprite);
                        if (width > 1)
                            GUI.DrawTexture(new Rect(position.X, position.Y + pen.Width, pen.Width, height - pen.Width * 2), System.Windows.Forms.Application.DefaultSprite);

                        break;
                    case Drawing2D.DashStyle.Dash:
                        float dash_step = pen.Width * 6;
                        for (float i = 0; i < width; i += dash_step)
                        {
                            float dash_width = dash_step - 2;
                            if (i + dash_width > width)
                                dash_width = width - i;
                            GUI.DrawTexture(new Rect(position.X + i, position.Y, dash_width, pen.Width), System.Windows.Forms.Application.DefaultSprite); // Top.
                            GUI.DrawTexture(new Rect(position.X + i, position.Y + height - pen.Width, dash_width, pen.Width), System.Windows.Forms.Application.DefaultSprite); // Bottom.
                        }
                        for (float i = 0; i < height; i += dash_step)
                        {
                            float dash_height = dash_step - 2;
                            if (i + dash_height > height)
                                dash_height = height - i;
                            GUI.DrawTexture(new Rect(position.X + width - pen.Width, position.Y + i, pen.Width, dash_height), System.Windows.Forms.Application.DefaultSprite); // Right.
                            GUI.DrawTexture(new Rect(position.X, position.Y + i, pen.Width, dash_height), System.Windows.Forms.Application.DefaultSprite); // Left.
                        }
                        break;
                }
            }
        }
        public string DrawPasswordField(string s, Font font, SolidBrush brush, float x, float y, float width, float height, HorizontalAlignment alignment)
        {
            if (Control == null) return s;

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
                var _font = System.Windows.Forms.Application.Resources.Fonts.Find(f => f.fontNames[0] == font.Name);
                if (_font != null)
                    GUI.skin.textField.font = _font;
                else
                    GUI.skin.textField.font = null;
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
                var _font = System.Windows.Forms.Application.Resources.Fonts.Find(f => f.fontNames[0] == "Arial");
                if (_font != null)
                    GUI.skin.textField.font = _font;
                GUI.skin.textField.fontSize = 12;
            }

            GUI.color = brush.Color.ToUColor();

            if (!_group)
            {
                var c_position = Control.PointToScreen(Point.Zero);
                return GUI.PasswordField(new Rect(c_position.X + x, c_position.Y + y, width, height), s, '*');
            }
            else
            {
                var c_position = Control.PointToScreen(Point.Zero);
                var g_position = _groupControlLast.PointToScreen(Point.Zero);
                var position = c_position - g_position + new PointF(x, y);

                return GUI.PasswordField(new Rect(position.X, position.Y, width, height), s, '*');
            }
        }
        public void DrawString(string s, Font font, SolidBrush brush, PointF point)
        {
            DrawString(s, font, brush, point.X, point.Y);
        }
        public void DrawString(string s, Font font, SolidBrush brush, float x, float y)
        {
            DrawString(s, font, brush, new RectangleF(x, y, 256, 64));
        }
        public void DrawString(string s, Font font, SolidBrush brush, float x, float y, StringFormat format)
        {
            DrawString(s, font, brush, new RectangleF(x, y, 256, 64), format);
        }
        public void DrawString(string s, Font font, SolidBrush brush, float x, float y, float width, float height)
        {
            DrawString(s, font, brush, x, y, width, height, new StringFormat());
        }
        public void DrawString(string s, Font font, SolidBrush brush, float x, float y, float width, float height, ContentAlignment alignment)
        {
            if (NoStrings) return;
            GUI.skin.label.alignment = TextAnchor.UpperLeft;
            switch (alignment)
            {
                case ContentAlignment.BottomCenter:
                    GUI.skin.label.alignment = TextAnchor.LowerCenter;
                    break;
                case ContentAlignment.BottomLeft:
                    GUI.skin.label.alignment = TextAnchor.LowerLeft;
                    break;
                case ContentAlignment.BottomRight:
                    GUI.skin.label.alignment = TextAnchor.LowerRight;
                    break;
                case ContentAlignment.MiddleCenter:
                    GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                    break;
                case ContentAlignment.MiddleLeft:
                    GUI.skin.label.alignment = TextAnchor.MiddleLeft;
                    break;
                case ContentAlignment.MiddleRight:
                    GUI.skin.label.alignment = TextAnchor.MiddleRight;
                    break;
                case ContentAlignment.TopCenter:
                    GUI.skin.label.alignment = TextAnchor.UpperCenter;
                    break;
                case ContentAlignment.TopLeft:
                    GUI.skin.label.alignment = TextAnchor.UpperLeft;
                    break;
                case ContentAlignment.TopRight:
                    GUI.skin.label.alignment = TextAnchor.UpperRight;
                    break;
            }

            int guiSkinFontSizeBuffer = GUI.skin.label.fontSize;
            if (font != null)
            {
                var _font = System.Windows.Forms.Application.Resources.Fonts.Find(f => f.fontNames[0] == font.Name);
                if (_font != null)
                    GUI.skin.label.font = _font;
                else
                {
                    GUI.skin.label.font = null;
                    UnityEngine.Debug.LogWarning(font.Name);
                }
                GUI.skin.label.fontSize = (int)(font.Size);
                bool styleBold = (font.Style & FontStyle.Bold) == FontStyle.Bold;
                bool styleItalic = (font.Style & FontStyle.Italic) == FontStyle.Italic;
                if (styleBold)
                {
                    if (styleItalic)
                        GUI.skin.label.fontStyle = UnityEngine.FontStyle.BoldAndItalic;
                    else
                        GUI.skin.label.fontStyle = UnityEngine.FontStyle.Bold;
                }
                else if (styleItalic)
                    GUI.skin.label.fontStyle = UnityEngine.FontStyle.Italic;
                else GUI.skin.label.fontStyle = UnityEngine.FontStyle.Normal;
            }
            else
            {
                var _font = System.Windows.Forms.Application.Resources.Fonts.Find(f => f.fontNames[0] == "Arial");
                if (_font != null)
                    GUI.skin.label.font = _font;
                GUI.skin.label.fontSize = (int)(12);
                GUI.skin.label.fontStyle = UnityEngine.FontStyle.Normal;
            }
            GUI.color = brush.Color.ToUColor();

            if (!_group)
            {
                Point c_position = Point.Empty;
                if (Control != null)
                    c_position = Control.PointToScreen(Point.Zero);
                GUI.Label(new Rect(c_position.X + x, c_position.Y + y, width, height), s);
            }
            else
            {
                //Point c_position = Point.Empty;
                //if (Control != null)
                //  c_position = Control.PointToScreen(Point.Zero);
                //var g_position = _groupControlLast.PointToScreen(Point.Zero);
                //var position = c_position - g_position + new PointF(x, y);

                GUI.Label(new Rect(x, y, width, height), s);
            }

            GUI.skin.label.fontSize = guiSkinFontSizeBuffer;
        }
        public void DrawString(string s, Font font, SolidBrush brush, float x, float y, float width, float height, HorizontalAlignment horizontalAlignment)
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
            DrawString(s, font, brush, x, y, width, height, alignment);
        }
        public void DrawString(string s, Font font, SolidBrush brush, float x, float y, float width, float height, StringFormat format)
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
            DrawString(s, font, brush, x, y, width, height, alignment);
        }
        public void DrawString(string s, Font font, SolidBrush brush, RectangleF layoutRectangle)
        {
            DrawString(s, font, brush, layoutRectangle, new StringFormat());
        }
        public void DrawString(string s, Font font, SolidBrush brush, RectangleF layoutRectangle, StringFormat format)
        {
            DrawString(s, font, brush, layoutRectangle.X, layoutRectangle.Y, layoutRectangle.Width, layoutRectangle.Height, format);
        }
        public string DrawTextArea(string s, Font font, SolidBrush brush, float x, float y, float width, float height)
        {
            if (Control == null) return s;

            GUI.skin.textArea.alignment = TextAnchor.UpperLeft;

            GUI.color = brush.Color.ToUColor();
            //GUI.skin.textArea.hover.textColor = brush.Color.ToUColor();
            if (font != null)
            {
                var _font = System.Windows.Forms.Application.Resources.Fonts.Find(f => f.fontNames[0] == font.Name);
                if (_font != null)
                    GUI.skin.textArea.font = _font;
                else
                    GUI.skin.textArea.font = null;
                GUI.skin.textArea.fontSize = (int)font.Size;
                bool styleBold = (font.Style & FontStyle.Bold) == FontStyle.Bold;
                bool styleItalic = (font.Style & FontStyle.Italic) == FontStyle.Italic;
                if (styleBold)
                {
                    if (styleItalic)
                        GUI.skin.textArea.fontStyle = UnityEngine.FontStyle.BoldAndItalic;
                    else
                        GUI.skin.textArea.fontStyle = UnityEngine.FontStyle.Bold;
                }
                else if (styleItalic)
                    GUI.skin.textArea.fontStyle = UnityEngine.FontStyle.Italic;
                else GUI.skin.textArea.fontStyle = UnityEngine.FontStyle.Normal;
            }
            else
            {
                var _font = System.Windows.Forms.Application.Resources.Fonts.Find(f => f.fontNames[0] == "Arial");
                if (_font != null)
                    GUI.skin.textArea.font = _font;
                GUI.skin.textArea.fontSize = 12;
            }

            if (!_group)
            {
                var c_position = Control.PointToScreen(Point.Zero);
                return GUI.TextArea(new Rect(c_position.X + x, c_position.Y + y, width, height), s);
            }
            else
            {
                var c_position = Control.PointToScreen(Point.Zero);
                var g_position = _groupControlLast.PointToScreen(Point.Zero);
                var position = c_position - g_position + new PointF(x, y);

                return GUI.TextArea(new Rect(position.X, position.Y, width, height), s);
            }
        }
        public string DrawTextField(string s, Font font, SolidBrush brush, RectangleF layoutRectangle, HorizontalAlignment alignment)
        {
            return DrawTextField(s, font, brush, layoutRectangle.X, layoutRectangle.Y, layoutRectangle.Width, layoutRectangle.Height, alignment);
        }
        public string DrawTextField(string s, Font font, SolidBrush brush, float x, float y, float width, float height, HorizontalAlignment alignment)
        {
            if (Control == null) return s;

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
                var _font = System.Windows.Forms.Application.Resources.Fonts.Find(f => f.fontNames[0] == font.Name);
                if (_font != null)
                    GUI.skin.textField.font = _font;
                else
                    GUI.skin.textField.font = null;
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
                var _font = System.Windows.Forms.Application.Resources.Fonts.Find(f => f.fontNames[0] == "Arial");
                if (_font != null)
                    GUI.skin.textField.font = _font;
                GUI.skin.textField.fontSize = 12;
            }

            GUI.color = brush.Color.ToUColor();

            if (!_group)
            {
                var c_position = Control.PointToScreen(Point.Zero);
                return GUI.TextField(new Rect(c_position.X + x, c_position.Y + y, width, height), s);
            }
            else
            {
                var c_position = Control.PointToScreen(Point.Zero);
                var g_position = _groupControlLast.PointToScreen(Point.Zero);
                var position = c_position - g_position + new PointF(x, y);

                return GUI.TextField(new Rect(position.X, position.Y, width, height), s);
            }
        }
        public void DrawTexture(Texture texture, RectangleF layoutRectangle)
        {
            DrawTexture(texture, layoutRectangle.X, layoutRectangle.Y, layoutRectangle.Width, layoutRectangle.Height);
        }
        public void DrawTexture(Texture texture, float x, float y, float width, float height)
        {
            DrawTexture(texture, x, y, width, height, Color.White);
        }
        public void DrawTexture(Texture texture, float x, float y, float width, float height, Color color)
        {
            if (Control == null) return;

            GUI.color = color.ToUColor();
            if (!_group)
            {
                var c_position = Control.PointToScreen(Point.Zero);
                GUI.DrawTexture(new Rect(c_position.X + x, c_position.Y + y, width, height), texture);
            }
            else
            {
                var c_position = Control.PointToScreen(Point.Zero);
                var g_position = _groupControlLast.PointToScreen(Point.Zero);

                GUI.DrawTexture(new Rect(c_position.X - g_position.X + x, c_position.Y - g_position.Y + y, width, height), texture);
            }
        }
        public void FillEllipse(SolidBrush brush, float x, float y, float width, float height)
        {
            GUI.color = brush.Color.ToUColor();

            if (Control != null && !_group)
            {
                var c_position = Control.PointToScreen(Point.Zero);
                GUI.DrawTexture(new Rect(c_position.X + x, c_position.Y + y, width, height), System.Windows.Forms.Application.DefaultSprite);
            }
            else
                GUI.DrawTexture(new Rect(x, y, width, height), System.Windows.Forms.Application.DefaultSprite);
        }
        public void FillRectangle(SolidBrush brush, Rectangle rect)
        {
            FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height);
        }
        public void FillRectangle(SolidBrush brush, int x, int y, int width, int height)
        {
            FillRectangle(brush, (float)x, (float)y, (float)width, (float)height);
        }
        public void FillRectangle(SolidBrush brush, float x, float y, float width, float height)
        {
            if (NoFill) return;
            //if (Control == null) return;
            if (brush.Color == Color.Transparent) return;

            //x += Control.Offset.X;
            //y += Control.Offset.Y;

            GUI.color = brush.Color.ToUColor();
            if (!_group)
            {
                Point c_position = Point.Empty;
                if (Control != null)
                    c_position = Control.PointToScreen(Point.Zero);

                GUI.DrawTexture(new Rect(c_position.X + x, c_position.Y + y, width, height), System.Windows.Forms.Application.DefaultSprite);
            }
            else
            {
                Point c_position = Point.Empty;
                if (Control != null)
                    c_position = Control.PointToScreen(Point.Zero);
                var g_position = _groupControlLast.PointToScreen(Point.Zero);
                GUI.DrawTexture(new Rect(c_position.X - g_position.X + x, c_position.Y - g_position.Y + y, width, height), System.Windows.Forms.Application.DefaultSprite);
            }
        }
        public SizeF MeasureString(string text, Font font)
        {
            return new SizeF() { Width = text.Length * 8, Height = font.Size };
        } // TODO: use GUI.label...
        public SizeF MeasureString(string text, Font font, int width, StringFormat format)
        {
            return new SizeF() { Width = text.Length * 6, Height = font.Size };
        }
    }
}
