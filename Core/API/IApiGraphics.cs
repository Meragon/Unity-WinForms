namespace System.Drawing.API
{
    public interface IApiGraphics
    {
        ITexture CreateTexture(int width, int height);
        ITexture CreateTexture(object original);

        void BeginGroup(float x, float y, float width, float height);
        void Clear(Color color);
        void DrawImage(Image image, float x, float y, float width, float height, float angle);
        void DrawImage(Image image, float x, float y, float width, float height, object material = null);
        void DrawLine(Pen pen, float x1, float y1, float x2, float y2, object material = null);
        void DrawPolygon(Pen pen, Point[] points, object material = null);
        void DrawRectangle(Pen pen, float x, float y, float width, float height, object material = null);
        void DrawString(string text, Font font, Color color, float x, float y, float width, float height, ContentAlignment align, object material = null);
        void EndGroup();
        void FillRectangle(Brush brush, float x, float y, float width, float height, object material = null);
        void FillRectangle(Color color, float x, float y, float width, float height, object material = null);
        void Focus();
        void FocusNext();
        SizeF MeasureString(string text, Font font);
    }
}
