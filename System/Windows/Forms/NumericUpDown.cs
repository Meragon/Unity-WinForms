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
#if UNITY_EDITOR
        private bool _toggleEditor;
#endif

        private decimal _minimum;
        private decimal _maximum;

        protected Pen borderPen = new Pen(Color.Transparent);
        protected decimal value;
        protected string valueText = "0";

        public Button ButtonIncrease { get; private set; }
        public Button ButtonDecrease { get; private set; }

        public Color BorderColor
        {
            get { return borderPen.Color; }
            set { borderPen.Color = value; }
        }
        public Color DisabledColor { get; set; }
        public decimal Increment { get; set; }
        public decimal Maximum
        {
            get { return _maximum; }
            set
            {
                if (value < _minimum) _minimum = value;
                _maximum = value;
                if (this.value < _minimum) this.value = _minimum;
                if (this.value > _maximum) this.value = _maximum;
            }
        }
        public decimal Minimum
        {
            get { return _minimum; }
            set
            {
                if (value > _maximum) _maximum = value;
                _minimum = value;
                if (this.value < _minimum) this.value = _minimum;
                if (this.value > _maximum) this.value = _maximum;
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
                ButtonIncrease.Image = ApplicationBehaviour.GdiImages.NumericUp;
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
                ButtonDecrease.Image = ApplicationBehaviour.GdiImages.NumericDown;
                ButtonDecrease.Click += delegate { if (Enabled) Value -= Increment; };

                Controls.Add(ButtonIncrease);
                Controls.Add(ButtonDecrease);
            }

            Resize += UpdateButtonsLocation;
            LostFocus += (s, a) => { ConfirmValue(); };
        }

        public event EventHandler ValueChanged = delegate { };

        protected void ConfirmValue()
        {
            decimal value = Value;
            if (decimal.TryParse(valueText, out value))
                if (Value != value)
                    Value = value;
        }
        public void HideButtons()
        {
            ButtonIncrease.Visible = false;
            ButtonDecrease.Visible = false;
        }
        protected override void OnLatePaint(PaintEventArgs e)
        {
            base.OnLatePaint(e);

            e.Graphics.DrawRectangle(borderPen, 0, 0, Width, Height);
        }
        protected override void OnKeyPress(KeyEventArgs e)
        {
            base.OnKeyPress(e);

            if (e.KeyCode == UnityEngine.KeyCode.Return)
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
            if (ButtonIncrease != null && ButtonIncrease.Visible)
                textPaddingRight = -16;

            var backColor = Enabled ? BackColor : DisabledColor;
            var foreColor = Enabled ? ForeColor : Color.Black;

            g.uwfFillRectangle(backColor, 0, 0, Width, Height);

            if (Focused)
                valueText = g.uwfDrawTextField(valueText, Font, foreColor, Padding.Left - 2, 0, Width + textPaddingRight + 4, Height, TextAlign);
            else
                g.uwfDrawString(valueText, Font, foreColor, Padding.Left, 0, Width + textPaddingRight, Height, TextAlign);
        }
        protected override object uwfOnPaintEditor(float width)
        {
            var component = base.uwfOnPaintEditor(width);

#if UNITY_EDITOR
            Editor.BeginGroup(width - 24);
            Editor.BeginVertical();

            _toggleEditor = Editor.Foldout("NumericUpDown", _toggleEditor);
            if (_toggleEditor)
            {
                var editorIncrement = Editor.Slider("Increment", (float)Increment, 0, 255);
                if (editorIncrement.Changed)
                    Increment = (decimal)editorIncrement.Value;

                float editorMinValue = short.MinValue;
                float editorMaxValue = short.MaxValue;
                if ((float)Maximum > editorMaxValue) editorMaxValue = (float)Maximum;
                if ((float)Minimum < editorMinValue) editorMinValue = (float)Minimum;

                var editorMaximum = Editor.Slider("Maximum", (float)Maximum, editorMinValue, editorMaxValue);
                if (editorMaximum.Changed)
                    Maximum = (decimal)editorMaximum.Value;

                var editorMinimum = Editor.Slider("Minimum", (float)Minimum, editorMinValue, editorMaxValue);
                if (editorMinimum.Changed)
                    Minimum = (decimal)editorMinimum.Value;

                var editorValue = Editor.Slider("Value", (float)Value, (float)Minimum, (float)Maximum);
                if (editorValue.Changed)
                    Value = (decimal)editorValue.Value;
            }
            Editor.EndVertical();
            Editor.EndGroup();
#endif

            return component;
        }

        public void ShowButtons()
        {
            ButtonIncrease.Visible = true;
            ButtonDecrease.Visible = true;
        }
        protected void UpdateButtonsLocation(object sender, EventArgs e)
        {
            if (ButtonIncrease != null)
                ButtonIncrease.Location = new Point(Width - 16, Height / 2 - 8);
            if (ButtonDecrease != null)
                ButtonDecrease.Location = new Point(Width - 16, Height / 2);
        }
    }
}
