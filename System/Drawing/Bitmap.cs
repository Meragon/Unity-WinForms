using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Drawing
{
    public class Bitmap : Image
    {
        public Bitmap(UnityEngine.Texture2D original)
        {
            uTexture = original;
        }

        public void Apply()
        {
            uTexture.Apply();
        }
        public Color GetPixel(int x, int y)
        {
            return Color.FromUColor(uTexture.GetPixel(x, y));
        }
        public void SetPixel(int x, int y, Color color)
        {
            uTexture.SetPixel(x, y, color.ToUColor());
        }
    }
}
