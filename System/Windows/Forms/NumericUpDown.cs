using System;
using System.Collections.Generic;
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
        private string _valueText = "0";

        public Button ButtonIncrease { get; private set; }
        public Button ButtonDecrease { get; private set; }

        public Color BorderColor { get; set; }
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
                _valueText = value.ToString();
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

        public NumericUpDown()
        {
            this.BackColor = Color.FromArgb(250, 250, 250);
            this.BorderColor = Color.FromArgb(175, 175, 175);
            this.Increment = 1;
            this.Maximum = 100;
            this.Minimum = 0;
            this.Padding = new Padding(4, 0, 4, 0);
            this.Size = new Drawing.Size(120, 20);
            this.TextAlign = HorizontalAlignment.Left;

            ButtonIncrease = new Button();
            ButtonIncrease.Anchor = AnchorStyles.Right;
            ButtonIncrease.Location = new Point(Width - 16, Height / 2 - 8);
            ButtonIncrease.Size = new Size(14, 8);
            ButtonIncrease.Name = "_numericButtonIncrease";
            ButtonIncrease.NormalColor = Color.FromArgb(236, 236, 236);
            ButtonIncrease.NormalBorderColor = Color.FromArgb(172, 172, 172);
            ButtonIncrease.HoverColor = Color.FromArgb(228, 241, 252);
            ButtonIncrease.HoverBorderColor = Color.FromArgb(126, 180, 234);
            ButtonIncrease.Image = ApplicationBehaviour.Resources.Reserved.NumericUp;
            ButtonIncrease.Click += delegate { if (Enabled) Value += Increment; };

            ButtonDecrease = new Button();
            ButtonDecrease.Anchor = AnchorStyles.Right;
            ButtonDecrease.Location = new Point(Width - 16, Height / 2);
            ButtonDecrease.Size = new Drawing.Size(14, 8);
            ButtonDecrease.Name = "_numericButtonDecrease";
            ButtonDecrease.NormalColor = Color.FromArgb(236, 236, 236);
            ButtonDecrease.NormalBorderColor = Color.FromArgb(172, 172, 172);
            ButtonDecrease.HoverColor = Color.FromArgb(228, 241, 252);
            ButtonDecrease.HoverBorderColor = Color.FromArgb(126, 180, 234);
            ButtonDecrease.Image = ApplicationBehaviour.Resources.Reserved.NumericDown;
            ButtonDecrease.Click += delegate { if (Enabled) Value -= Increment; };

            Controls.Add(ButtonIncrease);
            Controls.Add(ButtonDecrease);

            Resize += _UpdateButtonsLocation;
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
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (e.Delta < 0)
                Value--;
            if (e.Delta > 0)
                Value++;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            if (Enabled)
            {
                g.FillRectangle(new SolidBrush(BackColor), 0, 0, Width, Height);

                if (Focused)
                {
                    if (_shouldFocus)
                        UnityEngine.GUI.SetNextControlName(Name);

                    _valueText = g.DrawTextField(_valueText, Font, new SolidBrush(ForeColor), Padding.Left, 0, Width + (ButtonIncrease.Visible ? -16 : 0), Height, TextAlign);

                    if (_shouldFocus)
                    {
                        UnityEngine.GUI.FocusControl(Name);
                        _shouldFocus = false;
                    }
                }
                else
                    g.DrawString(_valueText, Font, new SolidBrush(ForeColor), Padding.Left, 0, Width + (ButtonIncrease.Visible ? -16 : 0), Height, TextAlign);

                decimal value = Value;
                if (!decimal.TryParse(_valueText, out value))
                {
                    // Do nothing.
                }
                else
                {
                    if (Value != value)
                        Value = value;
                }
            }
            else
                g.DrawString(Value.ToString(), Font, Brushes.Black, Padding.Left, 0, Width + (ButtonIncrease.Visible ? -24 : 0), Height, TextAlign);

            g.DrawRectangle(new Pen(BorderColor), 0, 0, Width, Height);
        }
        public void ShowButtons()
        {
            ButtonIncrease.Visible = true;
            ButtonDecrease.Visible = true;
        }

        public delegate void CallbackDelegate(object data);
    }
}
