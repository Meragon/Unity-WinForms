namespace System.Windows.Forms
{
    using System.ComponentModel;
    using System.Drawing;

    public class PictureBox : Control, ISupportInitialize
    {
        public PictureBox()
        {
            BorderStyle = BorderStyle.None;
        }

        public BorderStyle BorderStyle { get; set; }
        public Bitmap Image { get; set; }

        protected override Size DefaultSize
        {
            get { return new Size(100, 50); }
        }

        public void BeginInit()
        {
            // ImageLocation not supported yet.
        }
        public void EndInit()
        {
            // ImageLocation not supported yet.
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;

            var image = Image;
            if (image != null) // TODO: Animate image.
                g.DrawImage(image, 0, 0, image.Width, image.Height);

            ControlPaint.PrintBorder(g, ClientRectangle, BorderStyle, Border3DStyle.Sunken);

            base.OnPaint(e);
        }
    }
}
