namespace System.Drawing
{
    public abstract class Brush : ICloneable, IDisposable
    {
        public abstract object Clone();
        public void Dispose()
        {
        }
    }
}
