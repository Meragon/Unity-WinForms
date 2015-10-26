using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public class NumericUpDown : Control
    {
        private Button _buttonIncrease;
        private Button _buttonDecrease;
        private decimal _minimum;
        private decimal _maximum;
        private decimal _value;
        private string _valueText = "0";

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
            this.Increment = 1;
            this.Maximum = 100;
            this.Minimum = 0;
            this.Size = new Drawing.Size(120, 20);
            this.TextAlign = HorizontalAlignment.Left;

            _buttonIncrease = new Button();
            _buttonIncrease.Anchor = AnchorStyles.Right;
            _buttonIncrease.Location = new Point(Width - 16, Height / 2 - 8);
            _buttonIncrease.Size = new Size(14, 8);
            _buttonIncrease.Name = "_numericButtonIncrease";
            _buttonIncrease.NormalColor = Color.FromArgb(236, 236, 236);
            _buttonIncrease.NormalBorderColor = Color.FromArgb(172, 172, 172);
            _buttonIncrease.HoverColor = Color.FromArgb(228, 241, 252);
            _buttonIncrease.HoverBorderColor = Color.FromArgb(126, 180, 234);
            _buttonIncrease.Image = Application.Resources.Reserved.NumericUp;
            _buttonIncrease.Click += delegate { if (Enabled) Value += Increment; };

            _buttonDecrease = new Button();
            _buttonDecrease.Anchor = AnchorStyles.Right;
            _buttonDecrease.Location = new Point(Width - 16, Height / 2);
            _buttonDecrease.Size = new Drawing.Size(14, 8);
            _buttonDecrease.Name = "_numericButtonDecrease";
            _buttonDecrease.NormalColor = Color.FromArgb(236, 236, 236);
            _buttonDecrease.NormalBorderColor = Color.FromArgb(172, 172, 172);
            _buttonDecrease.HoverColor = Color.FromArgb(228, 241, 252);
            _buttonDecrease.HoverBorderColor = Color.FromArgb(126, 180, 234);
            _buttonDecrease.Image = Application.Resources.Reserved.NumericDown;
            _buttonDecrease.Click += delegate { if (Enabled) Value -= Increment; };

            Controls.Add(_buttonIncrease);
            Controls.Add(_buttonDecrease);

            Resize += _UpdateButtonsLocation;
        }

        private void _UpdateButtonsLocation(object sender, EventArgs e)
        {
            if (_buttonIncrease != null)
                _buttonIncrease.Location = new Point(Width - 16, Height / 2 - 8);
            if (_buttonDecrease != null)
                _buttonDecrease.Location = new Point(Width - 16, Height / 2);
        }

        public event EventHandler ValueChanged = delegate { };

        public void HideButtons()
        {
            _buttonIncrease.Visible = false;
            _buttonDecrease.Visible = false;
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
                g.FillRectangle(new SolidBrush(Color.FromArgb(250, 250, 250)), 0, 0, Width, Height);

                UnityEngine.GUI.color = Color.Black.ToUColor();
                _valueText = g.DrawTextField(_valueText, Font, new SolidBrush(ForeColor), 0, 0, Width + (_buttonIncrease.Visible ? -16 : 0), Height, TextAlign);
                //UnityEngine.GUI.TextField(new UnityEngine.Rect(screenPos.X, screenPos.Y, Width, Height), Value.ToString());

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
                g.DrawString(Value.ToString(), Font, Brushes.Black, 4, 0, Width + (_buttonIncrease.Visible ? -24 : 0), Height, TextAlign);

            g.DrawRectangle(new Pen(Color.FromArgb(175, 175, 175)), 0, 0, Width, Height);
        }
        public void ShowButtons()
        {
            _buttonIncrease.Visible = true;
            _buttonDecrease.Visible = true;
        }

        public delegate void CallbackDelegate(object data);
    }
}
