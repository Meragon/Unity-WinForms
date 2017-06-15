using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace System.Windows.Forms
{
    public class ColorPicker : Button
    {
        private readonly Pen _borderPen;
        private ColorPickerForm _currentForm;
        
        public Color Color { get; set; }

        public ColorPicker()
        {
            Color = Color.White;
            Size = new Size(128, 20);

            _borderPen = new Pen(BorderColor);
        }

        protected virtual void OnColorChanged(object sender, EventArgs e)
        {
            ColorChanged(this, EventArgs.Empty);
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (_currentForm == null)
            {
                _currentForm = new ColorPickerForm(this);
                _currentForm.Color = Color;
                _currentForm.ColorChanged += (object sender, EventArgs args) =>
                {
                    Color = ((ColorPickerForm)sender).Color;
                    OnColorChanged(this, args);
                };
                _currentForm.OnDisposing += (object sender, EventArgs args) =>
                {
                    _currentForm = null;
                };
                _currentForm.ShowDialog();
            }
            _currentForm.BringToFront();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            float alphaWidth = (float)Width * ((float)Color.A / 255);

            var borderColor = BorderColor;
            if (Hovered) borderColor = BorderHoverColor;
            _borderPen.Color = borderColor;

            e.Graphics.uwfFillRectangle(Color.FromArgb(255, Color), 0, 0, Width, Height - 3);
            e.Graphics.uwfFillRectangle(Color.Black, 0, Height - 3, Width, 3);
            e.Graphics.uwfFillRectangle(Color.White, 0, Height - 3, alphaWidth, 3);
            e.Graphics.DrawRectangle(_borderPen, 0, 0, Width, Height);
        }

        public event EventHandler ColorChanged = delegate { };
    }

    public class ColorPickerForm : Form
    {
        private ColorPicker _owner;

        private Color _color;

        private readonly AlphaPicker _alphaPicker;
        private readonly BrightnessSaturationPicker _bsPicker;
        private readonly HuePicker _huePicker;

        private readonly Label _aLabel;

        private readonly NumericUpDown _hNumeric;
        private readonly NumericUpDown _sNumeric;
        private readonly NumericUpDown _lNumeric;

        private readonly NumericUpDown _rNumeric;
        private readonly NumericUpDown _gNumeric;
        private readonly NumericUpDown _bNumeric;

        private readonly NumericUpDown _aNumeric;

        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;

                _rNumeric.ValueChanged -= _rNumeric_ValueChanged;
                _rNumeric.Value = _color.R;
                _rNumeric.ValueChanged += _rNumeric_ValueChanged;

                _gNumeric.ValueChanged -= _gNumeric_ValueChanged;
                _gNumeric.Value = _color.G;
                _gNumeric.ValueChanged += _gNumeric_ValueChanged;

                _bNumeric.ValueChanged -= _bNumeric_ValueChanged;
                _bNumeric.Value = _color.B;
                _bNumeric.ValueChanged += _bNumeric_ValueChanged;

                _alphaPicker.Alpha = (float)_color.A / 255;

                UpdateControlsFromRGB();
            }
        }

        public ColorPickerForm(ColorPicker owner)
        {
            _owner = owner;

            BackColor = Color.White;
            Size = new Size(188, 272);
            Location = new Point(
                Screen.PrimaryScreen.WorkingArea.Width / 2 - Width / 2,
                Screen.PrimaryScreen.WorkingArea.Height / 2 - Height / 2);
            uwfResizable = false;
            Text = "Pick a color";
            KeyPreview = true;
            TopMost = true;

            _bsPicker = new BrightnessSaturationPicker(128, 128);
            _bsPicker.Location = new Point(16, 32);
            _bsPicker.BrightnessChanged += _bsPicker_BrightnessChanged;
            _bsPicker.SaturationChanged += _bsPicker_SaturationChanged;

            Controls.Add(_bsPicker);

            _huePicker = new HuePicker(20, 128);
            _huePicker.Location = new Point(_bsPicker.Location.X + _bsPicker.Width + 8, _bsPicker.Location.Y);
            _huePicker.HueChanged += _huePicker_HueChanged;

            Controls.Add(_huePicker);

            var hLabel = new Label();
            hLabel.Text = "H:";
            hLabel.Location = new Point(_bsPicker.Location.X, _bsPicker.Location.Y + _bsPicker.Height + 8);
            var sLabel = new Label();
            sLabel.Text = "S:";
            sLabel.Location = new Point(hLabel.Location.X, hLabel.Location.Y + 22);
            var lLabel = new Label();
            lLabel.Text = "L:";
            lLabel.Location = new Point(hLabel.Location.X, sLabel.Location.Y + 22);

            Controls.Add(hLabel);
            Controls.Add(sLabel);
            Controls.Add(lLabel);

            _hNumeric = new NumericUpDown();
            _hNumeric.Minimum = 0;
            _hNumeric.Maximum = 360;
            _hNumeric.Location = new Point(hLabel.Location.X + 24, hLabel.Location.Y);
            _hNumeric.Size = new Drawing.Size(50, 20);
            _hNumeric.ValueChanged += _hNumeric_ValueChanged;
            _hNumeric.TextAlign = HorizontalAlignment.Center;
            _sNumeric = new NumericUpDown();
            _sNumeric.Minimum = 0;
            _sNumeric.Maximum = 255;
            _sNumeric.Location = new Point(sLabel.Location.X + 24, sLabel.Location.Y);
            _sNumeric.Size = new Drawing.Size(50, 20);
            _sNumeric.ValueChanged += _sNumeric_ValueChanged;
            _sNumeric.TextAlign = HorizontalAlignment.Center;
            _lNumeric = new NumericUpDown();
            _lNumeric.Minimum = 0;
            _lNumeric.Maximum = 255;
            _lNumeric.Location = new Point(lLabel.Location.X + 24, lLabel.Location.Y);
            _lNumeric.Size = new Drawing.Size(50, 20);
            _lNumeric.ValueChanged += _lNumeric_ValueChanged;
            _lNumeric.TextAlign = HorizontalAlignment.Center;

            Controls.Add(_hNumeric);
            Controls.Add(_sNumeric);
            Controls.Add(_lNumeric);

            var rLabel = new Label();
            rLabel.Text = "R:";
            rLabel.Location = new Point(_hNumeric.Location.X + _hNumeric.Width + 8, hLabel.Location.Y);
            var gLabel = new Label();
            gLabel.Text = "G:";
            gLabel.Location = new Point(rLabel.Location.X, sLabel.Location.Y);
            var bLabel = new Label();
            bLabel.Text = "B:";
            bLabel.Location = new Point(rLabel.Location.X, lLabel.Location.Y);

            Controls.Add(rLabel);
            Controls.Add(gLabel);
            Controls.Add(bLabel);

            _rNumeric = new NumericUpDown();
            _rNumeric.Minimum = 0;
            _rNumeric.Maximum = 255;
            _rNumeric.Location = new Point(rLabel.Location.X + 24, rLabel.Location.Y);
            _rNumeric.Size = new Size(50, 20);
            _rNumeric.TextAlign = HorizontalAlignment.Center;
            _rNumeric.ValueChanged += _rNumeric_ValueChanged;
            _gNumeric = new NumericUpDown();
            _gNumeric.Minimum = 0;
            _gNumeric.Maximum = 255;
            _gNumeric.Location = new Point(gLabel.Location.X + 24, gLabel.Location.Y);
            _gNumeric.Size = new Drawing.Size(50, 20);
            _gNumeric.TextAlign = HorizontalAlignment.Center;
            _gNumeric.ValueChanged += _gNumeric_ValueChanged;
            _bNumeric = new NumericUpDown();
            _bNumeric.Minimum = 0;
            _bNumeric.Maximum = 255;
            _bNumeric.Location = new Point(bLabel.Location.X + 24, bLabel.Location.Y);
            _bNumeric.Size = new Drawing.Size(50, 20);
            _bNumeric.TextAlign = HorizontalAlignment.Center;
            _bNumeric.ValueChanged += _bNumeric_ValueChanged;

            Controls.Add(_rNumeric);
            Controls.Add(_gNumeric);
            Controls.Add(_bNumeric);

            _alphaPicker = new AlphaPicker(_lNumeric.Location.X + _lNumeric.Width - lLabel.Location.X, 20);
            _alphaPicker.Location = new Point(lLabel.Location.X, lLabel.Location.Y + 26);
            _alphaPicker.AlphaChanged += _alphaPicker_AlphaChanged;
            _aLabel = new Label();
            _aLabel.Location = new Point(bLabel.Location.X, _alphaPicker.Location.Y);
            _aLabel.Text = "A:";
            _aNumeric = new NumericUpDown();
            _aNumeric.Minimum = 0;
            _aNumeric.Maximum = 255;
            _aNumeric.Value = 255;
            _aNumeric.Location = new Point(_bNumeric.Location.X, _aLabel.Location.Y);
            _aNumeric.Size = new Drawing.Size(50, 20);
            _aNumeric.TextAlign = HorizontalAlignment.Center;
            _aNumeric.ValueChanged += _aNumeric_ValueChanged;

            Controls.Add(_alphaPicker);
            Controls.Add(_aLabel);
            Controls.Add(_aNumeric);
        }

        private void _bsPicker_BrightnessChanged(object sender, EventArgs e)
        {
            _lNumeric.ValueChanged -= _lNumeric_ValueChanged;
            _lNumeric.Value = (int)(_bsPicker.Brightness * 255);
            _lNumeric.ValueChanged += _lNumeric_ValueChanged;

            Color rgbColor = Color.FromHsb(_huePicker.Hue, _bsPicker.Saturation, _bsPicker.Brightness);

            _rNumeric.ValueChanged -= _rNumeric_ValueChanged;
            _rNumeric.Value = rgbColor.R;
            _rNumeric.ValueChanged += _rNumeric_ValueChanged;
            _gNumeric.ValueChanged -= _gNumeric_ValueChanged;
            _gNumeric.Value = rgbColor.G;
            _gNumeric.ValueChanged += _gNumeric_ValueChanged;
            _bNumeric.ValueChanged -= _bNumeric_ValueChanged;
            _bNumeric.Value = rgbColor.B;
            _bNumeric.ValueChanged += _bNumeric_ValueChanged;

            _color = Color.FromArgb((int)(_alphaPicker.Alpha * 255), rgbColor);
            ColorChanged(this, null);
        }
        private void _bsPicker_SaturationChanged(object sender, EventArgs e)
        {
            _sNumeric.ValueChanged -= _sNumeric_ValueChanged;
            _sNumeric.Value = (int)(_bsPicker.Saturation * 255);
            _sNumeric.ValueChanged += _sNumeric_ValueChanged;

            Color rgbColor = Color.FromHsb(_huePicker.Hue, _bsPicker.Saturation, _bsPicker.Brightness);

            _rNumeric.ValueChanged -= _rNumeric_ValueChanged;
            _rNumeric.Value = rgbColor.R;
            _rNumeric.ValueChanged += _rNumeric_ValueChanged;
            _gNumeric.ValueChanged -= _gNumeric_ValueChanged;
            _gNumeric.Value = rgbColor.G;
            _gNumeric.ValueChanged += _gNumeric_ValueChanged;
            _bNumeric.ValueChanged -= _bNumeric_ValueChanged;
            _bNumeric.Value = rgbColor.B;
            _bNumeric.ValueChanged += _bNumeric_ValueChanged;

            _color = Color.FromArgb((int)(_alphaPicker.Alpha * 255), rgbColor); ;
            ColorChanged(this, null);
        }
        private void _huePicker_HueChanged(object sender, EventArgs e)
        {
            _bsPicker.SetHue(_huePicker.Hue);

            _hNumeric.ValueChanged -= _hNumeric_ValueChanged;
            _hNumeric.Value = (int)(_huePicker.Hue * 255);
            _hNumeric.ValueChanged += _hNumeric_ValueChanged;

            Color rgbColor = Color.FromHsb(_huePicker.Hue, _bsPicker.Saturation, _bsPicker.Brightness);

            _rNumeric.ValueChanged -= _rNumeric_ValueChanged;
            _rNumeric.Value = rgbColor.R;
            _rNumeric.ValueChanged += _rNumeric_ValueChanged;
            _gNumeric.ValueChanged -= _gNumeric_ValueChanged;
            _gNumeric.Value = rgbColor.G;
            _gNumeric.ValueChanged += _gNumeric_ValueChanged;
            _bNumeric.ValueChanged -= _bNumeric_ValueChanged;
            _bNumeric.Value = rgbColor.B;
            _bNumeric.ValueChanged += _bNumeric_ValueChanged;

            _color = Color.FromArgb((int)(_alphaPicker.Alpha * 255), rgbColor); ; ;
            ColorChanged(this, null);
        }

        private void _hNumeric_ValueChanged(object sender, EventArgs e)
        {
            _huePicker.Hue = (float)_hNumeric.Value / 255;
        }
        private void _sNumeric_ValueChanged(object sender, EventArgs e)
        {
            _bsPicker.Saturation = (float)_sNumeric.Value / 255;
        }
        private void _lNumeric_ValueChanged(object sender, EventArgs e)
        {
            _bsPicker.Brightness = (float)_lNumeric.Value / 255;
        }

        private void _rNumeric_ValueChanged(object sender, EventArgs e)
        {
            UpdateControlsFromRGB();
        }
        private void _gNumeric_ValueChanged(object sender, EventArgs e)
        {
            UpdateControlsFromRGB();
        }
        private void _bNumeric_ValueChanged(object sender, EventArgs e)
        {
            UpdateControlsFromRGB();
        }

        private void _alphaPicker_AlphaChanged(object sender, EventArgs e)
        {
            _aNumeric.ValueChanged -= _aNumeric_ValueChanged;
            _aNumeric.Value = (int)(_alphaPicker.Alpha * 255);
            _aNumeric.ValueChanged += _aNumeric_ValueChanged;

            _color = Color.FromArgb((int)(_alphaPicker.Alpha * 255), Color);
            ColorChanged(this, null);
        }
        private void _aNumeric_ValueChanged(object sender, EventArgs e)
        {
            _alphaPicker.AlphaChanged -= _alphaPicker_AlphaChanged;
            _alphaPicker.Alpha = (float)Convert.ToDouble(_aNumeric.Value / 255);
            _alphaPicker.AlphaChanged += _alphaPicker_AlphaChanged;

            _color = Color.FromArgb((int)(_alphaPicker.Alpha * 255), Color);
            ColorChanged(this, null);
        }
        private void UpdateControlsFromRGB()
        {
            Color rgbColor = Color.FromArgb(Convert.ToInt32(_rNumeric.Value), Convert.ToInt32(_gNumeric.Value), Convert.ToInt32(_bNumeric.Value));

            float hue = rgbColor.GetHue() / 360;

            _huePicker.HueChanged -= _huePicker_HueChanged;
            _huePicker.Hue = hue;
            _huePicker.HueChanged += _huePicker_HueChanged;
            _bsPicker.SetHue(hue);
            _bsPicker.BrightnessChanged -= _bsPicker_BrightnessChanged;
            _bsPicker.Brightness = rgbColor.GetBrightness();
            _bsPicker.BrightnessChanged += _bsPicker_BrightnessChanged;
            _bsPicker.SaturationChanged -= _bsPicker_SaturationChanged;
            _bsPicker.Saturation = rgbColor.GetSaturation();
            _bsPicker.SaturationChanged += _bsPicker_SaturationChanged;

            _hNumeric.ValueChanged -= _hNumeric_ValueChanged;
            _hNumeric.Value = Convert.ToInt32(rgbColor.GetHue());
            _hNumeric.ValueChanged += _hNumeric_ValueChanged;
            _sNumeric.ValueChanged -= _sNumeric_ValueChanged;
            _sNumeric.Value = Convert.ToInt32(rgbColor.GetSaturation() * 255);
            _sNumeric.ValueChanged += _sNumeric_ValueChanged;
            _lNumeric.ValueChanged -= _lNumeric_ValueChanged;
            _lNumeric.Value = Convert.ToInt32(rgbColor.GetBrightness() * 255);
            _lNumeric.ValueChanged += _lNumeric_ValueChanged;

            _rNumeric.ValueChanged -= _rNumeric_ValueChanged;
            _rNumeric.Value = rgbColor.R;
            _rNumeric.ValueChanged += _rNumeric_ValueChanged;
            _gNumeric.ValueChanged -= _gNumeric_ValueChanged;
            _gNumeric.Value = rgbColor.G;
            _gNumeric.ValueChanged += _gNumeric_ValueChanged;
            _bNumeric.ValueChanged -= _bNumeric_ValueChanged;
            _bNumeric.Value = rgbColor.B;
            _bNumeric.ValueChanged += _bNumeric_ValueChanged;

            _color = Color.FromArgb((int)(_alphaPicker.Alpha * 255), rgbColor);
            ColorChanged(this, null);
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

        public event EventHandler ColorChanged = delegate { };

        private class BrightnessSaturationPicker : Button
        {
            private float _brightness;
            private readonly Bitmap _image;
            private float _hueValue;
            private bool _mouseDown;
            private float _saturation;

            public float Brightness
            {
                get { return _brightness; }
                set
                {
                    _brightness = value;
                    BrightnessChanged(this, null);
                }
            }
            public float Saturation
            {
                get { return _saturation; }
                set
                {
                    _saturation = value;
                    SaturationChanged(this, null);
                }
            }

            public BrightnessSaturationPicker(int w, int h)
            {
                Size = new Size(w, h);

                _image = new Bitmap(w, h);
                _UpdateImage();

                uwfAppOwner.UpClick += Application_UpClick;
            }

            void Application_UpClick(object sender, MouseEventArgs e)
            {
                if (_mouseDown)
                    UpdateValues();

                _mouseDown = false;
            }

            private void _UpdateImage()
            {
                double hue = _hueValue;
                double saturation = 0f;
                double luminosity = 1f;

                for (int i = 0; i < _image.Width; i++)
                {
                    saturation = ((float)i / _image.Width);

                    for (int k = 0; k < _image.Height; k++)
                    {
                        luminosity = 1f - (float)k / _image.Height;

                        // HSL to RGB convertion.
                        Color pixelColor = Color.FromHsb(hue, saturation, luminosity);
                        _image.SetPixel(i, k, pixelColor);
                    }
                    _image.Apply();
                }
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

                Brightness = (float)(Height - mY) / Height;
                Saturation = (float)mX / Width;
                if (Brightness < 0) Brightness = 0;
                if (Brightness > 1) Brightness = 1;
                if (Saturation < 0) Saturation = 0;
                if (Saturation > 1) Saturation = 1;
            }

            public void SetHue(float value)
            {
                _hueValue = value;
                _UpdateImage();
            }

            protected override void Dispose(bool release_all)
            {
                uwfAppOwner.UpClick -= Application_UpClick;
                base.Dispose(release_all);
            }
            protected override void OnMouseDown(MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                    _mouseDown = true;
            }
            protected override void OnMouseMove(MouseEventArgs e)
            {
                if (_mouseDown)
                {
                    Brightness = (float)(Height - e.Y) / Height;
                    Saturation = (float)e.X / Width;
                    if (Brightness < 0) Brightness = 0;
                    if (Brightness > 1) Brightness = 1;
                    if (Saturation < 0) Saturation = 0;
                    if (Saturation > 1) Saturation = 1;
                }
            }
            protected override void OnMouseUp(MouseEventArgs e)
            {
                UpdateValues();
                _mouseDown = false;
            }
            protected override void OnPaint(PaintEventArgs e)
            {
                if (_image != null)
                    e.Graphics.DrawImage(_image, 0, 0, Width, Height);

                e.Graphics.DrawRectangle(new Pen(Color.White), Saturation * Width - 2, Height - Brightness * Height - 2, 4, 4);

                var borderColor = BorderColor;
                if (Hovered) borderColor = BorderHoverColor;
                e.Graphics.DrawRectangle(new Pen(borderColor), 0, 0, Width, Height);
            }

            public event EventHandler BrightnessChanged = delegate { };
            public event EventHandler SaturationChanged = delegate { };
        }

        private class HuePicker : Button
        {
            private readonly Bitmap _image;
            private float _hue;

            public float Hue
            {
                get { return _hue; }
                set
                {
                    _hue = value;
                    _UpdateImage();
                    HueChanged(this, null);
                }
            }

            public HuePicker(int w, int h)
            {
                Size = new Size(w, h);

                _image = new Bitmap(w, h);
                _UpdateImage();
            }
            private void _UpdateImage()
            {
                double hue = 0f;
                double saturation = .9f;
                double luminosity = .5f;

                for (int i = 0; i < _image.Width; i++)
                {
                    for (int k = 0; k < _image.Height; k++)
                    {
                        hue = (float)k / _image.Height;

                        // HSL to RGB convertion.
                        var pixelColor = Color.FromHsb(hue, saturation, luminosity);
                        _image.SetPixel(i, k, pixelColor);
                    }
                    _image.Apply();
                }
            }
            
            protected override void OnMouseUp(MouseEventArgs e)
            {
                Hue = (float)e.Y / Height;
                if (Hue < 0) Hue = 0;
                if (Hue > 1) Hue = 1;
            }
            protected override void OnPaint(PaintEventArgs e)
            {
                if (_image != null)
                    e.Graphics.DrawImage(_image, 0, 0, Width, Height);

                e.Graphics.DrawLine(new Pen(Color.White), 0, Hue * Height, Width, Hue * Height);

                var borderColor = BorderColor;
                if (Hovered) borderColor = BorderHoverColor;
                e.Graphics.DrawRectangle(new Pen(borderColor), 0, 0, Width, Height);
            }

            public event EventHandler HueChanged = delegate { };
        }

        private class AlphaPicker : Button
        {
            private float _alpha;
            private readonly Pen _borderPen;
            private readonly Pen _cursorPen;
            private readonly Bitmap _image;
            private bool _mouseDown;

            public float Alpha
            {
                get { return _alpha; }
                set
                {
                    var changed = _alpha != value;
                    if (changed)
                    {
                        _alpha = value;
                        AlphaChanged(this, null);
                    }
                }
            }

            public AlphaPicker(int w, int h)
            {
                Alpha = 1;
                Size = new Size(w, h);

                _borderPen = new Pen(BorderColor);
                _cursorPen = new Pen(Color.White);
                _image = new Bitmap(w, h);
                for (int i = 0; i < w; i++)
                {
                    int rgb = (int)(((float)i / w) * 255);
                    var currentColor = Color.FromArgb(rgb, rgb, rgb);
                    for (int k = 0; k < h; k++)
                        _image.SetPixel(i, k, currentColor);
                }
                _image.Apply();
            }
            
            protected override void OnMouseDown(MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                    _mouseDown = true;
            }
            protected override void OnMouseMove(MouseEventArgs e)
            {
                if (!_mouseDown) return;

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
                _mouseDown = false;
            }
            protected override void OnPaint(PaintEventArgs e)
            {
                var borderColor = BorderColor;
                if (Hovered) borderColor = BorderHoverColor;

                _borderPen.Color = borderColor;

                if (_image != null)
                    e.Graphics.DrawImage(_image, 0, 0, Width, Height);

                e.Graphics.DrawLine(_cursorPen, Alpha * Width, 0, Alpha * Width, Height);
                e.Graphics.DrawRectangle(_borderPen, 0, 0, Width, Height);
            }

            public event EventHandler AlphaChanged = delegate { };
        }
    }
}
