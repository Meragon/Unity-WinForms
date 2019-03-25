namespace System.Drawing
{
    public abstract class Brush : ICloneable, IDisposable
    {
        ~Brush() 
        {
            Dispose(false);
        }
        
        public abstract object Clone();
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
