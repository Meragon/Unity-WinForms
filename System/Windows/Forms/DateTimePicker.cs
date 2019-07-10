namespace System.Windows.Forms
{
    using System.Drawing;
    using System.Globalization;

    /// <summary>
    /// Note: Still missing some features.
    /// </summary>
    public class DateTimePicker : Control
    {
        internal Color uwfBorderHoverColor = Color.FromArgb(126, 180, 234);
        internal Color uwfBorderNormalColor = Color.FromArgb(171, 173, 179);
        internal Color uwfBorderSelectColor = Color.FromArgb(86, 157, 229);
        internal Color uwfButtonBorderHoverColor = Color.FromArgb(126, 180, 234);
        internal Color uwfButtonBorderSelectColor = Color.FromArgb(86, 157, 229);
        internal Color uwfButtonHoverColor = Color.FromArgb(229, 241, 252);
        internal Color uwfButtonSelectColor = Color.FromArgb(206, 229, 252);
        internal int uwfButtonWidth = 34;
        internal CultureInfo uwfCultureInfo;

        private readonly Pen borderPen = new Pen(Color.Transparent);
        private readonly Pen dropDownButtonPen = new Pen(Color.Transparent);

        private MonthCalendar monthControl;
        private DateTime value;
        private string valueText;

        public DateTimePicker()
        {
            uwfCultureInfo = Application.CurrentCulture;

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

            if (!IsMouseInsideDropDownButton())
                return;

            var screenLocation = PointToScreen(Point.Empty);
            screenLocation.Offset(0, Height);

            monthControl = new MonthCalendar();
            monthControl.uwfInnerPadding = new Padding(0);
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
            g.uwfDrawString(valueText, Font, ForeColor, 2, 0, Width - uwfButtonWidth - 2, Height);

            // Finding border color.
            var borderColor = uwfBorderNormalColor;
            if (selected)
                borderColor = uwfBorderSelectColor;
            else if (hovered)
                borderColor = uwfBorderHoverColor;

            borderPen.Color = borderColor;

            g.DrawRectangle(borderPen, 0, 0, width, height);

            // Dropdown button.
            var buttonHovered = IsMouseInsideDropDownButton();
            var buttonPressed = monthControl != null && !monthControl.IsDisposed;
            if (buttonHovered || buttonPressed)
            {
                var buttonBackColor = buttonPressed ? uwfButtonSelectColor : uwfButtonHoverColor;
                dropDownButtonPen.Color = buttonPressed ? uwfButtonBorderSelectColor : uwfButtonBorderHoverColor;

                g.uwfFillRectangle(buttonBackColor, width - uwfButtonWidth, 0, uwfButtonWidth, height);
                g.DrawRectangle(dropDownButtonPen, width - uwfButtonWidth, 0, uwfButtonWidth, height);
            }

            var buttonImage = uwfAppOwner.Resources.DateTimePicker;
            var arrowImage = uwfAppOwner.Resources.NumericDown;

            if (buttonImage != null)
                g.DrawImage(buttonImage, width - uwfButtonWidth + 5, 3, buttonImage.Width, buttonImage.Height);

            if (arrowImage != null)
                g.uwfDrawImage(arrowImage, Color.FromArgb(0x40, 0x40, 0x40), width - uwfButtonWidth + 20, 6, arrowImage.Width, arrowImage.Height);
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
            var dropDownButtonX = Width - uwfButtonWidth;

            return mclient.X > dropDownButtonX && ClientRectangle.Contains(mclient);
        }
    }
}
