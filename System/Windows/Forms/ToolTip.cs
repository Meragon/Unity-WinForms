namespace System.Windows.Forms
{
    using System.ComponentModel;
    using System.Drawing;

    public class ToolTip : Component
    {
        private static ToolTip instance;
        private static float alphaF;
        private static float alphaWait;
        private static float waitToShow;

        private Control control;
        private int initialDelay = 1000;
        private Point location;
        private string text;

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
                initialDelay = UnityEngine.Mathf.Clamp(value, 0, 32767);
            }
        }
        public Padding Padding { get; set; }
        public HorizontalAlignment TextAlign { get; set; }

        public void SetToolTip(Control control, string caption)
        {
            this.control = control;
            control.MouseEnter += control_MouseEnter;
            control.MouseLeave += control_MouseLeave;
            control.Disposed += control_Disposed;
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
            location = Control.MousePosition;
            location = new Point(location.X, location.Y + 24); // Cursor offset.
            this.text = text;
            instance = this;
            alphaF = 255;
            alphaWait = 1024;
            waitToShow = initialDelay;
        }

        internal static void OnPaint(PaintEventArgs e)
        {
            if (instance == null) return;

            if (waitToShow > 0)
            {
                waitToShow -= 1000 * swfHelper.GetDeltaTime();
                return;
            }
                
            var size = e.Graphics.MeasureString(instance.text, instance.Font) + new SizeF(16, 4);

            Point loc = instance.location;

            if (loc.X + size.Width + 2 > Screen.PrimaryScreen.WorkingArea.Width)
                loc = new Point(Screen.PrimaryScreen.WorkingArea.Width - (int)size.Width - 2, loc.Y);
            if (loc.Y + size.Height + 2 > Screen.PrimaryScreen.WorkingArea.Height)
                loc = new Point(loc.X, Screen.PrimaryScreen.WorkingArea.Height - (int)size.Height - 2);

            int shadowAlpha = 12 - 255 + (int)alphaF;
            var shadowColor = Color.FromArgb(shadowAlpha, 64, 64, 64);

            int stringHeight = (int)size.Height;

            e.Graphics.uwfFillRectangle(shadowColor, loc.X + 1, loc.Y + 1, size.Width + 3, stringHeight + 3);
            e.Graphics.uwfFillRectangle(shadowColor, loc.X + 2, loc.Y + 2, size.Width + 1, stringHeight + 1);
            e.Graphics.uwfFillRectangle(shadowColor, loc.X + 3, loc.Y + 3, size.Width - 1, stringHeight - 1);
                
            var borderColor = Color.FromArgb((int)alphaF, instance.BorderColor);
            var textColor = Color.FromArgb((int)alphaF, instance.ForeColor);
            var textFont = instance.Font;

            e.Graphics.uwfFillRectangle(Color.FromArgb((int)alphaF, instance.BackColor), loc.X, loc.Y, size.Width, stringHeight);
            e.Graphics.DrawRectangle(new Pen(borderColor), loc.X, loc.Y, size.Width, stringHeight);
            e.Graphics.uwfDrawString(
                instance.text,
                textFont,
                textColor,
                loc.X + instance.Padding.Left,
                loc.Y + instance.Padding.Top, 
                size.Width - instance.Padding.Bottom, 
                stringHeight - instance.Padding.Right, 
                instance.TextAlign);

            if (alphaWait > 0)
                alphaWait -= 1;
            else
            {
                if (alphaF > 0)
                    alphaF -= 1;
                else
                    instance = null;
            }
        }

        private void control_MouseLeave(object sender, EventArgs e)
        {
            if (instance == this)
                instance = null;
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
            if (instance != null && instance == this)
                instance = null;
        }
    }
}
