namespace System.Windows.Forms
{
    using System.Drawing;

    public class ColorPickerForm : Form
    {
        private new readonly Pen borderPen;
        private readonly AlphaPicker alphaPicker;
        private readonly ValueSaturationPicker vsPicker;
        private readonly HuePicker huePicker;

        private readonly Label aLabel;

        private readonly NumericUpDown hueNumeric;
        private readonly NumericUpDown saturationNumeric;
        private readonly NumericUpDown valueNumeric;

        private readonly NumericUpDown rNumeric;
        private readonly NumericUpDown gNumeric;
        private readonly NumericUpDown bNumeric;

        private readonly NumericUpDown aNumeric;

        private Color color;

        public ColorPickerForm()
        {
            borderPen = new Pen(Color.FromArgb(204, 206, 219));

            BackColor = Color.White;
            Size = new Size(188, 272);
            Location = new Point(
                Screen.PrimaryScreen.WorkingArea.Width / 2 - Width / 2,
                Screen.PrimaryScreen.WorkingArea.Height / 2 - Height / 2);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Text = "Pick a color";
            KeyPreview = true;
            TopMost = true;

            vsPicker = new ValueSaturationPicker(128, 128);
            vsPicker.Location = new Point(16, 32);
            vsPicker.ValueChanged += VsPickerValueChanged;
            vsPicker.SaturationChanged += VsPickerSaturationChanged;

            Controls.Add(vsPicker);

            huePicker = new HuePicker(20, 128);
            huePicker.Location = new Point(vsPicker.Location.X + vsPicker.Width + 8, vsPicker.Location.Y);
            huePicker.HueChanged += huePicker_HueChanged;

            Controls.Add(huePicker);

            var hueLabel = new Label();
            hueLabel.Text = "H:";
            hueLabel.Location = new Point(vsPicker.Location.X, vsPicker.Location.Y + vsPicker.Height + 8);
            var saturationLabel = new Label();
            saturationLabel.Text = "S:";
            saturationLabel.Location = new Point(hueLabel.Location.X, hueLabel.Location.Y + 22);
            var valueLabel = new Label();
            valueLabel.Text = "V:";
            valueLabel.Location = new Point(hueLabel.Location.X, saturationLabel.Location.Y + 22);

            Controls.Add(hueLabel);
            Controls.Add(saturationLabel);
            Controls.Add(valueLabel);

            hueNumeric = new NumericUpDown();
            hueNumeric.Minimum = 0;
            hueNumeric.Maximum = 359;
            hueNumeric.Increment = 5;
            hueNumeric.Location = new Point(hueLabel.Location.X + 24, hueLabel.Location.Y);
            hueNumeric.Size = new Size(50, 20);
            hueNumeric.ValueChanged += HueNumericValueChanged;
            hueNumeric.TextAlign = HorizontalAlignment.Center;
            saturationNumeric = new NumericUpDown();
            saturationNumeric.Minimum = 0;
            saturationNumeric.Maximum = 255;
            saturationNumeric.Location = new Point(saturationLabel.Location.X + 24, saturationLabel.Location.Y);
            saturationNumeric.Size = new Size(50, 20);
            saturationNumeric.ValueChanged += SaturationNumericValueChanged;
            saturationNumeric.TextAlign = HorizontalAlignment.Center;
            valueNumeric = new NumericUpDown();
            valueNumeric.Minimum = 0;
            valueNumeric.Maximum = 255;
            valueNumeric.Location = new Point(valueLabel.Location.X + 24, valueLabel.Location.Y);
            valueNumeric.Size = new Size(50, 20);
            valueNumeric.ValueChanged += ValueNumericValueChanged;
            valueNumeric.TextAlign = HorizontalAlignment.Center;

            Controls.Add(hueNumeric);
            Controls.Add(saturationNumeric);
            Controls.Add(valueNumeric);

            var rLabel = new Label();
            rLabel.Text = "R:";
            rLabel.Location = new Point(hueNumeric.Location.X + hueNumeric.Width + 8, hueLabel.Location.Y);
            var gLabel = new Label();
            gLabel.Text = "G:";
            gLabel.Location = new Point(rLabel.Location.X, saturationLabel.Location.Y);
            var bLabel = new Label();
            bLabel.Text = "B:";
            bLabel.Location = new Point(rLabel.Location.X, valueLabel.Location.Y);

            Controls.Add(rLabel);
            Controls.Add(gLabel);
            Controls.Add(bLabel);

            rNumeric = new NumericUpDown();
            rNumeric.Minimum = 0;
            rNumeric.Maximum = 255;
            rNumeric.Location = new Point(rLabel.Location.X + 24, rLabel.Location.Y);
            rNumeric.Size = new Size(50, 20);
            rNumeric.TextAlign = HorizontalAlignment.Center;
            rNumeric.ValueChanged += rNumeric_ValueChanged;
            gNumeric = new NumericUpDown();
            gNumeric.Minimum = 0;
            gNumeric.Maximum = 255;
            gNumeric.Location = new Point(gLabel.Location.X + 24, gLabel.Location.Y);
            gNumeric.Size = new Size(50, 20);
            gNumeric.TextAlign = HorizontalAlignment.Center;
            gNumeric.ValueChanged += gNumeric_ValueChanged;
            bNumeric = new NumericUpDown();
            bNumeric.Minimum = 0;
            bNumeric.Maximum = 255;
            bNumeric.Location = new Point(bLabel.Location.X + 24, bLabel.Location.Y);
            bNumeric.Size = new Size(50, 20);
            bNumeric.TextAlign = HorizontalAlignment.Center;
            bNumeric.ValueChanged += bNumeric_ValueChanged;

            Controls.Add(rNumeric);
            Controls.Add(gNumeric);
            Controls.Add(bNumeric);

            alphaPicker = new AlphaPicker(valueNumeric.Location.X + valueNumeric.Width - valueLabel.Location.X, 20);
            alphaPicker.Location = new Point(valueLabel.Location.X, valueLabel.Location.Y + 26);
            alphaPicker.AlphaChanged += alphaPicker_AlphaChanged;
            aLabel = new Label();
            aLabel.Location = new Point(bLabel.Location.X, alphaPicker.Location.Y);
            aLabel.Text = "A:";
            aNumeric = new NumericUpDown();
            aNumeric.Minimum = 0;
            aNumeric.Maximum = 255;
            aNumeric.Value = 255;
            aNumeric.Location = new Point(bNumeric.Location.X, aLabel.Location.Y);
            aNumeric.Size = new Size(50, 20);
            aNumeric.TextAlign = HorizontalAlignment.Center;
            aNumeric.ValueChanged += aNumeric_ValueChanged;

            Controls.Add(alphaPicker);
            Controls.Add(aLabel);
            Controls.Add(aNumeric);
        }

