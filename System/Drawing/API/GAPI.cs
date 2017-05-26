using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Drawing
{
    public abstract class GAPI
    {
        public abstract ITexture CreateTexture(int width, int height);
        public abstract ITexture CreateTexture(object original);

        public abstract void DrawImage(Image image, Color color, float x, float y, float width, float height, object material = null);
        public abstract void DrawString(string text, Font font, Color color, float x, float y, float width, float height, ContentAlignment align, object material = null);
        public abstract void FillRectangle(Color color, float x, float y, float width, float height, object material = null);
    }
}
