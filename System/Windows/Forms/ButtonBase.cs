using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace System.Windows.Forms
{
    public class ButtonBase : Control
    {
        private FlatStyle flatStyle = FlatStyle.Standard;
        private Bitmap imageFromList;
        private ContentAlignment imageAlign = ContentAlignment.MiddleCenter;
        private int imageIndex;
        private ImageList imageList;
        private ContentAlignment textAlign = ContentAlignment.MiddleCenter;
        private readonly Pen borderPen = new Pen(Color.Transparent);
        
        public FlatStyle FlatStyle
        {
            get { return flatStyle; }
            set { flatStyle = value; }
        }
        public Bitmap Image { get; set; }
        public ContentAlignment ImageAlign
        {
            get { return imageAlign; }
            set { imageAlign = value; }
        }
        public int ImageIndex
        {
            get { return imageIndex; }
            set
            {
                imageIndex = value;
                if (ImageList != null && imageIndex >= 0 && imageIndex < ImageList.Images.Count)
                    imageFromList = ImageList.Images[imageIndex] as Bitmap;
            }
        }
        public ImageList ImageList
        {
            get { return imageList; }
            set
            {
                imageList = value;
                if (ImageList != null && imageIndex >= 0 && imageIndex < ImageList.Images.Count)
                    imageFromList = ImageList.Images[imageIndex] as Bitmap;
            }
        }
        public virtual ContentAlignment TextAlign
        {
            get { return textAlign; }
            set { textAlign = value; }
        }

        internal Color uwfBorderColor = Color.FromArgb(172, 172, 172);
        internal Color uwfBorderDisableColor = Color.FromArgb(217, 217, 217);
        internal Color uwfBorderHoverColor = Color.FromArgb(126, 180, 234);
        internal Color uwfBorderSelectColor = Color.FromArgb(51, 153, 255);
        internal Color uwfDisableColor = Color.FromArgb(239, 239, 239);
        internal Color uwfHoverColor = Color.FromArgb(223, 238, 252);
        internal Bitmap uwfImageHover;
        internal Color uwfImageColor = Color.White;
        internal Color uwfImageHoverColor = Color.White;

        public ButtonBase()
        {
            BackColor = Color.FromArgb(234, 234, 234);
            BackgroundImageLayout = ImageLayout.Center;
            ForeColor = Color.FromArgb(64, 64, 64);
            Size = new Size(75, 23);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Return)
                this.OnClick(EventArgs.Empty);

            base.OnKeyUp(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;

            var enabled = Enabled;
            var height = Height;
            var width = Width;

            var backColorToDraw = Color.Transparent;
            var borderColorToDraw = Color.Transparent;

            // Get colors from style.
            switch (flatStyle)
            {
                case FlatStyle.Flat:
                    if (uwfHovered) backColorToDraw = Color.FromArgb(32, 0, 0, 0);
                    borderColorToDraw = Color.Black;
                    break;
                case FlatStyle.Popup:
                    borderColorToDraw = Color.FromArgb(85, 85, 85); // TODO: border width for top-left corner.
                    break;
                case FlatStyle.Standard:
                case FlatStyle.System:
                    // Back.
                    if (enabled)
                        backColorToDraw = uwfHovered == false ? BackColor : uwfHoverColor;
                    else
                        backColorToDraw = uwfDisableColor;

                    // Border.
                    if (enabled == false)
                        borderColorToDraw = uwfBorderDisableColor;
                    else if (uwfHovered)
                        borderColorToDraw = uwfBorderHoverColor;
                    else if (Focused)
                        borderColorToDraw = uwfBorderSelectColor;
                    else
                        borderColorToDraw = uwfBorderColor;
                    break;
            }

            borderPen.Color = borderColorToDraw;

            g.uwfFillRectangle(backColorToDraw, 0, 0, width, height);
            g.DrawRectangle(borderPen, 0, 0, width, height);

            // Background image.
            if (BackgroundImage != null)
            {
                var backgroundImage = BackgroundImage;
                var imageColorToPaint = uwfImageColor;
                if (uwfHovered)
                {
                    if (uwfImageHover != null)
                        backgroundImage = uwfImageHover;
                    imageColorToPaint = uwfImageHoverColor;
                }

                // Fix: wrong colors.
                switch (BackgroundImageLayout)
                {
                    default:
                    case ImageLayout.None:
                        g.uwfDrawImage(backgroundImage, imageColorToPaint, 0, 0, backgroundImage.Width, backgroundImage.Height);
                        break;
                    case ImageLayout.Center:
                        g.uwfDrawImage(backgroundImage, imageColorToPaint,
                            width / 2 - backgroundImage.Width / 2,
                            height / 2 - backgroundImage.Height / 2,
                            backgroundImage.Width,
                            backgroundImage.Height);
                        break;
                    case ImageLayout.Stretch:
                        g.uwfDrawImage(backgroundImage, imageColorToPaint, 0, 0, width, height);
                        break;
                    case ImageLayout.Zoom:
                        // TODO: not working.
                        break;
                }
            }

            // Image.
            var imageToPaint = Image;
            if (imageFromList != null)
                imageToPaint = imageFromList;

            if (imageToPaint != null)
            {
                var imageWidth = imageToPaint.Width;
                var imageHeight = imageToPaint.Height;
                var imageColorToPaint = uwfImageColor;
                if (uwfHovered)
                {
                    if (uwfImageHover != null)
                        imageToPaint = uwfImageHover;
                    imageColorToPaint = uwfImageHoverColor;
                }

                float ix = 0, iy = 0;
                const float borderOffset = 2f;

                switch (imageAlign)
                {
                    case ContentAlignment.BottomCenter:
                    case ContentAlignment.BottomLeft:
                    case ContentAlignment.BottomRight:
                        iy = Height - imageHeight - borderOffset;
                        break;
                    case ContentAlignment.MiddleCenter:
                    case ContentAlignment.MiddleLeft:
                    case ContentAlignment.MiddleRight:
                        iy = Height / 2f - imageHeight / 2f;
                        break;
                    case ContentAlignment.TopCenter:
                    case ContentAlignment.TopLeft:
                    case ContentAlignment.TopRight:
                        iy = borderOffset;
                        break;
                }

                switch (imageAlign)
                {
                    case ContentAlignment.BottomCenter:
                    case ContentAlignment.MiddleCenter:
                    case ContentAlignment.TopCenter:
                        ix = Width / 2f - imageWidth / 2f;
                        break;
                    case ContentAlignment.BottomLeft:
                    case ContentAlignment.MiddleLeft:
                    case ContentAlignment.TopLeft:
                        ix = borderOffset;
                        break;
                    case ContentAlignment.BottomRight:
                    case ContentAlignment.MiddleRight:
                    case ContentAlignment.TopRight:
                        ix = Width - imageWidth - borderOffset;
                        break;
                }

                g.uwfDrawImage(imageToPaint, imageColorToPaint, ix, iy, imageWidth, imageHeight);
            }

            // Text.
            var textColor = ForeColor;
            if (enabled == false) textColor = ForeColor + Color.FromArgb(0, 128, 128, 128);
            var padding = Padding;
            g.uwfDrawString(Text, Font, textColor,
                    padding.Left,
                    padding.Top,
                    width - padding.Horizontal,
                    height - padding.Vertical, textAlign);
        }
    }
}
