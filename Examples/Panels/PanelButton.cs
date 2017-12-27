namespace UnityWinForms.Examples.Panels
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public class PanelButton : BaseExamplePanel
    {
        private int clickCount;

        public override void Initialize()
        {
            // Normal.
            this.Create<Label>("Normal:");
            this.Create<Button>("button");

            // Style.
            this.Create<Label>("Styled:", false, 8, lineOffset);
            var buttonStyled = this.Create<Button>("button");
            buttonStyled.uwfBorderColor = Color.Transparent;
            buttonStyled.uwfBorderHoverColor = Color.Transparent;
            buttonStyled.uwfBorderSelectColor = Color.Transparent;
            buttonStyled.BackColor = Color.FromArgb(64, 64, 64);
            buttonStyled.ForeColor = Color.FromArgb(160, 160, 160);
            buttonStyled.uwfHoverColor = Color.FromArgb(74, 74, 74);

            // Background image.
            var imageLayouts = Enum.GetNames(typeof(ImageLayout));

            this.Create<Label>("Background Image:", false, 8, lineOffset);
            var comboBImage = this.Create<ComboBox>();
            var buttonBImage = this.Create<Button>("", true);

            comboBImage.Items.AddRange(imageLayouts);
            comboBImage.SelectedIndex = 2;
            comboBImage.SelectedIndexChanged += (sender, args) =>
            {
                var selectedItem = comboBImage.SelectedItem.ToString();
                buttonBImage.BackgroundImageLayout = (ImageLayout)Enum.Parse(typeof(ImageLayout), selectedItem);
            };
            buttonBImage.BackgroundImage = uwfAppOwner.Resources.FormResize;

            // Image.
            var imageAlign = Enum.GetNames(typeof(ContentAlignment));

            this.Create<Label>("Image:", false, 8, lineOffset);
            var comboImage = this.Create<ComboBox>();
            var buttonImage = this.Create<Button>("", true);

            comboImage.Items.AddRange(imageAlign);
            comboImage.SelectedIndex = 4;
            comboImage.SelectedIndexChanged += (sender, args) =>
            {
                var selectedItem = comboImage.SelectedItem.ToString();
                buttonImage.ImageAlign = (ContentAlignment)Enum.Parse(typeof(ContentAlignment), selectedItem);
            };
            buttonImage.Image = uwfAppOwner.Resources.RadioButton_Checked;

            // Disabled.
            this.Create<Label>("Disabled:", false, 8, lineOffset);
            var buttonDisabled = this.Create<Button>("button");
            buttonDisabled.Enabled = false;

            // FlatStyle.
            var flatStyles = Enum.GetNames(typeof(FlatStyle));

            this.Create<Label>("Flat styles:", false, 8, lineOffset);
            this.Create<Label>("Note: only affect border color, gradient fill is not supported", false, 8, 16);
            var comboFlatStyle = this.Create<ComboBox>();
            var buttonFlatStyle = this.Create<Button>("button", true);

            comboFlatStyle.Items.AddRange(flatStyles);
            comboFlatStyle.SelectedIndex = 2;
            comboFlatStyle.SelectedIndexChanged += (sender, args) =>
            {
                var selectedItem = comboFlatStyle.SelectedItem.ToString();
                buttonFlatStyle.FlatStyle = (FlatStyle)Enum.Parse(typeof(FlatStyle), selectedItem);
            };

            // Click.
            var labelClick = this.Create<Label>("Click:", false, 8, lineOffset);
            this.Create<Label>("Note: try to use spacebar or enter", false, 8, 16);
            var buttonClick = this.Create<Button>("+1");
            buttonClick.Click += (sender, args) =>
            {
                clickCount += 1;
                labelClick.Text = "Click: " + clickCount;
            };
            
            // Perform Click..
            this.Create<Label>("Invoke PerformClick on previous button", false, 8, lineOffset);
            var buttonPClick = this.Create<Button>("button");
            buttonPClick.Click += (sender, args) => buttonClick.PerformClick();

            // User paint.
            this.Create<Label>("User paint:", false, 8, lineOffset);
            this.Create<CustomButton>();
        }

        private class CustomButton : Button
        {
            private readonly Pen borderPen = new Pen(SystemColors.ActiveBorder);
            private readonly Brush rectBrush = Brushes.CadetBlue;
            private readonly Timer timer;

            private float rectX;

            public CustomButton()
            {
                timer = new Timer();
                timer.Interval = 1;
                timer.Tick += (sender, args) =>
                {
                    rectX += .5f;
                    if (rectX > Width)
                        rectX = -16;
                };
                timer.Start();
            }

            protected override void Dispose(bool release_all)
            {
                timer.Dispose();

                base.Dispose(release_all);
            }
            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.FillRectangle(rectBrush, rectX, 3, 16, 16);
                e.Graphics.DrawRectangle(borderPen, 0, 0, Width, Height);
            }
        }
    }
}
