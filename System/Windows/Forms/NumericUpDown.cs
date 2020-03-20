namespace System.Windows.Forms
{
    using System.Drawing;
    using System.Globalization;

    public class NumericUpDown : Control
    {
        internal static Func<CultureInfo> uwfGetCulture = () => Application.CurrentCulture;
        
        internal Pen borderPen = new Pen(Color.Transparent);
        internal Button uwfButtonDecrease;
        internal Button uwfButtonIncrease;
        internal Color uwfBorderColor;
        internal Color uwfBorderDisabledColor;
        internal Color uwfBorderFocusedColor;
        internal Color uwfBorderHoverColor;
        internal Color uwfDisabledBackColor;
        internal Color uwfDisabledForeColor;

        protected decimal value;
        protected string valueText = "0";
        
        private static readonly Padding defaultPadding = new Padding(4, 0, 4, 0);
        private bool hexadecimal;
        private decimal minimum;
        private decimal maximum;

        public NumericUpDown() : this(true)
        {
        }
        internal NumericUpDown(bool initButtons)
        {
            BackColor = SystemColors.Window;
            Increment = 1;
            InterceptArrowKeys = true;
            Maximum = 100;
            Minimum = 0;
            Padding = defaultPadding;
            TextAlign = HorizontalAlignment.Left;

            uwfBorderColor = SystemColors.ActiveBorder;
            uwfBorderDisabledColor = Color.FromArgb(0xAB, 0xAD, 0xB3);
            uwfBorderHoverColor = SystemColors.ActiveBorder;
            uwfBorderFocusedColor = SystemColors.ActiveBorder;
            uwfDisabledBackColor = SystemColors.Control;
            uwfDisabledForeColor = SystemColors.ActiveBorder;

            if (initButtons)
            {
                uwfButtonIncrease = new UpDownButton(this, true);
                uwfButtonIncrease.Location = new Point(Width - 16, Height / 2 - 8);
                uwfButtonIncrease.Image = uwfAppOwner.Resources.NumericUp;
                uwfButtonIncrease.uwfImageColor = Color.FromArgb(0x40, 0x40, 0x40);
                uwfButtonIncrease.uwfImageHoverColor = uwfButtonIncrease.uwfImageColor; 
                
                uwfButtonDecrease = new UpDownButton(this, false);
                uwfButtonDecrease.Location = new Point(Width - 16, Height / 2);
                uwfButtonDecrease.Image = uwfAppOwner.Resources.NumericDown;
                uwfButtonDecrease.uwfImageColor = Color.FromArgb(0x40, 0x40, 0x40);
                uwfButtonDecrease.uwfImageHoverColor = uwfButtonDecrease.uwfImageColor;
                
                Controls.Add(uwfButtonIncrease);
                Controls.Add(uwfButtonDecrease);
            }
        }

        public event EventHandler ValueChanged;

        public decimal Increment { get; set; }
        public bool InterceptArrowKeys { get; set; }
        public bool Hexadecimal
        {
            get { return hexadecimal; }
            set
            {
                hexadecimal = value;
                UpdateEditText();
            }
        }
        public decimal Maximum
        {
            get { return maximum; }
            set
            {
                if (value < minimum) minimum = value;
                
                maximum = value;
                
                if (this.value < minimum)
                {
                    this.value = minimum;
                    UpdateEditText();
                }

                if (this.value > maximum)
                {
                    this.value = maximum;
                    UpdateEditText();
                }
            }
        }
        public decimal Minimum
        {
            get { return minimum; }
            set
            {
                if (value > maximum) maximum = value;
                
                minimum = value;
                
                if (this.value < minimum)
                {
                    this.value = minimum;
                    UpdateEditText();
                }

                if (this.value > maximum)
                {
                    this.value = maximum;
                    UpdateEditText();
                }
            }
        }
        public HorizontalAlignment TextAlign { get; set; }
        public decimal Value
        {
            get { return value; }
            set
            {
                if (this.value == value)
                    return;

                this.value = Constrain(value);
                UpdateEditText();

                OnValueChanged(EventArgs.Empty);
            }
        }

        protected override Size DefaultSize
        {
            get { return new Size(120, 20); }
        }

        public virtual void DownButton()
        {
            Value -= Increment;
        }
        public virtual void UpButton()
        {
            Value += Increment;
        }

        protected internal override void uwfOnLatePaint(PaintEventArgs e)
        {
            base.uwfOnLatePaint(e);

            borderPen.Color = uwfBorderColor;
            if (!Enabled)
                borderPen.Color = uwfBorderDisabledColor;
            else if (Focused)
                borderPen.Color = uwfBorderFocusedColor;
            else if (hovered)
                borderPen.Color = uwfBorderHoverColor;
            
            e.Graphics.DrawRectangle(borderPen, 0, 0, Width, Height);
        }

        protected void ConfirmValue()
        {
            if (string.IsNullOrEmpty(valueText))
                return;

            try
            {
                if (Hexadecimal)
                    Value = Constrain(Convert.ToDecimal(Convert.ToInt32(valueText, 16)));
                else
                    Value = Constrain(decimal.Parse(valueText, uwfGetCulture()));
            }
            catch
            {
                UpdateEditText();
            }
        }
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            ConfirmValue();
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (InterceptArrowKeys)
            {
                if (e.KeyCode == Keys.Up)
                    UpButton();
                else if (e.KeyCode == Keys.Down)
                    DownButton();
            }
            
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Return)
                ConfirmValue();
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            if (Enabled)
                Cursor.Current = Cursors.IBeam;
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            
            Cursor.Current = Cursors.Default;
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

            var backColor = Enabled ? BackColor : uwfDisabledBackColor;
            var foreColor = Enabled ? ForeColor : uwfDisabledForeColor;

            g.uwfFillRectangle(backColor, 0, 0, Width, Height);

            if (Focused)
                valueText = g.uwfDrawTextField(valueText, Font, foreColor, Padding.Left - 2, 0, Width + textPaddingRight + 4, Height, TextAlign);
            else
                g.uwfDrawString(valueText, Font, foreColor, Padding.Left, 0, Width + textPaddingRight, Height, TextAlign);
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            UpdateButtonsLocation();
        }
        protected virtual void OnValueChanged(EventArgs e)
        {
            var handler = ValueChanged;
            if (handler != null) handler(this, e);
        }

        private decimal Constrain(decimal avalue)
        {
            if (avalue < minimum)
                avalue = minimum;

            if (avalue > maximum)
                avalue = maximum;

            return avalue;
        }
        private void UpdateButtonsLocation()
        {
            var width = Width;
            var height = Height;
            if (uwfButtonIncrease != null)
                uwfButtonIncrease.Location = new Point(width - 16, height / 2 - 8);
            if (uwfButtonDecrease != null)
                uwfButtonDecrease.Location = new Point(width - 16, height / 2);
        }
        private void UpdateEditText()
        {
            valueText = Hexadecimal 
                ? Convert.ToInt64(value).ToString("X") 
                : value.ToString(uwfGetCulture());
        }

        internal class UpDownButton : RepeatButton
        {
            private static readonly Color defaultBorderColor = Color.FromArgb(172, 172, 172);
            private static readonly Color defaultBorderHoverColor = Color.FromArgb(126, 180, 234);
            private static readonly Color defaultHoverColor = Color.FromArgb(228, 241, 252);
            private readonly NumericUpDown owner;
            private readonly bool upButton;

            internal UpDownButton(NumericUpDown owner, bool upButton)
            {
                this.owner = owner;
                this.upButton = upButton;

                Anchor = AnchorStyles.Right | AnchorStyles.Top;
                BackColor = SystemColors.Control;
                TabStop = false;

                uwfBorderColor = defaultBorderColor;
                uwfHoverColor = defaultHoverColor;
                uwfBorderHoverColor = defaultBorderHoverColor;
            }

            protected override Size DefaultSize
            {
                get { return new Size(14, 8); }
            }

            protected override void OnClick(EventArgs e)
            {
                base.OnClick(e);

                if (upButton)
                    owner.UpButton();
                else
                    owner.DownButton();
            }
        }
    }
}