        public event EventHandler ColorChanged;

        public Color Color
        {
            get { return color; }
            set
            {
                if (color == value)
                    return;
                
                color = value;

                rNumeric.ValueChanged -= rNumeric_ValueChanged;
                rNumeric.Value = color.R;
                rNumeric.ValueChanged += rNumeric_ValueChanged;

                gNumeric.ValueChanged -= gNumeric_ValueChanged;
                gNumeric.Value = color.G;
                gNumeric.ValueChanged += gNumeric_ValueChanged;

                bNumeric.ValueChanged -= bNumeric_ValueChanged;
                bNumeric.Value = color.B;
                bNumeric.ValueChanged += bNumeric_ValueChanged;

                alphaPicker.Alpha = (float)color.A / 255;

                UpdateControlsFromRGB();
                
                OnColorChanged(EventArgs.Empty);
            }
        }

        protected virtual void OnColorChanged(EventArgs e)
        {
            var handler = ColorChanged;
            if (handler != null)
                handler(this, e);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape || (e.KeyCode == Keys.W && e.Control))
                Close();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.DrawLine(borderPen, 1, uwfHeaderHeight - 1, Width - 1, uwfHeaderHeight - 1);
        }

        private void VsPickerValueChanged(object sender, EventArgs e)
        {
            valueNumeric.ValueChanged -= ValueNumericValueChanged;
            valueNumeric.Value = (int)(vsPicker.Value * 255);
            valueNumeric.ValueChanged += ValueNumericValueChanged;

            Color rgbColor = ColorTranslatorEx.FromHSV(huePicker.Hue * 359, vsPicker.Saturation, vsPicker.Value);

            rNumeric.ValueChanged -= rNumeric_ValueChanged;
            rNumeric.Value = rgbColor.R;
            rNumeric.ValueChanged += rNumeric_ValueChanged;
            gNumeric.ValueChanged -= gNumeric_ValueChanged;
            gNumeric.Value = rgbColor.G;
            gNumeric.ValueChanged += gNumeric_ValueChanged;
            bNumeric.ValueChanged -= bNumeric_ValueChanged;
            bNumeric.Value = rgbColor.B;
            bNumeric.ValueChanged += bNumeric_ValueChanged;

            color = Color.FromArgb((int)(alphaPicker.Alpha * 255), rgbColor);
            
            OnColorChanged(EventArgs.Empty);
        }
        private void VsPickerSaturationChanged(object sender, EventArgs e)
        {
            saturationNumeric.ValueChanged -= SaturationNumericValueChanged;
            saturationNumeric.Value = (int)(vsPicker.Saturation * 255);
            saturationNumeric.ValueChanged += SaturationNumericValueChanged;

            Color rgbColor = ColorTranslatorEx.FromHSV(huePicker.Hue * 359, vsPicker.Saturation, vsPicker.Value);

            rNumeric.ValueChanged -= rNumeric_ValueChanged;
            rNumeric.Value = rgbColor.R;
            rNumeric.ValueChanged += rNumeric_ValueChanged;
            gNumeric.ValueChanged -= gNumeric_ValueChanged;
            gNumeric.Value = rgbColor.G;
            gNumeric.ValueChanged += gNumeric_ValueChanged;
            bNumeric.ValueChanged -= bNumeric_ValueChanged;
            bNumeric.Value = rgbColor.B;
            bNumeric.ValueChanged += bNumeric_ValueChanged;

            color = Color.FromArgb((int)(alphaPicker.Alpha * 255), rgbColor);
            
            OnColorChanged(EventArgs.Empty);
        }
        private void huePicker_HueChanged(object sender, EventArgs e)
        {
            vsPicker.SetHue(huePicker.Hue);

            hueNumeric.ValueChanged -= HueNumericValueChanged;
            hueNumeric.Value = (int)((1f - huePicker.Hue) * 359);
            hueNumeric.ValueChanged += HueNumericValueChanged;

            Color rgbColor = ColorTranslatorEx.FromHSV(huePicker.Hue * 359, vsPicker.Saturation, vsPicker.Value);

            rNumeric.ValueChanged -= rNumeric_ValueChanged;
            rNumeric.Value = rgbColor.R;
            rNumeric.ValueChanged += rNumeric_ValueChanged;
            gNumeric.ValueChanged -= gNumeric_ValueChanged;
            gNumeric.Value = rgbColor.G;
            gNumeric.ValueChanged += gNumeric_ValueChanged;
            bNumeric.ValueChanged -= bNumeric_ValueChanged;
            bNumeric.Value = rgbColor.B;
            bNumeric.ValueChanged += bNumeric_ValueChanged;

            color = Color.FromArgb((int)(alphaPicker.Alpha * 255), rgbColor);
            
            OnColorChanged(EventArgs.Empty);
        }
        private void HueNumericValueChanged(object sender, EventArgs e)
        {
            huePicker.Hue = 1f - (float)hueNumeric.Value / 359f;
        }
        private void SaturationNumericValueChanged(object sender, EventArgs e)
        {
            vsPicker.Saturation = (float)saturationNumeric.Value / 255;
        }
        private void ValueNumericValueChanged(object sender, EventArgs e)
        {
            vsPicker.Value = (float)valueNumeric.Value / 255;
        }
        private void rNumeric_ValueChanged(object sender, EventArgs e)
        {
            UpdateControlsFromRGB();
        }
        private void gNumeric_ValueChanged(object sender, EventArgs e)
        {
            UpdateControlsFromRGB();
        }
        private void bNumeric_ValueChanged(object sender, EventArgs e)
        {
            UpdateControlsFromRGB();
        }
        private void alphaPicker_AlphaChanged(object sender, EventArgs e)
        {
            aNumeric.ValueChanged -= aNumeric_ValueChanged;
            aNumeric.Value = (int)(alphaPicker.Alpha * 255);
            aNumeric.ValueChanged += aNumeric_ValueChanged;

            color = Color.FromArgb((int)(alphaPicker.Alpha * 255), Color);
            
            OnColorChanged(EventArgs.Empty);
        }
        private void aNumeric_ValueChanged(object sender, EventArgs e)
        {
            alphaPicker.AlphaChanged -= alphaPicker_AlphaChanged;
            alphaPicker.Alpha = (float)Convert.ToDouble(aNumeric.Value / 255);
            alphaPicker.AlphaChanged += alphaPicker_AlphaChanged;

            color = Color.FromArgb((int)(alphaPicker.Alpha * 255), Color);
            
            OnColorChanged(EventArgs.Empty);
        }
        private void UpdateControlsFromRGB()
        {
            Color rgbColor = Color.FromArgb(Convert.ToInt32(rNumeric.Value), Convert.ToInt32(gNumeric.Value), Convert.ToInt32(bNumeric.Value));

            double hue;
            double saturation;
            double value;

            ColorTranslatorEx.ToHSV(rgbColor, out hue, out saturation, out value);
            hue /= 359;

            huePicker.HueChanged -= huePicker_HueChanged;
            huePicker.Hue = hue;
            huePicker.HueChanged += huePicker_HueChanged;
            vsPicker.SetHue(hue);
            vsPicker.ValueChanged -= VsPickerValueChanged;
            vsPicker.Value = (float)value;
            vsPicker.ValueChanged += VsPickerValueChanged;
            vsPicker.SaturationChanged -= VsPickerSaturationChanged;
            vsPicker.Saturation = (float)saturation;
            vsPicker.SaturationChanged += VsPickerSaturationChanged;

            hueNumeric.ValueChanged -= HueNumericValueChanged;
            hueNumeric.Value = Convert.ToInt32(hue * 359);
            hueNumeric.ValueChanged += HueNumericValueChanged;
            saturationNumeric.ValueChanged -= SaturationNumericValueChanged;
            saturationNumeric.Value = Convert.ToInt32(saturation * 255);
            saturationNumeric.ValueChanged += SaturationNumericValueChanged;
            valueNumeric.ValueChanged -= ValueNumericValueChanged;
            valueNumeric.Value = Convert.ToInt32(value * 255);
            valueNumeric.ValueChanged += ValueNumericValueChanged;

            rNumeric.ValueChanged -= rNumeric_ValueChanged;
            rNumeric.Value = rgbColor.R;
            rNumeric.ValueChanged += rNumeric_ValueChanged;
            gNumeric.ValueChanged -= gNumeric_ValueChanged;
            gNumeric.Value = rgbColor.G;
            gNumeric.ValueChanged += gNumeric_ValueChanged;
            bNumeric.ValueChanged -= bNumeric_ValueChanged;
            bNumeric.Value = rgbColor.B;
            bNumeric.ValueChanged += bNumeric_ValueChanged;

            color = Color.FromArgb((int)(alphaPicker.Alpha * 255), rgbColor);
            
            OnColorChanged(EventArgs.Empty);
        }

