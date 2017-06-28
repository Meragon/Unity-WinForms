namespace System.Windows.Forms
{
    using System.Drawing;

    public class CheckBox : ButtonBase
    {
        private readonly Pen borderPen = new Pen(Color.Transparent);
        private CheckState checkState;

        public CheckBox()
        {
            BackColor = Color.White;
            ForeColor = Color.Black;
            Size = new Drawing.Size(128, 17);
            TextAlign = ContentAlignment.MiddleLeft;

            uwfBorderColor = Color.FromArgb(112, 112, 112);
            uwfBorderHoverColor = Color.FromArgb(51, 153, 255);
            uwfBorderDisableColor = Color.FromArgb(188, 188, 188);
            uwfDisableColor = Color.FromArgb(230, 230, 230);
            uwfHoverColor = Color.FromArgb(243, 249, 255);

            Click += CheckBox_Click;
        }

        public event EventHandler CheckedChanged = delegate { };

        public bool Checked
        {
            get { return checkState != CheckState.Unchecked; }
            set { CheckState = value ? CheckState.Checked : CheckState.Unchecked; }
        }
        public CheckState CheckState
        {
            get
            {
                return checkState;
            }
            set
            {
                if (checkState != value)
                {
                    checkState = value;
                    OnCheckedChanged(EventArgs.Empty);
                }
            }
        }

        protected virtual void OnCheckedChanged(EventArgs e)
        {
            CheckedChanged(this, e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            var backColor = uwfDisableColor;
            borderPen.Color = uwfBorderDisableColor;
            if (Enabled)
            {
                if (uwfHovered)
                {
                    backColor = uwfHoverColor;
                    borderPen.Color = uwfBorderHoverColor;
                }
                else
                {
                    backColor = BackColor;
                    borderPen.Color = uwfBorderColor;
                }
            }

            var checkRectX = Padding.Left;
            var checkRectY = Padding.Top + Height / 2 - 6;
            var checkRectWH = 12;

            g.uwfFillRectangle(backColor, checkRectX, checkRectY, checkRectWH, checkRectWH);
            g.DrawRectangle(borderPen, checkRectX, checkRectY, checkRectWH, checkRectWH);
            if (Checked)
                g.DrawImage(uwfAppOwner.Resources.Checked, checkRectX, checkRectY, checkRectWH, checkRectWH);

            g.uwfDrawString(Text, Font, ForeColor, checkRectX + checkRectWH + 4, Padding.Top + 0, Width - 20, Height, TextAlign);
        }

        private void CheckBox_Click(object sender, EventArgs e)
        {
            Checked = !Checked;
            CheckedChanged(this, new EventArgs());
        }
    }
}
