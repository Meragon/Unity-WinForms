using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Unity.Controls
{
    public class BitmapText : Image
    {
        internal readonly Dictionary<int, BitmapChar> textSettings = new Dictionary<int, BitmapChar>();
        private readonly BitmapFont font;

        public Color Color { get; set; }
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
            Color = Color.White;
            this.font = font;
            Scale = 1f;
        }
        public void Apply()
        {
            var tSize = TextureSize;
            Texture = Graphics.ApiGraphics.CreateTexture(tSize.Width, tSize.Height);

            if (tSize.Width == 0 || tSize.Height == 0)
                return;
            

            // Clear texture with transparent color.
            var uBackColor = Color.Transparent;
            var clearColor = new Color[Texture.Width * Texture.Height];
            for (int i = 0; i < clearColor.Length; i++)
                clearColor[i] = uBackColor;
            Texture.SetPixels(clearColor);

            int xOffset = 0;
            float cursorY = GetCursorY();
            float cursorOffset = Texture.Height - cursorY;

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
                    var charPixels = textC.Texture.Texture.GetPixels();
                    var charTexture = Graphics.ApiGraphics.CreateTexture(textC.Texture.Width, textC.Texture.Height);
                    charTexture.SetPixels(charPixels);
                    charTexture.Apply();

                    // Scale texture if needed.
                    if ((int) cW != charTexture.Width || (int) cH != charTexture.Height)
                    {
                        // TODO: not working.
                        //TextureScaler.scale(charTexture, (int) cW, (int) cH);
                    }

                    var blendedCharPixels = charTexture.GetPixels();
                    for (int p = 0; p < blendedCharPixels.Length; p++)
                    {
                        // BlendMode: Multiply
                        var origColor = blendedCharPixels[p];
                        var bA = (origColor.A * charColor.A) / (255 * 255);
                        var bR = (origColor.R * charColor.R) / (255 * 255);
                        var bG = (origColor.G * charColor.G) / (255 * 255);
                        var bB = (origColor.B * charColor.B) / (255 * 255);
                        if (bA > 0)
                            blendedCharPixels[p] = Color.FromArgb(bA, bR, bG, bB);
                    }

                    Texture.SetPixels((int)cX, (int)(cursorOffset - bellowHeight), charTexture.Width, charTexture.Height, blendedCharPixels);
                }

                xOffset += (int)cA;
            }

            Texture.Apply();
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
                ForeColor = c;
                Scale = scale;
            }
        }
    }
}