        private class ValueSaturationPicker : Button
        {
            private readonly Pen borderPen = new Pen(Color.White);
            private readonly Pen cursorPen = new Pen(Color.White);
            private readonly Bitmap image;

            private float value;
            private double hueValue;
            private bool mouseDown;
            private float saturation;

            public ValueSaturationPicker(int w, int h)
            {
                Size = new Size(w, h);

                image = new Bitmap(w, h);
                UpdateImage();

                MouseHook.MouseUp += MouseHookUpHandler;
            }

            public event EventHandler ValueChanged = delegate { };
            public event EventHandler SaturationChanged = delegate { };

            public float Value
            {
                get { return value; }
                set
                {
                    this.value = value;
                    ValueChanged(this, EventArgs.Empty);
                }
            }
            public float Saturation
            {
                get { return saturation; }
                set
                {
                    saturation = value;
                    SaturationChanged(this, EventArgs.Empty);
                }
            }

            public void SetHue(double value)
            {
                hueValue = value;
                UpdateImage();
            }

            protected override void Dispose(bool release_all)
            {
                MouseHook.MouseUp -= MouseHookUpHandler;

                base.Dispose(release_all);
            }
            protected override void OnMouseDown(MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                    mouseDown = true;
            }
            protected override void OnMouseMove(MouseEventArgs e)
            {
                if (mouseDown)
                {
                    Value = (float)(Height - e.Y) / Height;
                    Saturation = (float)e.X / Width;
                    if (Value < 0) Value = 0;
                    if (Value > 1) Value = 1;
                    if (Saturation < 0) Saturation = 0;
                    if (Saturation > 1) Saturation = 1;
                }
            }
            protected override void OnMouseUp(MouseEventArgs e)
            {
                UpdateValues();
                mouseDown = false;

                base.OnMouseUp(e);
            }
            protected override void OnPaint(PaintEventArgs e)
            {
                if (image != null)
                    e.Graphics.DrawImage(image, 0, 0, Width, Height);

                e.Graphics.DrawRectangle(cursorPen, Saturation * Width - 2, Height - Value * Height - 2, 4, 4);

                var borderColor = uwfBorderColor;
                if (uwfHovered) borderColor = uwfBorderHoverColor;
                borderPen.Color = borderColor;

                e.Graphics.DrawRectangle(borderPen, 0, 0, Width, Height);
            }

