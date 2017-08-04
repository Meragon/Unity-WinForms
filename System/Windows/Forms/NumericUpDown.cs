namespace System.Windows.Forms
{
    using System.Drawing;

    public class NumericUpDown : Control
    {
        internal Pen borderPen = new Pen(Color.Transparent);

        protected decimal value;
        protected string valueText = "0";

        private decimal minimum;
        private decimal maximum;

        public NumericUpDown() : this(true)
        {

        }
        internal NumericUpDown(bool initButtons)
        {
            BackColor = Color.FromArgb(250, 250, 250);
            uwfBorderColor = Color.FromArgb(175, 175, 175);
            uwfDisabledColor = Color.FromArgb(240, 240, 240);
            Increment = 1;
            Maximum = 100;
            Minimum = 0;
            Padding = new Padding(4, 0, 4, 0);
            TextAlign = HorizontalAlignment.Left;

            if (initButtons)
            {
                uwfButtonIncrease = new RepeatButton();
                uwfButtonIncrease.Anchor = AnchorStyles.Right | AnchorStyles.Top;
                uwfButtonIncrease.Location = new Point(Width - 16, Height / 2 - 8);
                uwfButtonIncrease.Size = new Size(14, 8);
                uwfButtonIncrease.Name = "_numericButtonIncrease";
                uwfButtonIncrease.BackColor = Color.FromArgb(236, 236, 236);
                uwfButtonIncrease.uwfBorderColor = Color.FromArgb(172, 172, 172);
                uwfButtonIncrease.uwfHoverColor = Color.FromArgb(228, 241, 252);
                uwfButtonIncrease.uwfBorderHoverColor = Color.FromArgb(126, 180, 234);
                uwfButtonIncrease.Image = uwfAppOwner.Resources.NumericUp;
                uwfButtonIncrease.Click += delegate { if (Enabled) Value += Increment; };

                ButtonDecrease = new RepeatButton();
                ButtonDecrease.Anchor = AnchorStyles.Right | AnchorStyles.Top;
                ButtonDecrease.Location = new Point(Width - 16, Height / 2);
                ButtonDecrease.Size = new Drawing.Size(14, 8);
                ButtonDecrease.Name = "_numericButtonDecrease";
                ButtonDecrease.BackColor = Color.FromArgb(236, 236, 236);
                ButtonDecrease.uwfBorderColor = Color.FromArgb(172, 172, 172);
                ButtonDecrease.uwfHoverColor = Color.FromArgb(228, 241, 252);
                ButtonDecrease.uwfBorderHoverColor = Color.FromArgb(126, 180, 234);
                ButtonDecrease.Image = uwfAppOwner.Resources.NumericDown;
                ButtonDecrease.Click += delegate { if (Enabled) Value -= Increment; };

                Controls.Add(uwfButtonIncrease);
                Controls.Add(ButtonDecrease);
            }

            Resize += UpdateButtonsLocation;
            LostFocus += (s, a) => { ConfirmValue(); };
        }

        public event EventHandler ValueChanged = delegate { };

        public decimal Increment { get; set; }
        public decimal Maximum
        {
            get { return maximum; }
            set
            {
                if (value < minimum) minimum = value;
                maximum = value;
                if (this.value < minimum) this.value = minimum;
                if (this.value > maximum) this.value = maximum;
            }
        }
        public decimal Minimum
        {
            get { return minimum; }
            set
            {
                if (value > maximum) maximum = value;
                minimum = value;
                if (this.value < minimum) this.value = minimum;
                if (this.value > maximum) this.value = maximum;
            }
        }
        public HorizontalAlignment TextAlign { get; set; }
        public decimal Value
        {
            get { return value; }
            set
            {
                if (value > Maximum) value = Maximum;
                if (value < Minimum) value = Minimum;
                bool changed = this.value != value;
                this.value = value;
                valueText = value.ToString();
                if (changed)
                    ValueChanged(this, null);
            }
        }

        internal Color uwfBorderColor
        {
            get { return borderPen.Color; }
            set { borderPen.Color = value; }
        }
        internal Button ButtonDecrease { get; private set; }
        internal Button uwfButtonIncrease { get; private set; }
        internal Color uwfDisabledColor { get; set; }

        protected override Size DefaultSize
        {
            get { return new Size(120, 20); }
        }

        public void ShowButtons()
        {
            uwfButtonIncrease.Visible = true;
            ButtonDecrease.Visible = true;
        }
        public void HideButtons()
        {
            uwfButtonIncrease.Visible = false;
            ButtonDecrease.Visible = false;
        }

        protected void ConfirmValue()
        {
            decimal value = Value;
            if (decimal.TryParse(valueText, out value))
                if (Value != value)
                    Value = value;
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Return)
                ConfirmValue();
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (!Focused) return;

            if (e.Delta < 0)
                Value -= Increment;
            if (e.Delta > 0)
                Value += Increment;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;

            int textPaddingRight = 0;
            if (uwfButtonIncrease != null && uwfButtonIncrease.Visible)
                textPaddingRight = -16;

            var backColor = Enabled ? BackColor : uwfDisabledColor;
            var foreColor = Enabled ? ForeColor : Color.Black;

            g.uwfFillRectangle(backColor, 0, 0, Width, Height);

            if (Focused)
                valueText = g.uwfDrawTextField(valueText, Font, foreColor, Padding.Left - 2, 0, Width + textPaddingRight + 4, Height, TextAlign);
            else
                g.uwfDrawString(valueText, Font, foreColor, Padding.Left, 0, Width + textPaddingRight, Height, TextAlign);
        }

        protected override void uwfOnLatePaint(PaintEventArgs e)
        {
            base.uwfOnLatePaint(e);

            e.Graphics.DrawRectangle(borderPen, 0, 0, Width, Height);
        }

        private void UpdateButtonsLocation(object sender, EventArgs e)
        {
            if (uwfButtonIncrease != null)
                uwfButtonIncrease.Location = new Point(Width - 16, Height / 2 - 8);
            if (ButtonDecrease != null)
                ButtonDecrease.Location = new Point(Width - 16, Height / 2);
        }
    }
}
