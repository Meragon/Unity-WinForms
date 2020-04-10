namespace System.Windows.Forms
{
    using System.Drawing;

    public class ButtonBase : Control
    {
        // Back colors.
        internal Color uwfDisabledColor = SystemColors.Control;
        internal Color uwfHoverColor = Color.FromArgb(223, 238, 252);
        
        // Border colors.
        internal Color uwfBorderColor = SystemColors.ActiveBorder;
        internal Color uwfBorderDisableColor = Color.FromArgb(217, 217, 217);
        internal Color uwfBorderHoverColor = Color.FromArgb(126, 180, 234);
        internal Color uwfBorderSelectColor = SystemColors.Highlight;
        
        // Fore colors.
        internal Color uwfForeDisabledColor = SystemColors.InactiveCaption;
        
        // Image colors.
        internal Bitmap uwfImageHover;
        internal Color uwfImageColor = Color.White;
        internal Color uwfImageDisabledColor = Color.White;
        internal Color uwfImageHoverColor = Color.White;

        private const float IMAGE_BORDER_OFFSET = 2f;

        private readonly Pen borderPen = new Pen(Color.Transparent);

        private Bitmap imageFromList;
        private int imageIndex;
        private ImageList imageList;
        
        public ButtonBase()
        {
            BackgroundImageLayout = ImageLayout.Center;
            FlatStyle = FlatStyle.Standard;
            ImageAlign = ContentAlignment.MiddleCenter;
            TextAlign = ContentAlignment.MiddleCenter;
        }

        public FlatStyle FlatStyle { get; set; }
        public Bitmap Image { get; set; }
        public ContentAlignment ImageAlign { get; set; }
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
        public virtual ContentAlignment TextAlign { get; set; }

        protected override Size DefaultSize
        {
            get { return new Size(75, 23); }
        }

        internal override void RaiseOnMouseClick(MouseEventArgs e)
        {
            var iButtonControl = this as IButtonControl;
            if (iButtonControl != null)
            {
                OnMouseClick(e);
                iButtonControl.PerformClick(); // Originally in WndProc.
            }
            else
                base.RaiseOnMouseClick(e);
        }

        protected internal virtual void DrawTabDots(PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(uwfTabPen, 2, 2, Width - 4, Height - 4);
        }
        
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Return)
                OnClick(EventArgs.Empty);

            base.OnKeyUp(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;

            // Image.
            DrawImage(g); 

            // Text.
            var text = Text;
            var textX = Padding.Left;
            var textY = Padding.Top;
            var textW = Width - Padding.Horizontal;
            var textH = Height - Padding.Vertical;
            var textColor = ForeColor;
            var textFont = Font;

            if (!Enabled)
                textColor = uwfForeDisabledColor;
            
            g.uwfDrawString(text, textFont, textColor, textX, textY, textW, textH, TextAlign);

            // Border.
            var borderColorToDraw = Color.Transparent;

            // Get colors from style.
            switch (FlatStyle)
            {
                case FlatStyle.Flat:
                    borderColorToDraw = Color.Black;
                    break;
                case FlatStyle.Popup:
                    borderColorToDraw = SystemColors.ControlDark;
                    break;
                case FlatStyle.Standard:
                case FlatStyle.System:

                    // Border.
                    if (!Enabled)
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

            g.DrawRectangle(borderPen, 0, 0, Width, Height);
            
            if (uwfCanDrawTabDots && uwfDrawTabDots && Focused)
                DrawTabDots(e);
        }
        protected override void OnPaintBackground(PaintEventArgs args)
        {
            var backColor = BackColor;
            
            if (!Enabled)
                backColor = uwfDisabledColor;
            else if (hovered)
                backColor = uwfHoverColor;

            PaintBackground(args, ClientRectangle, backColor, Point.Empty);
        }

        private void DrawImage(Graphics g)
        {
            // Image.
            var imageToPaint = Image;
            if (imageFromList != null)
                imageToPaint = imageFromList;

            if (imageToPaint == null) return;

            var imageWidth = imageToPaint.Width;
            var imageHeight = imageToPaint.Height;
            var imageColorToPaint = uwfImageColor;
            if (Enabled == false)
                imageColorToPaint = uwfImageDisabledColor;
            else if (uwfHovered)
            {
                if (uwfImageHover != null)
                    imageToPaint = uwfImageHover;
                imageColorToPaint = uwfImageHoverColor;
            }

            float ix = 0, iy = 0;

            // Adjust Y.
            switch (ImageAlign)
            {
                case ContentAlignment.BottomCenter:
                case ContentAlignment.BottomLeft:
                case ContentAlignment.BottomRight:
                    iy = Height - imageHeight - IMAGE_BORDER_OFFSET;
                    break;
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.MiddleRight:
                    iy = Height / 2f - imageHeight / 2f;
                    break;
                case ContentAlignment.TopCenter:
                case ContentAlignment.TopLeft:
                case ContentAlignment.TopRight:
                    iy = IMAGE_BORDER_OFFSET;
                    break;
            }

            // Adjust X.
            switch (ImageAlign)
            {
                case ContentAlignment.BottomCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.TopCenter:
                    ix = Width / 2f - imageWidth / 2f;
                    break;
                case ContentAlignment.BottomLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.TopLeft:
                    ix = IMAGE_BORDER_OFFSET;
                    break;
                case ContentAlignment.BottomRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.TopRight:
                    ix = Width - imageWidth - IMAGE_BORDER_OFFSET;
                    break;
            }

            g.uwfDrawImage(imageToPaint, imageColorToPaint, ix, iy, imageWidth, imageHeight);
        }
    }
}
