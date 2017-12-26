namespace System.Windows.Forms
{
    using System.Drawing;
    using System.Globalization;

    /// <summary>
    /// Note: Still missing some features.
    /// </summary>
    public class DateTimePicker : Control
    {
        internal Color uwfBorderHoverColor = Color.FromArgb(23, 23, 23);
        internal Color uwfBorderNormalColor = Color.FromArgb(122, 122, 122);
        internal Color uwfBroderSelectColor = Color.FromArgb(0, 120, 215);
        internal CultureInfo uwfCultureInfo;
        internal int uwfDropDownButtonWidth = 34;

        private readonly Pen borderPen = new Pen(Color.Transparent);
        private readonly Pen dropDownButtonPen = new Pen(Color.Transparent);

        private DateTime value;
        private string valueText;

        public DateTimePicker()
        {
            uwfCultureInfo = Application.currentCulture;

            BackColor = Color.White;
            Value = DateTime.Now;

            SetStyle(ControlStyles.Selectable, true);
        }

        public DateTime Value
        {
            get { return value; }
            set
            {
                this.value = value;
                valueText = value.ToString("dd MMMM yyyy", uwfCultureInfo);
            }
        }

        protected override Size DefaultSize
        {
            get { return new Size(200, 20); }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (e.Button != MouseButtons.Left)
                return;

            if (IsMouseInsideDropDownButton() == false)
                return;

            var screenLocation = PointToScreen(Point.Empty);
            screenLocation.Offset(0, Height);

            var monthControl = new MonthCalendar();
            monthControl.uwfContext = true;
            monthControl.uwfShadowBox = true;
            monthControl.Location = screenLocation;
            monthControl.Value = Value;
            monthControl.DateChanged += (sender, args) =>
            {
                Value = monthControl.Value;

                monthControl.Dispose();
            };
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            var height = Height;
            var width = Width;

            // Value.
            g.uwfDrawString(valueText, Font, ForeColor, 2, 0, Width - uwfDropDownButtonWidth - 2, Height);

            // Finding border color.
            var borderColor = uwfBorderNormalColor;
            if (selected)
                borderColor = uwfBroderSelectColor;
            else if (hovered)
                borderColor = uwfBorderHoverColor;

            borderPen.Color = borderColor;

            g.DrawRectangle(borderPen, 0, 0, width, height);

            // Dropdown button.
            if (IsMouseInsideDropDownButton())
            {
                dropDownButtonPen.Color = uwfBroderSelectColor;
                g.DrawRectangle(dropDownButtonPen, width - uwfDropDownButtonWidth, 0, uwfDropDownButtonWidth, height);
            }

            var buttonImage = uwfAppOwner.Resources.DateTimePicker;
            var arrowImage = uwfAppOwner.Resources.NumericDown;
            g.DrawImage(buttonImage, width - uwfDropDownButtonWidth + 5, 3, buttonImage.Width, buttonImage.Height);
            g.DrawImage(arrowImage, width - uwfDropDownButtonWidth + 19, 6, arrowImage.Width, arrowImage.Height);
        }
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // DateTimePicker control is not using Background image, 
            // the only thing we need is to paint back color here.

            // NOTE: it's also not supporting transparent color.
            pevent.Graphics.uwfFillRectangle(BackColor, 0, 0, Width, Height);
        }

        private bool IsMouseInsideDropDownButton()
        {
            var mclient = PointToClient(MousePosition);
            var dropDownButtonX = Width - uwfDropDownButtonWidth;

            return mclient.X > dropDownButtonX && ClientRectangle.Contains(mclient);
        }
    }
}
