using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.API;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UE = UnityEngine;

namespace Unity.API
{
    public class UnityGdi : IApiGraphics
    {
        private readonly UE.Texture2D defaultTexture;

        public UnityGdi(UE.Texture2D defTexture)
        {
            defaultTexture = defTexture;
        }

        public ITexture CreateTexture(int width, int height)
        {
            return new UnityGdiTexture(width, height);
        }
        public ITexture CreateTexture(object original)
        {
            return new UnityGdiTexture(original as UE.Texture2D);
        }
        public void DrawImage(Image image, Color color, float x, float y, float width, float height, object material = null)
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
                    mat.color = color.ToUnityColor();
                    UnityEngine.Graphics.DrawTexture(new UE.Rect(x, y, width, height), textureToDraw, mat);
                    return;
                }
            }

            UE.GUI.color = color.ToUnityColor();
            UE.GUI.DrawTexture(new UE.Rect(x, y, width, height), textureToDraw);
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

            int guiSkinFontSizeBuffer = GUI_SetFont(UE.GUI.skin.label, font);

            UE.GUI.color = color.ToUnityColor();
            UE.GUI.skin.label.alignment = uAlign;
            UE.GUI.Label(new UE.Rect(x, y, width, height), text);

            UE.GUI.skin.label.fontSize = guiSkinFontSizeBuffer;
        }
        public void FillRectangle(Color color, float x, float y, float width, float height, object material = null)
        {
            UE.GUI.color = color.ToUnityColor();
            UE.GUI.DrawTexture(new UE.Rect(x, y, width, height), defaultTexture);
        }

        private static int GUI_SetFont(UE.GUIStyle style, Font font)
        {
            int guiSkinFontSizeBuffer = UE.GUI.skin.label.fontSize;
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
