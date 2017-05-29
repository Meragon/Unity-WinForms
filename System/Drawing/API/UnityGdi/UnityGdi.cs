using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnityEngine;

namespace System.Drawing
{
    public class UnityGdi : GAPI
    {
        private readonly Texture2D defaultTexture;

        public UnityGdi(Texture2D defTexture)
        {
            defaultTexture = defTexture;
        }

        public override ITexture CreateTexture(int width, int height)
        {
            return new UnityGdiTexture(width, height);
        }
        public override ITexture CreateTexture(object original)
        {
            return new UnityGdiTexture(original as Texture2D);
        }
        public override void DrawImage(Image image, Color color, float x, float y, float width, float height, object material = null)
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
                var mat = material as Material;
                if (mat != null)
                {
                    mat.color = color.ToUnityColor();
                    UnityEngine.Graphics.DrawTexture(new Rect(x, y, width, height), textureToDraw, mat);
                    return;
                }
            }

            GUI.color = color.ToUnityColor();
            GUI.DrawTexture(new Rect(x, y, width, height), textureToDraw);
        }
        public override void DrawString(string text, Font font, Color color, float x, float y, float width, float height, ContentAlignment align, object material = null)
        {
            if (text == null || font == null) return;

            // TODO: material not supported.

            // Set align.
            var uAlign = TextAnchor.UpperLeft;
            switch (align)
            {
                case ContentAlignment.BottomCenter:
                    uAlign = TextAnchor.LowerCenter;
                    break;
                case ContentAlignment.BottomLeft:
                    uAlign = TextAnchor.LowerLeft;
                    break;
                case ContentAlignment.BottomRight:
                    uAlign = TextAnchor.LowerRight;
                    break;
                case ContentAlignment.MiddleCenter:
                    uAlign = TextAnchor.MiddleCenter;
                    break;
                case ContentAlignment.MiddleLeft:
                    uAlign = TextAnchor.MiddleLeft;
                    break;
                case ContentAlignment.MiddleRight:
                    uAlign = TextAnchor.MiddleRight;
                    break;
                case ContentAlignment.TopCenter:
                    uAlign = TextAnchor.UpperCenter;
                    break;
                case ContentAlignment.TopLeft:
                    uAlign = TextAnchor.UpperLeft;
                    break;
                case ContentAlignment.TopRight:
                    uAlign = TextAnchor.UpperRight;
                    break;
            }

            int guiSkinFontSizeBuffer = GUI_SetFont(GUI.skin.label, font);

            GUI.color = color.ToUnityColor();
            GUI.skin.label.alignment = uAlign;
            GUI.Label(new Rect(x, y, width, height), text);

            GUI.skin.label.fontSize = guiSkinFontSizeBuffer;
        }
        public override void FillRectangle(Color color, float x, float y, float width, float height, object material = null)
        {
            GUI.color = color.ToUnityColor();
            GUI.DrawTexture(new Rect(x, y, width, height), defaultTexture);
        }

        private static int GUI_SetFont(GUIStyle style, Font font)
        {
            int guiSkinFontSizeBuffer = GUI.skin.label.fontSize;
            if (font != null)
            {
                if (font.UFont == null)
                {
                    var fonts = System.Windows.Forms.ApplicationBehaviour.Resources.Fonts;
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
    }
}
