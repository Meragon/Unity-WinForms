namespace System.Windows.Forms
{
    using System.Drawing;

    public class CheckBox : ButtonBase
    {
        internal Color uwfBoxBackColor = Color.White;
        internal Color uwfBoxBackHoverColor = Color.FromArgb(243, 249, 255);
        internal Color uwfBoxBackDisableColor = Color.FromArgb(230, 230, 230);
        internal Color uwfMarkColor = Color.FromArgb(15, 15, 15);
        internal Color uwfMarkHoverColor = Color.FromArgb(15, 15, 15);

        private readonly Pen borderPen = new Pen(Color.Transparent);
        private CheckState checkState;
        private bool measureText;
        private SizeF textSize;

        public CheckBox()
        {
            BackColor = Color.Transparent;
            TextAlign = ContentAlignment.MiddleLeft;

            uwfBorderColor = Color.FromArgb(112, 112, 112);
            uwfBorderHoverColor = Color.FromArgb(51, 153, 255);
            uwfBorderDisableColor = Color.FromArgb(188, 188, 188);
            uwfHoverColor = Color.Transparent;

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

        protected override Size DefaultSize
        {
            get { return new Size(104, 24); }
        }

        protected virtual void OnCheckedChanged(EventArgs e)
        {
            CheckedChanged(this, e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;

            var boxBackColor = uwfBoxBackDisableColor;
            var markColor = uwfMarkColor;
            
            borderPen.Color = uwfBorderDisableColor;
            
            if (Enabled)
            {
                if (uwfHovered)
                {
                    boxBackColor = uwfBoxBackHoverColor;
                    borderPen.Color = uwfBorderHoverColor;
                    markColor = uwfMarkHoverColor;
                }
                else
                {
                    boxBackColor = uwfBoxBackColor;
                    borderPen.Color = uwfBorderColor;
                }
            }

            var checkRectX = Padding.Left;
            var checkRectY = Padding.Top + Height / 2 - 6;
            var checkRectWH = 12;

            g.uwfFillRectangle(boxBackColor, checkRectX, checkRectY, checkRectWH, checkRectWH);
            g.DrawRectangle(borderPen, checkRectX, checkRectY, checkRectWH, checkRectWH);
            if (Checked)
                g.uwfDrawImage(uwfAppOwner.Resources.Checked, markColor, checkRectX, checkRectY, checkRectWH, checkRectWH);

            g.uwfDrawString(Text, Font, ForeColor, checkRectX + checkRectWH + 4, Padding.Top + 0, Width - 20, Height, TextAlign);

            if (uwfCanDrawTabDots && uwfDrawTabDots && Focused && !string.IsNullOrEmpty(Text))
            {
                if (measureText)
                    textSize = g.MeasureString(Text, Font);
                    
                g.DrawRectangle( uwfTabPen, checkRectX + checkRectWH + 2, (Height - textSize.Height) / 2 + 2, textSize.Width + 4, textSize.Height - 4);
            }
        }
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            measureText = true;
        }

        private void CheckBox_Click(object sender, EventArgs e)
        {
            Checked = !Checked;
            CheckedChanged(this, new EventArgs());
        }
    }
}
