using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public class NumericUpDown : Control
    {
        private decimal _minimum;
        private decimal _maximum;
        private bool _shouldFocus;
        private decimal _value;

        protected string valueText = "0";

        public Button ButtonIncrease { get; private set; }
        public Button ButtonDecrease { get; private set; }

        public Color BorderColor { get; set; }
        public Color DisabledColor { get; set; }
        public decimal Increment { get; set; }
        public decimal Maximum
        {
            get { return _maximum; }
            set
            {
                if (value < _minimum) _minimum = value;
                _maximum = value;
                if (_value < _minimum) _value = _minimum;
                if (_value > _maximum) _value = _maximum;
            }
        }
        public decimal Minimum
        {
            get { return _minimum; }
            set
            {
                if (value > _maximum) _maximum = value;
                _minimum = value;
                if (_value < _minimum) _value = _minimum;
                if (_value > _maximum) _value = _maximum;
            }
        }
        public HorizontalAlignment TextAlign { get; set; }
        public decimal Value
        {
            get { return _value; }
            set
            {
                if (value > Maximum) value = Maximum;
                if (value < Minimum) value = Minimum;
                bool changed = _value != value;
                _value = value;
                valueText = value.ToString();
                if (changed)
                {
                    ValueChanged(this, null);
                    if (ValueChangedCallback != null)
                        ValueChangedCallback.Invoke(ValueChangedCallbackInfo);
                }
            }
        }
        public CallbackDelegate ValueChangedCallback { get; set; }
        public object ValueChangedCallbackInfo { get; set; }

        public NumericUpDown() : this(true)
        {
            
        }
        public NumericUpDown(bool initButtons)
        {
            this.BackColor = Color.FromArgb(250, 250, 250);
            this.BorderColor = Color.FromArgb(175, 175, 175);
            this.CanSelect = true;
            this.DisabledColor = Color.FromArgb(240, 240, 240);
            this.Increment = 1;
            this.Maximum = 100;
            this.Minimum = 0;
            this.Padding = new Padding(4, 0, 4, 0);
            this.Size = new Drawing.Size(120, 20);
            this.TextAlign = HorizontalAlignment.Left;

            if (initButtons)
            {
                ButtonIncrease = new RepeatButton();
                ButtonIncrease.Anchor = AnchorStyles.Right | AnchorStyles.Top;
                ButtonIncrease.CanSelect = false;
                ButtonIncrease.Location = new Point(Width - 16, Height / 2 - 8);
                ButtonIncrease.Size = new Size(14, 8);
                ButtonIncrease.Name = "_numericButtonIncrease";
                ButtonIncrease.BackColor = Color.FromArgb(236, 236, 236);
                ButtonIncrease.BorderColor = Color.FromArgb(172, 172, 172);
                ButtonIncrease.HoverColor = Color.FromArgb(228, 241, 252);
                ButtonIncrease.BorderHoverColor = Color.FromArgb(126, 180, 234);
                ButtonIncrease.Image = ApplicationBehaviour.Resources.Images.NumericUp;
                ButtonIncrease.Click += delegate { if (Enabled) Value += Increment; };

                ButtonDecrease = new RepeatButton();
                ButtonDecrease.Anchor = AnchorStyles.Right | AnchorStyles.Top;
                ButtonDecrease.CanSelect = false;
                ButtonDecrease.Location = new Point(Width - 16, Height / 2);
                ButtonDecrease.Size = new Drawing.Size(14, 8);
                ButtonDecrease.Name = "_numericButtonDecrease";
                ButtonDecrease.BackColor = Color.FromArgb(236, 236, 236);
                ButtonDecrease.BorderColor = Color.FromArgb(172, 172, 172);
                ButtonDecrease.HoverColor = Color.FromArgb(228, 241, 252);
                ButtonDecrease.BorderHoverColor = Color.FromArgb(126, 180, 234);
                ButtonDecrease.Image = ApplicationBehaviour.Resources.Images.NumericDown;
                ButtonDecrease.Click += delegate { if (Enabled) Value -= Increment; };

                Controls.Add(ButtonIncrease);
                Controls.Add(ButtonDecrease);
            }

            Resize += _UpdateButtonsLocation;
            LostFocus += (s, a) => { _ConfirmValue(); };
        }

        private void _ConfirmValue()
        {
            decimal value = Value;
            if (decimal.TryParse(valueText, out value))
                if (Value != value)
                    Value = value;
        }
        private void _UpdateButtonsLocation(object sender, EventArgs e)
        {
            if (ButtonIncrease != null)
                ButtonIncrease.Location = new Point(Width - 16, Height / 2 - 8);
            if (ButtonDecrease != null)
                ButtonDecrease.Location = new Point(Width - 16, Height / 2);
        }

        public event EventHandler ValueChanged = delegate { };

        public override void Focus()
        {
            base.Focus();
            _shouldFocus = true;
        }
        public void HideButtons()
        {
            ButtonIncrease.Visible = false;
            ButtonDecrease.Visible = false;
        }
        protected override void OnLatePaint(PaintEventArgs e)
        {
            base.OnLatePaint(e);

            e.Graphics.DrawRectangle(new Pen(BorderColor), 0, 0, Width, Height);
        }
        protected override void OnKeyPress(KeyEventArgs e)
        {
            base.OnKeyPress(e);

            if (e.KeyCode == UnityEngine.KeyCode.Return)
                _ConfirmValue();
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (Focused)
            {
                if (e.Delta < 0)
                    Value--;
                if (e.Delta > 0)
                    Value++;
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            int textPaddingRight = 0;
            if (ButtonIncrease != null && ButtonIncrease.Visible)
                textPaddingRight = -16;

            if (Enabled)
            {
                g.FillRectangle(BackColor, 0, 0, Width, Height);

                if (Focused)
                {
                    if (_shouldFocus) e.Graphics.FocusNext();

                    valueText = g.DrawTextField(valueText, Font, ForeColor, Padding.Left - 2, 0, Width + textPaddingRight + 4, Height, TextAlign);

                    if (_shouldFocus)
                    {
                        e.Graphics.Focus();
                        _shouldFocus = false;
                    }
                }
                else
                    g.DrawString(valueText, Font, ForeColor, Padding.Left, 0, Width + textPaddingRight, Height, TextAlign);
            }
            else
            {
                g.FillRectangle(DisabledColor, 0, 0, Width, Height);
                g.DrawString(Value.ToString(), Font, Color.Black, Padding.Left, 0, Width + textPaddingRight, Height, TextAlign);
            }
        }
        public void ShowButtons()
        {
            ButtonIncrease.Visible = true;
            ButtonDecrease.Visible = true;
        }

        public delegate void CallbackDelegate(object data);
    }
}
