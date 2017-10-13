namespace System.Windows.Forms
{
    using System.ComponentModel;
    using System.Drawing;

    public class PictureBox : Control, ISupportInitialize
    {
        private BorderStyle borderStyle = BorderStyle.None;

        public BorderStyle BorderStyle
        {
            get { return borderStyle; }
            set { borderStyle = value; }
        }
        public Bitmap Image { get; set; }

        protected override Size DefaultSize
        {
            get { return new Size(100, 50); }
        }

        public void BeginInit()
        {
        }
        public void EndInit()
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;

            var image = Image;
            if (image != null) // TODO: Animate image.
                g.DrawImage(image, 0, 0, image.Width, image.Height);

            ControlPaint.PrintBorder(g, ClientRectangle, borderStyle, Border3DStyle.Sunken);

            base.OnPaint(e);
        }
    }
}
