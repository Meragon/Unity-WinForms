namespace System.Windows.Forms
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;

    public class ToolTip : Component
    {
        internal static ToolTip instance;
        internal int alphaState; // 0: none; 1: 0 to 255; 2 : alphaWait; 3: 255 to 0

        private static readonly Queue<ToolTip> items = new Queue<ToolTip>();

        private float alphaF;
        private float alphaWait; // seconds. Wait time before hide. 
        private Control control;
        private int initialDelay = 1000;
        private Point location;
        private string text;
        private float waitToShow;

        public ToolTip()
        {
            BackColor = Color.White;
            BorderColor = Color.FromArgb(118, 118, 118);
            Font = new Font("Arial", 12);
            ForeColor = Color.FromArgb(118, 118, 118);
            Padding = new Padding(4);
            TextAlign = HorizontalAlignment.Center;
        }

        public Color BackColor { get; set; }
        public Color BorderColor { get; set; }
        public Font Font { get; set; }
        public Color ForeColor { get; set; }
        public int InitialDelay
        {
            get { return initialDelay; }
            set
            {
                initialDelay = MathHelper.Clamp(value, 0, 32767);
            }
        }
        public Padding Padding { get; set; }
        public HorizontalAlignment TextAlign { get; set; }

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
                instance.Paint(e);

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
        private void Paint(PaintEventArgs e)
        {
            if (waitToShow > 0)
            {
                waitToShow -= 1000 * swfHelper.GetDeltaTime();
                return;
            }

            var g = e.Graphics;

            var size = g.MeasureString(text, Font) + new SizeF(16, 4);

            Point loc = location;

            if (loc.X + size.Width + 2 > Screen.PrimaryScreen.WorkingArea.Width)
                loc = new Point(Screen.PrimaryScreen.WorkingArea.Width - (int)size.Width - 2, loc.Y);
            if (loc.Y + size.Height + 2 > Screen.PrimaryScreen.WorkingArea.Height)
                loc = new Point(loc.X, Screen.PrimaryScreen.WorkingArea.Height - (int)size.Height - 2);

            int shadowAlpha = 12 - 255 + (int)alphaF;
            var shadowColor = Color.FromArgb(shadowAlpha, 64, 64, 64);

            int stringHeight = (int)size.Height;

            var locX = loc.X;
            var locY = loc.Y;

            g.uwfFillRectangle(shadowColor, locX + 1, locY + 1, size.Width + 3, stringHeight + 3);
            g.uwfFillRectangle(shadowColor, locX + 2, locY + 2, size.Width + 1, stringHeight + 1);
            g.uwfFillRectangle(shadowColor, locX + 3, locY + 3, size.Width - 1, stringHeight - 1);

            var borderColor = Color.FromArgb((int)alphaF, BorderColor);
            var textColor = Color.FromArgb((int)alphaF, ForeColor);
            var textFont = Font;

            g.uwfFillRectangle(Color.FromArgb((int)alphaF, BackColor), locX, locY, size.Width, stringHeight);
            g.DrawRectangle(new Pen(borderColor), locX, locY, size.Width, stringHeight);
            g.uwfDrawString(
                text,
                textFont,
                textColor,
                locX + Padding.Left,
                locY + Padding.Top,
                size.Width - Padding.Bottom,
                stringHeight - Padding.Right,
                TextAlign);

            switch (alphaState)
            {
                case 0:
                    alphaState = 1;
                    break;
                case 1:
                    if (alphaF < 255)
                        alphaF += swfHelper.GetDeltaTime() * 510f; // .5f sec.
                    else
                    {
                        alphaF = 255;
                        alphaState = 2;
                    }
                    break;
                case 2:
                    if (alphaWait > 0)
                        alphaWait -= swfHelper.GetDeltaTime();
                    else
                    {
                        alphaWait = 0;
                        alphaState = 3;
                    }
                    break;
                case 3:
                    if (alphaF > 0)
                        alphaF -= swfHelper.GetDeltaTime() * 510f; // .5f sec.
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
