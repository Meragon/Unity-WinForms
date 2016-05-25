using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public class BitmapLabel : Control
    {
        internal Bitmap _bakedTexture;
        private CharSettings[] _charSettings;
        private BitmapFont _font;
        private string _text;
        
        public new Color ForeColor
        {
            set
            {
                if (_charSettings != null)
                    for (int i = 0; i < _charSettings.Length; i++)
                        _charSettings[i].ForeColor = value;
            }
        }
        public float Scale
        {
            set
            {
                if (_charSettings != null)
                    for (int i = 0; i < _charSettings.Length; i++)
                        _charSettings[i].Scale = value;

                if (AutoSize)
                    Size = TextureSize;
            }
        }
        public bool ShowRects { get; set; }
        public new string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                _charSettings = new CharSettings[_text.Length];
                for (int i = 0; i < _charSettings.Length; i++)
                    _charSettings[i] = new CharSettings();

                if (AutoSize)
                    Size = TextureSize;
            }
        }
        public Size TextureSize
        {
            get
            {
                float tW = 0;
                float tH = 0;

                float minOffset = 0;
                float minBottomPart = 0;

                for (int i = 0; i < Text.Length; i++)
                {
                    if (_font.textureList.ContainsKey(Text[i]) == false) continue;
                    var textC = _font.textureList[Text[i]];
                    if (textC == null) continue;

                    float _scale = _charSettings[i].Scale;

                    float cOY = textC.OffsetY * _scale;
                    float cH = textC.Texture.Height * _scale;
                    float cBp = cH - cOY;

                    if (cOY > minOffset)
                        minOffset = cOY;
                    if (cBp > minBottomPart)
                        minBottomPart = cBp;

                    tW += textC.Advance * _scale;
                }

                tH = minOffset + minBottomPart;

                return new Size((int)tW, (int)tH);
            }
        }

        public CharSettings this[int index]
        {
            get { return _charSettings[index]; }
        }

        public BitmapLabel(BitmapFont font)
        {
            if (font == null || font.Loaded == false)
                throw new NullReferenceException("font");

            _font = font;
            
            AutoSize = true;
        }

        public void Bake()
        {
            var tSize = TextureSize;
            _bakedTexture = new Bitmap(tSize.Width, tSize.Height);

            // Clear baked texture with transparent color.
            for (int y = 0; y < _bakedTexture.Height; y++)
                for (int x = 0; x < _bakedTexture.Width; x++)
                    _bakedTexture.SetPixel(x, y, Color.Transparent);

            int xOffset = 0;
            for (int i = 0; i < Text.Length; i++)
            {
                if (_font.textureList.ContainsKey(Text[i]) == false) continue;
                var textC = _font.textureList[Text[i]];
                if (textC == null) continue;

                float _scale = _charSettings[i].Scale;
                Color _foreColor = _charSettings[i].ForeColor;

                float cX = xOffset + textC.OffsetX * _scale;
                float cY = GetCursor() - textC.OffsetY * _scale;
                float cW = textC.Texture.Width * _scale;
                float cH = textC.Texture.Height * _scale;
                float cA = textC.Advance * _scale;

                if (cW <= 0 || cH <= 0)
                {
                    // Skip
                }
                else
                {
                    var origPixels = textC.Texture.uTexture.GetPixels32();
                    var newTexture = new UnityEngine.Texture2D(textC.Texture.Width, textC.Texture.Height);
                    newTexture.name = "backedGlyphTexture";
                    newTexture.SetPixels32(origPixels);
                    newTexture.Apply();
                    // Scale texture if needed.
                    if ((int)cW != newTexture.width || (int)cH != newTexture.height)
                    {
                        TextureScaler.scale(newTexture, (int)cW, (int)cH);
                        //TextureScale.Bilinear(newTexture, (int)cW, (int)cH);
                        newTexture.Apply();
                    }
                    
                    var newImagePixels = newTexture.GetPixels32();
                    for (int p = 0; p < newImagePixels.Length; p++)
                    {
                        // BlendMode: Multiply
                        var origColor = Color.FromUColor(newImagePixels[p]);
                        var blendColorA = (float)(origColor.A * _foreColor.A) / 255;
                        var blendColorR = (float)(origColor.R * _foreColor.R) / 255;
                        var blendColorG = (float)(origColor.G * _foreColor.G) / 255;
                        var blendColorB = (float)(origColor.B * _foreColor.B) / 255;
                        var blendColor = Color.FromArgb((int)blendColorA, (int)blendColorR, (int)blendColorG, (int)blendColorB);
                        newImagePixels[p] = blendColor.ToUColor();
                    }

                    _bakedTexture.uTexture.SetPixels32((int)cX, (int)cY, newTexture.width, newTexture.height, newImagePixels);
                }

                xOffset += (int)cA;
            }

            _bakedTexture.Apply();

            if (AutoSize)
                Size = tSize;
        }
        public void ClearBake()
        {
            _bakedTexture = null;
        }
        private float GetCursor()
        {
            float cursor = 0;
            for (int i = 0; i < Text.Length; i++)
            {
                if (_font.textureList.ContainsKey(Text[i]) == false) continue;
                var textC = _font.textureList[Text[i]];
                if (textC == null) continue;

                float _scale = _charSettings[i].Scale;

                float cOY = textC.OffsetY * _scale;
                float cH = textC.Texture.Height * _scale;
                float cBp = cH - cOY;

                if (cOY > cursor)
                    cursor = cOY;
            }
            return cursor;
        }

        //private float _testHue;

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(BackColor, 0, 0, Width, Height);

            if (string.IsNullOrEmpty(Text)) return;

            if (_bakedTexture == null)
            {
                var tSize = TextureSize;

                int xOffset = 0;
                for (int i = 0; i < Text.Length; i++)
                {
                    if (_font.textureList.ContainsKey(Text[i]) == false) continue;
                    var textC = _font.textureList[Text[i]];
                    if (textC == null) continue;

                    /*if (textC.Id == 'H' || textC.Id == 'W')
                    {
                        _testHue += UnityEngine.Time.deltaTime * 5;
                        if (_testHue > 255)
                            _testHue = 0;
                        _charSettings[i].ForeColor = ColorPickerForm.GetColorFromHSL(_testHue / 255, 0.5f, .5f);
                    }*/

                    float _scale = _charSettings[i].Scale;
                    Color _foreColor = _charSettings[i].ForeColor;

                    float cX = xOffset + textC.OffsetX * _scale;
                    float cY = GetCursor() - textC.OffsetY * _scale;
                    float cW = textC.Texture.Width * _scale;
                    float cH = textC.Texture.Height * _scale;
                    float cA = textC.Advance * _scale;
                    if (ShowRects)
                        e.Graphics.DrawRectangle(new Pen(Color.FromArgb(64, 64, 64, 64)), cX, cY, cW, cH);
                    e.Graphics.DrawTexture(textC.Texture, cX, cY, cW, cH, _foreColor);
                    xOffset += (int)cA;
                }
            }
            else
            {
                e.Graphics.DrawTexture(_bakedTexture.uTexture, 0, 0, _bakedTexture.Width, _bakedTexture.Height);
            }
        }
        protected override object OnPaintEditor(float width)
        {
            var control = base.OnPaintEditor(width);

            Editor.NewLine(1);

            var textBuffer = Editor.TextField("Text", Text);
            if (textBuffer.Changed)
            {
                _bakedTexture = null;
                Text = textBuffer;
            }

            Editor.Label("Baked", _bakedTexture != null);

            if (_bakedTexture == null)
            {
                var showRectsBuffer = Editor.BooleanField("ShowRects", ShowRects);
                if (showRectsBuffer.Changed)
                    ShowRects = showRectsBuffer.Value;
            }

            if (Editor.Button("Bake"))
                Bake();
            if (Editor.Button("ClearBake"))
                ClearBake();

            if (Editor.Button("Scale x0.5"))
                Scale = .5f;
            if (Editor.Button("Scale x1"))
                Scale = 1f;
            if (Editor.Button("Scale x2"))
                Scale = 2f;

            return control;
        }
        public override void Refresh()
        {
            base.Refresh();
            Bake();
        }

        public class CharSettings
        {
            public Color ForeColor { get; set; }
            public float Scale { get; set; }

            public CharSettings()
            {
                this.ForeColor = Color.Black;
                this.Scale = 1f;
            }
        }
    }
}
