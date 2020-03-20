namespace System.Windows.Forms
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;

    public class ToolTip : Component
    {
        private const int   SCREEN_MIN_MARGIN = 2;
        private const int   SHADOW_STRENGTH   = 12;
        private const float ALPHA_TIME_SEC    = .5f;
        private const float ALPHA_SPEED       = 255f / ALPHA_TIME_SEC;
        
        internal static ToolTip instance;
        internal int alphaState; // 0: none; 1: 0 to 255; 2 : alphaWait; 3: 255 to 0
        
        private static readonly Queue<ToolTip> items = new Queue<ToolTip>();

        private readonly Pen borderPen = new Pen(Color.Black);
        private float   alphaF;
        private float   alphaWait; // seconds. Wait time before hide. 
        private Control control;
        private int     initialDelay = 1000;
        private Point   location;
        private string  text;
        private float   waitToShow;

        public ToolTip()
        {
            BackColor = SystemColors.Info;
            ForeColor = SystemColors.uwfInfoText;
            
            uwfBorderColor = Color.FromArgb(118, 118, 118);
            uwfFont = new Font("Arial", 12);
            uwfPadding = new Padding(8, 2, 8, 2);
        }

        public Color BackColor { get; set; }
        public Color ForeColor { get; set; }
        public int InitialDelay
        {
            get { return initialDelay; }
            set { initialDelay = MathHelper.Clamp(value, 0, 32767); }
        }

        internal Color uwfBorderColor { get; set; }
        internal Font uwfFont { get; set; }
        internal Padding uwfPadding { get; set; }
        
        public void SetToolTip(Control control, string caption)
        {
            this.control = control;
            control.MouseEnter -= control_MouseEnter;
            control.MouseEnter += control_MouseEnter;
            control.MouseLeave -= control_MouseLeave;
            control.MouseLeave += control_MouseLeave;
            control.Disposed -= control_Disposed;
            control.Disposed += control_Disposed;
            control.VisibleChanged -= control_Disposed;
            control.VisibleChanged += control_Disposed;
            text = caption;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="point">Useless right now (will use Control.MousePosition)</param>
        public void Show(string text, Point point)
        {
            if (string.IsNullOrEmpty(text))
                return;

            location = Control.MousePosition;
            location = new Point(location.X, location.Y + 24); // Cursor offset.
            this.text = text;
            alphaF = 0;
            alphaWait = 5;
            alphaState = 0;
            waitToShow = initialDelay;

            if (instance != this && items.Contains(this) == false)
                items.Enqueue(this);
        }

        internal static void OnPaint(PaintEventArgs e)
        {
            if (instance == null && items.Count > 0)
                instance = items.Dequeue();

            if (instance != null)
            {
                instance.PaintInternal(e);

                TryHideInstance();
            }
        }

        protected override void Dispose(bool release_all)
        {
            base.Dispose(release_all);

            if (control != null)
            {
                control.MouseEnter -= control_MouseEnter;
                control.MouseLeave -= control_MouseLeave;
                control.Disposed -= control_Disposed;
                control.VisibleChanged -= control_Disposed;
            }
        }

        private static void ForceHideInstance()
        {
            if (instance != null)
                instance.alphaState = 3;
        }
        private static void TryHideInstance()
        {
            if (instance != null && items.Count > 0 && instance.alphaState != 3)
                instance.alphaState = 3;
        }

        private void control_MouseLeave(object sender, EventArgs e)
        {
            ForceHideInstance();
        }
        private void control_MouseEnter(object sender, EventArgs e)
        {
            if (control != null && control.Visible && !control.Disposing)
            {
                var position = Control.MousePosition.Add(new Point(0, 18));

                Show(text, position);
            }
        }
        private void control_Disposed(object sender, EventArgs e)
        {
            ForceHideInstance();
        }
        
        private void PaintInternal(PaintEventArgs e)
        {
            if (waitToShow > 0)
            {
                waitToShow -= 1000 * swfHelper.GetDeltaTime();
                return;
            }
            
            UpdateAlpha();

            var g = e.Graphics;

            var screenSize = Screen.PrimaryScreen.WorkingArea;
            var textSize = g.MeasureString(text, uwfFont);
            
            var renderingW = (int) textSize.Width + uwfPadding.Horizontal;
            var renderingH = (int) textSize.Height + uwfPadding.Vertical;
            var renderingX = MathHelper.Clamp(location.X, 0, screenSize.Width - SCREEN_MIN_MARGIN - renderingW);
            var renderingY = MathHelper.Clamp(location.Y, 0, screenSize.Height - SCREEN_MIN_MARGIN - renderingH);

            var alpha = MathHelper.Clamp((int) alphaF, 1, 255);
            
            // Shadow.
            var shadowAlpha = SHADOW_STRENGTH / (255 / alpha);
            var shadowColor = Color.FromArgb(shadowAlpha, 64, 64, 64);
            
            g.uwfFillRectangle(shadowColor, renderingX + 1, renderingY + 1, renderingW + 3, renderingH + 3);
            g.uwfFillRectangle(shadowColor, renderingX + 2, renderingY + 2, renderingW + 1, renderingH + 1);
            g.uwfFillRectangle(shadowColor, renderingX + 3, renderingY + 3, renderingW - 1, renderingH - 1);

            // Background.
            var backColor = Color.FromArgb(alpha, BackColor);
            
            g.uwfFillRectangle(backColor, renderingX, renderingY, renderingW, renderingH);
            
            // Border.
            borderPen.Color = Color.FromArgb(alpha, uwfBorderColor);
            
            g.DrawRectangle(borderPen, renderingX, renderingY, renderingW, renderingH);
            
            // Text.
            var textColor = Color.FromArgb(alpha, ForeColor);
            var textFont = uwfFont;
            var textX = renderingX + uwfPadding.Left;
            var textY = renderingY + uwfPadding.Top;
            var textWidth = renderingW - uwfPadding.Horizontal;
            var textHeight = renderingH - uwfPadding.Vertical;
            
            g.uwfDrawString(text, textFont, textColor, textX, textY, textWidth, textHeight);
        }
        
        private void UpdateAlpha()
        {
            switch (alphaState)
            {
                case 0:
                    alphaState = 1;
                    break;
                
                case 1:
                    // 0 to 255.
                    if (alphaF < 255)
                        alphaF += swfHelper.GetDeltaTime() * ALPHA_SPEED;
                    else
                    {
                        alphaF = 255;
                        alphaState = 2;
                    }

                    break;
                
                case 2:
                    // Wait.
                    if (alphaWait > 0)
                        alphaWait -= swfHelper.GetDeltaTime();
                    else
                    {
                        alphaWait = 0;
                        alphaState = 3;
                    }

                    break;
                
                case 3:
                    // 255 to 0.
                    if (alphaF > 0)
                        alphaF -= swfHelper.GetDeltaTime() * ALPHA_SPEED;
                    else
                    {
                        alphaF = 0;
                        instance = null;
                    }

                    break;
            }
        }
    }
}