            private void MouseHookUpHandler(object sender, MouseEventArgs e)
            {
                if (mouseDown)
                    UpdateValues();

                mouseDown = false;
            }
            private void UpdateImage()
            {
                double hue = hueValue;
                double saturation = 0f;
                double value = 1f;

                var imageWidth = image.Width;
                var imageHeight = image.Height;

                for (int i = 0; i < imageWidth; i++)
                {
                    saturation = (float)i / imageWidth;

                    for (int k = 0; k < imageHeight; k++)
                    {
                        value = (float)k / imageHeight;

                        // HSL to RGB convertion.
                        Color pixelColor = ColorTranslatorEx.FromHSV(hue * 359, saturation, value);
                        image.SetPixel(i, k, pixelColor);
                    }
                }

                image.Apply();
            }
            private void UpdateValues()
            {
                var mLoc = PointToClient(MousePosition);

                float mX = mLoc.X;
                float mY = mLoc.Y;

                if (mX < 0) mX = 0;
                else if (mX > Width) mX = Width;

                if (mY < 0) mY = 0;
                else if (mY > Height) mY = Height;

                Value = (float)(Height - mY) / Height;
                Saturation = (float)mX / Width;
                if (Value < 0) Value = 0;
                if (Value > 1) Value = 1;
                if (Saturation < 0) Saturation = 0;
                if (Saturation > 1) Saturation = 1;
            }
        }

