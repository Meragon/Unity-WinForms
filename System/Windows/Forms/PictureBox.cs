using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    [Serializable]
    public class PictureBox : Control
    {
        public Bitmap Image { get; set; }
        public Color ImageBackColor { get; set; }
        public Color ImageBorderColor { get; set; }
        public Color ImageColor { get; set; }
        public ImageLayout ImageLayout { get; set; }

        public PictureBox()
        {
            ImageBackColor = Color.Transparent;
            ImageColor = Color.White;
            ImageLayout = Forms.ImageLayout.Center;
            Size = new Size(100, 50);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (Image != null && Image.uTexture != null)
            {
                Rectangle rect = new Rectangle();
                var iLayout = ImageLayout;
                if (iLayout == Forms.ImageLayout.CenterZoom)
                {
                    if (Image.Width > Width || Image.Height > Height)
                        iLayout = Forms.ImageLayout.Zoom;
                    else
                        iLayout = Forms.ImageLayout.Center;
                }

                switch (iLayout)
                {
                    default:
                    case ImageLayout.None:
                        rect = new Rectangle(0, 0, Image.Width, Image.Height);
                        break;
                    case ImageLayout.Center:
                        rect = new Rectangle(Width / 2 - Image.Width / 2, Height / 2 - Image.Height / 2, Image.Width, Image.Height);
                        break;
                    case ImageLayout.Stretch:
                        rect = new Rectangle(0, 0, Width, Height);
                        break;
                    case ImageLayout.Zoom:
                        float innerAspectRatio = Image.Width / (float)Image.Height;
                        float outerAspectRatio = Width / (float)Height;

                        float resizeFactor = (innerAspectRatio >= outerAspectRatio) ?
                        (Width / (float)Image.Width) :
                        (Height / (float)Image.Height);

                        float newWidth = Image.Width * resizeFactor;
                        float newHeight = Image.Height * resizeFactor;
                        float newLeft = (Width - newWidth) / 2f;
                        float newTop = (Height - newHeight) / 2f;

                        rect = new RectangleF(newLeft, newTop, newWidth, newHeight);
                        break;
                }
                e.Graphics.FillRectangle(new SolidBrush(ImageBackColor), rect);
                e.Graphics.DrawTexture(Image, rect);
                e.Graphics.DrawRectangle(new Pen(ImageBorderColor), rect);
            }
        }
    }
}
