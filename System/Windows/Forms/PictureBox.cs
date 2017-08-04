namespace System.Windows.Forms
{
    using System.ComponentModel;
    using System.Drawing;

    [Serializable]
    public class PictureBox : Control, ISupportInitialize
    {
        private ImageLayout backgroundImageLayout;
        private Bitmap image;
        private Rectangle rect;

        public PictureBox()
        {
            backgroundImageLayout = ImageLayout.Center;

            UpdateRect(this, EventArgs.Empty);
            Resize += UpdateRect;
        }

        public override ImageLayout BackgroundImageLayout
        {
            get { return backgroundImageLayout; }
            set
            {
                backgroundImageLayout = value;
                UpdateRect(this, EventArgs.Empty);
            }
        }
        public Bitmap Image
        {
            get { return image; }
            set
            {
                image = value;
                UpdateRect(this, EventArgs.Empty);
            }
        }

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
            base.OnPaint(e);

            if (Image == null || Image.uTexture == null) return;

            e.Graphics.DrawImage(Image, rect);
        }

        private void UpdateRect(object sender, EventArgs e)
        {
            if (Image == null) return;

            var iLayout = BackgroundImageLayout;
            var imageWidth = Image.Width;
            var imageHeight = Image.Height;
            var width = Width;
            var height = Height;

            if (iLayout == ImageLayout.CenterZoom)
            {
                if (imageWidth > width || imageHeight > height)
                    iLayout = ImageLayout.Zoom;
                else
                    iLayout = ImageLayout.Center;
            }

            switch (iLayout)
            {
                default:
                    rect = new Rectangle(0, 0, imageWidth, imageHeight);
                    break;
                case ImageLayout.Center:
                    rect = new Rectangle(width / 2 - imageWidth / 2, height / 2 - imageHeight / 2, imageWidth, imageHeight);
                    break;
                case ImageLayout.Stretch:
                    rect = new Rectangle(0, 0, width, height);
                    break;
                case ImageLayout.Zoom:
                    float innerAspectRatio = imageWidth / (float)imageHeight;
                    float outerAspectRatio = width / (float)height;

                    float resizeFactor = (innerAspectRatio >= outerAspectRatio) ?
                                             (width / (float)imageWidth) :
                                             (height / (float)imageHeight);

                    float newWidth = imageWidth * resizeFactor;
                    float newHeight = imageHeight * resizeFactor;
                    float newLeft = (width - newWidth) / 2f;
                    float newTop = (height - newHeight) / 2f;

                    rect = new Rectangle((int)newLeft, (int)newTop, (int)newWidth, (int)newHeight);
                    break;
            }
        }
    }
}
