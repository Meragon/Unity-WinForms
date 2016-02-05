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
        public UnityEngine.Texture2D Image { get; set; }
        public Color ImageColor { get; set; }
        public ImageLayout ImageLayout { get; set; }

        public PictureBox()
        {
            ImageColor = Color.White;
            ImageLayout = Forms.ImageLayout.Center;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            if (Image != null)
            {
                switch (ImageLayout)
                {
                    default:
                    case ImageLayout.None:
                        g.DrawTexture(Image, 0, 0, Image.width, Image.height, ImageColor);
                        break;
                    case ImageLayout.Center:
                        g.DrawTexture(Image, Width / 2 - Image.width / 2, Height / 2 - Image.height / 2, Image.width, Image.height, ImageColor);
                        break;
                    case ImageLayout.Stretch:
                        g.DrawTexture(Image, 0, 0, Width, Height, ImageColor);
                        break;
                    case ImageLayout.Zoom:
                        // TODO: not working.
                        break;
                }
            }
        }
    }
}
