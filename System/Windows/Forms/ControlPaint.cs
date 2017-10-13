namespace System.Windows.Forms
{
    using System.Drawing;
    using System.Drawing.Drawing2D;

    public static class ControlPaint
    {
        // It's fine to have a single pen for all borders, because it's single-threading application.
        private static readonly Pen borderPen1 = new Pen(Color.Transparent);
        private static readonly Pen borderPen2 = new Pen(Color.Transparent);
        private static readonly Pen borderPen3 = new Pen(Color.Transparent);
        private static readonly Pen borderPen4 = new Pen(Color.Transparent);

        public static Color Dark(Color baseColor)
        {
            return new HLSColor(baseColor).Darker(0.5f);
        }
        public static Color Dark(Color baseColor, float percOfDarkDark)
        {
            return new HLSColor(baseColor).Darker(percOfDarkDark);
        }
        public static Color DarkDark(Color baseColor)
        {
            return new HLSColor(baseColor).Darker(1.0f);
        }
        public static void DrawBorder(Graphics graphics, Rectangle bounds, Color color, ButtonBorderStyle style)
        {
            switch (style)
            {
                case ButtonBorderStyle.None:
                    break;

                case ButtonBorderStyle.Dotted:
                case ButtonBorderStyle.Dashed:
                case ButtonBorderStyle.Solid:
                    DrawBorderSimple(graphics, bounds, color, style);
                    break;

                case ButtonBorderStyle.Inset:
                case ButtonBorderStyle.Outset:
                    DrawBorderComplex(graphics, bounds, color, style);
                    break;
            }
        }
        public static void DrawBorder3D(Graphics graphics, Rectangle rectangle)
        {
            DrawBorder3D(
                graphics,
                rectangle.X,
                rectangle.Y,
                rectangle.Width,
                rectangle.Height,
                Border3DStyle.Etched,
                Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom);
        }
        public static void DrawBorder3D(Graphics graphics, Rectangle rectangle, Border3DStyle style)
        {
            DrawBorder3D(
                graphics,
                rectangle.X,
                rectangle.Y,
                rectangle.Width,
                rectangle.Height,
                style,
                Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom);
        }
        public static void DrawBorder3D(Graphics graphics, Rectangle rectangle, Border3DStyle style, Border3DSide sides)
        {
            DrawBorder3D(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, style, sides);
        }
        public static void DrawBorder3D(Graphics graphics, int x, int y, int width, int height)
        {
            DrawBorder3D(
                graphics,
                x,
                y,
                width,
                height,
                Border3DStyle.Etched,
                Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom);
        }
        public static void DrawBorder3D(Graphics graphics, int x, int y, int width, int height, Border3DStyle style)
        {
            DrawBorder3D(graphics, x, y, width, height, style, Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom);
        }
        public static void DrawBorder3D(Graphics graphics, int x, int y, int width, int height, Border3DStyle style, Border3DSide sides)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");

            if ((style & Border3DStyle.Adjust) == Border3DStyle.Adjust)
            {
                var sz = SystemInformation.Border3DSize;
                x -= sz.Width;
                y -= sz.Height;
                width += sz.Width;
                height += sz.Height;
                style &= ~Border3DStyle.Adjust;
            }

            switch (style)
            {
                case Border3DStyle.Bump:
                    DrawBorderStyleBump(graphics, x, y, width, height, sides);
                    break;
                case Border3DStyle.Etched:
                    DrawBorderStyleEtched(graphics, x, y, width, height, sides);
                    break;
                case Border3DStyle.Flat:
                    DrawBorderStyleFlat(graphics, x, y, width, height, sides);
                    break;
                case Border3DStyle.Raised:
                    DrawBorderStyleRaised(graphics, x, y, width, height, sides, true, true);
                    break;
                case Border3DStyle.RaisedInner:
                    DrawBorderStyleRaised(graphics, x, y, width, height, sides, true, false);
                    break;
                case Border3DStyle.RaisedOuter:
                    DrawBorderStyleRaised(graphics, x, y, width, height, sides, false, true);
                    break;
                case Border3DStyle.Sunken:
                    DrawBorderStyleSunken(graphics, x, y, width, height, sides, true, true);
                    break;
                case Border3DStyle.SunkenInner:
                    DrawBorderStyleSunken(graphics, x, y, width, height, sides, true, false);
                    break;
                case Border3DStyle.SunkenOuter:
                    DrawBorderStyleSunken(graphics, x, y, width, height, sides, false, true);
                    break;
            }
        }

        public static Color Light(Color baseColor)
        {
            return new HLSColor(baseColor).Lighter(0.5f);
        }
        public static Color Light(Color baseColor, float percOfLightLight)
        {
            return new HLSColor(baseColor).Lighter(percOfLightLight);
        }
        public static Color LightLight(Color baseColor)
        {
            return new HLSColor(baseColor).Lighter(1.0f);
        }

        internal static Rectangle CalculateBackgroundImageRectangle(Rectangle bounds, Image backgroundImage, ImageLayout imageLayout)
        {
            var result = bounds;
            if (backgroundImage == null)
                return result;

            switch (imageLayout)
            {
                case ImageLayout.Stretch:
                    result.Size = bounds.Size;
                    break;

                case ImageLayout.None:
                    result.Size = backgroundImage.Size;
                    break;

                case ImageLayout.Center:
                    result.Size = backgroundImage.Size;
                    var szCtl = bounds.Size;

                    if (szCtl.Width > result.Width)
                        result.X = (szCtl.Width - result.Width) / 2;
                    if (szCtl.Height > result.Height)
                        result.Y = (szCtl.Height - result.Height) / 2;
                    break;

                case ImageLayout.Zoom:
                    var imageSize = backgroundImage.Size;
                    float xRatio = (float)bounds.Width / imageSize.Width;
                    float yRatio = (float)bounds.Height / imageSize.Height;
                    if (xRatio < yRatio)
                    {
                        result.Width = bounds.Width;
                        result.Height = (int)(imageSize.Height * xRatio + .5f);
                        if (bounds.Y >= 0)
                            result.Y = (bounds.Height - result.Height) / 2;
                    }
                    else
                    {
                        result.Height = bounds.Height;
                        result.Width = (int)(imageSize.Width * yRatio + .5f);
                        if (bounds.X >= 0)
                            result.X = (bounds.Width - result.Width) / 2;
                    }
                    break;
            }
            return result;
        }
        internal static void DrawBackgroundImage(Graphics g, Image backgroundImage, Color backColor, ImageLayout backgroundImageLayout, Rectangle bounds, Rectangle clipRect)
        {
            DrawBackgroundImage(g, backgroundImage, backColor, backgroundImageLayout, bounds, clipRect, Point.Empty, RightToLeft.No);
        }
        internal static void DrawBackgroundImage(Graphics g, Image backgroundImage, Color backColor, ImageLayout backgroundImageLayout, Rectangle bounds, Rectangle clipRect, Point scrollOffset)
        {
            DrawBackgroundImage(g, backgroundImage, backColor, backgroundImageLayout, bounds, clipRect, scrollOffset, RightToLeft.No);
        }
        internal static void DrawBackgroundImage(Graphics g, Image backgroundImage, Color backColor, ImageLayout backgroundImageLayout, Rectangle bounds, Rectangle clipRect, Point scrollOffset, RightToLeft rightToLeft)
        {
            if (g == null)
                throw new ArgumentNullException("g");

            if (backgroundImageLayout == ImageLayout.Tile)
            {
                var imageWidth = backgroundImage.Width;
                var imageHeight = backgroundImage.Height;

                // TODO: better to use shader.
                for (int y = 0; y < bounds.Height; y += imageHeight)
                    for (int x = 0; x < bounds.Width; x += imageWidth)
                        g.DrawImage(backgroundImage, x, y, imageWidth, imageHeight);
            }
            else
            {
                var imageRectangle = CalculateBackgroundImageRectangle(bounds, backgroundImage, backgroundImageLayout);
                if (rightToLeft == RightToLeft.Yes && backgroundImageLayout == ImageLayout.None)
                    imageRectangle.X += clipRect.Width - imageRectangle.Width;

                g.uwfFillRectangle(backColor, clipRect);

                if (clipRect.Contains(imageRectangle) == false)
                {
                    switch (backgroundImageLayout)
                    {
                        case ImageLayout.Stretch:
                        case ImageLayout.Zoom:
                            imageRectangle.Intersect(clipRect);
                            g.DrawImage(backgroundImage, imageRectangle);
                            break;
                        default:
                            {
                                imageRectangle.Offset(clipRect.Location);
                                var imageRect = imageRectangle;
                                imageRect.Intersect(clipRect);
                                g.DrawImage(backgroundImage, imageRect);
                            }
                            break;
                    }
                }
                else
                    g.DrawImage(backgroundImage, imageRectangle);
            }
        }
        internal static bool IsDarker(Color c1, Color c2)
        {
            var hc1 = new HLSColor(c1);
            var hc2 = new HLSColor(c2);
            return hc1.Luminosity < hc2.Luminosity;
        }
        internal static void PrintBorder(Graphics graphics, Rectangle bounds, BorderStyle style, Border3DStyle b3dStyle)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");

            switch (style)
            {
                case BorderStyle.FixedSingle:
                    DrawBorder(graphics, bounds, SystemColors.WindowFrame, ButtonBorderStyle.Solid);
                    break;
                case BorderStyle.Fixed3D:
                    DrawBorder3D(graphics, bounds, b3dStyle);
                    break;
                case BorderStyle.None:
                    break;
            }
        }

        private static DashStyle BorderStyleToDashStyle(ButtonBorderStyle borderStyle)
        {
            switch (borderStyle)
            {
                case ButtonBorderStyle.Dotted: return DashStyle.Dot;
                case ButtonBorderStyle.Dashed: return DashStyle.Dash;
                case ButtonBorderStyle.Solid: return DashStyle.Solid;
                default:
                    return DashStyle.Solid;
            }
        }
        private static void DrawBorderComplex(Graphics graphics, Rectangle bounds, Color color, ButtonBorderStyle style)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");

            borderPen1.DashStyle = DashStyle.Solid;

            if (style == ButtonBorderStyle.Inset)
            { // button being pushed
                var hls = new HLSColor(color);

                // top + left
                borderPen1.Color = hls.Darker(1.0f);
                graphics.DrawLine(borderPen1, bounds.X, bounds.Y, bounds.X + bounds.Width - 1, bounds.Y);
                graphics.DrawLine(borderPen1, bounds.X, bounds.Y, bounds.X, bounds.Y + bounds.Height - 1);

                // bottom + right
                borderPen1.Color = hls.Lighter(1.0f);
                graphics.DrawLine(borderPen1, bounds.X, bounds.Y + bounds.Height - 1, bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 1);
                graphics.DrawLine(borderPen1, bounds.X + bounds.Width - 1, bounds.Y, bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 1);

                // Top + left inset
                borderPen1.Color = hls.Lighter(0.5f);
                graphics.DrawLine(borderPen1, bounds.X + 1, bounds.Y + 1, bounds.X + bounds.Width - 2, bounds.Y + 1);
                graphics.DrawLine(borderPen1, bounds.X + 1, bounds.Y + 1, bounds.X + 1, bounds.Y + bounds.Height - 2);

                // bottom + right inset
                if (color == SystemColors.Control)
                {
                    borderPen1.Color = SystemColors.ControlLight;
                    graphics.DrawLine(borderPen1, bounds.X + 1, bounds.Y + bounds.Height - 2, bounds.X + bounds.Width - 2, bounds.Y + bounds.Height - 2);
                    graphics.DrawLine(borderPen1, bounds.X + bounds.Width - 2, bounds.Y + 1, bounds.X + bounds.Width - 2, bounds.Y + bounds.Height - 2);
                }
            }
            else
            {
                // Standard button
                bool stockColor = color == SystemColors.Control;
                var hls = new HLSColor(color);

                // top + left
                borderPen1.Color = stockColor ? SystemColors.ControlLightLight : hls.Lighter(1.0f);
                graphics.DrawLine(borderPen1, bounds.X, bounds.Y, bounds.X + bounds.Width - 1, bounds.Y);
                graphics.DrawLine(borderPen1, bounds.X, bounds.Y, bounds.X, bounds.Y + bounds.Height - 1);

                // bottom + right
                borderPen1.Color = stockColor ? SystemColors.ControlDarkDark : hls.Darker(1.0f);
                graphics.DrawLine(borderPen1, bounds.X, bounds.Y + bounds.Height - 1, bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 1);
                graphics.DrawLine(borderPen1, bounds.X + bounds.Width - 1, bounds.Y, bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 1);

                // top + left inset
                if (stockColor)
                    borderPen1.Color = SystemInformation.HighContrast ? SystemColors.ControlLight : SystemColors.Control;
                else
                    borderPen1.Color = color;

                graphics.DrawLine(borderPen1, bounds.X + 1, bounds.Y + 1, bounds.X + bounds.Width - 2, bounds.Y + 1);
                graphics.DrawLine(borderPen1, bounds.X + 1, bounds.Y + 1, bounds.X + 1, bounds.Y + bounds.Height - 2);

                // Bottom + right inset                        
                borderPen1.Color = stockColor ? SystemColors.ControlDark : hls.Darker(0.5f);
                graphics.DrawLine(borderPen1, bounds.X + 1, bounds.Y + bounds.Height - 2, bounds.X + bounds.Width - 2, bounds.Y + bounds.Height - 2);
                graphics.DrawLine(borderPen1, bounds.X + bounds.Width - 2, bounds.Y + 1, bounds.X + bounds.Width - 2, bounds.Y + bounds.Height - 2);
            }
        }
        private static void DrawBorderSimple(Graphics graphics, Rectangle bounds, Color color, ButtonBorderStyle style)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");

            borderPen1.Color = color;
            borderPen1.DashStyle = style != ButtonBorderStyle.Solid ? BorderStyleToDashStyle(style) : DashStyle.Solid;

            graphics.DrawRectangle(borderPen1, bounds.X, bounds.Y, bounds.Width, bounds.Height);
        }
        private static void DrawBorderStyleBump(Graphics graphics, int x, int y, int width, int height, Border3DSide sides)
        {
            var left = (sides & Border3DSide.Left) != 0;
            var top = (sides & Border3DSide.Top) != 0;
            var right = (sides & Border3DSide.Right) != 0;
            var bottom = (sides & Border3DSide.Bottom) != 0;

            borderPen1.DashStyle = DashStyle.Solid;
            borderPen2.DashStyle = DashStyle.Solid;
            borderPen1.Color = SystemColors.ControlLight;
            borderPen2.Color = SystemColors.ControlDarkDark;

            if (left)
            {
                graphics.DrawLine(borderPen1, x, y, x, y + height);
                graphics.DrawLine(borderPen2, x + 1, y, x + 1, y + height);
            }
            if (top)
            {
                var top2x1 = left ? x + 1 : x;
                var top2x2 = right ? x + width - 2 : x + width;
                graphics.DrawLine(borderPen1, x, y, x + width, y);
                graphics.DrawLine(borderPen2, top2x1, y + 1, top2x2, y + 1);
            }
            if (right)
            {
                var right1y2 = bottom ? y + height - 1 : y + height;
                graphics.DrawLine(borderPen1, x + width - 2, y, x + width - 2, right1y2);
                graphics.DrawLine(borderPen2, x + width - 1, y, x + width - 1, y + height);
            }
            if (bottom)
            {
                var bottom1x2 = right ? x + width - 1 : x + width;
                graphics.DrawLine(borderPen1, x, y + height - 2, bottom1x2, y + height - 2);
                graphics.DrawLine(borderPen2, x, y + height - 1, x + width, y + height - 1);
            }
        }
        private static void DrawBorderStyleEtched(Graphics graphics, int x, int y, int width, int height, Border3DSide sides)
        {
            var left = (sides & Border3DSide.Left) != 0;
            var top = (sides & Border3DSide.Top) != 0;
            var right = (sides & Border3DSide.Right) != 0;
            var bottom = (sides & Border3DSide.Bottom) != 0;

            borderPen1.DashStyle = DashStyle.Solid;
            borderPen2.DashStyle = DashStyle.Solid;
            borderPen1.Color = SystemColors.ControlDark;
            borderPen2.Color = Color.White;

            if (left)
            {
                graphics.DrawLine(borderPen1, x, y, x, y + height);
                graphics.DrawLine(borderPen2, x + 1, y, x + 1, y + height);
            }
            if (top)
            {
                var top2x1 = left ? x + 1 : x;
                var top2x2 = right ? x + width - 1 : x + width;
                graphics.DrawLine(borderPen1, x, y, x + width, y);
                graphics.DrawLine(borderPen2, top2x1, y + 1, top2x2, y + 1);
            }
            if (right)
                graphics.DrawLine(borderPen1, x + width - 1, y, x + width - 1, y + height);
            if (bottom)
                graphics.DrawLine(borderPen1, x, y + height - 1, y + width, y + height - 1);
        }
        private static void DrawBorderStyleFlat(Graphics graphics, int x, int y, int width, int height, Border3DSide sides)
        {
            var left = (sides & Border3DSide.Left) != 0;
            var top = (sides & Border3DSide.Top) != 0;
            var right = (sides & Border3DSide.Right) != 0;
            var bottom = (sides & Border3DSide.Bottom) != 0;

            borderPen1.DashStyle = DashStyle.Solid;
            borderPen2.DashStyle = DashStyle.Solid;
            borderPen1.Color = SystemColors.ControlDark;
            borderPen2.Color = SystemColors.Control;

            if (left)
            {
                graphics.DrawLine(borderPen1, x, y, x, y + height);
                graphics.DrawLine(borderPen2, x + 1, y, x + 1, y + height);
            }
            if (top)
            {
                var top2x1 = left ? x + 1 : x;
                var top2x2 = right ? x + width - 2 : x + width;
                graphics.DrawLine(borderPen1, x, y, x + width, y);
                graphics.DrawLine(borderPen2, top2x1, y + 1, top2x2, y + 1);
            }
            if (right)
            {
                var right1y1 = top ? y + 1 : y;
                var right1y2 = bottom ? y + height - 1 : y + height;
                graphics.DrawLine(borderPen2, x + width - 2, right1y1, x + width - 2, right1y2);
                graphics.DrawLine(borderPen1, x + width - 1, y, x + width - 1, y + height);
            }
            if (bottom)
            {
                var bottom1x1 = left ? x + 1 : x;
                var bottom1x2 = right ? x + width - 1 : x + width;
                graphics.DrawLine(borderPen2, bottom1x1, y + height - 2, bottom1x2, y + height - 2);
                graphics.DrawLine(borderPen1, x, y + height - 1, x + width, y + height - 1);
            }
        }
        private static void DrawBorderStyleRaised(Graphics graphics, int x, int y, int width, int height, Border3DSide sides, bool inner, bool outer)
        {
            var left = (sides & Border3DSide.Left) != 0;
            var top = (sides & Border3DSide.Top) != 0;
            var right = (sides & Border3DSide.Right) != 0;
            var bottom = (sides & Border3DSide.Bottom) != 0;

            borderPen1.DashStyle = DashStyle.Solid;
            borderPen2.DashStyle = DashStyle.Solid;
            borderPen3.DashStyle = DashStyle.Solid;
            borderPen4.DashStyle = DashStyle.Solid;
            borderPen1.Color = outer ? SystemColors.ControlLight : Color.Transparent;
            borderPen2.Color = Color.White;
            borderPen3.Color = outer ? SystemColors.ControlDarkDark : Color.Transparent;
            borderPen4.Color = SystemColors.ControlDark;

            if (left)
            {
                graphics.DrawLine(borderPen1, x, y, x, y + height);
                if (inner)
                    graphics.DrawLine(borderPen2, x + 1, y, x + 1, y + height);
            }
            if (top)
            {
                var top1x2 = right ? x + width - 1 : x + width;
                var top2x1 = left ? x + 1 : x;
                var top2x2 = right ? x + width - 2 : x + width;
                graphics.DrawLine(borderPen1, x, y, top1x2, y);
                if (inner)
                    graphics.DrawLine(borderPen2, top2x1, y + 1, top2x2, y + 1);
            }
            if (right)
            {
                var right2y1 = top ? y + 1 : y;
                var right2y2 = bottom ? y + height - 1 : y + height;
                graphics.DrawLine(borderPen3, x + width - 1, y, x + width - 1, y + height);
                if (inner)
                    graphics.DrawLine(borderPen4, x + width - 2, right2y1, x + width - 2, right2y2);
            }
            if (bottom)
            {
                var bottom2x1 = left ? x + 1 : x;
                var bottom2x2 = right ? x + width - 2 : x + width;
                graphics.DrawLine(borderPen3, x, y + height - 1, x + width, y + height - 1);
                if (inner)
                    graphics.DrawLine(borderPen4, bottom2x1, y + height - 2, bottom2x2, y + height - 2);
            }
        }
        private static void DrawBorderStyleSunken(Graphics graphics, int x, int y, int width, int height, Border3DSide sides, bool inner, bool outer)
        {
            var left = (sides & Border3DSide.Left) != 0;
            var top = (sides & Border3DSide.Top) != 0;
            var right = (sides & Border3DSide.Right) != 0;
            var bottom = (sides & Border3DSide.Bottom) != 0;

            borderPen1.DashStyle = DashStyle.Solid;
            borderPen2.DashStyle = DashStyle.Solid;
            borderPen3.DashStyle = DashStyle.Solid;
            borderPen4.DashStyle = DashStyle.Solid;
            borderPen1.Color = outer ? SystemColors.ControlDark : Color.Transparent;
            borderPen2.Color = SystemColors.ControlDarkDark;
            borderPen3.Color = outer ? Color.White : Color.Transparent;
            borderPen4.Color = SystemColors.ControlLight;

            if (left)
            {
                graphics.DrawLine(borderPen1, x, y, x, y + height);
                if (inner)
                    graphics.DrawLine(borderPen2, x + 1, y, x + 1, y + height);
            }
            if (top)
            {
                var top1x2 = right ? x + width - 1 : x + width;
                var top2x1 = left ? x + 1 : x;
                var top2x2 = right ? x + width - 2 : x + width;
                graphics.DrawLine(borderPen1, x, y, top1x2, y);
                if (inner)
                    graphics.DrawLine(borderPen2, top2x1, y + 1, top2x2, y + 1);
            }
            if (right)
            {
                var right2y1 = top ? y + 1 : y;
                var right2y2 = bottom ? y + height - 1 : y + height;
                graphics.DrawLine(borderPen3, x + width - 1, y, x + width - 1, y + height);
                if (inner)
                    graphics.DrawLine(borderPen4, x + width - 2, right2y1, x + width - 2, right2y2);
            }
            if (bottom)
            {
                var bottom2x1 = left ? x + 1 : x;
                var bottom2x2 = right ? x + width - 2 : x + width;
                graphics.DrawLine(borderPen3, x, y + height - 1, x + width, y + height - 1);
                if (inner)
                    graphics.DrawLine(borderPen4, bottom2x1, y + height - 2, bottom2x2, y + height - 2);
            }
        }

        private struct HLSColor
        {
            private const int ShadowAdj = -333;
            private const int HilightAdj = 500;

            private const int Range = 240;
            private const int HLSMax = Range;
            private const int RGBMax = 255;
            private const int Undefined = HLSMax * 2 / 3;

            private readonly int hue;
            private readonly int saturation;
            private readonly int luminosity;

            private readonly bool isSystemColors_Control;

            public HLSColor(Color color)
            {
                isSystemColors_Control = color == SystemColors.Control;

                int r = color.R;
                int g = color.G;
                int b = color.B;

                var max = Math.Max(Math.Max(r, g), b);
                var min = Math.Min(Math.Min(r, g), b);
                var sum = max + min;

                luminosity = (sum * HLSMax + RGBMax) / (2 * RGBMax);

                var dif = max - min;
                if (dif == 0)
                {
                    saturation = 0;
                    hue = Undefined;
                }
                else
                {
                    if (luminosity <= HLSMax / 2)
                        saturation = (dif * HLSMax + sum / 2) / sum;
                    else
                        saturation = (dif * HLSMax + (2 * RGBMax - sum) / 2) / (2 * RGBMax - sum);

                    var rdelta = ((max - r) * (HLSMax / 6) + dif / 2) / dif;
                    var gdelta = ((max - g) * (HLSMax / 6) + dif / 2) / dif;
                    var bdelta = ((max - b) * (HLSMax / 6) + dif / 2) / dif;

                    if (r == max)
                        hue = bdelta - gdelta;
                    else if (g == max)
                        hue = HLSMax / 3 + rdelta - bdelta;
                    else
                        hue = 2 * HLSMax / 3 + gdelta - rdelta;

                    if (hue < 0)
                        hue += HLSMax;
                    if (hue > HLSMax)
                        hue -= HLSMax;
                }
            }

            public int Luminosity { get { return luminosity; } }

            public static bool operator ==(HLSColor a, HLSColor b)
            {
                return a.Equals(b);
            }
            public static bool operator !=(HLSColor a, HLSColor b)
            {
                return !a.Equals(b);
            }

            public override bool Equals(object o)
            {
                if (o is HLSColor == false)
                    return false;

                var c = (HLSColor)o;
                return hue == c.hue &&
                       saturation == c.saturation &&
                       luminosity == c.luminosity &&
                       isSystemColors_Control == c.isSystemColors_Control;
            }
            public override int GetHashCode()
            {
                return hue << 6 | saturation << 2 | luminosity;
            }
            public Color Darker(float percDarker)
            {
                if (isSystemColors_Control)
                {
                    if (percDarker == 0.0f)
                        return SystemColors.ControlDark;

                    if (percDarker == 1.0f)
                        return SystemColors.ControlDarkDark;

                    var dark = SystemColors.ControlDark;
                    var darkDark = SystemColors.ControlDarkDark;

                    int dr = dark.R - darkDark.R;
                    int dg = dark.G - darkDark.G;
                    int db = dark.B - darkDark.B;

                    return Color.FromArgb(
                        (byte)(dark.R - (byte)(dr * percDarker)),
                        (byte)(dark.G - (byte)(dg * percDarker)),
                        (byte)(dark.B - (byte)(db * percDarker)));
                }

                int oneLum = 0;
                int zeroLum = NewLuma(ShadowAdj, true);

                return ColorFromHLS(hue, zeroLum - (int)((zeroLum - oneLum) * percDarker), saturation);
            }
            public Color Lighter(float percLighter)
            {
                if (isSystemColors_Control)
                {
                    if (percLighter == 0.0f)
                        return SystemColors.ControlLight;

                    if (percLighter == 1.0f)
                        return SystemColors.ControlLightLight;

                    var light = SystemColors.ControlLight;
                    var lightLight = SystemColors.ControlLightLight;

                    int dr = light.R - lightLight.R;
                    int dg = light.G - lightLight.G;
                    int db = light.B - lightLight.B;

                    return Color.FromArgb(
                        (byte)(light.R - (byte)(dr * percLighter)),
                        (byte)(light.G - (byte)(dg * percLighter)),
                        (byte)(light.B - (byte)(db * percLighter)));
                }

                int zeroLum = luminosity;
                int oneLum = NewLuma(HilightAdj, true);

                return ColorFromHLS(hue, zeroLum + (int)((oneLum - zeroLum) * percLighter), saturation);
            }

            private static Color ColorFromHLS(int hue, int luminosity, int saturation)
            {
                byte r, g, b;

                if (saturation == 0)
                    r = g = b = (byte)(luminosity * RGBMax / HLSMax);
                else
                {
                    int magic2;
                    if (luminosity <= HLSMax / 2)
                        magic2 = (luminosity * (HLSMax + saturation) + HLSMax / 2) / HLSMax;
                    else
                        magic2 = luminosity + saturation - (luminosity * saturation + HLSMax / 2) / HLSMax;
                    var magic1 = 2 * luminosity - magic2;

                    r = (byte)((HueToRGB(magic1, magic2, hue + HLSMax / 3) * RGBMax + HLSMax / 2) / HLSMax);
                    g = (byte)((HueToRGB(magic1, magic2, hue) * RGBMax + HLSMax / 2) / HLSMax);
                    b = (byte)((HueToRGB(magic1, magic2, hue - HLSMax / 3) * RGBMax + HLSMax / 2) / HLSMax);
                }

                return Color.FromArgb(r, g, b);
            }
            private static int HueToRGB(int n1, int n2, int hue)
            {
                if (hue < 0)
                    hue += HLSMax;

                if (hue > HLSMax)
                    hue -= HLSMax;

                if (hue < HLSMax / 6)
                    return n1 + ((n2 - n1) * hue + HLSMax / 12) / (HLSMax / 6);
                if (hue < HLSMax / 2)
                    return n2;
                if (hue < HLSMax * 2 / 3)
                    return n1 + ((n2 - n1) * (HLSMax * 2 / 3 - hue) + HLSMax / 12) / (HLSMax / 6);

                return n1;
            }
            private static int NewLuma(int luminosity, int n, bool scale)
            {
                if (n == 0)
                    return luminosity;

                if (scale)
                {
                    if (n > 0)
                        return (int)((luminosity * (1000 - n) + (Range + 1L) * n) / 1000);

                    return luminosity * (n + 1000) / 1000;
                }

                int newLum = luminosity;
                newLum += (int)((long)n * Range / 1000);

                if (newLum < 0)
                    newLum = 0;
                if (newLum > HLSMax)
                    newLum = HLSMax;

                return newLum;
            }

            private int NewLuma(int n, bool scale)
            {
                return NewLuma(luminosity, n, scale);
            }
        }
    }
}
