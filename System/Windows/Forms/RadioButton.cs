namespace System.Windows.Forms
{
    using System.Drawing;

    public class RadioButton : ButtonBase
    {
        private readonly AppGdiImages resources;
        private bool autoCheck = true;
        private bool firstfocus = true;
        private bool isChecked;

        public RadioButton()
        {
            BackColor = Color.Transparent;
            Padding = new Padding(18, 0, 4, 0);
            TextAlign = ContentAlignment.MiddleLeft;
            TabStop = false;

            resources = uwfAppOwner.Resources;
            uwfBorderColor = Color.Transparent;
            uwfBorderHoverColor = Color.Transparent;
            uwfBorderSelectColor = Color.Transparent;
            uwfHoverColor = Color.Transparent;
        }

        public event EventHandler CheckedChanged = delegate { };

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

        protected virtual void OnCheckedChanged(EventArgs e)
        {
            if (CheckedChanged != null)
                CheckedChanged(this, e);
        }
        protected override void OnClick(EventArgs e)
        {
            if (autoCheck)
                Checked = true;

            base.OnClick(e);
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

        private void PerformAutoUpdates(bool tabbedInto)
        {
            if (autoCheck == false)
                return;

            if (firstfocus)
                WipeTabStops(tabbedInto);

            TabStop = isChecked;

            if (isChecked == false)
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

                if (tabbedInto == false)
                    childRadioButton.firstfocus = false;
                if (childRadioButton.autoCheck)
                    childRadioButton.TabStop = false;
            }
        }
    }
}
