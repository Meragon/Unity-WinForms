namespace System.Windows.Forms
{
    using System.Drawing;

    public class RadioButton : ButtonBase
    {
        private readonly AppGdiImages resources;
        private bool autoCheck = true;
        private bool firstfocus = true;
        private bool isChecked;
        private bool  measureText;
        private SizeF textSize;

        public RadioButton()
        {
            BackColor = Color.Transparent;
            Padding = new Padding(18, 0, 4, 0);
            TextAlign = ContentAlignment.MiddleLeft;

            resources = uwfAppOwner.Resources;
            uwfBorderColor = Color.Transparent;
            uwfBorderHoverColor = Color.Transparent;
            uwfBorderSelectColor = Color.Transparent;
            uwfHoverColor = Color.Transparent;
        }

        public event EventHandler CheckedChanged;

        public bool AutoCheck
        {
            get { return autoCheck; }
            set
            {
                if (autoCheck == value)
                    return;

                autoCheck = value;
                PerformAutoUpdates(false);
            }
        }
        public bool Checked
        {
            get { return isChecked; }
            set
            {
                if (isChecked == value)
                    return;

                isChecked = value;
                PerformAutoUpdates(false);
                OnCheckedChanged(EventArgs.Empty);
            }
        }

        protected override Size DefaultSize
        {
            get { return new Size(104, 24); }
        }

        protected internal override void DrawTabDots(PaintEventArgs e)
        {
            if (measureText)
                textSize = e.Graphics.MeasureString(Text, Font);

            e.Graphics.DrawRectangle(uwfTabPen, Padding.Left - 2, (Height - textSize.Height) / 2 + 2, textSize.Width + 4, textSize.Height - 4);
        }

        protected virtual void OnCheckedChanged(EventArgs e)
        {
            var handler = CheckedChanged;
            if (handler != null)
                handler(this, e);
        }
        protected override void OnClick(EventArgs e)
        {
            if (autoCheck)
                Checked = true;

            base.OnClick(e);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            
            if (e.KeyCode == Keys.Down)
                NextButton();
            
            if (e.KeyCode == Keys.Up)
                PreviousButton();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var radioImage = resources.RadioButton_Unchecked;
            if (hovered)
                radioImage = resources.RadioButton_Hovered;

            e.Graphics.DrawImage(radioImage, 0, 4, 16, 16);
            if (isChecked)
                e.Graphics.DrawImage(resources.RadioButton_Checked, 0, 4, 16, 16);
        }
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            measureText = true;
        }

        private void NextButton()
        {
            var parent = Parent;
            if (parent == null)
                return;

            var buttons = parent.Controls.FindAll(b => b is RadioButton);
            if (buttons.Count == 0)
                return;

            var index = buttons.FindIndex(b => b == this) + 1;
            if (index >= buttons.Count)
                index = 0;

            ((RadioButton) buttons[index]).Checked = true;
            buttons[index].Focus();
        }
        private void PerformAutoUpdates(bool tabbedInto)
        {
            if (!autoCheck)
                return;

            if (firstfocus)
                WipeTabStops(tabbedInto);

            TabStop = isChecked;

            if (!isChecked)
                return;

            var parent = Parent;
            if (parent == null)
                return;

            var parentControls = parent.Controls;
            for (int i = 0; i < parentControls.Count; i++)
            {
                var childRadioButton = parentControls[i] as RadioButton;
                if (childRadioButton == null || childRadioButton == this)
                    continue;

                childRadioButton.Checked = false;
            }
        }
        private void PreviousButton()
        {
            var parent = Parent;
            if (parent == null)
                return;

            var buttons = parent.Controls.FindAll(b => b is RadioButton);
            if (buttons.Count == 0)
                return;

            var index = buttons.FindIndex(b => b == this) - 1;
            if (index < 0)
                index = buttons.Count - 1;

            ((RadioButton) buttons[index]).Checked = true;
            buttons[index].Focus();
        }
        private void WipeTabStops(bool tabbedInto)
        {
            var parent = Parent;
            if (parent == null)
                return;

            var parentControls = parent.Controls;
            for (int i = 0; i < parentControls.Count; i++)
            {
                var childRadioButton = parentControls[i] as RadioButton;
                if (childRadioButton == null)
                    continue;

                if (!tabbedInto)
                    childRadioButton.firstfocus = false;
                
                if (childRadioButton.autoCheck)
                    childRadioButton.TabStop = false;
            }
        }
    }
}
