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
        private ColorPickerForm _currentForm;
        
        public Color Color { get; set; }

        public ColorPicker()
        {
            Color = Color.White;
            Size = new Size(128, 20);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (_currentForm == null)
            {
                _currentForm = new ColorPickerForm(this);
                _currentForm.Color = Color;
                _currentForm.ColorChanged += (object sender, EventArgs args) =>
                {
                    Color = _currentForm.Color;
                    ColorChanged(this, null);
                };
                _currentForm.OnDisposing += (object sender, EventArgs args) =>
                {
                    _currentForm = null;
                };
                _currentForm.Show();
            }
            _currentForm.BringToFront();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(255, Color)), 0, 0, Width, Height - 3);

            float alphaWidth = (float)Width * ((float)Color.A / 255);
            e.Graphics.FillRectangle(new SolidBrush(Color.Black), 0, Height - 3, Width, 3);
            e.Graphics.FillRectangle(new SolidBrush(Color.White), 0, Height - 3, alphaWidth, 3);

            var borderColor = BorderColor;
            if (Hovered) borderColor = BorderHoverColor;
            e.Graphics.DrawRectangle(new Pen(borderColor), 0, 0, Width, Height);
        }

        public event EventHandler ColorChanged = delegate { };
    }

    public class ColorPickerForm : Form
    {
        private ColorPicker _owner;

        private Color _color;

        private AlphaPicker _alphaPicker;
        private BrightnessSaturationPicker _bsPicker;
        private HuePicker _huePicker;

        private Label _hLabel;
        private Label _sLabel;
        private Label _lLabel;

        private Label _rLabel;
        private Label _gLabel;
        private Label _bLabel;

        private Label _aLabel;

        private NumericUpDown _hNumeric;
        private NumericUpDown _sNumeric;
        private NumericUpDown _lNumeric;

        private NumericUpDown _rNumeric;
        private NumericUpDown _gNumeric;
        private NumericUpDown _bNumeric;

        private NumericUpDown _aNumeric;

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

            Size = new Size(188, 264);
            Location = new Point(
                Screen.PrimaryScreen.WorkingArea.Width / 2 - Width / 2,
                Screen.PrimaryScreen.WorkingArea.Height / 2 - Height / 2);
            Resizable = false;
            Text = "Pick a color";
            TopMost = true;

            _bsPicker = new BrightnessSaturationPicker(128, 128);
            _bsPicker.Location = new Point(16, 24);
            _bsPicker.BrightnessChanged += _bsPicker_BrightnessChanged;
            _bsPicker.SaturationChanged += _bsPicker_SaturationChanged;

            Controls.Add(_bsPicker);

            _huePicker = new HuePicker(20, 128);
            _huePicker.Location = new Point(_bsPicker.Location.X + _bsPicker.Width + 8, _bsPicker.Location.Y);
            _huePicker.HueChanged += _huePicker_HueChanged;

            Controls.Add(_huePicker);

            _hLabel = new Label();
            _hLabel.Text = "H:";
            _hLabel.Location = new Point(_bsPicker.Location.X, _bsPicker.Location.Y + _bsPicker.Height + 8);
            _sLabel = new Label();
            _sLabel.Text = "S:";
            _sLabel.Location = new Point(_hLabel.Location.X, _hLabel.Location.Y + 22);
            _lLabel = new Label();
            _lLabel.Text = "L:";
            _lLabel.Location = new Point(_hLabel.Location.X, _sLabel.Location.Y + 22);

            Controls.Add(_hLabel);
            Controls.Add(_sLabel);
            Controls.Add(_lLabel);

            _hNumeric = new NumericUpDown();
            _hNumeric.Minimum = 0;
            _hNumeric.Maximum = 360;
            _hNumeric.Location = new Point(_hLabel.Location.X + 24, _hLabel.Location.Y);
            _hNumeric.Size = new Drawing.Size(50, 20);
            _hNumeric.ValueChanged += _hNumeric_ValueChanged;
            _hNumeric.TextAlign = HorizontalAlignment.Center;
            _sNumeric = new NumericUpDown();
            _sNumeric.Minimum = 0;
            _sNumeric.Maximum = 255;
            _sNumeric.Location = new Point(_sLabel.Location.X + 24, _sLabel.Location.Y);
            _sNumeric.Size = new Drawing.Size(50, 20);
            _sNumeric.ValueChanged += _sNumeric_ValueChanged;
            _sNumeric.TextAlign = HorizontalAlignment.Center;
            _lNumeric = new NumericUpDown();
            _lNumeric.Minimum = 0;
            _lNumeric.Maximum = 255;
            _lNumeric.Location = new Point(_lLabel.Location.X + 24, _lLabel.Location.Y);
            _lNumeric.Size = new Drawing.Size(50, 20);
            _lNumeric.ValueChanged += _lNumeric_ValueChanged;
            _lNumeric.TextAlign = HorizontalAlignment.Center;

            Controls.Add(_hNumeric);
            Controls.Add(_sNumeric);
            Controls.Add(_lNumeric);

            _rLabel = new Label();
            _rLabel.Text = "R:";
            _rLabel.Location = new Point(_hNumeric.Location.X + _hNumeric.Width + 8, _hLabel.Location.Y);
            _gLabel = new Label();
            _gLabel.Text = "G:";
            _gLabel.Location = new Point(_rLabel.Location.X, _sLabel.Location.Y);
            _bLabel = new Label();
            _bLabel.Text = "B:";
            _bLabel.Location = new Point(_rLabel.Location.X, _lLabel.Location.Y);

            Controls.Add(_rLabel);
            Controls.Add(_gLabel);
            Controls.Add(_bLabel);

            _rNumeric = new NumericUpDown();
            _rNumeric.Minimum = 0;
            _rNumeric.Maximum = 255;
            _rNumeric.Location = new Point(_rLabel.Location.X + 24, _rLabel.Location.Y);
            _rNumeric.Size = new Size(50, 20);
            _rNumeric.TextAlign = HorizontalAlignment.Center;
            _rNumeric.ValueChanged += _rNumeric_ValueChanged;
            _gNumeric = new NumericUpDown();
            _gNumeric.Minimum = 0;
            _gNumeric.Maximum = 255;
            _gNumeric.Location = new Point(_gLabel.Location.X + 24, _gLabel.Location.Y);
            _gNumeric.Size = new Drawing.Size(50, 20);
            _gNumeric.TextAlign = HorizontalAlignment.Center;
            _gNumeric.ValueChanged += _gNumeric_ValueChanged;
            _bNumeric = new NumericUpDown();
            _bNumeric.Minimum = 0;
            _bNumeric.Maximum = 255;
            _bNumeric.Location = new Point(_bLabel.Location.X + 24, _bLabel.Location.Y);
            _bNumeric.Size = new Drawing.Size(50, 20);
            _bNumeric.TextAlign = HorizontalAlignment.Center;
            _bNumeric.ValueChanged += _bNumeric_ValueChanged;

            Controls.Add(_rNumeric);
            Controls.Add(_gNumeric);
            Controls.Add(_bNumeric);

            _alphaPicker = new AlphaPicker(_lNumeric.Location.X + _lNumeric.Width - _lLabel.Location.X, 20);
            _alphaPicker.Location = new Point(_lLabel.Location.X, _lLabel.Location.Y + 26);
            _alphaPicker.AlphaChanged += _alphaPicker_AlphaChanged;
            _aLabel = new Label();
            _aLabel.Location = new Point(_bLabel.Location.X, _alphaPicker.Location.Y);
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

        void _bsPicker_BrightnessChanged(object sender, EventArgs e)
        {
            _lNumeric.ValueChanged -= _lNumeric_ValueChanged;
            _lNumeric.Value = (int)(_bsPicker.Brightness * 255);
            _lNumeric.ValueChanged += _lNumeric_ValueChanged;

            Color rgbColor = GetColorFromHSL(_huePicker.Hue, _bsPicker.Saturation, _bsPicker.Brightness);

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
        void _bsPicker_SaturationChanged(object sender, EventArgs e)
        {
            _sNumeric.ValueChanged -= _sNumeric_ValueChanged;
            _sNumeric.Value = (int)(_bsPicker.Saturation * 255);
            _sNumeric.ValueChanged += _sNumeric_ValueChanged;

            Color rgbColor = GetColorFromHSL(_huePicker.Hue, _bsPicker.Saturation, _bsPicker.Brightness);

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
        void _huePicker_HueChanged(object sender, EventArgs e)
        {
            _bsPicker.SetHue(_huePicker.Hue);

            _hNumeric.ValueChanged -= _hNumeric_ValueChanged;
            _hNumeric.Value = (int)(_huePicker.Hue * 255);
            _hNumeric.ValueChanged += _hNumeric_ValueChanged;

            Color rgbColor = GetColorFromHSL(_huePicker.Hue, _bsPicker.Saturation, _bsPicker.Brightness);

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

        void _alphaPicker_AlphaChanged(object sender, EventArgs e)
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
        private void UpdateBSTexture()
        {

        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == UnityEngine.KeyCode.Escape ||
                (e.KeyCode == UnityEngine.KeyCode.W && e.Modifiers == UnityEngine.EventModifiers.Control))
                Close();
        }

        public event EventHandler ColorChanged = delegate { };

        private class BrightnessSaturationPicker : Button
        {
            private float _brightness;
            private UnityEngine.Texture2D _image;
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

                _image = new UnityEngine.Texture2D(w, h);
                _UpdateImage();

                Owner.UpClick += Application_UpClick;
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

                for (int i = 0; i < _image.width; i++)
                {
                    saturation = ((float)i / _image.width);

                    for (int k = 0; k < _image.height; k++)
                    {
                        luminosity = (float)k / _image.height;

                        // HSL to RGB convertion.
                        Color pixelColor = GetColorFromHSL(hue, saturation, luminosity);
                        _image.SetPixel(i, k, pixelColor.ToUColor());
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

            public override void Dispose()
            {
                Owner.UpClick -= Application_UpClick;
                base.Dispose();
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
                    e.Graphics.DrawTexture(_image, 0, 0, Width, Height);

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
            private UnityEngine.Texture2D _image;
            private float _hue;
            private bool _mouseDown;

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

                _image = new UnityEngine.Texture2D(w, h);
                _UpdateImage();

                Owner.UpClick += Application_UpClick;
            }

            void Application_UpClick(object sender, MouseEventArgs e)
            {
                _mouseDown = false;
            }

            private void _UpdateImage()
            {
                double hue = 0f;
                double saturation = .9f;
                double luminosity = .5f;

                for (int i = 0; i < _image.width; i++)
                {
                    for (int k = 0; k < _image.height; k++)
                    {
                        hue = (float)(_image.height - k - 1) / _image.height;

                        // HSL to RGB convertion.
                        Color pixelColor = GetColorFromHSL(hue, saturation, luminosity);
                        _image.SetPixel(i, k, pixelColor.ToUColor());
                    }
                    _image.Apply();
                }
            }

            public override void Dispose()
            {
                Owner.UpClick -= Application_UpClick;
                base.Dispose();
            }
            protected override void OnMouseDown(MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                    _mouseDown = true;
            }
            protected override void OnMouseMove(MouseEventArgs e)
            {
                // Pretty slow.

                /*if (_mouseDown)
                {
                    Hue = (float)e.Y / Height;
                    if (Hue < 0) Hue = 0;
                    if (Hue > 1) Hue = 1;
                }*/
            }
            protected override void OnMouseUp(MouseEventArgs e)
            {
                Hue = (float)e.Y / Height;
                if (Hue < 0) Hue = 0;
                if (Hue > 1) Hue = 1;
                _mouseDown = false;
            }
            protected override void OnPaint(PaintEventArgs e)
            {
                if (_image != null)
                    e.Graphics.DrawTexture(_image, 0, 0, Width, Height);

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
            private UnityEngine.Texture2D _image;
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

                _image = new UnityEngine.Texture2D(w, h);
                for (int i = 0; i < w; i++)
                {
                    int rgb = (int)(((float)i / w) * 255);
                    Color currentColor = Color.FromArgb(rgb, rgb, rgb);
                    for (int k = 0; k < h; k++)
                    {
                        _image.SetPixel(i, k, currentColor.ToUColor());
                    }
                }
                _image.Apply();

                Owner.UpClick += Application_UpClick;
            }

            void Application_UpClick(object sender, MouseEventArgs e)
            {
                if (_mouseDown)
                {
                    var mX = PointToClient(MousePosition).X;

                    float _Alpha = 0;
                    if (mX < 0) _Alpha = 0;
                    else if (mX > Width) _Alpha = 1;
                    else _Alpha = (float)mX / Width;

                    Alpha = _Alpha;
                }
                _mouseDown = false;
            }

            public override void Dispose()
            {
                Owner.UpClick -= Application_UpClick;
                base.Dispose();
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
                    Alpha = (float)e.X / Width;
                    if (Alpha < 0) Alpha = 0;
                    if (Alpha > 1) Alpha = 1;
                }
            }
            protected override void OnMouseUp(MouseEventArgs e)
            {
                Alpha = (float)e.X / Width;
                if (Alpha < 0) Alpha = 0;
                if (Alpha > 1) Alpha = 1;
                _mouseDown = false;
            }
            protected override void OnPaint(PaintEventArgs e)
            {
                if (_image != null)
                    e.Graphics.DrawTexture(_image, 0, 0, Width, Height);

                e.Graphics.DrawLine(new Pen(Color.White), Alpha * Width, 0, Alpha * Width, Height);

                var borderColor = BorderColor;
                if (Hovered) borderColor = BorderHoverColor;
                e.Graphics.DrawRectangle(new Pen(borderColor), 0, 0, Width, Height);
            }

            public event EventHandler AlphaChanged = delegate { };
        }

        public static Color GetColorFromHSL(double hue, double saturation, double luminosity)
        {
            double r = 0, g = 0, b = 0;
            if (luminosity != 0)
            {
                if (saturation == 0)
                    r = g = b = luminosity;
                else
                {
                    double temp2 = _GetTemp2(hue, saturation, luminosity);
                    double temp1 = 2.0f * luminosity - temp2;

                    r = _GetColorComponent(temp1, temp2, hue + 1.0f / 3.0f);
                    g = _GetColorComponent(temp1, temp2, hue);
                    b = _GetColorComponent(temp1, temp2, hue - 1.0f / 3.0f);
                }
            }
            return Color.FromArgb((int)(255 * r), (int)(255 * g), (int)(255 * b));
        }
        private static double _GetColorComponent(double temp1, double temp2, double temp3)
        {
            temp3 = _MoveIntoRange(temp3);
            if (temp3 < 1.0f / 6.0f)
                return temp1 + (temp2 - temp1) * 6.0f * temp3;
            else if (temp3 < 0.5f)
                return temp2;
            else if (temp3 < 2.0f / 3.0f)
                return temp1 + ((temp2 - temp1) * ((2.0f / 3.0f) - temp3) * 6.0f);
            else
                return temp1;
        }
        private static double _GetTemp2(double h, double s, double l)
        {
            double temp2;
            if (l < 0.5f)
                temp2 = l * (1.0f + s);
            else
                temp2 = l + s - (l * s);
            return temp2;
        }
        private static double _MoveIntoRange(double temp3)
        {
            if (temp3 < 0.0f)
                temp3 += 1.0f;
            else if (temp3 > 1.0f)
                temp3 -= 1.0f;
            return temp3;
        }
    }
}
