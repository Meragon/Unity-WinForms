using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Drawing.API
{
    public interface IApiGraphics
    {
        ITexture CreateTexture(int width, int height);
        ITexture CreateTexture(object original);

        void DrawImage(Image image, Color color, float x, float y, float width, float height, object material = null);
        void DrawString(string text, Font font, Color color, float x, float y, float width, float height, ContentAlignment align, object material = null);
        void FillRectangle(Color color, float x, float y, float width, float height, object material = null);
    }
}
