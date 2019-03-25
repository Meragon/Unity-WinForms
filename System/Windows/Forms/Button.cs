namespace System.Windows.Forms
{
    [Serializable]
    public class Button : ButtonBase, IButtonControl
    {
        public Button()
        {
            SetStyle(ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, false);
        }

        public virtual DialogResult DialogResult { get; set; }

        public void NotifyDefault(bool value)
        {
            // ?
        }
        public void PerformClick()
        {
            OnClick(EventArgs.Empty);
        }
        public override string ToString()
        {
            return base.ToString() + ", Text: " + Text;
        }

        protected override void OnClick(EventArgs e)
        {
            var form = FindFormInternal();
            if (form != null)
                form.DialogResult = DialogResult;

            base.OnClick(e);
        }
    }
}
