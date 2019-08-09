namespace Unity.API
{
    using System;
    using System.Drawing;
    using System.Drawing.API;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    using UE = UnityEngine;

    public class UnityGdi : IApiGraphics
    {
        public static UE.Color cursorSelectionColor = Color.FromArgb(128, SystemColors.Highlight).ToUnityColor();
        public static UE.Texture2D defaultTexture = UnityWinForms.DefaultSprite;

        private static readonly UE.GUIContent textContent = new UE.GUIContent(""); // GUIContent.Temp(text) replacement.
        private static readonly UE.Rect sourceRect = new UE.Rect(0f, 0f, 1f, 1f);
        private static readonly UE.Material blendMaterial = new UE.Material(UE.Shader.Find("Hidden/Internal-GUITextureClip"));
        
        public void BeginGroup(float x, float y, float width, float height)
        {
            UE.GUI.BeginGroup(new UE.Rect(x, y, width, height), UE.GUIContent.none, UE.GUIStyle.none);
        }
        public void Clear(Color color)
        {
            UE.GUI.color = color.ToUnityColor();
            UE.GUI.skin.settings.selectionColor = cursorSelectionColor;
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
        public void DrawImage(Image image, float x, float y, float width, float height, float angle)
        {
            if (image == null || width <= 0 || height <= 0)
                return;

            var unityGdiSprite = image.Texture as UnityGdiSprite;
            if (unityGdiSprite != null && unityGdiSprite.sprite != null)
            {
                var spriteRect = unityGdiSprite.sprite.rect;
                var texture = unityGdiSprite.sprite.texture;
                var sx = spriteRect.x / texture.width;
                var sy = spriteRect.y / texture.height;
                var sw = spriteRect.width / texture.width;
                var sh = spriteRect.height / texture.height;
                UE.GUI.color = image.Color.ToUnityColor();
                UE.GUI.DrawTextureWithTexCoords(new UE.Rect(x, y, width, height), unityGdiSprite.sprite.texture, new UE.Rect(sx, sy, sw, sh));
                return;
            }

            var textureToDraw = defaultTexture;
            var imageTexture = image.Texture as UnityGdiTexture;
            if (imageTexture != null)
                textureToDraw = imageTexture.texture;

            if (angle != 0)
            {
                UE.Matrix4x4 matrixBackup = UE.GUI.matrix;
                UE.GUIUtility.RotateAroundPivot(angle, new UE.Vector2(x + width / 2, y + height / 2));
                UE.GUI.DrawTexture(new UE.Rect(x, y, width, height), ((UnityGdiTexture)image.Texture).texture);
                UE.GUI.matrix = matrixBackup;
                return;
            }

            UE.GUI.color = image.Color.ToUnityColor();
            UE.GUI.DrawTexture(new UE.Rect(x, y, width, height), textureToDraw);
        }
        public void DrawImage(Image image, float x, float y, float width, float height, object material = null)
        {
            var textureToDraw = defaultTexture;
            if (image != null)
            {
                var imageTexture = image.Texture as UnityGdiTexture;
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
            if (UE.Event.current.type != UE.EventType.Repaint)
                return;
            
            var unityColor = penColor.ToUnityColor();
            
            // Diagonal lines.
            // Slower, dash not supported, more precise.
            if (x1 != x2 && y1 != y2)
            {
                float dx = x2 - x1;
                float dy = y2 - y1;
                float length = (float) System.Math.Sqrt(dx * dx + dy * dy);

                if (length < 0.001f)
                    return;

                float wdx = penWidth * dy / length;
                float wdy = penWidth * dx / length;
         
                var matrix = UE.Matrix4x4.identity;
                matrix.m00 = dx;
                matrix.m01 = -wdx;
                matrix.m03 = x1 + 0.5f * wdx;
                matrix.m10 = dy;
                matrix.m11 = wdy;
                matrix.m13 = y1 - 0.5f * wdy;
         
                UE.GL.PushMatrix();
                UE.GL.MultMatrix(matrix);
                UE.Graphics.DrawTexture(sourceRect, defaultTexture, sourceRect, 0, 0, 0, 0, unityColor, blendMaterial);
                UE.GL.PopMatrix();
                
                return;
            }
            
            float x = 0;
            float y = 0;
            float width = 0;
            float height = 0;

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
                    
            var penDash = pen.DashStyle;
            
            switch (penDash)
            {
                case DashStyle.Solid:
                    UE.Graphics.DrawTexture(new UE.Rect(x, y, width, height), defaultTexture, sourceRect, 0, 0, 0, 0, unityColor, blendMaterial);
                    break;
                case DashStyle.Dash:
                    float dashStep = penWidth * 6;
                    const float dashSpacing = 2;
                    
                    if (y1 == y2) // Horizontal.
                        for (float i = 0; i < width; i += dashStep)
                        {
                            float dashWidth = dashStep - dashSpacing;
                            if (i + dashWidth > width)
                                dashWidth = width - i;

                            UE.Graphics.DrawTexture(new UE.Rect(x + i, y, dashWidth, penWidth), defaultTexture, sourceRect, 0, 0, 0, 0, unityColor, blendMaterial);
                        }

                    if (x1 == x2) // Vertical.
                        for (float i = 0; i < height; i += dashStep)
                        {
                            float dashHeight = dashStep - dashSpacing;
                            if (i + dashHeight > height)
                                dashHeight = height - i;

                            UE.Graphics.DrawTexture(new UE.Rect(x + width - penWidth, y + i, penWidth, dashHeight), defaultTexture, sourceRect, 0, 0, 0, 0, unityColor, blendMaterial);
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
            if (UE.Event.current.type != UE.EventType.Repaint)
                return;
            
            var color = penColor.ToUnityColor();
            var penDash = pen.DashStyle;
            
            switch (penDash)
            {
                case System.Drawing.Drawing2D.DashStyle.Solid:
                    UE.Graphics.DrawTexture(new UE.Rect(x, y, width, penWidth), defaultTexture, sourceRect, 0, 0, 0, 0, color, blendMaterial);
                    UE.Graphics.DrawTexture(new UE.Rect(x + width - penWidth, y + penWidth, penWidth, height - penWidth * 2), defaultTexture, sourceRect, 0, 0, 0, 0, color, blendMaterial);
                    
                    if (height > 1)
                        UE.Graphics.DrawTexture(new UE.Rect(x, y + height - penWidth, width, penWidth), defaultTexture, sourceRect, 0, 0, 0, 0, color, blendMaterial);

                    if (width > 1)
                        UE.Graphics.DrawTexture(new UE.Rect(x, y + penWidth, penWidth, height - penWidth * 2), defaultTexture, sourceRect, 0, 0, 0, 0, color, blendMaterial);

                    break;
                
                case System.Drawing.Drawing2D.DashStyle.Dash:
                    float dash_step = penWidth * 6;
                    for (float i = 0; i < width; i += dash_step)
                    {
                        float dash_width = dash_step - 2;
                        if (i + dash_width > width)
                            dash_width = width - i;

                        UE.Graphics.DrawTexture(new UE.Rect(x + i, y, dash_width, penWidth), defaultTexture, sourceRect, 0, 0, 0, 0, color, blendMaterial); // Top.
                        UE.Graphics.DrawTexture(new UE.Rect(x + i, y + height - penWidth, dash_width, penWidth), defaultTexture, sourceRect, 0, 0, 0, 0, color, blendMaterial); // Bottom.
                    }
                    for (float i = 0; i < height; i += dash_step)
                    {
                        float dash_height = dash_step - 2;
                        if (i + dash_height > height)
                            dash_height = height - i;
                        
                        UE.Graphics.DrawTexture(new UE.Rect(x + width - penWidth, y + i, penWidth, dash_height), defaultTexture, sourceRect, 0, 0, 0, 0, color, blendMaterial); // Right.
                        UE.Graphics.DrawTexture(new UE.Rect(x, y + i, penWidth, dash_height), defaultTexture, sourceRect, 0, 0, 0, 0, blendMaterial); // Left.
                    }
                    break;
                
                case DashStyle.Dot:
                    for (float i = 0; i < height; i += 2)
                    {
                        UE.Graphics.DrawTexture(new UE.Rect(x + width - 1, y + i, 1, 1), defaultTexture, sourceRect, 0, 0, 0, 0, color, blendMaterial); // Right.
                        UE.Graphics.DrawTexture(new UE.Rect(x, y + i, 1, 1), defaultTexture, sourceRect, 0, 0, 0, 0, color, blendMaterial); // Left.
                    }
                    for (float i = 0; i < width; i += 2)
                    {
                        UE.Graphics.DrawTexture(new UE.Rect(x + i, y, 1, 1), defaultTexture, sourceRect, 0, 0, 0, 0, color, blendMaterial); // Top.
                        UE.Graphics.DrawTexture(new UE.Rect(x + i, y + height - 1, 1, 1), defaultTexture, sourceRect, 0, 0, 0, 0, color, blendMaterial); // Bottom.
                    }
                    
                    break;
            }
        }
        public void DrawString(string text, Font font, Color color, float x, float y, float width, float height, ContentAlignment align, object material = null)
        {
            if (text == null || font == null) return;

            if (UE.Event.current.type != UE.EventType.Repaint)
                return;
            
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

            textContent.text = text;
            
            // It's faster to invoke less methods and use your own GUIContent. Not that much, but anyway.
            // NOTE: +1 because something changed in a newer versions of Unity (testing on 2019.1.12f1).
            UE.GUI.Label(new UE.Rect(x, y, width + 1, height), textContent, labelSkin);

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
            if (color.A <= 0)
                return;
            
            if (UE.Event.current.type != UE.EventType.Repaint)
                return;
            
            var rect = new UE.Rect(x, y, width, height);

            if (material == null || material is UE.Material == false)
            {
                UE.Graphics.DrawTexture(rect, defaultTexture, sourceRect, 0, 0, 0, 0, color.ToUnityColor(), blendMaterial);
                return;
            }

            var umat = (UE.Material)material;
            umat.color = color.ToUnityColor();
            UE.Graphics.DrawTexture(rect, defaultTexture, sourceRect, 0, 0, 0, 0, umat);
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

            textContent.text = text;
            
            var size = labelSkin.CalcSize(textContent);

            labelSkin.fontSize = guiSkinFontSizeBuffer;

            return new SizeF(size.x, size.y);
        }
        
        internal string uwfDrawPasswordField(string s, Font font, Color color, float x, float y, float width, float height, HorizontalAlignment alignment)
        {
            if (s == null) s = "";

            UE.GUI.skin.textField.alignment = UE.TextAnchor.UpperLeft;
            switch (alignment)
            {
                case HorizontalAlignment.Center:
                    UE.GUI.skin.textField.alignment = UE.TextAnchor.MiddleCenter;
                    break;
                default:
                    UE.GUI.skin.textField.alignment = UE.TextAnchor.MiddleLeft;
                    break;
                case HorizontalAlignment.Right:
                    UE.GUI.skin.textField.alignment = UE.TextAnchor.MiddleRight;
                    break;
            }

            GUI_SetFont(UE.GUI.skin.textField, font);
            
            UE.GUI.color = color.ToUnityColor();
            UE.GUI.skin.textField.hover.background = null;
            UE.GUI.skin.textField.active.background = null;
            UE.GUI.skin.textField.focused.background = null;
            UE.GUI.skin.textField.normal.background = null;
            
            return UE.GUI.PasswordField(new UE.Rect(x, y, width, height), s, '*');
        }
        internal string uwfDrawTextArea(string s, Font font, Color color, float x, float y, float width, float height)
        {
            if (s == null) s = "";

            UE.GUI.skin.textArea.alignment = UE.TextAnchor.UpperLeft;

            UE.GUI.color = color.ToUnityColor();
            //GUI.skin.textArea.hover.textColor = brush.Color.ToUColor();

            GUI_SetFont(UE.GUI.skin.textArea, font);

            UE.GUI.skin.textArea.hover.background = null;
            UE.GUI.skin.textArea.active.background = null;
            UE.GUI.skin.textArea.focused.background = null;
            UE.GUI.skin.textArea.normal.background = null;

            return UE.GUI.TextArea(new UE.Rect(x, y, width, height), s);
        }
        internal string uwfDrawTextField(string s, Font font, Color color, float x, float y, float width, float height, HorizontalAlignment alignment)
        {
            if (s == null) s = "";

            UE.GUI.skin.textField.alignment = UE.TextAnchor.UpperLeft;
            switch (alignment)
            {
                case HorizontalAlignment.Center:
                    UE.GUI.skin.textField.alignment = UE.TextAnchor.MiddleCenter;
                    break;
                default:
                    UE.GUI.skin.textField.alignment = UE.TextAnchor.MiddleLeft;
                    break;
                case HorizontalAlignment.Right:
                    UE.GUI.skin.textField.alignment = UE.TextAnchor.MiddleRight;
                    break;
            }

            GUI_SetFont(UE.GUI.skin.textField, font);

            UE.GUI.color = color.ToUnityColor();
            UE.GUI.skin.textField.hover.background = null;
            UE.GUI.skin.textField.active.background = null;
            UE.GUI.skin.textField.focused.background = null;
            UE.GUI.skin.textField.normal.background = null;
            
            return UE.GUI.TextField(new UE.Rect(x, y, width, height), s);
        }
        
        private static int GUI_SetFont(UE.GUIStyle style, Font font)
        {
            int guiSkinFontSizeBuffer = style.fontSize;
            if (font != null)
            {
                if (font.fontObject == null && Unity.API.UnityWinForms.gResources != null)
                {
                    var fonts = Unity.API.UnityWinForms.gResources.Fonts;
                    if (fonts != null)
                        for (int i = 0; i < fonts.Count; i++)
                        {
                            var fontItem = fonts[i];
                            if (fontItem.fontNames[0] != font.Name) continue;

                            font.fontObject = fontItem;
                            break;
                        }
                }

                if (font.fontObject != null)
                    style.font = (UE.Font)font.fontObject;
                else
                {
                    style.font = null;
#if UNITY_EDITOR
                    UnityEngine.Debug.LogError("Font not found: " + font.Name);
#endif
                }

                var fontStyle = font.Style;
                style.fontSize = (int)font.Size;
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
                    style.fontSize = 12;
                    style.fontStyle = UnityEngine.FontStyle.Normal;
                }
            }
            return guiSkinFontSizeBuffer;
        }
    }
}
