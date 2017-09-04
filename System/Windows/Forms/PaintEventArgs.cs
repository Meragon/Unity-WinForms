namespace System.Windows.Forms
{
    using System.Drawing;

    public class PaintEventArgs
    {
        public Rectangle ClipRectangle { get; set; }
        public Graphics Graphics { get; set;  }
    }
}
