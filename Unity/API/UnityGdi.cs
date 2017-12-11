namespace Unity.API
{
    using System;
    using System.Drawing;
    using System.Drawing.API;
    using System.Drawing.Drawing2D;

    using UE = UnityEngine;

    public class UnityGdi : IApiGraphics
    {
        public static UE.Texture2D defaultTexture;
        public static bool GL_LINES;

        private readonly PointF defaultPivot = PointF.Empty;

        public UnityGdi(UE.Texture2D defTexture)
        {
            defaultTexture = defTexture;
        }

        public void BeginGroup(float x, float y, float width, float height)
        {
            UE.GUI.BeginGroup(new UE.Rect(x, y, width, height));
        }
        public void Clear(Color color)
        {
            UE.GUI.color = color.ToUnityColor();
        }
        public ITexture CreateTexture(int width, int height)
        {
            return new UnityGdiTexture(width, height);
        }
        public ITexture CreateTexture(object original)
        {
            var texture = original as UE.Texture2D;
            if (texture != null)
                return new UnityGdiTexture(texture);

            var sprite = original as UE.Sprite;
            if (sprite != null)
                return new UnityGdiSprite(sprite);

            throw new NotSupportedException();
        }
        public void DrawImage(Image image, Color color, float x, float y, float width, float height, float angle)
        {
            if (image == null || width <= 0 || height <= 0)
                return;

            var unityGdiSprite = image.uTexture as UnityGdiSprite;
            if (unityGdiSprite != null && unityGdiSprite.sprite != null)
            {
                var spriteRect = unityGdiSprite.sprite.rect;
                var texture = unityGdiSprite.sprite.texture;
                var sx = spriteRect.x / texture.width;
                var sy = spriteRect.y / texture.height;
                var sw = spriteRect.width / texture.width;
                var sh = spriteRect.height / texture.height;
                UE.GUI.color = color.ToUnityColor();
                UE.GUI.DrawTextureWithTexCoords(new UE.Rect(x, y, width, height), unityGdiSprite.sprite.texture, new UE.Rect(sx, sy, sw, sh));
                return;
            }

            var textureToDraw = defaultTexture;
            var imageTexture = image.uTexture as UnityGdiTexture;
            if (imageTexture != null)
                textureToDraw = imageTexture.texture;

            if (angle != 0)
            {
                UE.Matrix4x4 matrixBackup = UE.GUI.matrix;
                UE.GUIUtility.RotateAroundPivot(angle, new UE.Vector2(x + width / 2, y + height / 2));
                UE.GUI.DrawTexture(new UE.Rect(x, y, width, height), ((UnityGdiTexture)image.uTexture).texture);
                UE.GUI.matrix = matrixBackup;
                return;
            }

            UE.GUI.color = color.ToUnityColor();
            UE.GUI.DrawTexture(new UE.Rect(x, y, width, height), textureToDraw);
        }
        public void DrawImage(Image image, float x, float y, float width, float height, object material = null)
        {
            var textureToDraw = defaultTexture;
            if (image != null)
            {
                var imageTexture = image.uTexture as UnityGdiTexture;
                if (imageTexture != null)
                    textureToDraw = imageTexture.texture;
            }

            // Draw default texture with material if possible.
            if (material != null)
            {
                var mat = material as UE.Material;
                if (mat != null)
                {
                    UnityEngine.Graphics.DrawTexture(new UE.Rect(x, y, width, height), textureToDraw, mat);
                    return;
                }
            }

            UE.GUI.color = UE.Color.white;
            UE.GUI.DrawTexture(new UE.Rect(x, y, width, height), textureToDraw);
        }
        public void DrawLine(Pen pen, float x1, float y1, float x2, float y2, object material = null)
        {
            var penColor = pen.Color;
            var penWidth = pen.Width;

            if (penColor.A <= 0 || penWidth <= 0) return;

            float x = 0;
            float y = 0;
            float width = 0;
            float height = 0;

            if (x1 != x2 && y1 != y2)
            {
                if (GL_LINES)
                {
                    UE.GL.Begin(UE.GL.LINES);
                    UE.GL.Color(pen.Color.ToUnityColor());
                    UE.GL.Vertex3(x1, y1, 0);
                    UE.GL.Vertex3(x2, y2, 0);
                    UE.GL.End();
                    return;
                }

                float xDiff = x2 - x1;
                float yDiff = y2 - y1;

                var angle = Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;
                var pivot = defaultPivot;
                if (penWidth > 2)
                    pivot = new PointF(0, penWidth / 2f);

                uwfDrawTexture(defaultTexture, x1, y1, (float)Math.Sqrt(xDiff * xDiff + yDiff * yDiff), penWidth, penColor, (float)angle, pivot);
                return;
            }

            if (x1 == x2)
            {
                if (y1 > y2) y1 += penWidth;
                else y2 += penWidth;
                x = x1;
                width = penWidth;
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
                height = penWidth;
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

            UE.GUI.color = penColor.ToUnityColor();

            var penDash = pen.DashStyle;
            switch (penDash)
            {
                case DashStyle.Solid:
                    UE.GUI.DrawTexture(new UE.Rect(x, y, width, height), defaultTexture);
                    break;
                case DashStyle.Dash:
                    float dash_step = penWidth * 6;
                    if (y1 == y2)
                        for (float i = 0; i < width; i += dash_step)
                        {
                            float dash_width = dash_step - 2;
                            if (i + dash_width > width)
                                dash_width = width - i;

                            UE.GUI.DrawTexture(new UE.Rect(x + i, y, dash_width, penWidth), defaultTexture);
                        }

                    if (x1 == x2)
                        for (float i = 0; i < height; i += dash_step)
                        {
                            float dash_height = dash_step - 2;
                            if (i + dash_height > height)
                                dash_height = height - i;

                            UE.GUI.DrawTexture(new UE.Rect(x + width - penWidth, y + i, penWidth, dash_height), defaultTexture);
                        }
                    break;
            }
        }
        public void DrawPolygon(Pen pen, Point[] points, object material = null)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (i + 1 >= points.Length) break;

                UE.GL.Begin(UE.GL.LINES);
                UE.GL.Color(pen.Color.ToUnityColor());

                UE.GL.Vertex3(points[i].X, points[i].Y, 0);
                UE.GL.Vertex3(points[i + 1].X, points[i + 1].Y, 0);

                UE.GL.End();
            }
        }
        public void DrawRectangle(Pen pen, float x, float y, float width, float height, object material = null)
        {
            var penColor = pen.Color;
            var penWidth = pen.Width;

            if (penColor.A <= 0 || penWidth <= 0) return;
            UE.GUI.color = penColor.ToUnityColor();

            var penDash = pen.DashStyle;
            switch (penDash)
            {
                case System.Drawing.Drawing2D.DashStyle.Solid:
                    UE.GUI.DrawTexture(new UE.Rect(x, y, width, penWidth), defaultTexture);
                    UE.GUI.DrawTexture(new UE.Rect(x + width - penWidth, y + penWidth, penWidth, height - penWidth * 2), defaultTexture);
                    if (height > 1)
                        UE.GUI.DrawTexture(new UE.Rect(x, y + height - penWidth, width, penWidth), defaultTexture);

                    if (width > 1)
                        UE.GUI.DrawTexture(new UE.Rect(x, y + penWidth, penWidth, height - penWidth * 2), defaultTexture);

                    break;
                case System.Drawing.Drawing2D.DashStyle.Dash:
                    float dash_step = penWidth * 6;
                    for (float i = 0; i < width; i += dash_step)
                    {
                        float dash_width = dash_step - 2;
                        if (i + dash_width > width)
                            dash_width = width - i;

                        UE.GUI.DrawTexture(new UE.Rect(x + i, y, dash_width, penWidth), defaultTexture); // Top.
                        UE.GUI.DrawTexture(new UE.Rect(x + i, y + height - penWidth, dash_width, penWidth), defaultTexture); // Bottom.
                    }
                    for (float i = 0; i < height; i += dash_step)
                    {
                        float dash_height = dash_step - 2;
                        if (i + dash_height > height)
                            dash_height = height - i;
                        UE.GUI.DrawTexture(new UE.Rect(x + width - penWidth, y + i, penWidth, dash_height), defaultTexture); // Right.
                        UE.GUI.DrawTexture(new UE.Rect(x, y + i, penWidth, dash_height), defaultTexture); // Left.
                    }
                    break;
            }
        }
        public void DrawString(string text, Font font, Color color, float x, float y, float width, float height, ContentAlignment align, object material = null)
        {
            if (text == null || font == null) return;

            // TODO: material not supported.

            // Set align.
            var uAlign = UE.TextAnchor.UpperLeft;
            switch (align)
            {
                case ContentAlignment.BottomCenter:
                    uAlign = UE.TextAnchor.LowerCenter;
                    break;
                case ContentAlignment.BottomLeft:
                    uAlign = UE.TextAnchor.LowerLeft;
                    break;
                case ContentAlignment.BottomRight:
                    uAlign = UE.TextAnchor.LowerRight;
                    break;
                case ContentAlignment.MiddleCenter:
                    uAlign = UE.TextAnchor.MiddleCenter;
                    break;
                case ContentAlignment.MiddleLeft:
                    uAlign = UE.TextAnchor.MiddleLeft;
                    break;
                case ContentAlignment.MiddleRight:
                    uAlign = UE.TextAnchor.MiddleRight;
                    break;
                case ContentAlignment.TopCenter:
                    uAlign = UE.TextAnchor.UpperCenter;
                    break;
                case ContentAlignment.TopLeft:
                    uAlign = UE.TextAnchor.UpperLeft;
                    break;
                case ContentAlignment.TopRight:
                    uAlign = UE.TextAnchor.UpperRight;
                    break;
            }

            var labelSkin = UE.GUI.skin.label;
            int guiSkinFontSizeBuffer = GUI_SetFont(labelSkin, font);

            UE.GUI.color = color.ToUnityColor();
            labelSkin.alignment = uAlign;
            UE.GUI.Label(new UE.Rect(x, y, width, height), text);

            labelSkin.fontSize = guiSkinFontSizeBuffer;
        }
        public void EndGroup()
        {
            UE.GUI.EndGroup();
        }
        public void FillRectangle(Brush brush, float x, float y, float width, float height, object material = null)
        {
            var solidBrush = brush as SolidBrush;
            if (solidBrush != null)
                FillRectangle(solidBrush.Color, x, y, width, height, material);
        }
        public void FillRectangle(Color color, float x, float y, float width, float height, object material = null)
        {
            var rect = new UE.Rect(x, y, width, height);

            if (material == null || material is UE.Material == false)
            {
                UE.GUI.color = color.ToUnityColor();
                UE.GUI.DrawTexture(rect, defaultTexture);
                return;
            }

            var umat = (UE.Material)material;
            umat.color = color.ToUnityColor();
            UE.Graphics.DrawTexture(rect, defaultTexture, umat);
        }
        public void Focus()
        {
            UE.GUI.FocusControl("nextControl");
        }
        public void FocusNext()
        {
            UE.GUI.SetNextControlName("nextControl");
        }
        public SizeF MeasureString(string text, Font font)
        {
            var labelSkin = UE.GUI.skin.label;
            int guiSkinFontSizeBuffer = GUI_SetFont(labelSkin, font);

            var size = labelSkin.CalcSize(new UE.GUIContent(text));

            labelSkin.fontSize = guiSkinFontSizeBuffer;

            return new SizeF(size.x, size.y);
        }

        private static int GUI_SetFont(UE.GUIStyle style, Font font)
        {
            int guiSkinFontSizeBuffer = UE.GUI.skin.label.fontSize;
            if (font != null)
            {
                if (font.UFont == null && Unity.API.UnityWinForms.gResources != null)
                {
                    var fonts = Unity.API.UnityWinForms.gResources.Fonts;
                    if (fonts != null)
                        for (int i = 0; i < fonts.Count; i++)
                        {
                            var fontItem = fonts[i];
                            if (fontItem.fontNames[0] != font.Name) continue;

                            font.UFont = fontItem;
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

                var fontStyle = font.Style;
                style.fontSize = (int)(font.Size);
                bool styleBold = (fontStyle & FontStyle.Bold) == FontStyle.Bold;
                bool styleItalic = (fontStyle & FontStyle.Italic) == FontStyle.Italic;
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
                if (UnityWinForms.gResources.Fonts.Count > 0)
                {
                    var _font = UnityWinForms.gResources.Fonts[0];
                    if (_font != null)
                        style.font = _font;
                    style.fontSize = (int)(12);
                    style.fontStyle = UnityEngine.FontStyle.Normal;
                }
            }
            return guiSkinFontSizeBuffer;
        }
        private void uwfDrawTexture(UE.Texture texture, float x, float y, float width, float height, Color color, float angle, PointF pivot)
        {
            if (texture == null) return;

            UE.GUI.color = color.ToUnityColor();

            if (angle != 0)
            {
                UE.Matrix4x4 matrixBackup = UE.GUI.matrix;
                UE.GUIUtility.RotateAroundPivot(angle, new UE.Vector2(x + pivot.X, y + pivot.Y));
                UE.GUI.DrawTexture(new UE.Rect(x, y, width, height), texture);
                UE.GUI.matrix = matrixBackup;
            }
            else
                UE.GUI.DrawTexture(new UE.Rect(x, y, width, height), texture);
        }
    }
}
