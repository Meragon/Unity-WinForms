namespace System.Drawing.API
{
    public interface ITexture
    {
        int Height { get; }
        int Width { get; }

        void    Apply();
        void    Clear(Color color);
        Color   GetPixel(int x, int y);
        Color[] GetPixels();
        Color[] GetPixels(int x, int y, int width, int height);
        void    SetPixel(int x, int y, Color color);
        void    SetPixels(Color[] colors);
        void    SetPixels(int x, int y, int width, int height, Color[] colors);
    }
}