        private class HuePicker : Button
        {
            private readonly Pen borderPen = new Pen(Color.White);
            private readonly Pen cursorPen = new Pen(Color.White);
            private readonly Bitmap image;

            private double hue;
            private bool mouseDown;

            public HuePicker(int w, int h)
            {
                Size = new Size(w, h);

                image = new Bitmap(w, h);
                UpdateImage();

                MouseHook.MouseUp += UwfAppOwnerOnUpClick;
            }

            public event EventHandler HueChanged = delegate { };

            public double Hue
            {
                get { return hue; }
                set
                {
                    hue = value;
                    UpdateImage();
                    HueChanged(this, EventArgs.Empty);
                }
            }

            protected override void Dispose(bool release_all)
            {
                MouseHook.MouseUp -= UwfAppOwnerOnUpClick;

                base.Dispose(release_all);
            }
            protected override void OnMouseDown(MouseEventArgs e)
            {
                mouseDown = true;

                base.OnMouseDown(e);
            }
            protected override void OnMouseUp(MouseEventArgs e)
            {
                mouseDown = false;

                Hue = 1f - (float)e.Y / Height;
                if (Hue < 0) Hue = 0;
                if (Hue > 1) Hue = 1;

                base.OnMouseUp(e);
            }
            protected override void OnMouseMove(MouseEventArgs e)
            {
                if (mouseDown)
                {
                    Hue = 1f - (float)e.Y / Height;
                    if (Hue < 0) Hue = 0;
                    if (Hue > 1) Hue = 1;
                }

                base.OnMouseMove(e);
            }
            protected override void OnPaint(PaintEventArgs e)
            {
                var borderColor = uwfBorderColor;
                if (uwfHovered) borderColor = uwfBorderHoverColor;
                borderPen.Color = borderColor;

                if (image != null)
                    e.Graphics.DrawImage(image, 0, 0, Width, Height);

                e.Graphics.DrawLine(cursorPen, 0, (float)(1 - Hue) * Height, Width, (float)(1 - Hue) * Height);
                e.Graphics.DrawRectangle(borderPen, 0, 0, Width, Height);
            }

