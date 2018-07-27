namespace System.Windows.Forms
{
    using System.Drawing;

    public class ColorPicker : Button
    {
        private readonly Pen cpBorderPen = new Pen(Color.FromArgb(172, 172, 172));
        private readonly SolidBrush cpColorBrush = new SolidBrush(Color.White);
        private readonly SolidBrush cpAlphaBackBrush = new SolidBrush(Color.Black);
        private readonly SolidBrush cpAlphaFrontBrush = new SolidBrush(Color.White);

        private Color borderColor;
        private Color color;

        public ColorPicker()
        {
            BorderColor = Color.FromArgb(172, 172, 172);
            BorderHoverColor = Color.FromArgb(126, 180, 234);
            Color = Color.White;
            Size = new Size(128, 20);
        }

        public event EventHandler ColorChanged = delegate { };

        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = value;
                cpBorderPen.Color = value;
            }
        }
        public Color BorderHoverColor { get; set; }
        public Color Color
        {
            get { return color; }
            set
            {
                color = value;
                cpColorBrush.Color = Color.FromArgb(255, value);
            }
        }

        protected virtual void OnColorChanged(object sender, EventArgs e)
        {
            ColorChanged(this, EventArgs.Empty);
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            var formStartPosition = FormStartPosition.CenterParent;
            var parentForm = FindFormInternal();
            if (parentForm == null)
                formStartPosition = FormStartPosition.WindowsDefaultLocation;

            var colorPickerForm = new ColorPickerForm();
            colorPickerForm.Color = Color;
            colorPickerForm.StartPosition = formStartPosition;
            colorPickerForm.ColorChanged += (sender, args) =>
            {
                Color = ((ColorPickerForm)sender).Color;
                OnColorChanged(this, args);
            };
            colorPickerForm.ShowDialog(parentForm);
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            cpBorderPen.Color = BorderHoverColor;
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            cpBorderPen.Color = BorderColor;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            float alphaWidth = Width * ((float)Color.A / 255);

            e.Graphics.FillRectangle(cpColorBrush, 0, 0, Width, Height - 3);
            e.Graphics.FillRectangle(cpAlphaBackBrush, 0, Height - 3, Width, 3);
            e.Graphics.FillRectangle(cpAlphaFrontBrush, 0, Height - 3, alphaWidth, 3);
            e.Graphics.DrawRectangle(cpBorderPen, 0, 0, Width, Height);
        }
    }
}
