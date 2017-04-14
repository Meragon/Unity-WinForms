using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using UnityEngine;

namespace System.Drawing
{
    public class BitmapText : Image
    {
        internal readonly Dictionary<int, BitmapChar> textSettings = new Dictionary<int, BitmapChar>();
        private readonly BitmapFont font;

        public float Scale { get; set; }
        public bool ShowRects { get; set; }
        public string Text { get; set; }
        public Size TextureSize
        {
            get
            {
                int tW = 0;
                float tH = 0;

                float minOffset = 0;
                float minBottomPart = 0;

                for (int i = 0; i < Text.Length; i++)
                {
                    if (font.textureList.ContainsKey(Text[i]) == false) continue;
                    var textC = font.textureList[Text[i]];
                    if (textC == null) continue;

                    var cSettings = CharAt(i);
                    float _scale = cSettings.Scale;

                    float cOY = textC.OffsetY * _scale;
                    float cH = textC.Texture.Height * _scale;
                    float cBp = cH - cOY;

                    if (cOY > minOffset)
                        minOffset = cOY;
                    if (cBp > minBottomPart)
                        minBottomPart = cBp;

                    tW += (int)(textC.Advance * _scale);
                }

                tH = minOffset + minBottomPart;

                if (tW != 0) tW += 1;

                return new Size(tW, (int)tH);
            }
        }

        public BitmapText(BitmapFont font)
        {
            this.Color = Color.White;
            this.font = font;
            this.Scale = 1f;
        }
        public override void Apply()
        {
            var tSize = TextureSize;
            uTexture = new Bitmap(tSize.Width, tSize.Height);

            if (tSize.Width == 0 || tSize.Height == 0)
                return;
            

            // Clear texture with transparent color.
            var uBackColor = Color.Transparent.ToUColor();
            var clearColor = new Color32[uTexture.width * uTexture.height];
            for (int i = 0; i < clearColor.Length; i++)
                clearColor[i] = uBackColor;
            uTexture.SetPixels32(clearColor);

            int xOffset = 0;
            float cursorY = GetCursorY();
            float cursorOffset = uTexture.height - cursorY;

            for (int i = 0; i < Text.Length; i++)
            {
                var charItem = Text[i];
                if (font.textureList.ContainsKey(charItem) == false) continue;

                var textC = font.textureList[charItem];
                if (textC == null) continue;

                var charSettings = CharAt(i);
                var charScale = charSettings.Scale;
                var charColor = charSettings.ForeColor;

                float cW = textC.Texture.Width * charScale;
                float cH = textC.Texture.Height * charScale;
                float cA = textC.Advance * charScale;
                float cX = xOffset + textC.OffsetX * charScale;
                float bellowHeight = cH - textC.OffsetY * charScale;

                if (cW > 0 && cH > 0)
                {
                    var charPixels = textC.Texture.uTexture.GetPixels32();
                    var charTexture = new UnityEngine.Texture2D(textC.Texture.Width, textC.Texture.Height);
                    charTexture.SetPixels32(charPixels);
                    charTexture.Apply();

                    // Scale texture if needed.
                    if ((int)cW != charTexture.width || (int)cH != charTexture.height)
                        TextureScaler.scale(charTexture, (int)cW, (int)cH);

                    var blendedCharPixels = charTexture.GetPixels32();
                    for (int p = 0; p < blendedCharPixels.Length; p++)
                    {
                        // BlendMode: Multiply
                        var origColor = blendedCharPixels[p];
                        var bA = (float)(origColor.a * charColor.A) / (255 * 255);
                        var bR = (float)(origColor.r * charColor.R) / (255 * 255);
                        var bG = (float)(origColor.g * charColor.G) / (255 * 255);
                        var bB = (float)(origColor.b * charColor.B) / (255 * 255);
                        if (bA > 0)
                            blendedCharPixels[p] = new UnityEngine.Color(bR, bG, bB, bA);
                    }

                    uTexture.SetPixels32((int)cX, (int)(cursorOffset - bellowHeight), charTexture.width, charTexture.height, blendedCharPixels);
                }

                xOffset += (int)cA;
            }

            uTexture.Apply();
        }
        public BitmapChar CharAt(int index)
        {
            if (textSettings.ContainsKey(index) == false)
                textSettings.Add(index, new BitmapChar(Color, Scale));
            var bc = textSettings[index];
            bc.Char = Text[index];
            return bc;
        }
        public int WidthAt(int index)
        {
            int tW = 0;
            
            for (int i = 0; i < Text.Length; i++)
            {
                if (i >= index) return tW;

                if (font.textureList.ContainsKey(Text[i]) == false) continue;
                var textC = font.textureList[Text[i]];
                if (textC == null) continue;

                var cSettings = CharAt(i);
                float _scale = cSettings.Scale;
                
                tW += (int)(textC.Advance * _scale);
            }

            return tW;
        }

        private float GetCursorY()
        {
            float cursor = 0;
            for (int i = 0; i < Text.Length; i++)
            {
                if (font.textureList.ContainsKey(Text[i]) == false) continue;
                var textC = font.textureList[Text[i]];
                if (textC == null) continue;

                var cSettings = CharAt(i);
                float _scale = cSettings.Scale;
                float cOY = textC.OffsetY * _scale;

                if (cOY > cursor)
                    cursor = cOY;
            }
            return cursor;
        }

        public class BitmapChar
        {
            public char Char { get; set; }
            public Color ForeColor { get; set; }
            public float Scale { get; set; }

            public BitmapChar() : this(Color.Black, 1f)
            {

            }
            public BitmapChar(Color c, float scale)
            {
                this.ForeColor = c;
                this.Scale = scale;
            }
        }
    }
}