            private void UwfAppOwnerOnUpClick(object sender, MouseEventArgs mouseEventArgs)
            {
                mouseDown = false;
            }
            private void UpdateImage()
            {
                double hue = 0f;
                double saturation = .9f;
                double luminosity = .5f;

                var imageWidth = image.Width;
                var imageHeight = image.Height;

                for (int i = 0; i < imageWidth; i++)
                {
                    for (int k = 0; k < imageHeight; k++)
                    {
                        hue = (float)k / imageHeight;

                        // HSL to RGB convertion.
                        var pixelColor = ColorTranslatorEx.FromHsb(hue, saturation, luminosity);
                        image.SetPixel(i, k, pixelColor);
                    }
                }

                image.Apply();
            }
        }

        private class AlphaPicker : Button
        {
            private readonly Pen borderPen;
            private readonly Pen cursorPen;
            private readonly Bitmap image;

            private float alpha;
            private bool mouseDown;

            public AlphaPicker(int w, int h)
            {
                Alpha = 1;
                Size = new Size(w, h);

                borderPen = new Pen(uwfBorderColor);
                cursorPen = new Pen(Color.White);
                image = new Bitmap(w, h);
                for (int i = 0; i < w; i++)
                {
                    int rgb = (int)(((float)i / w) * 255);
                    var currentColor = Color.FromArgb(rgb, rgb, rgb);
                    for (int k = 0; k < h; k++)
                        image.SetPixel(i, k, currentColor);
                }
                image.Apply();
            }

            public event EventHandler AlphaChanged = delegate { };

            public float Alpha
            {
                get { return alpha; }
                set
                {
                    var changed = alpha != value;
                    if (changed)
                    {
                        alpha = value;
                        AlphaChanged(this, EventArgs.Empty);
                    }
                }
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                    mouseDown = true;
            }
            protected override void OnMouseMove(MouseEventArgs e)
            {
                if (!mouseDown) return;

                Alpha = (float)e.X / Width;
                if (Alpha < 0) Alpha = 0;
                if (Alpha > 1) Alpha = 1;
            }
            protected override void OnMouseUp(MouseEventArgs e)
            {
                var mclient = PointToClient(MousePosition);
                var x = mclient.X;
                if (x <= 0) x = 0;
                if (x > Width) x = Width;

                Alpha = (float)x / Width;
                if (Alpha < 0) Alpha = 0;
                if (Alpha > 1) Alpha = 1;
                mouseDown = false;

                base.OnMouseUp(e);
            }
            protected override void OnPaint(PaintEventArgs e)
            {
                var borderColor = uwfBorderColor;
                if (uwfHovered) borderColor = uwfBorderHoverColor;

                borderPen.Color = borderColor;

                if (image != null)
                    e.Graphics.DrawImage(image, 0, 0, Width, Height);

                e.Graphics.DrawLine(cursorPen, Alpha * Width, 0, Alpha * Width, Height);
                e.Graphics.DrawRectangle(borderPen, 0, 0, Width, Height);
            }
        }
    }
}